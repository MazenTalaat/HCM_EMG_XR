using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the progress bar UI for displaying normalized EMG activity for a selected muscle.
/// Updates the bar value in real-time based on RMS EMG and MVIC data.
/// </summary>
public class BarController : MonoBehaviour
{
    public ProgressBar progressBar; // Reference to the ProgressBar UI component

    // Maps muscle names to their corresponding EMG channel indices
    private Dictionary<string, int> muscleNameToChannelIndex = new Dictionary<string, int>
    {
        { "L_Deltoid_Anterior", 0 },
        { "L_Deltoid_Medius", 1 },
        { "L_Deltoid_Posterior", 2 },
        { "R_Deltoid_Anterior", 3 },
        { "R_Deltoid_Medius", 4 },
        { "R_Deltoid_Posterior", 5 }
    };

    // Called once per frame
    void Update()
    {
        UpdateProgressBarWithEmgData();
    }

    /// <summary>
    /// Updates the progress bar value based on normalized EMG data for the selected muscle.
    /// </summary>
    private void UpdateProgressBarWithEmgData()
    {
        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                int channelIndex = muscleNameToChannelIndex[progressBar.Title];
                // Calculate normalized EMG activity as a percentage
                progressBar.BarValue = (int)(
                    MuscleValuesRepo.rmsEmgValues[channelIndex] /
                    MuscleValuesRepo.mvicValues[channelIndex] * 100
                );
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Couldn't get muscle data for progress bar.");
            }
        }
    }
}
