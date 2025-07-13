using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the color of muscle materials on the avatar based on real-time EMG data.
/// Colors are updated according to the RMS EMG value normalized by MVIC for each muscle.
/// </summary>
public class MuscleColourController : MonoBehaviour
{
    [Header("Avatar Reference")]
    public GameObject avatar; // The avatar GameObject with SkinnedMeshRenderer

    /// <summary>
    /// Muscle Material Index Mapping:
    ///  3  L_Scapular_part_of_deltoid_Pbr      (Posterior, Left)
    ///  4  L_Clavicular_part_of_deltoid_Pbr    (Anterior, Left)
    ///  6  L_Acromial_part_of_deltoid_Pbr      (Medius, Left)
    /// 10  R_Scapular_part_of_deltoid_Pbr      (Posterior, Right)
    /// 11  R_Clavicular_part_of_deltoid_Pbr    (Anterior, Right)
    /// 13  R_Acromial_part_of_deltoid_Pbr      (Medius, Right)
    /// </summary>

    // Called once per frame
    void Update()
    {
        UpdateMuscleColors();
    }

    /// <summary>
    /// Updates the color of each muscle material on the avatar based on EMG data.
    /// </summary>
    private void UpdateMuscleColors()
    {
        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            try
            {
                var materialIndex = 0;
                var meshRenderer = avatar.GetComponent<SkinnedMeshRenderer>();
                var materials = meshRenderer.materials;

                foreach (var material in materials)
                {
                    // Set color based on muscle and EMG data
                    switch (material.name)
                    {
                        case "L_Scapular_part_of_deltoid_Pbr (Instance)": // Left Posterior
                            materials[materialIndex].color =
                                new Color(0, 0, 1, GetNormalizedEmgValue(0)); // Blue
                            break;

                        case "L_Clavicular_part_of_deltoid_Pbr (Instance)": // Left Anterior
                            materials[materialIndex].color =
                                new Color(1, 0, 0, GetNormalizedEmgValue(1)); // Red
                            break;

                        case "L_Acromial_part_of_deltoid_Pbr (Instance)": // Left Medius
                            materials[materialIndex].color =
                                new Color(0, 1, 0, GetNormalizedEmgValue(2)); // Green
                            break;

                        case "R_Scapular_part_of_deltoid_Pbr (Instance)": // Right Posterior
                            materials[materialIndex].color =
                                new Color(0, 0, 1, GetNormalizedEmgValue(3)); // Blue
                            break;

                        case "R_Clavicular_part_of_deltoid_Pbr (Instance)": // Right Anterior
                            materials[materialIndex].color =
                                new Color(1, 0, 0, GetNormalizedEmgValue(4)); // Red
                            break;

                        case "R_Acromial_part_of_deltoid_Pbr (Instance)": // Right Medius
                            materials[materialIndex].color =
                                new Color(0, 1, 0, GetNormalizedEmgValue(5)); // Green
                            break;

                        default:
                            // No color update for other materials
                            break;
                    }
                    materialIndex++;
                }
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Couldn't get muscle data");
            }
        }
    }

    /// <summary>
    /// Returns the normalized EMG value (RMS/MVIC) for the given muscle index.
    /// </summary>
    /// <param name="muscleIndex">Index of the muscle channel</param>
    /// <returns>Normalized EMG value (float)</returns>
    private float GetNormalizedEmgValue(int muscleIndex)
    {
        // Prevent division by zero
        float mvic = MuscleValuesRepo.mvicValues[muscleIndex];
        return mvic > 0 ? MuscleValuesRepo.rmsEmgValues[muscleIndex] / mvic : 0f;
    }
}
