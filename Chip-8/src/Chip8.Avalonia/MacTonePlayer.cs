using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Chip8.Avalonia;

internal sealed class MacTonePlayer : IDisposable
{
    private const int FrequencyHz = 740;
    private const int SampleRateHz = 8000;
    private const int DurationSeconds = 60;
    private const short Amplitude = 6000;

    private readonly object _sync = new();
    private Process? _playbackProcess;

    public void SetEnabled(bool enabled)
    {
        lock (_sync)
        {
            if (enabled)
            {
                StartPlayback_NoLock();
                return;
            }

            StopPlayback_NoLock();
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            StopPlayback_NoLock();
        }
    }

    private void StartPlayback_NoLock()
    {
        if (!OperatingSystem.IsMacOS())
        {
            Console.Write('\a');
            return;
        }

        if (_playbackProcess is { HasExited: false })
        {
            return;
        }

        StopPlayback_NoLock();

        string tonePath = EnsureToneFile();
        ProcessStartInfo startInfo = new ProcessStartInfo("afplay")
        {
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add(tonePath);

        try
        {
            Process process = new Process { StartInfo = startInfo };
            if (process.Start())
            {
                _playbackProcess = process;
            }
            else
            {
                process.Dispose();
            }
        }
        catch
        {
            Console.Write('\a');
        }
    }

    private void StopPlayback_NoLock()
    {
        if (_playbackProcess is null)
        {
            return;
        }

        try
        {
            if (!_playbackProcess.HasExited)
            {
                _playbackProcess.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            // Best effort stop.
        }

        _playbackProcess.Dispose();
        _playbackProcess = null;
    }

    private static string EnsureToneFile()
    {
        string tonePath = Path.Combine(Path.GetTempPath(), "chip8-tone-740hz.wav");
        if (File.Exists(tonePath))
        {
            return tonePath;
        }

        WriteSquareWaveWav(tonePath);
        return tonePath;
    }

    private static void WriteSquareWaveWav(string path)
    {
        int sampleCount = SampleRateHz * DurationSeconds;
        int dataSizeBytes = sampleCount * sizeof(short);
        int periodSamples = Math.Max(2, SampleRateHz / FrequencyHz);
        int halfPeriod = periodSamples / 2;

        using FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        using BinaryWriter writer = new(stream, Encoding.ASCII);

        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(36 + dataSizeBytes);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));

        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1); // PCM
        writer.Write((short)1); // Mono
        writer.Write(SampleRateHz);
        writer.Write(SampleRateHz * sizeof(short));
        writer.Write((short)sizeof(short));
        writer.Write((short)16);

        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(dataSizeBytes);

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = (i % periodSamples) < halfPeriod ? Amplitude : (short)-Amplitude;
            writer.Write(sample);
        }
    }
}
