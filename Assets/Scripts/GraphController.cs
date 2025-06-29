using QualisysRealTime.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphController : MonoBehaviour
{
    public GameObject dd;
    public List<int> emgIndex;

    private List<GameObject> lineList = new List<GameObject>();
    private DD_DataDiagram m_DataDiagram;

    private float tempMVIC = 1;


    // Use this for initialization
    void Start()
    {
        if (null == dd)
        {
            Debug.LogWarning("can not find a gameobject of DataDiagram");
            return;
        }
        m_DataDiagram = dd.GetComponent<DD_DataDiagram>();

        m_DataDiagram.PreDestroyLineEvent += (s, e) => { lineList.Remove(e.line); };

        AddLines();

        InvokeRepeating("ContinueInput", 0.01f, MuscleValuesRepo.timeStep);
    }

    void Update()
    {
        if (MuscleValuesRepo.getMVIC)
        {
            tempMVIC = Math.Max(MuscleValuesRepo.MVIC[emgIndex[0]], MuscleValuesRepo.MVIC[emgIndex[1]]);
            tempMVIC = Math.Max(tempMVIC, MuscleValuesRepo.MVIC[emgIndex[2]]);
            m_DataDiagram.m_CentimeterPerCoordUnitY = (13.0f/1300)*(1300/ (1.1f * tempMVIC));
        }
    }

    void AddLines()
    {

        if (null == m_DataDiagram)
            return;

        Color colorRed = Color.HSVToRGB(0, 0.8f, 0.8f);
        Color colorGreen = Color.HSVToRGB(0.33f, 0.8f, 0.8f);
        Color colorBlue = Color.HSVToRGB(0.66f, 0.8f, 0.8f);

        GameObject lineRed = m_DataDiagram.AddLine(colorRed.ToString(), colorRed);
        if (null != lineRed)
            lineList.Add(lineRed);

        GameObject lineGreen = m_DataDiagram.AddLine(colorGreen.ToString(), colorGreen);
        if (null != lineGreen)
            lineList.Add(lineGreen);

        GameObject lineBlue = m_DataDiagram.AddLine(colorBlue.ToString(), colorBlue);
        if (null != lineBlue)
            lineList.Add(lineBlue);
    }

    private void ContinueInput()
    {

        if (null == m_DataDiagram)
            return;

        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                m_DataDiagram.InputPoint(lineList[0], new Vector2(MuscleValuesRepo.timeStep, MuscleValuesRepo.rmsEmgData[emgIndex[0]]));
                m_DataDiagram.InputPoint(lineList[1], new Vector2(MuscleValuesRepo.timeStep, MuscleValuesRepo.rmsEmgData[emgIndex[1]]));
                m_DataDiagram.InputPoint(lineList[2], new Vector2(MuscleValuesRepo.timeStep, MuscleValuesRepo.rmsEmgData[emgIndex[2]]));
            }
            catch (System.Exception)
            {
                print("Couldn't get muscle data");
            }

        }
    }
}
