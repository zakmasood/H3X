## 🧱 MVP To-Do List for *Hexagonal*

### 🔹 1. Core Systems

#### ✅ Grid System

* [x] Implement pointy-top **hexagonal grid** with axial coordinates.
* [x] Support tile selection / highlighting via mouse.
* [x] Allow tile data storage (terrain, contents, building ID, etc.).

#### ✅ Camera & Input

* [x] Basic 3D isometric camera (pan, zoom, rotate optional).
* [x] Click-to-select tile/building.
* [ ] Hotkey or UI-based build mode toggling.

---

### 🔹 2. Resource System

#### ✅ Resource Definitions

* [ ] Define **Raw**, **Refined**, **Final**, **Transient**, **Trash** resources.
* [ ] Use ScriptableObjects or JSON/YAML for extensibility.

#### ✅ Resource Storage & Flow

* [ ] Tile-based inventory (how many units of a resource on each tile).
* [ ] Global resource tracking for UI/debugging.

---

### 🔹 3. Building System

#### ✅ Building Placement

* [ ] Allow placement of core buildings:

  * [ ] Drill
  * [ ] Furnace
  * [ ] Crafter
  * [ ] Roller
  * [ ] Storage
* [ ] Prevent invalid placements (e.g., on already-occupied tiles).

#### ✅ Building Execution Logic

* [ ] Drill produces `1u` of raw resource per second.
* [ ] Furnace consumes raw + fuel to produce refined resource.
* [ ] Crafter combines resources into final products.
* [ ] Roller refines ingots into wire or sheets.

---

### 🔹 4. Automation & Timing

* [ ] Add basic **tick-based execution loop** (e.g., 1-second steps).
* [ ] Buildings auto-execute if conditions are met (inputs + space for output).
* [ ] Add queue/logic system to simulate conveyors later (optional for MVP).

---

### 🔹 5. UI

* [ ] Show selected tile data (resource, building, contents).
* [ ] Global UI: total resources owned, tick count, FPS, etc.
* [ ] Building menu (basic UI to place buildings, view recipes).

---

### 🔹 6. Visuals & Feedback

* [ ] Basic visual for each building (colored cubes or simple models).
* [ ] Animated drilling/smelting indicators (e.g., particle/smoke).
* [ ] Resource transfer effect (optional).

---

### 🔹 7. Save/Load (Basic)

* [ ] Save hex grid + building/resource state to JSON.
* [ ] Load and restore the world from save.

---

### 🔹 8. Win/Goal Mechanic (Optional MVP Polish)

* [ ] Add simple **quest goal**: e.g., produce 10 copper wires.
* [ ] Display goal progress in UI.

---

## 🧩 Bonus/Stretch for MVP Polishing

* [ ] Add Trash/Waste mechanics (slag, toxic waste).
* [ ] Add a basic quest chain (via dialogue or popups).
* [ ] Integrate sound effects (drill hum, smelting sizzle, etc.).
* [ ] Add a tech unlock system (basic progression wall).

---

### 📦 Final MVP Deliverables

* Fully working hex grid with resource and building systems
* Functional automation (drill → furnace → roller → final product)
* Visual + UI feedback for player actions
* One or two goal-oriented tasks to complete
