using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MicroLibrary;
using System.IO;

namespace CHIP_8
{
	public partial class MainForm : Form
	{
		const int SCREEN_WIDTH = 64;
		const int SCREEN_HEIGHT = 32;

		//the emulator
		Chip8 chip8;
		int modifier = 10;

		//use a system timer to get double precision interval for the system timer
		MicroTimer hiResTimer = new MicroTimer((long)(1000000.0f / 60.0f)); //60 Hz

		Graphics g;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			//TODO: setup the graphics
			setupGraphics();

			//TODO: setup the input ???
			setupInput();

			//kick off the emulatore
			//need to be able to run many of these thread safe
			chip8 = new Chip8();
			chip8.Initialize();
			chip8.LoadProgram("C:\\Users\\Patrick\\Desktop\\myChip8-bin-src\\pong2.c8"); //TODO: defer this

			hiResTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(hiResTick);
			hiResTimer.Enabled = true;
		}

		void hiResTick(object sender, MicroTimerEventArgs timerEventArgs)
		{
			chip8.EmulateCycle();

			if (chip8.drawFlag)
				drawGraphics();
		}

		private void setupInput()
		{            
			//nothing needed here right now since key bindings are done in the designer
			return;
		}

        private void setupGraphics()
		{
			//set the display width
			graphicsDevice.Width = SCREEN_WIDTH * modifier;
			graphicsDevice.Height = SCREEN_HEIGHT * modifier;            

			//assign the device an image to display
			if (graphicsDevice.Image == null)
			{
				graphicsDevice.Image = new Bitmap(graphicsDevice.Width,
						graphicsDevice.Height);
			}

			g = Graphics.FromImage(graphicsDevice.Image);
			g.Clear(Color.Black);
		}

        private void drawGraphics()
        {
            //chip8.DebugRender();
            updateGraphics();
        }
        
		private void updateGraphics()
		{
            g.Clear(Color.Black);          
            //g.DrawImage((Image)convertChip8GxfToDrawableBitmap(chip8.gfx), new Rectangle(0, 0, graphicsDevice.Width, graphicsDevice.Height));
            g.DrawImage((Image)convertChip8GxfToDrawableBitmap(chip8.gfx), new Rectangle(0, 0, 320,160));

			graphicsDevice.Invalidate();
		}

        private Bitmap convertChip8GxfToDrawableBitmap(byte[] byteArray)
        {
            //first create a bitmap at normal screen size

            //Convert the gfx array into a bitmap
            Bitmap bmp = new Bitmap(SCREEN_WIDTH, SCREEN_HEIGHT);

            for (int y = 0; y < SCREEN_HEIGHT; ++y)
            {
                for (int x = 0; x < SCREEN_WIDTH; ++x)
                {
                    if (chip8.gfx[(y * SCREEN_WIDTH) + x] == 0)
                        bmp.SetPixel(x, y, Color.Black);
                    else
                        bmp.SetPixel(x, y, Color.White);
                }
            }

            return bmp;
        }

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				this.Close();

			if (e.KeyCode == Keys.D1)       chip8.key[0x1] = 1;
			else if (e.KeyCode == Keys.D2)  chip8.key[0x2] = 1;
			else if (e.KeyCode == Keys.D3)  chip8.key[0x3] = 1;
			else if (e.KeyCode == Keys.D4)  chip8.key[0xC] = 1;

			else if (e.KeyCode == Keys.G) chip8.key[0x4] = 1;
			else if (e.KeyCode == Keys.W) chip8.key[0x5] = 1;
			else if (e.KeyCode == Keys.E) chip8.key[0x6] = 1;
			else if (e.KeyCode == Keys.R) chip8.key[0xD] = 1;

			else if (e.KeyCode == Keys.A) chip8.key[0x7] = 1;
			else if (e.KeyCode == Keys.S) chip8.key[0x8] = 1;
			else if (e.KeyCode == Keys.D) chip8.key[0x9] = 1;
			else if (e.KeyCode == Keys.F) chip8.key[0xE] = 1;

			else if (e.KeyCode == Keys.Z) chip8.key[0xA] = 1;
			else if (e.KeyCode == Keys.X) chip8.key[0x0] = 1;
			else if (e.KeyCode == Keys.C) chip8.key[0xB] = 1;
			else if (e.KeyCode == Keys.V) chip8.key[0xF] = 1;
		}

		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.D1)       chip8.key[0x1] = 0;
			else if (e.KeyCode == Keys.D2) chip8.key[0x2] = 0;
			else if (e.KeyCode == Keys.D3) chip8.key[0x3] = 0;
			else if (e.KeyCode == Keys.D4) chip8.key[0xC] = 0;

			else if (e.KeyCode == Keys.G) chip8.key[0x4] = 0;
			else if (e.KeyCode == Keys.W) chip8.key[0x5] = 0;
			else if (e.KeyCode == Keys.E) chip8.key[0x6] = 0;
			else if (e.KeyCode == Keys.R) chip8.key[0xD] = 0;

			else if (e.KeyCode == Keys.A) chip8.key[0x7] = 0;
			else if (e.KeyCode == Keys.S) chip8.key[0x8] = 0;
			else if (e.KeyCode == Keys.D) chip8.key[0x9] = 0;
			else if (e.KeyCode == Keys.F) chip8.key[0xE] = 0;

			else if (e.KeyCode == Keys.Z) chip8.key[0xA] = 0;
			else if (e.KeyCode == Keys.X) chip8.key[0x0] = 0;
			else if (e.KeyCode == Keys.C) chip8.key[0xB] = 0;
			else if (e.KeyCode == Keys.V) chip8.key[0xF] = 0;
		}

		private void openProgramToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//load program via file dialog
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.FileOk += new CancelEventHandler(ofd_FileOk);
			ofd.ShowDialog();
		}

		void ofd_FileOk(object sender, CancelEventArgs e)
		{
			//a file has been selected, so load rom into the emulator
			OpenFileDialog ofd = (OpenFileDialog)sender;
			chip8.LoadProgram(ofd.FileName);
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}        
	} 
}
