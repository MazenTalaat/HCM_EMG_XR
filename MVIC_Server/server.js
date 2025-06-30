// ----- server.js -----
import express from "express";
import fs from "fs";

const PORT = 8080;
const SAVE_FILE = "values.json";

// 1) Load or initialise the 12-float array
let values = Array(12).fill(0.0);
if (fs.existsSync(SAVE_FILE)) {
  try { values = JSON.parse(fs.readFileSync(SAVE_FILE)); }
  catch { console.warn("Save file corrupted – starting with zeros."); }
}

// 2) Minimal Express app
const app = express();
app.use(express.json());                 // parse JSON bodies
app.use(express.static("public"));       // serve front-end files

// 3) REST endpoints
app.get("/api/values", (req, res) => res.json(values));

app.put("/api/values/:idx", (req, res) => {
  const i = Number(req.params.idx);
  const v = Number(req.body.value);
  if (!Number.isFinite(v) || i < 0 || i >= values.length) {
    return res.status(400).json({error: "Bad index or value"});
  }
  values[i] = v;
  fs.writeFileSync(SAVE_FILE, JSON.stringify(values)); // simple persistence
  res.json({ok: true, values});
});

app.post("/api/reset", (req, res) => {
  values.fill(1.0);
  fs.writeFileSync(SAVE_FILE, JSON.stringify(values));
  res.json({ ok: true, values });
});

// 4) Start
app.listen(PORT, () => console.log(`⇢ http://localhost:${PORT}`));
