# MVIC Node Server Documentation
This folder contains a small Express application that stores maximum voluntary isometric contraction (MVIC) values for different participants.

## Requirements
* Node.js v24.1.0 or newer

## Installation
```bash
npm init -y
npm i express
```

## Running
```bash
node server.js
```

To build exe:
```bash
npm install -g bun
bun build ./server.js --compile --outfile mvic_server
```

The server listens on **http://localhost:8080** and serves a simple web interface from the `public` directory. Values are persisted to `values.json` and are accessed by the Unity project through a REST API.
