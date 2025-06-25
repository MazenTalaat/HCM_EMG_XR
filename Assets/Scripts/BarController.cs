using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    public ProgressBar progressBar;
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
                progressBar.BarValue = (int)(MuscleValuesRepo.rmsEmgData[0] / MuscleValuesRepo.MVIC[0] * 100);
            }
            catch (System.Exception)
            {
                print("Couldn't get muscle data");
            }
        }
    }
}
