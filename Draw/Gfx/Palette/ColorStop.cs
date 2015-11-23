// DEPRECATED

using System;
using System.Drawing;

namespace CA.Gfx.Palette
{
	public class ColorStop
	{
		private byte position;
		private Color colour;
		
		public ColorStop ()
		{
			position = 0;
			colour = Color.Black;
		}
		
		public ColorStop (byte p)
		{
			setPosition(p);
			colour = Color.Black;
		}
		
		public ColorStop (byte p, Color c)
		{
			setPosition(p);
			colour = c;
		}
		
		public void setPosition (byte p)
		{
			if (p > 100) {
				Console.WriteLine("Notice: ColorStop.position cannot exceed a value of 100");
				p = 100;
			}
			position = p;
		}
		
		public byte getPosition ()
		{
			return position;
		}
		
		public void setColor (Color c)
		{
			colour = c;
		}
		
		public Color getColor ()
		{
			return colour;
		}
	}
}

