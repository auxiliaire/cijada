using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using CA.Gfx;
using CA.Gfx.Palette;

namespace CA.Engine
{
	public class Controller
	{
		public bool continued = true; //false;
		public bool uploaded = false;
		public bool drawing = false;
		public Bitmap Surface;
		public Bitmap Blurred;
		public Calc Calc;
		public int maxLength;
		public int mX = -1;
		public int mY = -1;

		//public Controller (Bitmap bm, int w, int h, Map paletteMap)
		public Controller (Bitmap bm, int w, int h, List<Gfx.Palette.GradientEditor.GradientStop> paletteMap)
		{
			Surface = bm;
			maxLength = w * h;
			// Here it starts
		    System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
		        new Rectangle(0, 0, Surface.Width, Surface.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadWrite,
		        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		    // Create an array to store image data
			Space s = new Space(w, h);
		    // Use the Marshal class to copy image data
		    Marshal.Copy(lockData.Scan0, s.Cells, 0, maxLength);
			Calc = new Calc(s, paletteMap);
			if(!continued && !uploaded) {
				Calc.Randomize();
			}
		    // Copy image data back
		    Marshal.Copy(s.Cells, 0, lockData.Scan0, maxLength);
		    // Unlock image
		    Surface.UnlockBits(lockData);
			// Here it ends
		}

		public void loadSurface (Bitmap bm) {
			Surface = bm;
			maxLength = bm.Width * bm.Height;
			// Here it starts
		    System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
		        new Rectangle(0, 0, Surface.Width, Surface.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadWrite,
		        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		    // Create an array to store image data
			Space s = new Space(bm.Width, bm.Height);
		    // Use the Marshal class to copy image data
		    Marshal.Copy(lockData.Scan0, s.Cells, 0, maxLength);
			Calc.setSpace(s);
		    // Copy image data back
		    Marshal.Copy(s.Cells, 0, lockData.Scan0, maxLength);
		    // Unlock image
		    Surface.UnlockBits(lockData);
			// Here it ends
		}
		
		public void addCells(Point p) {
			int pos = Surface.Width * p.Y + p.X;
			if ((pos >= 0) && (pos < maxLength)) {
				Graphics g = Graphics.FromImage(Surface);
				Pen pen = new Pen(Calc.cAlive); //Color.FromArgb(cDead));
				if ((mX >= 0) && (mX < Surface.Width) && (mY >= 0) && (mY < Surface.Height)) {
					g.DrawLine(pen, mX, mY, p.X, p.Y);
				}
				mX = p.X;
				mY = p.Y;
		 	    System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
			        new Rectangle(0, 0, Surface.Width, Surface.Height),
			        System.Drawing.Imaging.ImageLockMode.ReadWrite,
			        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			    // Use the Marshal class to copy image data
			    Marshal.Copy(lockData.Scan0, Calc.Space.Cells, 0, maxLength);
				Calc.Space.Cells[pos] = Calc.cAlive.ToArgb(); //~0xF1DFB8 | (int)Filter.alpha;
				Calc.Space.Ages[pos] = 1;
			    // Copy image data back
			    Marshal.Copy(Calc.Space.Cells, 0, lockData.Scan0, maxLength);
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
		    Marshal.Copy(lockData.Scan0, Calc.Space.Cells, 0, maxLength);
			
			Calc.proceed();
			
		    // Copy image data back
		    Marshal.Copy(Calc.Space.Cells, 0, lockData.Scan0, maxLength);
			//blurred = Blur((Bitmap)Surface.Clone(), 1);
		    // Unlock image
		    Surface.UnlockBits(lockData);
		}

		public void Reset() {
			Graphics g = Graphics.FromImage(Surface);
			Brush b = new SolidBrush(Calc.cDead); //Color.FromArgb(cDead));
			g.FillRectangle(b, new Rectangle(0, 0, Surface.Width, Surface.Height));
			System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
		        new Rectangle(0, 0, Calc.Space.Width, Calc.Space.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadWrite,
		        System.Drawing.Imaging.PixelFormat.Format32bppArgb
			);
		    // Use the Marshal class to copy image data
		    Marshal.Copy(lockData.Scan0, Calc.Space.Cells, 0, maxLength);
		    // Copy image data back
		    Marshal.Copy(Calc.Space.Cells, 0, lockData.Scan0, maxLength);
		    Surface.UnlockBits(lockData);
			g.Dispose();
			Calc.Reset();
		}
		
		public void Randomize() {
			Graphics g = Graphics.FromImage(Surface);
			Brush b = new SolidBrush(Calc.cDead); //Color.FromArgb(cDead));
			g.FillRectangle(b, new Rectangle(0, 0, Surface.Width, Surface.Height));
			System.Drawing.Imaging.BitmapData lockData = Surface.LockBits(
		        new Rectangle(0, 0, Calc.Space.Width, Calc.Space.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadWrite,
		        System.Drawing.Imaging.PixelFormat.Format32bppArgb
			);
		    // Use the Marshal class to copy image data
		    Marshal.Copy(lockData.Scan0, Calc.Space.Cells, 0, maxLength);
			Calc.Randomize();
		    // Copy image data back
		    Marshal.Copy(Calc.Space.Cells, 0, lockData.Scan0, maxLength);
		    Surface.UnlockBits(lockData);
			g.Dispose();
			Calc.Reset();
		}

	}
}

