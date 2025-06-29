using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    public ProgressBar progressBar;
    private Dictionary<string, int> _musclesMap = new Dictionary<string, int>
    {
        { "L_Deltoid_Anterior", 0 },
        { "L_Deltoid_Medius", 1 },
        { "L_Deltoid_Posterior", 2 },
        { "R_Deltoid_Anterior", 3 },
        { "R_Deltoid_Medius", 4 },
        { "R_Deltoid_Posterior", 5 }
    };
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GetEmgData();
    }

    private void GetEmgData()
    {
        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                progressBar.BarValue = (int)(MuscleValuesRepo.rmsEmgData[_musclesMap[progressBar.Title]] / MuscleValuesRepo.MVIC[_musclesMap[progressBar.Title]] * 100);
            }
            catch (System.Exception)
            {
                print("Couldn't get muscle data");
            }
        }
    }
}
