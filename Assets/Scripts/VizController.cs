using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the visibility of visualization UI elements (avatar, progress bar, graph).
/// Provides methods to toggle each visualization based on user selection.
/// </summary>
public class VizController : MonoBehaviour
{
    public GameObject progressBarPanel; // Reference to the ProgressBar UI panel
    public GameObject graphPanel;       // Reference to the Graph UI panel
    public GameObject avatarObject;     // Reference to the Avatar GameObject

    /// <summary>
    /// Toggles the visibility of a visualization element based on the provided index.
    /// 0 = Avatar, 1 = Progress Bar, 2 = Graph
    /// </summary>
    /// <param name="visualizationIndex">Index of the visualization to toggle</param>
    public void ToggleVisualization(int visualizationIndex)
    {
        switch (visualizationIndex)
        {
            case 0:
                avatarObject.SetActive(!avatarObject.activeSelf);
                break;
            case 1:
                progressBarPanel.SetActive(!progressBarPanel.activeSelf);
                break;
            case 2:
                graphPanel.SetActive(!graphPanel.activeSelf);
                break;
            default:
                // No action for invalid index
                break;
        }
    }
}
