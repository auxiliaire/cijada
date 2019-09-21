using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CA.Gfx.Palette.GradientEditor
{
	public class GradientEditor : Control
	{
		private List<GradientStop> map;

		private const int EDIT_HEIGHT = 10;
		private const int STOP_WIDTH = 7;
		private const int WIDTH = 100;
		private const int HEIGHT = 25;
		private int gradientWidth;
		private int gradientHeight;
		private int gradientX;
		private int gradientY;

		// GradientStop moving related vars:
		private Point _point_begin;
		private Control _control_moving = null;
		private int x_offset_on_client_control;
		private ToolTip geditTT = null;

		public GradientEditor ()
		{
			map = new List<GradientStop>();
			_init();
		}

		public GradientEditor (List<GradientStop> m)
		{
			map = m;
			_initStops();
			_init();
		}

		protected void _initStops()
		{
			foreach (GradientStop gs in map) {
				Control ctrl = (Control)gs;
				_addStopEventHandlers(ctrl);
				AddStop(gs, PosToX(gs.Position));
			}
		}

		protected void _addStopEventHandlers(Control ctrl) {
			ctrl.MouseDown += new MouseEventHandler(StopMouseDownHandler);
			ctrl.MouseMove += new MouseEventHandler(StopMouseMoveHandler);
			ctrl.MouseUp   += new MouseEventHandler(StopMouseUpHandler);
		}

		protected void _removeStopEventHandlers(Control ctrl) {
			ctrl.MouseDown -= new MouseEventHandler(StopMouseDownHandler);
			ctrl.MouseMove -= new MouseEventHandler(StopMouseMoveHandler);
			ctrl.MouseUp   -= new MouseEventHandler(StopMouseUpHandler);
		}

		protected void _init()
		{
			Width = WIDTH;
			Height = HEIGHT;
            DoubleBuffered = true;
			if (geditTT == null) {
				geditTT = new ToolTip();
	        	geditTT.SetToolTip(this, "Click to add color, Shift + Click to remove one.");
			}
		}

		protected void _initSize()
		{
			gradientWidth = Width - STOP_WIDTH;
			gradientHeight = ClientRectangle.Height - EDIT_HEIGHT - 2;
	        gradientX = ClientRectangle.X + (STOP_WIDTH / 2);
			gradientY = ClientRectangle.Y + EDIT_HEIGHT + 1;

			int stop = 0;
			int p = 0;
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			GradientStop first = null;
			GradientStop last = null;
			if (map.Count > 0) {
				foreach (GradientStop gs in map) {
					p++;
					if (p == 1) {
						if (first == null) {
							first = gs;
							last = null;
						} else if (last != null) {
							first = last;
							last = gs;
							p = 0;
							stop++;
						}
					} else { // 2
						last = gs;
						p = 0;
						stop++;
					}
					if ((first != null) && (last != null)) {
						p1.X = PosToX(first.GetPosition());
						p1.Y = gradientY + 1;
						p2.X = PosToX(last.GetPosition());
						p2.Y = gradientY + 1;
						AddStop(first, p1.X);
					} // end if
				} // end foreach
				AddStop(last, p2.X);
			}
		}

		protected override void OnSizeChanged (EventArgs e)
		{
			base.OnSizeChanged (e);
			_initSize();
		}

		// Old OnPaint method
        protected void _OnPaint(PaintEventArgs pe)
	    {
	        // Calling the base class OnPaint
	        base.OnPaint(pe);

			int stop = 0;
			int p = 0;
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			GradientStop first = null;
			GradientStop last = null;
			Pen pen = new Pen(Color.Black);
			foreach (GradientStop gs in map) {
				p++;
				if (p == 1) {
					if (first == null) {
						first = gs;
						last = null;
					} else if (last != null) {
						first = last;
						last = gs;
						p = 0;
						stop++;
					}
				} else { // 2
					last = gs;
					p = 0;
					stop++;
				}
				if ((first != null) && (last != null)) {
					p1.X = gradientX + 1 + PosToX(first.GetPosition());
					p1.Y = gradientY + 1;
					p2.X = gradientX + 1 + PosToX(last.GetPosition());
					p2.Y = gradientY + 1;
                    Console.WriteLine("p1.X: {0}, p2.X: {1}", p1.X, p2.X);
                    Brush b;
                    if (p1.X != p2.X)
                    {
                        b = new System.Drawing.Drawing2D.LinearGradientBrush(
                            p1, p2, _c(first.Color), _c(last.Color));
                    }
                    else
                    {
                        b = new SolidBrush(last.Color);
                    }
			        pe.Graphics.FillRectangle (
						b,
						p1.X,
						gradientY,
						p2.X - p1.X,
						gradientHeight
					);
			        b.Dispose();
				} // end if
			} // end foreach
			pe.Graphics.DrawRectangle(
				pen,
				gradientX,
				gradientY,
				gradientWidth,
				gradientHeight
			);
			pen.Dispose();
	    }

		protected System.Drawing.Drawing2D.LinearGradientBrush GetGradientBrush() {
            int p = 0;
            Point p1 = new Point(gradientX + 1, gradientY + 1);
            Point p2 = new Point(gradientX + 1 + gradientWidth, gradientY + 1 + gradientHeight);
            Color[] clrArray = new Color[map.Count];
            float[] posArray = new float[map.Count];
            foreach (GradientStop gs in map)
            {
                clrArray[p] = _c(gs.Color);
                posArray[p] = (0.01f * gs.GetPosition());
                p++;
            } // end foreach
            System.Drawing.Drawing2D.LinearGradientBrush b = new System.Drawing.Drawing2D.LinearGradientBrush(p1, p2, Color.Blue, Color.Red);

            System.Drawing.Drawing2D.ColorBlend colorBlend = new System.Drawing.Drawing2D.ColorBlend
            {
                Colors = clrArray,
                Positions = posArray
            };
            // Set InterpolationColors property
            b.InterpolationColors = colorBlend;
			return b;
		}

        protected override void OnPaint(PaintEventArgs e)
        {
            // Calling the base class OnPaint
            base.OnPaint(e);

			System.Drawing.Drawing2D.LinearGradientBrush b = GetGradientBrush();

            e.Graphics.FillRectangle(b, gradientX + 1, gradientY + 1, gradientWidth, gradientHeight);

			Pen pen = new Pen(Color.Black);
            e.Graphics.DrawRectangle(
                pen,
                gradientX,
                gradientY,
                gradientWidth,
                gradientHeight
            );
            pen.Dispose();
			b.Dispose();
        }

		protected override void OnMouseDown (MouseEventArgs e)
		{
			base.OnMouseUp (e);

			if (e.X > gradientX && e.X < (gradientX + gradientWidth)
			    && e.Y > gradientY && e.Y < (gradientY + gradientHeight)
			) {
				Bitmap bmp = new Bitmap(this.Width, this.Height);
				Graphics g = Graphics.FromImage(bmp);

				System.Drawing.Drawing2D.LinearGradientBrush b = GetGradientBrush();

	            g.FillRectangle(b, gradientX + 1, 0, gradientWidth, Height);
				b.Dispose();

				Color clr = bmp.GetPixel(e.X, e.Y);

				bmp.Dispose();
				g.Dispose();

				GradientStop grs = new GradientStop(clr, XToPos(e.X - (STOP_WIDTH / 2)));
				SetColorStop(grs);
				map.Sort();
				MapChanged(this, new EventArgs());
				Invalidate();
			}
		}

        protected virtual void StopMouseDownHandler(object sender, MouseEventArgs e)
		{
            int x;
            int y;
			int pos = ((GradientStop)sender).Position;
		    if (
                // while left button pressed (could define other keys)
                e.Button == MouseButtons.Left
                &&
                // ignore containing (parent) control
                ! "CA.Gfx.Palette.GradientEditor.GradientEditor"
                .Equals(sender.GetType().ToString())
				&& pos > 0
				&& pos < 100
            ) {
				// Checking Shift + click
				if (Control.ModifierKeys == Keys.Shift) {
					int cPos;
		            foreach (Control ctrl in Controls)
		            {
						cPos = ((GradientStop)ctrl).Position;
						if (pos == cPos) {
							// Removing GradientStop
							RemoveColorStop((GradientStop) ctrl);
							_removeStopEventHandlers(ctrl);
							Controls.Remove(ctrl);
							ctrl.Dispose();
							Invalidate();
							return;
						}
					}
				}
				// else Normal Click
                x_offset_on_client_control = e.X;

                x = x_offset_on_client_control + ((Control)sender).Location.X;
                y = e.Y + ((Control)sender).Location.Y;
                _point_begin = new Point(x, y);
            }

            foreach (Control ctrl in Controls)
            {
				pos = ((GradientStop)ctrl).Position;
                if (ctrl.Bounds.Contains(_point_begin)
					&& pos > 0
					&& pos < 100
				) {
                    _control_moving = ctrl;
                }
            }
		}

		protected virtual void StopMouseMoveHandler (object sender, MouseEventArgs e)
		{
			int pos = ((GradientStop)sender).Position;
        	if (
                ! "CA.Gfx.Palette.GradientEditor.GradientEditor".Equals(sender.GetType().ToString())
                && _control_moving != null
                && e.Button == MouseButtons.Left
				&& pos > 0
				&& pos < 100
            ) {
                Point pt = Cursor.Position;
                int l =
                                (this.PointToClient(pt)).X
                                -
                                x_offset_on_client_control
                                ;
				if (l > (gradientX - (_control_moving.Width / 2) - 1)
					&& l < (gradientX - (_control_moving.Width / 2) + 1 + gradientWidth)
				) {
					pos = XToPos(l);
					if (pos > 0 && pos < 100) {
						_control_moving.Left = l;
						GradientStop gs = (GradientStop)_control_moving;
						gs.Position = pos;
						map.Sort();
						MapChanged(this, new EventArgs());
						Invalidate();
					}
				}
            }
		}

		protected virtual void StopMouseUpHandler (object sender, MouseEventArgs e)
		{
			int pos = ((GradientStop)sender).Position;
            if (
                _control_moving != null
                && e.Button == MouseButtons.Left
				&& pos > 0
				&& pos < 100
            ) {
                int x;
                int y;

                x = ((Control)sender).Location.X;
                y = ((Control)sender).Location.Y;

                Point _point_end = new Point(x, y);
				if (_point_end.X > (gradientX - (_control_moving.Width / 2) - 1)
					&& _point_end.X < (gradientX - (_control_moving.Width / 2) + 1 + gradientWidth)
				) {
					pos = XToPos(_point_end.X);
					if (pos > 0 && pos < 100) {
						_control_moving.Left = _point_end.X;
						GradientStop gs = (GradientStop)_control_moving;
						gs.Position = pos;
						map.Sort();
						MapChanged(this, new EventArgs());
						Invalidate();
					}
				}
                _control_moving = null;
            }
		}

		protected GradientStop FindStop(Color c, int p)
		{
			foreach (Control ctrl in Controls) {
				GradientStop gs = (GradientStop)ctrl;
				if (gs.Color == c && gs.Position == p) {
					return gs;
				}
			}
			return null;
		}

		protected void AddStop(GradientStop gs, int x) {
			gs.Left = x;
			gs.Top = 1;
			gs.Parent = this;
		}

		protected int XToPos(int x) {
			double ratio = 100.0f / gradientWidth;
			int start = x - gradientX + (STOP_WIDTH / 2);
			int pos = (int)(start * ratio);
			return pos;
		}

		protected int PosToX(int p) {
			double ratio = 100.0f / gradientWidth;
			int start = (int)(p / ratio);
			int x = start + gradientX - (STOP_WIDTH / 2);
			return x;
		}

		public void SetColorStop(GradientStop s) {
			map.Add(s);
			_addStopEventHandlers((Control)s);
			AddStop(s, PosToX(s.Position));
		}

		public void RemoveColorStop(GradientStop gs) {
			map.Remove(gs);
		}

		public List<GradientStop> GetMap() {
			return map;
		}

		public void SetMap(List<GradientStop> m) {
			// Removing all previously defined stops:
			foreach (Control ctrl in Controls) {
				RemoveColorStop((GradientStop) ctrl);
				_removeStopEventHandlers(ctrl);
				ctrl.Dispose();
			}
			Controls.Clear();
			map = new List<GradientStop>();
			// Setting new stops:
			foreach (GradientStop gs in m) {
				SetColorStop(new GradientStop(gs.Color, gs.Position));
			}
			MapChanged(this, new EventArgs());
			Invalidate();
		}

		public delegate void MapChangedEventHandler (object sender, EventArgs e);

		public event MapChangedEventHandler MapChanged;

		// Converts all colors to grayscale if Disabled
		protected Color _c(Color c)
		{
			if (Enabled) return c;
			// Else (Disabled):
			int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
			return Color.FromArgb(luma, luma, luma);
		}

	}
}

