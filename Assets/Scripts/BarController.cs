using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    public ProgressBar progressBar;
    private List<float> emgData = new List<float>();
    private int counter = 0;
    private float timeStep = 0.1f;
    private int movingWindow = 20;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetEMG_Data", 0.01f, timeStep);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetEMG_Data()
    {
        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                float rmsEmgData = RMSCalculation(RTClient.GetInstance().GetAnalogChannel("BI_EMG 1").Values);
                progressBar.BarValue = rmsEmgData / 5f;

                if (counter == movingWindow)
                {
                    emgData.Add(rmsEmgData);
                    emgData.RemoveAt(0);
                }
                else
                {
                    counter++;
                    emgData.Add(rmsEmgData);
                }
            }
            catch (System.Exception)
            {
                print("Couldn't get muscle data");
            }

        }
    }

    float RMSCalculation(float[] arr)
    {
        if (arr.Length == 0)
            return 0;
        float square = 0;
        float mean, root;

        // Calculate square
        for (int i = 0; i < arr.Length; i++)
        {
            square += (float)System.Math.Pow(arr[i], 2);
        }

        // Calculate Mean
        mean = (square / (arr.Length));

        // Calculate Root
        root = (float)System.Math.Sqrt(mean);

        return root;
    }
}
