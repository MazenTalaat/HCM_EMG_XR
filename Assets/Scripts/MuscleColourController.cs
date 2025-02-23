using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleColourController : MonoBehaviour
{
    private List<float> emgData = new List<float>();
    private int counter = 0;
    private float timeStep = 0.1f;
    private int movingWindow = 20;

    public GameObject avatar;

    /// <summary>
    /// Muscles Material Index
    /// 3 L_Scapular_part_of_deltoid_Pbr
    /// 4 L_Clavicular_part_of_deltoid_Pbr
    /// 5 L_Clavicular_head_of_pectoralis_Pbr
    /// 6 L_Acromial_part_of_deltoid_Pbr
    /// 
    /// 10 R_Scapular_part_of_deltoid_Pbr
    /// 11 R_Clavicular_part_of_deltoid_Pbr
    /// 12 R_Clavicular_head_of_pectoralis_Pbr
    /// 13 R_Acromial_part_of_deltoid_Pbr
    /// </summary>

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
                //print(rmsEmgData);
                var matCounter = 0;
                foreach (var mat in avatar.GetComponent<SkinnedMeshRenderer>().materials)
                {
                    if (mat.name == "L_Scapular_part_of_deltoid_Pbr (Instance)")
                    {
                        avatar.GetComponent<SkinnedMeshRenderer>().materials[matCounter].color = new Color(255, 0, 0, rmsEmgData / 500f);
                        break;
                    }
                    matCounter++;
                }

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
