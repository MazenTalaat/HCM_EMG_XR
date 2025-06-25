using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphController : MonoBehaviour
{

    List<GameObject> lineList = new List<GameObject>();

    private DD_DataDiagram m_DataDiagram;

    // Use this for initialization
    void Start()
    {
        GameObject dd = GameObject.Find("DataDiagram");
        if (null == dd)
        {
            Debug.LogWarning("can not find a gameobject of DataDiagram");
            return;
        }
        m_DataDiagram = dd.GetComponent<DD_DataDiagram>();

        m_DataDiagram.PreDestroyLineEvent += (s, e) => { lineList.Remove(e.line); };

        AddALine();

        InvokeRepeating("ContinueInput", 0.01f, MuscleValuesRepo.timeStep);
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

    private void ContinueInput()
    {

        if (null == m_DataDiagram)
            return;

        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                m_DataDiagram.InputPoint(lineList[0], new Vector2(MuscleValuesRepo.timeStep, MuscleValuesRepo.rmsEmgData[0]));
            }
            catch (System.Exception)
            {
                print("Couldn't get muscle data");
            }

        }
    }
}
