using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QualisysRealTime.Unity;
using QTMRealTimeSDK;
using TMPro;
using Oculus.Interaction;
using UnityEngine.UI;

public class ServerController : MonoBehaviour
{
    public GameObject toggleButtonPrefab;
    public GameObject scrollViewContent;
    public TextMeshProUGUI statusText;
    private List<DiscoveryResponse> discoveryResponses;
    private List<int> emg1;
    private string serverIP;
    private short serverPort;

    // Start is called before the first frame update
    void Start()
    {
        UpdateServers();
    }

    public void UpdateServers()
    {
        if (RTClient.GetInstance().ConnectionState != RTConnectionState.Connected)
        {
            statusText.color = Color.white;
        }
        for (int i=0; i<scrollViewContent.transform.childCount; i++)
        {
            Destroy(scrollViewContent.transform.GetChild(i).gameObject);
        }

        discoveryResponses = RTClient.GetInstance().GetServers();
        foreach (DiscoveryResponse server in discoveryResponses)
        {
            var toggleButtonGO = GameObject.Instantiate(toggleButtonPrefab, scrollViewContent.transform);
            toggleButtonGO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Host name: " + server.HostName + "\nIP: " + server.IpAddress + ":" + server.Port;
            toggleButtonGO.GetComponent<ToggleDeselect>().group = scrollViewContent.GetComponent<ToggleGroup>();
            toggleButtonGO.GetComponent<ToggleDeselect>().onValueChanged.AddListener(delegate {
                if (toggleButtonGO.GetComponent<ToggleDeselect>().isOn)
                {
                    serverIP = server.IpAddress;
                    serverPort = server.Port;
                }
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            print(RTClient.GetInstance().GetAnalogChannel("BI_EMG 1").Values.Length);
            
            //foreach(var v in RTClient.GetInstance().AnalogChannels[0].Values)
            //{
            //    print(v);
            //}

        }
    }

    public void ConnectOnClick()
    {
        StartCoroutine(Connect());
    }

    IEnumerator Connect()
    {
        RTClient.GetInstance().StartConnecting(serverIP, serverPort, false, true, false, false, true, true, false);
        statusText.text = "Connecting ...";
        statusText.color = Color.yellow;
        print("Connecting ...");

        while (RTClient.GetInstance().ConnectionState == RTConnectionState.Connecting)
        {
            yield return null;
        }
        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            //SendMessageUpwards("Disable");
            statusText.text = "Connected";
            statusText.color = Color.green;
            print("Connected");
        }
        else
        {
            statusText.text = "Could not connect to this server";
            statusText.color = Color.red;
            print("Could not connect to this server");
        }
    }
}
