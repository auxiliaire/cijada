using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CA.Gfx.Palette.GradientEditor
{
	[Serializable()]
	public class GradientStop : System.Windows.Forms.Control, IComparable<GradientStop>, ISerializable
	{
		public Color color;
		public int position;

		private const int STOP_WIDTH = 7;
		private const int STOP_HEIGHT = 10;

		public GradientStop ()
		{
			_init();
		}

		public GradientStop (Color c)
		{
			color = c;
			_init();
		}

		public GradientStop (Color c, int p)
		{
			color = c;
			position = p;
			_init ();
		}

		protected void _init()
		{
			Width = STOP_WIDTH;
			Height = STOP_HEIGHT;
		}

		protected override void OnPaint (System.Windows.Forms.PaintEventArgs pe)
		{
			base.OnPaint (pe);

			Pen pen = new Pen(Color.Black);
			int bLeft = 0;
			int bRight = STOP_WIDTH - 1;
			int center = (int)(STOP_WIDTH / 2);
			Point[] points = new Point[6];
			points[0] = new Point(center + 1, STOP_HEIGHT);
			points[1] = new Point(bLeft, STOP_HEIGHT - 4);
			points[2] = new Point(bLeft, 0);
			points[3] = new Point(bRight, 0);
			points[4] = new Point(bRight, STOP_HEIGHT - 4);
			points[5] = new Point(center, STOP_HEIGHT - 1);
			//Console.WriteLine("Color: {0}", color);
			//Console.WriteLine("0: {0}, 1: {1}, 2: {2}, 3: {3}, 4: {4}, 5: {5}", points[0], points[1], points[2], points[3], points[4], points[5]);
			Brush c = new SolidBrush(_c(color));
			pe.Graphics.FillPolygon(c, points);
			pe.Graphics.DrawPolygon(pen, points);
			c.Dispose();
			pen.Dispose();
		}

		protected override void OnClick (EventArgs e)
		{
			base.OnClick (e);

			if (Parent.Controls.IndexOf(this) != 0) {
				this.BringToFront();
			} else {
				this.SendToBack();
			}
			Invalidate();
		}

		protected override void OnMouseDoubleClick (System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDoubleClick (e);
			ColorDialog dialog = new ColorDialog();
			if (dialog.ShowDialog(this) == DialogResult.OK) {
				color = dialog.Color;
				Invalidate();
			}
		}

		public void setPosition (int p)
		{
			if (p > 100) {
				Console.WriteLine("Notice: ColorStop.position cannot exceed a value of 100");
				p = 100;
			}
			position = p;
		}
		
		public int getPosition ()
		{
			return position;
		}
		
		public void setColor (Color c)
		{
			color = c;
		}

		public Color getColor ()
		{
			return color;
		}

		public int CompareTo(GradientStop gs)
		{
			return position.CompareTo(gs.position);
		}

		// Converts all colors to grayscale if Disabled
		protected Color _c(Color c)
		{
			if (Enabled) return c;
			// Else (Disabled):
			int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
			return Color.FromArgb(luma, luma, luma);
		}

		// SERIALIZATION RELATED:

		public GradientStop(SerializationInfo info, StreamingContext ctxt)
		{
			this.color = (Color)info.GetValue("Color", typeof(Color));
			this.position = (int)info.GetValue("Position", typeof(int));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Color", this.color);
			info.AddValue("Position", this.position);
		}
	}
}

