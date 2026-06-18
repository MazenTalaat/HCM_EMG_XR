# A Protocol for Assessing Virtual and Mixed Reality Environments with Real-Time Electromyographic (EMG) Biofeedback During Upper Limb Physical Exercises

This Unity project enables real-time streaming and visualization of electromyography (EMG) and motion-capture data in extended reality (XR) environments. It integrates with the Qualisys Track Manager (QTM) via the Qualisys Unity SDK and provides a Node.js backend for managing Maximum Voluntary Isometric Contraction (MVIC) values.

---

## Features

- **Real-time EMG streaming** from QTM using the Qualisys Unity SDK.
- **MVIC value management** via a Node.js Express server.
- **XR visualization** of muscle activity, including:
  - Dynamic avatar muscle coloring based on EMG data.
  - EMG progress bars and graphs.
  - UI panels for MVIC values and server connection status.
- **Server discovery and connection** for QTM servers.
- **MVIC value persistence** and editing via a web UI.

---

## Getting Started

### Prerequisites

- **Unity 2022.3.18f1** or newer.
- **Node.js** (for MVIC server).
- Qualisys Track Manager (QTM) running and streaming analog EMG data.

### Setup Instructions

#### 1. Clone the Repository

```bash
git clone [<your-repo-url>](https://github.com/MazenTalaat/HCM_EMG_XR.git)
cd HCM_EMG_XR
```

#### 2. Open the Project in Unity

- Launch Unity Hub.
- Add the cloned folder as a new project.
- Open the project and allow Unity to resolve all packages.

#### 3. Start the MVIC Server

The `MVIC_Server` folder contains a Node.js Express server for storing and editing MVIC values.

```bash
cd MVIC_Server
cat README.md
```
And follow the instructions.

- The server runs at `http://localhost:8080` (or your configured IP).
- Access the web UI to manage MVIC values stored in `values.json`.

#### 4. Configure QTM Streaming

- Ensure QTM is running and streaming analog EMG channels.
- The Unity project connects to QTM using the IP specified in `Assets/Scripts/EndPoints.cs` (`QTMServerIp`).
- The Qualisys Unity SDK is included in `Assets/Qualisys`.

---

## Project Structure

- `Assets/Scripts/`  
  Main Unity scripts for EMG streaming, visualization, server communication, and UI control.
- `Assets/Qualisys/`  
  Qualisys Unity SDK and sample scenes.
- `MVIC_Server/`  
  Node.js Express server for MVIC value management.
- `Assets/README.md`  
  Additional documentation for Qualisys SDK integration.

---

## Usage

- **Connect to QTM:**  
  Use the server selection UI to discover and connect to available QTM servers.
- **Stream EMG Data:**  
  EMG data is sampled and visualized in real time on the avatar, progress bars, and graphs.
- **MVIC Tracking:**  
  Toggle MVIC tracking to update and persist maximum contraction values.
- **Edit MVIC Values:**  
  Use the MVIC server web UI to manually adjust values as needed.

---

## Troubleshooting

- Ensure QTM and the MVIC server are running and accessible from your Unity machine.
- Update IP addresses in `Assets/Scripts/EndPoints.cs` if running on a LAN.
- Check Unity console for connection or data warnings.

---

## Credits

- **Qualisys Unity SDK**  
  Used for real-time motion and EMG data streaming.
- **Node.js Express**  
  Used for MVIC value persistence and web UI.
  
