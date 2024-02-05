using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour {

    List<GameObject> lineList = new List<GameObject>();

    private DD_DataDiagram m_DataDiagram;
    //private RectTransform DDrect;

    private bool m_IsContinueInput = true;
    //private float m_Input = 0f;
    private float h = 0;

    private List<float> emgData = new List<float>();
    private int counter = 0;

    void AddALine() {

        if (null == m_DataDiagram)
            return;

        Color color = Color.HSVToRGB((h += 0.1f) > 1 ? (h - 1) : h, 0.8f, 0.8f);
        GameObject line = m_DataDiagram.AddLine(color.ToString(), color);
        if (null != line)
            lineList.Add(line);
    }

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

        InvokeRepeating("ContinueInput", 0.01f, 0.01f);
    }

    // Update is called once per frame
    //void Update () {

    //}

    //private void FixedUpdate() {

    //    m_Input += Time.deltaTime;
    //    ContinueInput(m_Input);
    //}

    float RMSCalculation(float[] arr)
    {
        if (arr.Length == 0)
            return 0;
        int square = 0;
        float mean, root = 0;

        // Calculate square
        for (int i = 0; i < arr.Length; i++)
        {
            square += (int)System.Math.Pow(arr[i], 2);
        }

        // Calculate Mean
        mean = (square / (float)(arr.Length));

        // Calculate Root
        root = (float)System.Math.Sqrt(mean);

        return root;
    }

    private void ContinueInput() {

        if (null == m_DataDiagram)
            return;

        if (false == m_IsContinueInput)
            return;

        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            float x = RMSCalculation(RTClient.GetInstance().GetAnalogChannel("BI_EMG 1").Values);
            //print(RTClient.GetInstance().GetAnalogChannel("BI_EMG 1").Values.Length);
            //print(x);
            
            if(counter == 20)
            {
                emgData.Add(x);
                emgData.RemoveAt(0);
                foreach (GameObject l in lineList)
                {
                    m_DataDiagram.InputPoint(l, new Vector2(0.01f, RMSCalculation(emgData.ToArray())));
                    print(RMSCalculation(emgData.ToArray()));
                }
            }
            else
            {
                counter++;
                emgData.Add(x);
            }
        }
    }

    public void onButton() {

        if (null == m_DataDiagram)
            return;

        foreach (GameObject l in lineList) {
            m_DataDiagram.InputPoint(l, new Vector2(1, Random.value * 4f));
        }
    }

    public void OnAddLine() {
        AddALine();
    }

    public void OnRectChange() {

        if (null == m_DataDiagram)
            return;

        Rect rect = new Rect(Random.value * Screen.width, Random.value * Screen.height,
            Random.value * Screen.width / 2, Random.value * Screen.height / 2);

        m_DataDiagram.rect = rect;
    }

    public void OnContinueInput() {

        m_IsContinueInput = !m_IsContinueInput;

    }

}
