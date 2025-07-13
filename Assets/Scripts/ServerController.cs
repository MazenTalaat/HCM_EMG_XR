using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QualisysRealTime.Unity;
using QTMRealTimeSDK;
using TMPro;
using Oculus.Interaction;
using UnityEngine.UI;
using System;

public class ServerController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject toggleButtonPrefab; // Prefab for server selection toggle button
    public GameObject scrollViewContent;  // Content container for server list
    public GameObject serverSelectionPanel; // Cylinder panel for server selection
    public TextMeshProUGUI statusText;    // UI text for connection status
    public List<Button> serverActionButtons; // Buttons for connect/disconnect actions

    [Header("Avatar Reference")]
    public GameObject avatar; // Avatar GameObject to activate on connection

    private List<DiscoveryResponse> availableServers; // List of discovered QTM servers

    // Current server IP and port for connection
    private string selectedServerIp = EndPoints.QTMServerIp;
    private short selectedServerPort = 22222;

    // Called on script initialization
    void Start()
    {
        RefreshServerList();
        serverActionButtons[0].interactable = true;  // Connect
        serverActionButtons[1].interactable = true;  // Refresh
        serverActionButtons[2].interactable = false; // Disconnect
        ConnectOnClick();
    }

    /// <summary>
    /// Refreshes the server list UI and populates with discovered QTM servers.
    /// </summary>
    public void RefreshServerList()
    {
        // Clear previous server entries
        for (int i = 0; i < scrollViewContent.transform.childCount; i++)
        {
            Destroy(scrollViewContent.transform.GetChild(i).gameObject);
        }

        availableServers = RTClient.GetInstance().GetServers();
        foreach (DiscoveryResponse server in availableServers)
        {
            var toggleButtonGO = Instantiate(toggleButtonPrefab, scrollViewContent.transform);
            toggleButtonGO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
                $"Host name: {server.HostName}\nIP: {server.IpAddress}:{server.Port}";

            toggleButtonGO.GetComponent<ToggleDeselect>().group = scrollViewContent.GetComponent<ToggleGroup>();
            toggleButtonGO.GetComponent<ToggleDeselect>().onValueChanged.AddListener(delegate {
                if (toggleButtonGO.GetComponent<ToggleDeselect>().isOn)
                {
                    selectedServerIp = server.IpAddress;
                    selectedServerPort = server.Port;
                }
            });
        }
    }

    // Called once per frame
    void Update()
    {
        // Toggle server selection panel with Start button or 'm' key
        if (OVRInput.GetDown(OVRInput.Button.Start) || Input.GetKeyDown("m"))
        {
            serverSelectionPanel.SetActive(!serverSelectionPanel.activeSelf);
        }
    }

    /// <summary>
    /// Initiates connection to the selected QTM server.
    /// </summary>
    public void ConnectOnClick()
    {
        StartCoroutine(ConnectToServer());
    }

    /// <summary>
    /// Coroutine: Connects to the selected QTM server and updates UI status.
    /// </summary>
    IEnumerator ConnectToServer()
    {
        RTClient.GetInstance().StartConnecting(selectedServerIp, selectedServerPort, false, true, false, false, true, true, false);
        statusText.text = "Connecting ...";
        statusText.color = Color.yellow;
        Debug.Log("Connecting ...");

        // Wait for connection to complete
        while (RTClient.GetInstance().ConnectionState == RTConnectionState.Connecting)
        {
            serverActionButtons[0].interactable = false;
            serverActionButtons[1].interactable = false;
            yield return null;
        }
        if (RTClient.GetInstance().ConnectionState == RTConnectionState.Connected)
        {
            serverActionButtons[0].interactable = false;
            serverActionButtons[1].interactable = false;
            serverActionButtons[2].interactable = true;
            serverSelectionPanel.SetActive(false);
            statusText.text = "Connected";
            statusText.color = Color.green;
            Debug.Log("Connected");
        }
        else
        {
            serverActionButtons[0].interactable = true;
            serverActionButtons[1].interactable = true;
            serverActionButtons[2].interactable = false;
            statusText.text = "Could not connect to this server";
            statusText.color = Color.red;
            Debug.Log("Could not connect to this server");
        }
    }

    /// <summary>
    /// Disconnects from the current QTM server and resets UI/Avatar.
    /// </summary>
    public void DisconnectOnClick()
    {
        serverActionButtons[0].interactable = true;
        serverActionButtons[1].interactable = true;
        serverActionButtons[2].interactable = false;
        statusText.text = "Waiting";
        statusText.color = Color.white;
        avatar.SetActive(false);
        RTClient.GetInstance().Disconnect();
    }
}
