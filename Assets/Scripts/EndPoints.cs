using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores network endpoint addresses for MVIC and QTM servers used in EMG data acquisition.
/// </summary>
public class EndPoints : MonoBehaviour
{
    // Base URL for the MVIC server (used for saving and retrieving MVIC values)
    public static string MVICServerUrl = "http://192.168.0.115:8080";
    //public static string MVICServerUrl = "http://127.0.0.1:8080";

    // IP address for the Qualisys Track Manager (QTM) server (used for real-time EMG streaming)
    public static string QTMServerIp = "192.168.0.100";
    //public static string QTMServerIp = "127.0.0.1"; 

    
}
