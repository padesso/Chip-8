using System;
using System.Threading;
using System.Threading.Tasks;

namespace CHIP_8
{
	sealed class Chip8TonePlayer : IDisposable
	{
		const int FrequencyHz = 740;
		const int PulseDurationMs = 16;

		readonly object sync = new object();
		CancellationTokenSource playbackCts;

		public void SetEnabled(bool enabled)
		{
			lock (sync)
			{
				if (enabled)
				{
					if (playbackCts != null)
						return;

					playbackCts = new CancellationTokenSource();
					CancellationToken token = playbackCts.Token;
					_ = Task.Run(() => BeepLoop(token), token);
					return;
				}

				StopPlayback_NoLock();
			}
		}

		public void Dispose()
		{
			lock (sync)
			{
				StopPlayback_NoLock();
			}
		}

		void StopPlayback_NoLock()
		{
			if (playbackCts == null)
				return;

			playbackCts.Cancel();
			playbackCts.Dispose();
			playbackCts = null;
		}

		static void BeepLoop(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					Console.Beep(FrequencyHz, PulseDurationMs);
				}
				catch
				{
					return;
				}
			}
		}
	}
}
