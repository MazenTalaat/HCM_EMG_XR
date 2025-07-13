using QualisysRealTime.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the EMG data graph visualization for selected muscle channels.
/// Handles dynamic scaling, line creation, and real-time data input.
/// </summary>
public class GraphController : MonoBehaviour
{
    public GameObject dataDiagramObject; // Reference to DD_DataDiagram GameObject
    public List<int> muscleChannelIndices; // Indices of EMG channels to plot

    private List<GameObject> graphLines = new List<GameObject>(); // List of line GameObjects for the graph
    private DD_DataDiagram dataDiagram; // Reference to DD_DataDiagram component

    private float currentMVIC = 1; // Current max MVIC value for scaling

    // Called on script initialization
    void Start()
    {
        if (dataDiagramObject == null)
        {
            Debug.LogWarning("Cannot find DataDiagram GameObject.");
            return;
        }
        dataDiagram = dataDiagramObject.GetComponent<DD_DataDiagram>();

        // Remove line from list when destroyed
        dataDiagram.PreDestroyLineEvent += (s, e) => { graphLines.Remove(e.line); };

        CreateGraphLines();

        // Start feeding data to the graph at regular intervals
        InvokeRepeating(nameof(InputEmgDataToGraph), 0.01f, MuscleValuesRepo.SamplingInterval);
    }

    // Called once per frame
    void Update()
    {
        // Adjust graph scale if MVIC values have been updated
        if (MuscleValuesRepo.isMVICTrackingActive)
        {
            AdjustDiagramScale();
        }
    }

    /// <summary>
    /// Adjusts the Y-axis scale of the graph based on the highest MVIC value among selected channels.
    /// </summary>
    public void AdjustDiagramScale()
    {
        currentMVIC = Math.Max(
            MuscleValuesRepo.mvicValues[muscleChannelIndices[0]],
            Math.Max(
                MuscleValuesRepo.mvicValues[muscleChannelIndices[1]],
                MuscleValuesRepo.mvicValues[muscleChannelIndices[2]]
            )
        );

        if (dataDiagram == null)
        {
            dataDiagram = dataDiagramObject.GetComponent<DD_DataDiagram>();
        }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        dataDiagram.m_CentimeterPerCoordUnitY = (13.0f / 1300) * (1300 / (1.1f * currentMVIC));
#elif UNITY_ANDROID
        dataDiagram.m_CentimeterPerCoordUnitY = (13.0f / 1300) * (1300 / (1.1f * currentMVIC)) / 2;
#endif
    }

    /// <summary>
    /// Creates colored lines for each muscle channel in the graph.
    /// </summary>
    void CreateGraphLines()
    {
        if (dataDiagram == null)
            return;

        Color colorRed = Color.HSVToRGB(0, 0.8f, 0.8f);
        Color colorGreen = Color.HSVToRGB(0.33f, 0.8f, 0.8f);
        Color colorBlue = Color.HSVToRGB(0.66f, 0.8f, 0.8f);

        GameObject lineRed = dataDiagram.AddLine(colorRed.ToString(), colorRed);
        if (lineRed != null)
            graphLines.Add(lineRed);

        GameObject lineGreen = dataDiagram.AddLine(colorGreen.ToString(), colorGreen);
        if (lineGreen != null)
            graphLines.Add(lineGreen);

        GameObject lineBlue = dataDiagram.AddLine(colorBlue.ToString(), colorBlue);
        if (lineBlue != null)
            graphLines.Add(lineBlue);
    }

    /// <summary>
    /// Inputs the latest EMG RMS data points into the graph for each channel.
    /// </summary>
    private void InputEmgDataToGraph()
    {
        if (dataDiagram == null)
            return;

        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                dataDiagram.InputPoint(graphLines[0], new Vector2(MuscleValuesRepo.SamplingInterval, MuscleValuesRepo.rmsEmgValues[muscleChannelIndices[0]]));
                dataDiagram.InputPoint(graphLines[1], new Vector2(MuscleValuesRepo.SamplingInterval, MuscleValuesRepo.rmsEmgValues[muscleChannelIndices[1]]));
                dataDiagram.InputPoint(graphLines[2], new Vector2(MuscleValuesRepo.SamplingInterval, MuscleValuesRepo.rmsEmgValues[muscleChannelIndices[2]]));
            }
            catch (Exception)
            {
                Debug.LogWarning("Couldn't get muscle data for graph input.");
            }
        }
    }
}
