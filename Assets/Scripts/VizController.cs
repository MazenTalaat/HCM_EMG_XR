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

    }

    public void showViz(int index)
    {
        switch (index)
        {
            case 0:
                avatar.SetActive(!avatar.activeSelf);
                break;
            case 1:
                progressBar.SetActive(!progressBar.activeSelf);
                break;
            case 2:
                graph.SetActive(!graph.activeSelf);
                break;

            default:
                break;
        }
    }
}
