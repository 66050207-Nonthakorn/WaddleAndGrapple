# Waddle and Grapple

> **Final Assignment — ComputerGame Course (Year 3, Semester 2)**

A 2D platformer built with **MonoGame** (.NET 9) featuring a penguin protagonist who can grapple, collect fish, fight enemies, and navigate trap-filled levels.

---

## Gameplay

- **Move** — Arrow keys / WASD
- **Jump** — Space (double-jump with power-up)
- **Grapple** — Pickaxe / hook mechanic to cross large gaps
- **Pause** — Escape
- **Quick restart** — R
- **Skip to next level** — F10 (debug)

---

## Features

- Tile-based levels loaded from **Tiled** map editor (`.tmj` / `.json`)
- Custom ECS-style engine (`Engine/`) with components, scenes, physics, and collision
- Enemy AI with patrol, chase, and attack states
- Traps: Saw, Laser, Spike
- Collectibles: Fish (coins), Double Jump, Speed Boost, Slow Time power-ups
- Checkpoint system with per-section respawn points
- Parallax background layers
- Cutscenes and level select screen
- Progression saved to disk (`Game/Save/Progression.json`)

---

## Project Structure

```
Engine/         Core engine: GameObject, Scene, Components, Managers, UI
Game/           Game-specific logic: Player, Enemy, Traps, Collectibles, Scenes
Content/        MonoGame content pipeline assets (textures, fonts, maps)
Assets/         Source art (Aseprite sprites, tilesets)
```

---

## Tech Stack

| | |
|---|---|
| Framework | [MonoGame](https://monogame.net/) 3.8.4 |
| Runtime | .NET 9 |
| UI | [Gum](https://github.com/vchelaru/Gum) (MonoGameGum) |
| Map editor | [Tiled](https://www.mapeditor.org/) |
| Language | C# 13 |

---

## Building & Running

```bash
dotnet run
```

Requires the MonoGame content pipeline (`mgcb`) to be installed. Content is built automatically on first run via `MonoGame.Content.Builder.Task`.