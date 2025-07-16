using QualisysRealTime.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles sampling of six EMG analog channels from Qualisys RT, maintains a sliding-window RMS,
/// and tracks the Maximum Voluntary Isometric Contraction (MVIC) for each channel.
/// Also updates UI elements and communicates with MVIC_Client for data persistence.
/// </summary>
public class MuscleValuesRepo : MonoBehaviour
{
    [Header("UI References")]
    public GameObject graphPanel; // Panel displaying EMG graphs
    public GameObject mvicTextPanel; // Panel displaying MVIC values
    public List<TMPro.TextMeshProUGUI> mvicTextFields; // Text fields for MVIC display

    public static bool isMVICTrackingActive = false; // Indicates if MVIC tracking is enabled

    private static int ChannelCount = 6;
    // Channel names for EMG analog input (update as needed for your setup)
    private string[] channelNames = { "L_Deltoid_Anterior", "L_Deltoid_Medius", "L_Deltoid_Posterior", "R_Deltoid_Anterior", "R_Deltoid_Medius", "R_Deltoid_Posterior" };

    private int samplesCollected = 0;
    public static float SamplingInterval = 0.05f; // Time between EMG samples (seconds)
    private int rmsWindowSize = 20; // Number of samples in RMS moving window

    private List<float> latestEmgSample = Enumerable.Repeat(0f, ChannelCount).ToList(); // Latest EMG sample per channel
    private List<List<float>> rmsSampleWindow; // Sliding window of samples for RMS calculation

    public static List<float> rmsEmgValues = Enumerable.Repeat(0f, ChannelCount).ToList(); // Current RMS value per channel
    public static List<float> mvicValues = Enumerable.Repeat(1f, ChannelCount).ToList(); // MVIC value per channel

    public MVIC_Client mvicClient; // Handles server communication for MVIC values

    private RTClient rtClient; // Qualisys RT client instance

    public List<GraphController> graphControllers; // Graph controllers for each channel

    // Called on script initialization
    void Start()
    {
        // Initialize RMS sample window for each channel
        rmsSampleWindow = new List<List<float>>();
        for (int i = 0; i < ChannelCount; i++)
        {
            rmsSampleWindow.Add(new List<float>());
        }

        rtClient = RTClient.GetInstance();
        InvokeRepeating(nameof(SampleEmgData), 0.01f, SamplingInterval);
    }

    // Called once per frame
    void Update()
    {
        // Update MVIC text fields if tracking is active
        if (isMVICTrackingActive)
        {
            for (int i = 0; i < ChannelCount; i++)
            {
                mvicTextFields[i].text = mvicValues[i].ToString();
            }
        }
    }

    /// <summary>
    /// Samples EMG data from Qualisys RT, updates RMS and MVIC values, and sends MVIC to server if tracking.
    /// </summary>
    private void SampleEmgData()
    {
        if (rtClient.ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                // Sample each channel and calculate RMS for current sample
                for (int i = 0; i < ChannelCount; ++i)
                {
                    latestEmgSample[i] = CalculateRMS(rtClient.GetAnalogChannel(channelNames[i]).Values);
                }

                // Maintain sliding window for RMS calculation
                if (samplesCollected == rmsWindowSize)
                {
                    for (int i = 0; i < ChannelCount; i++)
                    {
                        rmsSampleWindow[i].Add(latestEmgSample[i]);
                        rmsSampleWindow[i].RemoveAt(0);
                        rmsEmgValues[i] = CalculateRMS(rmsSampleWindow[i].ToArray());
                    }
                }
                else
                {
                    samplesCollected++;
                    for (int i = 0; i < ChannelCount; i++)
                    {
                        rmsSampleWindow[i].Add(latestEmgSample[i]);
                    }
                }

                // Update MVIC values and send to server if tracking is active
                if (isMVICTrackingActive)
                {
                    for (int i = 0; i < ChannelCount; i++)
                    {
                        mvicValues[i] = Math.Max(mvicValues[i], rmsEmgValues[i]);
                    }
                    StartCoroutine(mvicClient.SendMVICValues(
                        mvicValues.ToArray(),
                        () => Debug.Log("MVIC PUT success"),
                        err => Debug.LogError($"MVIC PUT failed: {err}")
                    ));
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("Couldn't get muscle data");
            }
        }
    }

    /// <summary>
    /// Toggles MVIC tracking and updates UI/graph scaling.
    /// </summary>
    public void ToggleMVICTracking()
    {
        mvicTextPanel.SetActive(!mvicTextPanel.activeSelf);
        isMVICTrackingActive = !isMVICTrackingActive;
        foreach (var graph in graphControllers)
        {
            graph.AdjustDiagramScale();
        }
    }

    /// <summary>
    /// Fetches MVIC values from server and updates UI/graph scaling.
    /// </summary>
    public void FetchMVICFromServer()
    {
        StartCoroutine(mvicClient.GetMVICValues(
            vals =>
            {
                mvicValues = vals.ToList();
                for (int i = 0; i < ChannelCount; i++)
                {
                    mvicTextFields[i].text = mvicValues[i].ToString();
                }

                foreach (var graph in graphControllers)
                {
                    graph.AdjustDiagramScale();
                }
            },
            err => Debug.LogError($"MVIC GET failed: {err}")
        ));
    }

    /// <summary>
    /// Calculates RMS value for a set of samples.
    /// </summary>
    /// <param name="samples">Array of EMG samples</param>
    /// <returns>RMS value</returns>
    float CalculateRMS(float[] samples)
    {
        if (samples == null || samples.Length == 0) return 0f;

        double sumSq = 0.0;
        for (int i = 0; i < samples.Length; ++i)
            sumSq += samples[i] * samples[i];
        return (float)Math.Sqrt(sumSq / samples.Length);
    }
}
