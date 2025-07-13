# HCM_EMG_XR

This project contains a Unity application for streaming electromyography (EMG) and motion-capture data in XR. It relies on the Qualisys Unity SDK for connecting to Qualisys Track Manager (QTM) and includes a small Node.js service for editing maximum voluntary isometric contraction (MVIC) values.

## Opening the Unity project
1. Install **Unity 2022.3.18f1** (or later).
2. Clone this repository.
3. Open the folder with Unity Hub and let Unity resolve the packages.

## MVIC server
The folder `MVIC_Server` provides an Express server that stores participant-specific MVIC values. Start it with:

```bash
cd MVIC_Server
npm install
npm start
```

The server listens on `http://localhost:8080` and serves a small web UI for managing the values in `values.json`.

## Qualisys streaming
The `Assets/Qualisys` directory ships with the Qualisys Unity SDK and several sample scenes. Refer to `Assets/README.md` for details on connecting Unity to QTM.