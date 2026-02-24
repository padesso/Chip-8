# Chip-8 Emulator

A [CHIP-8](https://en.wikipedia.org/wiki/CHIP-8) emulator built with C# and WPF targeting .NET Framework 4.

## Features

- Full CHIP-8 instruction set emulation
- 64×32 pixel display rendered at 640×320 (10× scaling) using WPF `WriteableBitmap`
- 60 Hz timer updates with 10 CPU cycles per frame (~600 Hz effective clock)
- High-resolution timer (`MicroTimer`) for accurate frame pacing
- Load any CHIP-8 ROM via a file-open dialog
- 23 classic ROMs included in the `ROMS/` directory

## Getting Started

### Prerequisites

- Windows
- [Visual Studio](https://visualstudio.microsoft.com/) with .NET Framework 4 workload installed

### Build & Run

1. Open `Chip-8/CHIP-8.sln` in Visual Studio.
2. Build the solution (`Ctrl+Shift+B`).
3. Run the project (`F5` or `Ctrl+F5`).
4. Use **File → Open Program** to load a ROM from the `ROMS/` directory.

## Keyboard Mapping

The original CHIP-8 system used a 16-key hexadecimal keypad. The emulator maps it to the left side of a standard keyboard:

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

Press **Escape** to exit the emulator.

## Included ROMs

The `ROMS/` directory contains the following classic CHIP-8 programs:

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

## Technical Overview

| Component      | Details                                                   |
|----------------|-----------------------------------------------------------|
| Memory         | 4 KB (4096 bytes); programs loaded at `0x200`            |
| Display        | 64×32 monochrome pixels, XOR sprite drawing              |
| Registers      | 16 × 8-bit general purpose (`V0`–`VF`), 16-bit `I`      |
| Stack          | 16 levels                                                 |
| Timers         | Delay timer and sound timer, decremented at 60 Hz        |
| Input          | 16-key hexadecimal keypad                                 |
| Font           | Built-in 4×5 pixel sprites for characters `0`–`F`       |

