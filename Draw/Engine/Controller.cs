using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace CA.Engine
{
	public class Controller
	{
		public bool Continued { get; set; } = true;
		public bool Uploaded { get; set; } = false;
		public bool Drawing { get; set; } = false;
		public Bitmap Surface { get; set; }
		public Bitmap Blurred { get; set; }
		public Calc Calc { get; set; }
		public int MaxLength { get; set; }
		public int M_X { get; set; } = -1;
		public int M_Y { get; set; } = -1;

        public Controller (Bitmap bm, int w, int h, List<Gfx.Palette.GradientEditor.GradientStop> paletteMap)
		{
			Surface = bm;
			MaxLength = w * h;
			// Here it starts
		    System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
		        new Rectangle(0, 0, Surface.Width, Surface.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadWrite,
		        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		    // Create an array to store image data
			Space s = new Space(w, h);
		    // Use the Marshal class to copy image data
		    Marshal.Copy(lockData.Scan0, s.Cells, 0, MaxLength);
			Calc = new Calc(s, paletteMap);
			if(!Continued && !Uploaded) {
				Calc.Randomize();
			}
		    // Copy image data back
		    Marshal.Copy(s.Cells, 0, lockData.Scan0, MaxLength);
		    // Unlock image
		    Surface.UnlockBits(lockData);
			// Here it ends
		}

		public void LoadSurface (Bitmap bm) {
			Surface = bm;
			MaxLength = bm.Width * bm.Height;
			// Here it starts
		    System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
		        new Rectangle(0, 0, Surface.Width, Surface.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadWrite,
		        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		    // Create an array to store image data
			Space s = new Space(bm.Width, bm.Height);
		    // Use the Marshal class to copy image data
		    Marshal.Copy(lockData.Scan0, s.Cells, 0, MaxLength);
			Calc.SetSpace(s);
		    // Copy image data back
		    Marshal.Copy(s.Cells, 0, lockData.Scan0, MaxLength);
		    // Unlock image
		    Surface.UnlockBits(lockData);
			// Here it ends
		}
		
		public void AddCells(Point p) {
			int pos = Surface.Width * p.Y + p.X;
			if ((pos >= 0) && (pos < MaxLength)) {
				Graphics g = Graphics.FromImage(Surface);
				Pen pen = new Pen(Calc.CellAlive);
				if ((M_X >= 0) && (M_X < Surface.Width) && (M_Y >= 0) && (M_Y < Surface.Height)) {
					g.DrawLine(pen, M_X, M_Y, p.X, p.Y);
				}
				M_X = p.X;
				M_Y = p.Y;
		 	    System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
			        new Rectangle(0, 0, Surface.Width, Surface.Height),
			        System.Drawing.Imaging.ImageLockMode.ReadWrite,
			        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			    // Use the Marshal class to copy image data
			    Marshal.Copy(lockData.Scan0, Calc.Space.Cells, 0, MaxLength);
				Calc.Space.Cells[pos] = Calc.CellAlive.ToArgb();
				Calc.Space.Ages[pos] = 1;
			    // Copy image data back
			    Marshal.Copy(Calc.Space.Cells, 0, lockData.Scan0, MaxLength);
			    // Unlock image
			    Surface.UnlockBits(lockData);
				// Here it ends
				g.Dispose();
			}
		}
		
		public void HandleTick() {
			System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
		        new Rectangle(0, 0, Calc.Space.Width, Calc.Space.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadWrite,
		        System.Drawing.Imaging.PixelFormat.Format32bppArgb
			);
		    // Use the Marshal class to copy image data
		    Marshal.Copy(lockData.Scan0, Calc.Space.Cells, 0, MaxLength);
			
			Calc.Proceed();
			
		    // Copy image data back
		    Marshal.Copy(Calc.Space.Cells, 0, lockData.Scan0, MaxLength);

		    // Unlock image
		    Surface.UnlockBits(lockData);
		}

		public void Reset() {
			Graphics g = Graphics.FromImage(Surface);
			Brush b = new SolidBrush(Calc.CellDead);
			g.FillRectangle(b, new Rectangle(0, 0, Surface.Width, Surface.Height));
			System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
		        new Rectangle(0, 0, Calc.Space.Width, Calc.Space.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadWrite,
		        System.Drawing.Imaging.PixelFormat.Format32bppArgb
			);
		    // Use the Marshal class to copy image data
		    Marshal.Copy(lockData.Scan0, Calc.Space.Cells, 0, MaxLength);
		    // Copy image data back
		    Marshal.Copy(Calc.Space.Cells, 0, lockData.Scan0, MaxLength);
		    Surface.UnlockBits(lockData);
			g.Dispose();
			Calc.Reset();
		}
		
		public void Randomize() {
			Graphics g = Graphics.FromImage(Surface);
			Brush b = new SolidBrush(Calc.CellDead);
			g.FillRectangle(b, new Rectangle(0, 0, Surface.Width, Surface.Height));
			System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
		        new Rectangle(0, 0, Calc.Space.Width, Calc.Space.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadWrite,
		        System.Drawing.Imaging.PixelFormat.Format32bppArgb
			);
		    // Use the Marshal class to copy image data
		    Marshal.Copy(lockData.Scan0, Calc.Space.Cells, 0, MaxLength);
			Calc.Randomize();
		    // Copy image data back
		    Marshal.Copy(Calc.Space.Cells, 0, lockData.Scan0, MaxLength);
		    Surface.UnlockBits(lockData);
			g.Dispose();
			Calc.Reset();
		}

	}
}

