using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Chip8Core = Chip8.Core.Chip8;

namespace Chip8.Avalonia;

public partial class MainWindow : Window
{
    private const int CyclesPerFrame = 10;

    private readonly Chip8Core _chip8 = new();
    private readonly DispatcherTimer _frameTimer = new();

    private static readonly Dictionary<Key, int> KeyMap = new()
    {
        [Key.D1] = 0x1,
        [Key.D2] = 0x2,
        [Key.D3] = 0x3,
        [Key.D4] = 0xC,
        [Key.Q] = 0x4,
        [Key.W] = 0x5,
        [Key.E] = 0x6,
        [Key.R] = 0xD,
        [Key.A] = 0x7,
        [Key.S] = 0x8,
        [Key.D] = 0x9,
        [Key.F] = 0xE,
        [Key.Z] = 0xA,
        [Key.X] = 0x0,
        [Key.C] = 0xB,
        [Key.V] = 0xF,
    };

    public MainWindow()
    {
        InitializeComponent();

        _chip8.Initialize();
        Display.FrameBuffer = _chip8.gfx;

        _chip8.BeepRequested += OnBeepRequested;

        _frameTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
        _frameTimer.Tick += FrameTimer_Tick;

        Opened += MainWindow_Opened;
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Opened(object? sender, EventArgs e)
    {
        _frameTimer.Start();

        string? defaultRom = TryFindDefaultRom();
        if (defaultRom is not null)
        {
            _chip8.LoadProgram(defaultRom);
        }
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _frameTimer.Stop();
        _chip8.BeepRequested -= OnBeepRequested;
    }

    private void FrameTimer_Tick(object? sender, EventArgs e)
    {
        for (int i = 0; i < CyclesPerFrame; i++)
        {
            _chip8.EmulateCycle();
        }

        _chip8.UpdateTimers();

        if (_chip8.drawFlag)
        {
            _chip8.drawFlag = false;
            Display.InvalidateVisual();
        }
    }

    private async void OpenProgram_Click(object? sender, RoutedEventArgs e)
    {
        if (StorageProvider is null)
        {
            return;
        }

        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open CHIP-8 ROM",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("CHIP-8 ROM")
                {
                    Patterns = new[] { "*.c8", "*.ch8", "*.rom", "*.*" }
                }
            }
        });

        if (files.Count == 0)
        {
            return;
        }

        string? localPath = files[0].TryGetLocalPath();
        if (string.IsNullOrWhiteSpace(localPath))
        {
            return;
        }

        _chip8.LoadProgram(localPath);
        Display.InvalidateVisual();
    }

    private void Quit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (KeyMap.TryGetValue(e.Key, out int key))
        {
            _chip8.key[key] = 1;
        }
    }

    private void Window_KeyUp(object? sender, KeyEventArgs e)
    {
        if (KeyMap.TryGetValue(e.Key, out int key))
        {
            _chip8.key[key] = 0;
        }
    }

    private static string? TryFindDefaultRom()
    {
        string currentDirPath = Path.Combine(Environment.CurrentDirectory, "ROMS", "PONG");
        if (File.Exists(currentDirPath))
        {
            return currentDirPath;
        }

        DirectoryInfo? dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            string candidate = Path.Combine(dir.FullName, "ROMS", "PONG");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        return null;
    }

    private static void OnBeepRequested()
    {
        Console.Write('\a');
    }
}
