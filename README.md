# Chip-8 Emulator

A [CHIP-8](https://en.wikipedia.org/wiki/CHIP-8) emulator in C# targeting .NET 9, organized as a shared core plus desktop UI frontends:

- `Chip8.Wpf` for Windows
- `Chip8.Avalonia` for macOS (and other cross-platform desktop targets)

## Project Layout

```text
Chip-8/
├── CHIP-8.sln            # Full solution (Core + WPF + Avalonia)
├── CHIP-8.Mac.sln        # macOS-friendly solution (Core + Avalonia)
├── ROMS/                 # Included ROMs
└── src/
    ├── Chip8.Core/       # Emulator CPU/memory/timers
    ├── Chip8.Wpf/        # Windows WPF frontend
    └── Chip8.Avalonia/   # Cross-platform desktop frontend
```

## Features

- Full CHIP-8 instruction set emulation
- 64x32 monochrome display
- 60 Hz timer updates with 10 CPU cycles per frame (~600 Hz effective clock)
- Shared emulator core reused by both desktop UIs
- 23 classic ROMs included in `ROMS/`

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

On macOS with Homebrew:

```bash
brew install --cask dotnet-sdk
```

## Repository Hygiene

This repository tracks source code, solution/project files, and ROM assets only.
Local SDK/runtime/cache folders are intentionally ignored and should never be committed:

- `.dotnet/`
- `.dotnet-home/`
- `.local/`
- `.nuget/`
- `Library/`

## Build and Run on macOS (Avalonia)

```bash
cd Chip-8
dotnet --info
dotnet restore CHIP-8.Mac.sln
dotnet build CHIP-8.Mac.sln
dotnet run --project src/Chip8.Avalonia
```

The app attempts to load `ROMS/PONG` automatically when started from the `Chip-8` directory.
Use **File -> Open Program** to load another ROM.

## Build and Run on Windows (WPF)

```bash
cd Chip-8
dotnet restore CHIP-8.sln
dotnet build CHIP-8.sln
dotnet run --project src/Chip8.Wpf
```

## Keyboard Mapping

The emulator uses the standard CHIP-8 keypad mapping:

| CHIP-8 Key | Keyboard Key |
|:----------:|:------------:|
| `1`        | `1`          |
| `2`        | `2`          |
| `3`        | `3`          |
| `C`        | `4`          |
| `4`        | `Q`          |
| `5`        | `W`          |
| `6`        | `E`          |
| `D`        | `R`          |
| `7`        | `A`          |
| `8`        | `S`          |
| `9`        | `D`          |
| `E`        | `F`          |
| `A`        | `Z`          |
| `0`        | `X`          |
| `B`        | `C`          |
| `F`        | `V`          |

Press `Escape` to quit.

## Included ROMs

| ROM        | Description              |
|------------|--------------------------|
| 15PUZZLE   | Sliding 15-puzzle game   |
| BLINKY     | Pac-Man clone            |
| BLITZ      | Bombing run game         |
| BRIX       | Breakout clone           |
| CONNECT4   | Connect Four game        |
| GUESS      | Number guessing game     |
| HIDDEN     | Card matching game       |
| INVADERS   | Space Invaders clone     |
| KALEID     | Kaleidoscope demo        |
| MAZE       | Maze generator           |
| MERLIN     | Simon Says clone         |
| MISSILE    | Missile defense game     |
| PONG       | Pong (one player)        |
| PONG2      | Pong (two player)        |
| PUZZLE     | Sliding puzzle           |
| SYZYGY     | Puzzle game              |
| TANK       | Tank game                |
| TETRIS     | Tetris clone             |
| TICTAC     | Tic-Tac-Toe              |
| UFO        | UFO shooting game        |
| VBRIX      | Vertical Breakout clone  |
| VERS       | Worm game                |
| WIPEOFF    | Paddle game              |
