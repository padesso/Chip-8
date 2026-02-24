using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MicroLibrary;
using Microsoft.Win32;

namespace CHIP_8
{
	public partial class MainWindow : Window
	{
		const int SCREEN_WIDTH = 64;
		const int SCREEN_HEIGHT = 32;

		//the emulator
		Chip8 chip8;

		//use a system timer to get double precision interval for the system timer
		MicroTimer hiResTimer = new MicroTimer((long)(1000000.0f / 60.0f)); //60 Hz

		WriteableBitmap bitmap;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//setup the graphics
			setupGraphics();

			//setup the input
			setupInput();

			//kick off the emulator
			chip8 = new Chip8();
			chip8.Initialize();

			hiResTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(hiResTick);
			hiResTimer.Enabled = true;
		}

		void hiResTick(object sender, MicroTimerEventArgs timerEventArgs)
		{
			chip8.EmulateCycle();

			if (chip8.drawFlag)
				Dispatcher.Invoke(drawGraphics);
		}

		private void setupInput()
		{
			//nothing needed here right now since key bindings are done in XAML
		}

		private void setupGraphics()
		{
			bitmap = new WriteableBitmap(SCREEN_WIDTH, SCREEN_HEIGHT, 96, 96, PixelFormats.Bgr32, null);
			graphicsDevice.Source = bitmap;
		}

		private void drawGraphics()
		{
			updateGraphics();
		}

		private void updateGraphics()
		{
			bitmap.Lock();
			unsafe
			{
				IntPtr backBuffer = bitmap.BackBuffer;
				int stride = bitmap.BackBufferStride;

				for (int y = 0; y < SCREEN_HEIGHT; y++)
				{
					for (int x = 0; x < SCREEN_WIDTH; x++)
					{
						int offset = y * stride + x * 4;
						int color = chip8.gfx[(y * SCREEN_WIDTH) + x] == 0 ? 0x00000000 : 0x00FFFFFF;
						*((int*)(backBuffer + offset)) = color;
					}
				}
			}
			bitmap.AddDirtyRect(new Int32Rect(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT));
			bitmap.Unlock();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (chip8 == null) return;

			if (e.Key == Key.Escape)
				this.Close();

			if (e.Key == Key.D1)      chip8.key[0x1] = 1;
			else if (e.Key == Key.D2) chip8.key[0x2] = 1;
			else if (e.Key == Key.D3) chip8.key[0x3] = 1;
			else if (e.Key == Key.D4) chip8.key[0xC] = 1;

			else if (e.Key == Key.Q) chip8.key[0x4] = 1;
			else if (e.Key == Key.W) chip8.key[0x5] = 1;
			else if (e.Key == Key.E) chip8.key[0x6] = 1;
			else if (e.Key == Key.R) chip8.key[0xD] = 1;

			else if (e.Key == Key.A) chip8.key[0x7] = 1;
			else if (e.Key == Key.S) chip8.key[0x8] = 1;
			else if (e.Key == Key.D) chip8.key[0x9] = 1;
			else if (e.Key == Key.F) chip8.key[0xE] = 1;

			else if (e.Key == Key.Z) chip8.key[0xA] = 1;
			else if (e.Key == Key.X) chip8.key[0x0] = 1;
			else if (e.Key == Key.C) chip8.key[0xB] = 1;
			else if (e.Key == Key.V) chip8.key[0xF] = 1;
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if (chip8 == null) return;

			if (e.Key == Key.D1)      chip8.key[0x1] = 0;
			else if (e.Key == Key.D2) chip8.key[0x2] = 0;
			else if (e.Key == Key.D3) chip8.key[0x3] = 0;
			else if (e.Key == Key.D4) chip8.key[0xC] = 0;

			else if (e.Key == Key.Q) chip8.key[0x4] = 0;
			else if (e.Key == Key.W) chip8.key[0x5] = 0;
			else if (e.Key == Key.E) chip8.key[0x6] = 0;
			else if (e.Key == Key.R) chip8.key[0xD] = 0;

			else if (e.Key == Key.A) chip8.key[0x7] = 0;
			else if (e.Key == Key.S) chip8.key[0x8] = 0;
			else if (e.Key == Key.D) chip8.key[0x9] = 0;
			else if (e.Key == Key.F) chip8.key[0xE] = 0;

			else if (e.Key == Key.Z) chip8.key[0xA] = 0;
			else if (e.Key == Key.X) chip8.key[0x0] = 0;
			else if (e.Key == Key.C) chip8.key[0xB] = 0;
			else if (e.Key == Key.V) chip8.key[0xF] = 0;
		}

		private void OpenProgram_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == true)
			{
				chip8.LoadProgram(ofd.FileName);
			}
		}

		private void Quit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
