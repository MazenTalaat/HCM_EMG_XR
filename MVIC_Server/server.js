// ----- server.js -----
import express from "express";
import fs from "fs";

const PORT = 8080;
const SAVE_FILE = "values.json";
const N_MUSCLES = 6;
const DEF_ARRAY = () => Array(N_MUSCLES).fill(1.0);

/* ------------- data store ------------------------------------------ */
let store = {};               // { id: [6 floats] }
if (fs.existsSync(SAVE_FILE)) {
  try { store = JSON.parse(fs.readFileSync(SAVE_FILE)); }
  catch { console.warn("Bad store file – starting fresh."); }
}
const getArray = id => (store[id] ??= DEF_ARRAY());
const save = () => fs.writeFileSync(SAVE_FILE, JSON.stringify(store, null, 2));

/* ------------- NEW: “current participant” pointer ------------------ */
let currentId = "001";        // default when server boots

/* ------------- express app ----------------------------------------- */
const app = express();
app.use(express.json());
app.use(express.static("public"));

/* -------- REST API (ID-specific, used by HTML page) ---------------- */
app.get("/api/values/:id", (req, res) => res.json(getArray(req.params.id)));
app.put("/api/values/:id/:idx", (req, res) => {
  const { id, idx } = req.params;
  const i = +idx, v = +req.body.value;
  if (!Number.isFinite(v) || i < 0 || i >= N_MUSCLES) return res.status(400).end();
  getArray(id)[i] = v; save(); res.json({ ok: true });
});
app.post("/api/reset/:id", (req, res) => {
  store[req.params.id] = DEF_ARRAY(); save(); res.json({ ok: true });
});

/* -------- NEW: set / read the “currentId” -------------------------- */
app.post("/api/current/:id", (req, res) => {           // HTML → server
  currentId = req.params.id.trim(); getArray(currentId); save();
  res.json({ ok: true, id: currentId });
});
app.get("/api/current", (req, res) => res.json({ id: currentId }));

/* -------- FIXED endpoints for Unity -------------------------------- */
app.get("/api/values", (req, res) => res.json(getArray(currentId)));
app.put("/api/values/:idx", (req, res) => {
  const i = +req.params.idx, v = +req.body.value;
  if (!Number.isFinite(v) || i < 0 || i >= N_MUSCLES) return res.status(400).end();
  getArray(currentId)[i] = v; save(); res.json({ ok: true });
});
app.post("/api/reset", (req, res) => {        // optional
  store[currentId] = DEF_ARRAY(); save(); res.json({ ok: true });
});

/* -------- start ---------------------------------------------------- */
app.listen(PORT, () => console.log(`⇢ http://localhost:${PORT}`));
