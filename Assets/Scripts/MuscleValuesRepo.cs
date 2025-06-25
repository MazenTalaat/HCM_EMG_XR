using QualisysRealTime.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Samples six EMG analog channels from Qualisys RT, keeps a sliding-window RMS,
/// and tracks the Maximum Voluntary Isometric Contraction (MVIC) for each channel.
/// </summary>
public class MuscleValuesRepo : MonoBehaviour
{
    private static int _channelCount = 6;
    private string[] _channelNames = { "BI_EMG 1", "BI_EMG 1", "BI_EMG 1", "BI_EMG 1", "BI_EMG 1", "BI_EMG 1" };

    private int _samplesCollected = 0;
    public static float timeStep = 0.05f;
    private int _movingWindowSize = 20;

    private List<float> _rawEmgData = Enumerable.Repeat(0f, _channelCount).ToList();
    private List<List<float>> _rmsWindow;

    public static List<float> rmsEmgData = Enumerable.Repeat(0f, _channelCount).ToList();
    public static List<float> MVIC = Enumerable.Repeat(1f, _channelCount).ToList();

    private RTClient _rt;

    // Start is called before the first frame update
    void Start()
    {
        _rmsWindow = new List<List<float>>();
        for (int i = 0; i < _channelCount; i++)
        {
            _rmsWindow.Add(new List<float>());
        }

        _rt = RTClient.GetInstance();
        InvokeRepeating(nameof(GetEmgData), 0.01f, timeStep);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void GetEmgData()
    {
        if (_rt.ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                for (int i = 0; i < _channelCount; ++i)
                {
                    _rawEmgData[i] = RMSCalculation(_rt.GetAnalogChannel(_channelNames[i]).Values);
                }

                if (_samplesCollected == _movingWindowSize)
                {
                    for (int i = 0; i < _channelCount; i++)
                    {
                        _rmsWindow[i].Add(_rawEmgData[i]);
                        _rmsWindow[i].RemoveAt(0);
                        rmsEmgData[i] = RMSCalculation(_rmsWindow[i].ToArray());
                    }
                }
                else
                {
                    _samplesCollected++;
                    for (int i = 0; i < _channelCount; i++)
                    {
                        _rmsWindow[i].Add(_rawEmgData[i]);
                    }
                }

                for (int i = 0; i < _channelCount; i++)
                {
                    MVIC[i] = Math.Max(MVIC[i], rmsEmgData[i]);
                }

                //print("Raw: " + rawEmgData[0].ToString() + " RMS: " + rmsEmgData[0].ToString() + " MVIC: " + MVIC[0].ToString());
            }
            catch (Exception)
            {
                print("Couldn't get muscle data");
            }
        }
    }

    float RMSCalculation(float[] samples)
    {
        if (samples == null || samples.Length == 0) return 0f;

        double sumSq = 0.0;
        for (int i = 0; i < samples.Length; ++i)
            sumSq += samples[i] * samples[i];
        return (float)Math.Sqrt(sumSq / samples.Length);
    }
}
