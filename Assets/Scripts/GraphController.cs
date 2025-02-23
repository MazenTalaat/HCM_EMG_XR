using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphController : MonoBehaviour {

    List<GameObject> lineList = new List<GameObject>();

    private DD_DataDiagram m_DataDiagram;
    private List<float> emgData = new List<float>();
    private int counter = 0;
    private float timeStep = 0.1f;
    private int movingWindow = 20;

    // Use this for initialization
    void Start () {
        GameObject dd = GameObject.Find("DataDiagram");
        if(null == dd) {
            Debug.LogWarning("can not find a gameobject of DataDiagram");
            return;
        }
        m_DataDiagram = dd.GetComponent<DD_DataDiagram>();

        m_DataDiagram.PreDestroyLineEvent += (s, e) => { lineList.Remove(e.line); };

        AddALine();

        InvokeRepeating("ContinueInput", 0.01f, timeStep);
    }
    void AddALine()
    {

        if (null == m_DataDiagram)
            return;

        Color color = Color.HSVToRGB(0, 0.8f, 0.8f);
        GameObject line = m_DataDiagram.AddLine(color.ToString(), color);
        if (null != line)
            lineList.Add(line);
    }

    private void ContinueInput() {

        if (null == m_DataDiagram)
            return;

        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                float rmsEmgData = RMSCalculation(RTClient.GetInstance().GetAnalogChannel("BI_EMG 1").Values);

                if (counter == movingWindow)
                {
                    emgData.Add(rmsEmgData);
                    emgData.RemoveAt(0);

                    m_DataDiagram.InputPoint(lineList[0], new Vector2(timeStep, RMSCalculation(emgData.ToArray())));
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
