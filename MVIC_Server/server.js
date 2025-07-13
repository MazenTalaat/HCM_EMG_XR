// ----- MVIC Server -----
import express from "express";
import fs from "fs";

const PORT = 8080;
const STORE_FILENAME = "values.json";
const NUM_MUSCLES = 6;
const defaultMuscleArray = () => Array(NUM_MUSCLES).fill(1.0);

/* ------------------- Persistent Data Store ------------------------- */
// Structure: { participantId: [6 muscle values] }
let participantStore = {};
if (fs.existsSync(STORE_FILENAME)) {
  try {
    participantStore = JSON.parse(fs.readFileSync(STORE_FILENAME));
  } catch {
    console.warn("Corrupted store file – starting with empty store.");
  }
}
// Returns array for participant, creates default if missing
const getMuscleArray = participantId =>
  (participantStore[participantId] ??= defaultMuscleArray());
// Save store to disk
const saveStore = () =>
  fs.writeFileSync(STORE_FILENAME, JSON.stringify(participantStore, null, 2));

/* ------------------- Current Participant Pointer ------------------- */
let currentParticipantId = "001"; // Default participant on server start

/* ------------------- Express App Setup ----------------------------- */
const app = express();
app.use(express.json());
app.use(express.static("public"));

/* ------------------- REST API: Per-Participant --------------------- */
// Get muscle values for a specific participant
app.get("/api/values/:participantId", (req, res) =>
  res.json(getMuscleArray(req.params.participantId))
);
// Update a single muscle value for a participant
app.put("/api/values/:participantId/:muscleIdx", (req, res) => {
  const { participantId, muscleIdx } = req.params;
  const idx = +muscleIdx, value = +req.body.value;
  if (!Number.isFinite(value) || idx < 0 || idx >= NUM_MUSCLES)
    return res.status(400).end();
  getMuscleArray(participantId)[idx] = value;
  saveStore();
  res.json({ ok: true });
});
// Reset all muscle values for a participant
app.post("/api/reset/:participantId", (req, res) => {
  participantStore[req.params.participantId] = defaultMuscleArray();
  saveStore();
  res.json({ ok: true });
});

/* ------------------- Current Participant Endpoints ----------------- */
// Set the current participant
app.post("/api/current/:participantId", (req, res) => {
  currentParticipantId = req.params.participantId.trim();
  getMuscleArray(currentParticipantId);
  saveStore();
  res.json({ ok: true, id: currentParticipantId });
});
// Get the current participant
app.get("/api/current", (req, res) =>
  res.json({ id: currentParticipantId })
);

/* ------------------- Unity-Fixed Endpoints ------------------------- */
// Get muscle values for the current participant
app.get("/api/values", (req, res) =>
  res.json(getMuscleArray(currentParticipantId))
);
// Update a single muscle value for the current participant
app.put("/api/values/:muscleIdx", (req, res) => {
  const idx = +req.params.muscleIdx, value = +req.body.value;
  if (!Number.isFinite(value) || idx < 0 || idx >= NUM_MUSCLES)
    return res.status(400).end();
  getMuscleArray(currentParticipantId)[idx] = value;
  saveStore();
  res.json({ ok: true });
});
// Reset all muscle values for the current participant
app.post("/api/reset", (req, res) => {
  participantStore[currentParticipantId] = defaultMuscleArray();
  saveStore();
  res.json({ ok: true });
});

/* ------------------- Start Server ---------------------------------- */
app.listen(PORT, () =>
  console.log(`⇢ Server running at http://localhost:${PORT}`)
);
