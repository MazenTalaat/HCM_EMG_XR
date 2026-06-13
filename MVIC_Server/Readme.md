# MVIC Node Server

This project is a lightweight Express.js server for managing and editing Maximum Voluntary Isometric Contraction (MVIC) values for shoulder muscles, per participant. It provides a REST API for Unity integration and a web interface for manual editing.

---

## Features

- **Persistent MVIC Data:** Stores per-participant muscle values in `values.json`.
- **REST API:** Endpoints for reading, updating, and resetting muscle values.
- **Web Interface:** Edit values and switch participants via a browser.
- **Unity Integration:** Fixed endpoints for Unity to read/write current participant data.
- **Offline Support:** Uses local Tailwind CSS and static assets.

---

## Requirements

- Node.js v24.1.0 or newer

---

## Installation

```bash
npm init -y
npm install express
```

---

## Running the Server

```bash
node server.js
```

The server will start at **http://localhost:8080** and serve the web interface from the `public` directory.

---

## Building a Standalone Executable (Optional)

```bash
npm install -g bun
bun build ./server.js --compile --outfile mvic_server
```

---

## API Overview

### Per-Participant Endpoints

- `GET /api/values/:participantId`  
  Returns array of 6 muscle values for the given participant.

- `PUT /api/values/:participantId/:muscleIdx`  
  Updates a single muscle value for the participant.

- `POST /api/reset/:participantId`  
  Resets all muscle values for the participant to default (1.0).

### Current Participant Endpoints

- `POST /api/current/:participantId`  
  Sets the current participant.

- `GET /api/current`  
  Gets the current participant ID.

### Unity-Fixed Endpoints

- `GET /api/values`  
  Gets muscle values for the current participant.

- `PUT /api/values/:muscleIdx`  
  Updates a single muscle value for the current participant.

- `POST /api/reset`  
  Resets all muscle values for the current participant.

---

## Web Interface

Open [http://localhost:8080](http://localhost:8080) in your browser.  
- Select or enter a participant ID.
- Edit muscle values for left/right shoulder.
- Save or reset values.
- View anatomy image for reference.

---

## Data Persistence

All changes are saved to `values.json` in the project root.  
If the file is missing or corrupted, a new one is created automatically.

---

## File Structure

- `server.js` – Main Express server and API logic
- `public/` – Static web assets (HTML, CSS, images)
- `values.json` – Persistent data store (auto-generated)
- `Readme.md` – Documentation
