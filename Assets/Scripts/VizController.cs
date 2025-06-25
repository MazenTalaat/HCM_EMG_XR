using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VizController : MonoBehaviour
{
    public GameObject progressBar;
    public GameObject graph;
    public GameObject avatar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // One -> A, Two -> B, Three -> X, Four -> Y
        if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown("a"))
        {
            progressBar.SetActive(!progressBar.activeSelf);
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown("b"))
        {
            graph.SetActive(!graph.activeSelf);
        }
        else if (OVRInput.GetDown(OVRInput.Button.Three) || Input.GetKeyDown("x"))
        {
            avatar.SetActive(!avatar.activeSelf);
        }
    }
}
