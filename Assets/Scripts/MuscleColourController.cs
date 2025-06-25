using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleColourController : MonoBehaviour
{
    public GameObject avatar;
    /// <summary>
    /// Muscles Material Index
    /// 3 L_Scapular_part_of_deltoid_Pbr
    /// 4 L_Clavicular_part_of_deltoid_Pbr
    /// XX5 L_Clavicular_head_of_pectoralis_Pbr
    /// 6 L_Acromial_part_of_deltoid_Pbr
    /// 
    /// 10 R_Scapular_part_of_deltoid_Pbr
    /// 11 R_Clavicular_part_of_deltoid_Pbr
    /// XX12 R_Clavicular_head_of_pectoralis_Pbr
    /// 13 R_Acromial_part_of_deltoid_Pbr
    /// </summary>

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
                var matCounter = 0;
                foreach (var mat in avatar.GetComponent<SkinnedMeshRenderer>().materials)
                {
                    switch (mat.name)
                    {
                        case "L_Scapular_part_of_deltoid_Pbr (Instance)": // 3
                            avatar.GetComponent<SkinnedMeshRenderer>().materials[matCounter].color =
                                new Color(255, 0, 0, MuscleValuesRepo.rmsEmgData[0] / MuscleValuesRepo.MVIC[0]);
                            break;

                        case "L_Clavicular_part_of_deltoid_Pbr (Instance)": // 4
                            avatar.GetComponent<SkinnedMeshRenderer>().materials[matCounter].color =
                                new Color(255, 0, 0, MuscleValuesRepo.rmsEmgData[1] / MuscleValuesRepo.MVIC[1]);
                            break;

                        case "L_Acromial_part_of_deltoid_Pbr (Instance)": // 6
                            avatar.GetComponent<SkinnedMeshRenderer>().materials[matCounter].color =
                                new Color(255, 0, 0, MuscleValuesRepo.rmsEmgData[2] / MuscleValuesRepo.MVIC[2]);
                            break;

                        case "R_Scapular_part_of_deltoid_Pbr (Instance)": // 10
                            avatar.GetComponent<SkinnedMeshRenderer>().materials[matCounter].color =
                                new Color(255, 0, 0, MuscleValuesRepo.rmsEmgData[3] / MuscleValuesRepo.MVIC[3]);
                            break;

                        case "R_Clavicular_part_of_deltoid_Pbr (Instance)": // 11
                            avatar.GetComponent<SkinnedMeshRenderer>().materials[matCounter].color =
                                new Color(255, 0, 0, MuscleValuesRepo.rmsEmgData[4] / MuscleValuesRepo.MVIC[4]);
                            break;

                        case "R_Acromial_part_of_deltoid_Pbr (Instance)": // 13
                            avatar.GetComponent<SkinnedMeshRenderer>().materials[matCounter].color =
                                new Color(255, 0, 0, MuscleValuesRepo.rmsEmgData[5] / MuscleValuesRepo.MVIC[5]);
                            break;

                        default:
                            break;
                    }
                    matCounter++;
                }
            }
            catch (System.Exception)
            {
                print("Couldn't get muscle data");
            }
        }
    }
}
