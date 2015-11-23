using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using CA.Gfx.Palette;
using CA.Win32;

namespace CA.Gfx.Palette.GradientEditor
{
	public class GradientEditor : System.Windows.Forms.Control
	{
		//public Map map;
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
		private Point _point_end;
		private Control _control_moving = null;
		private int x_offset_on_client_control;
		private int y_offset_on_client_control;
		private ToolTip geditTT = null;

		public GradientEditor ()
		{
			//map = new Map();
			map = new List<GradientStop>();
			_init();
		}

		//public GradientEditor (Map m)
		public GradientEditor (List<GradientStop> m)
		{
			map = m;
			_initStops();
			_init();
		}

		protected void _initStops()
		{
			//foreach (var pair in map.getMap()) {
			foreach (GradientStop gs in map) {
				Control ctrl = (Control)gs;
				_addStopEventHandlers(ctrl);
				addStop(gs, posToX(gs.position));
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
	        gradientX = ClientRectangle.X + (int)(STOP_WIDTH / 2);
			gradientY = ClientRectangle.Y + EDIT_HEIGHT + 1;

			//double ratio = 100.0f / gradientWidth;
			int stop = 0;
			int p = 0;
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			//Gfx.Palette.ColorStop first = null;
			//Gfx.Palette.ColorStop last = null;
			Gfx.Palette.GradientEditor.GradientStop first = null;
			Gfx.Palette.GradientEditor.GradientStop last = null;
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
						//int start = (int)(first.getPosition() / ratio);
						//int end = (int)(last.getPosition() / ratio);
						//int walk = end - start;
						//Console.WriteLine("Start: {0}", start);
						p1.X = posToX(first.getPosition()); //start + gradientX + 1;
						p1.Y = gradientY + 1;
						p2.X = posToX(last.getPosition()); // end + gradientX + 1;
						//Console.WriteLine("End: {0}", end);
						p2.Y = gradientY + 1;
						addStop(first, p1.X);
						//addStop(last, p2.X);
					} // end if
				} // end foreach
				addStop(last, p2.X);
			}
		}

		protected override void OnSizeChanged (EventArgs e)
		{
			base.OnSizeChanged (e);
			_initSize();
		}

		// Old OnPaint method
	    //protected override void OnPaint(PaintEventArgs pe)
        protected void _OnPaint(PaintEventArgs pe)
	    {
	        // Calling the base class OnPaint
	        base.OnPaint(pe);

			//gradientWidth = Width - STOP_WIDTH;
			//gradientHeight = ClientRectangle.Height - EDIT_HEIGHT - 2;
	        //gradientX = ClientRectangle.X + (int)(STOP_WIDTH / 2);
			//gradientY = ClientRectangle.Y + EDIT_HEIGHT + 1;

			//double ratio = 100.0f / gradientWidth;
			int stop = 0;
			int p = 0;
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			//Gfx.Palette.ColorStop first = null;
			//Gfx.Palette.ColorStop last = null;
			Gfx.Palette.GradientEditor.GradientStop first = null;
			Gfx.Palette.GradientEditor.GradientStop last = null;
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
					//int start = (int)(first.getPosition() / ratio);
					//int end = (int)(last.getPosition() / ratio);
					//Console.WriteLine("Start: {0}", start);
					p1.X = gradientX + 1 + posToX(first.getPosition()); // start + gradientX + 1;
					p1.Y = gradientY + 1;
					p2.X = gradientX + 1 + posToX(last.getPosition()); // end + gradientX + 1;
					//Console.WriteLine("End: {0}", end);
					p2.Y = gradientY + 1;
                    Console.WriteLine("p1.X: {0}, p2.X: {1}", p1.X, p2.X);
                    Brush b;
                    if (p1.X != p2.X)
                    {
                        b = new System.Drawing.Drawing2D.LinearGradientBrush(
                            p1, p2, _c(first.getColor()), _c(last.getColor()));
                    }
                    else
                    {
                        b = new SolidBrush(last.getColor());
                    }
					//GradientStop gs = new GradientStop(first.getColor());
					//gs.Left = p1.X - (int)(gs.Width / 2) - 1;
					//gs.Top = 1;
					//gs.Parent = this;
					//_addStop(first.getColor(), first.getPosition(), p1.X);
					//_addStop(first, p1.X);
					//Console.WriteLine("p1: {0}, p2: {1}, gx: {2}, gw: {3}", p1.X, p2.X, gradientX, gradientWidth);
			        pe.Graphics.FillRectangle (
						b,
						p1.X,
						gradientY,
						p2.X - p1.X,
						gradientHeight
					);
					//gs = new GradientStop(last.getColor());
					//gs.Left = p2.X - (int)(gs.Width / 2) - 1;
					//gs.Top = 1;
					//gs.Parent = this;
					//addStop(last.getColor(), last.getPosition(), p2.X);
					//addStop(last, p2.X);
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
			//pen = new Pen(Color.Green);
			//pe.Graphics.DrawRectangle(pen, ClientRectangle);
			pen.Dispose();
	    }

		protected System.Drawing.Drawing2D.LinearGradientBrush getGradientBrush() {
            int p = 0;
            Point p1 = new Point(gradientX + 1, gradientY + 1);
            Point p2 = new Point(gradientX + 1 + gradientWidth, gradientY + 1 + gradientHeight);
            Color[] clrArray = new Color[map.Count];
            float[] posArray = new float[map.Count];
            foreach (GradientStop gs in map)
            {
                clrArray[p] = _c(gs.getColor());
                posArray[p] = (float)(0.01f * gs.getPosition());
                p++;
            } // end foreach
            System.Drawing.Drawing2D.LinearGradientBrush b = new System.Drawing.Drawing2D.LinearGradientBrush(p1, p2, Color.Blue, Color.Red);

            System.Drawing.Drawing2D.ColorBlend colorBlend = new System.Drawing.Drawing2D.ColorBlend();
            colorBlend.Colors = clrArray;
            colorBlend.Positions = posArray;
            // Set InterpolationColors property
            b.InterpolationColors = colorBlend;
			return b;
		}

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calling the base class OnPaint
            base.OnPaint(pe);

            //gradientWidth = Width - STOP_WIDTH;
            //gradientHeight = ClientRectangle.Height - EDIT_HEIGHT - 2;
            //gradientX = ClientRectangle.X + (int)(STOP_WIDTH / 2);
            //gradientY = ClientRectangle.Y + EDIT_HEIGHT + 1;

            //double ratio = 100.0f / gradientWidth;
            //int stop = 0;

			System.Drawing.Drawing2D.LinearGradientBrush b = getGradientBrush();

            pe.Graphics.FillRectangle(b, gradientX + 1, gradientY + 1, gradientWidth, gradientHeight);

			Pen pen = new Pen(Color.Black);
            pe.Graphics.DrawRectangle(
                pen,
                gradientX,
                gradientY,
                gradientWidth,
                gradientHeight
            );
            //pen = new Pen(Color.Green);
            //pe.Graphics.DrawRectangle(pen, ClientRectangle);
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

				System.Drawing.Drawing2D.LinearGradientBrush b = getGradientBrush();

	            g.FillRectangle(b, gradientX + 1, 0, gradientWidth, Height);
				b.Dispose();

				Color clr = bmp.GetPixel(e.X, e.Y);

				bmp.Dispose();
				g.Dispose();

				GradientStop grs = new GradientStop(clr, xToPos(e.X - (int)(STOP_WIDTH / 2)));
				setColorStop(grs);
				map.Sort();
				MapChanged(this, new EventArgs());
				Invalidate();
			}
		}

        protected virtual void StopMouseDownHandler(object sender, MouseEventArgs e)
		{
            int x;
            int y;
			int pos = ((GradientStop)sender).position;
			//Console.WriteLine("ˇˇˇˇˇˇˇˇ pos: {0}", pos);
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
						cPos = ((GradientStop)ctrl).position;
						if (pos == cPos) {
							// Removing GradientStop
							removeColorStop((GradientStop) ctrl);
							_removeStopEventHandlers(ctrl);
							Controls.Remove(ctrl);
							ctrl.Dispose();
							Invalidate();
							return;
						}
					}
				}
				// else Normal Click
                Point pt = Cursor.Position;
                x_offset_on_client_control = e.X;
                y_offset_on_client_control = e.Y;

                x = x_offset_on_client_control + ((Control)sender).Location.X;
                y = y_offset_on_client_control + ((Control)sender).Location.Y;
                pt = new Point(x, y);

                _point_begin = pt;
            }

            foreach (Control ctrl in Controls)
            {
				pos = ((GradientStop)ctrl).position;
                if (ctrl.Bounds.Contains(_point_begin)
					&& pos > 0
					&& pos < 100
				) {
                    _control_moving = ctrl;
                }
            }
            return;
		}

		protected virtual void StopMouseMoveHandler (object sender, MouseEventArgs e)
		{
			int pos = ((GradientStop)sender).position;
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
				if (l > (gradientX - (int)(_control_moving.Width / 2) - 1)
					&& l < (gradientX - (int)(_control_moving.Width / 2) + 1 + gradientWidth)
				) {
					pos = xToPos(l);
					if (pos > 0 && pos < 100) {
						//Console.WriteLine("m.pos: {0}", pos);
						_control_moving.Left = l;
						GradientStop gs = (GradientStop)_control_moving;
						gs.position = pos;
						map.Sort();
						MapChanged(this, new EventArgs());
						Invalidate();
					}
				}
                //_control_moving.Top =
                //                (this.PointToClient(pt)).Y
                //                -
                //                y_offset_on_client_control
                //                ;
            }
		}

		protected virtual void StopMouseUpHandler (object sender, MouseEventArgs e)
		{
			int pos = ((GradientStop)sender).position;
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

                Point pt = new Point(x, y);
                _point_end = pt;
                //_control_moving.Location = _point_end;
				if (_point_end.X > (gradientX - (int)(_control_moving.Width / 2) - 1)
					&& _point_end.X < (gradientX - (int)(_control_moving.Width / 2) + 1 + gradientWidth)
				) {
					pos = xToPos(_point_end.X);
					if (pos > 0 && pos < 100) {
						//Console.WriteLine("^^^^^^^^ pos: {0}", pos);
						_control_moving.Left = _point_end.X;
						GradientStop gs = (GradientStop)_control_moving;
						gs.position = pos;
						map.Sort();
						MapChanged(this, new EventArgs());
						Invalidate();
					}
				}
                _control_moving = null;
            }
            return;
		}

		protected GradientStop findStop(Color c, int p)
		{
			foreach (Control ctrl in Controls) {
				GradientStop gs = (GradientStop)ctrl;
				if (gs.color == c && gs.position == p) {
					return gs;
				}
			}
			return null;
		}

		protected void addStop(GradientStop gs, int x) {
			gs.Left = x; // - (int)(gs.Width / 2) - 1;
			gs.Top = 1;
			gs.Parent = this;
		}

		protected int xToPos(int x) {
			double ratio = 100.0f / gradientWidth;
			//int start = x - gradientX - 1;
			int start = x - gradientX + (int)(STOP_WIDTH / 2);
			int pos = (int)(start * ratio);
			return pos;
		}

		protected int posToX(int p) {
			double ratio = 100.0f / gradientWidth;
			int start = (int)(p / ratio);
			//int x = start + gradientX + 1;
			int x = start + gradientX - (int)(STOP_WIDTH / 2);
			return x; //start;
		}

		public void setColorStop(GradientStop s) {
			map.Add(s);
			_addStopEventHandlers((Control)s);
			addStop(s, posToX(s.position));
		}

		public void removeColorStop(GradientStop gs) { //int order) {
			map.Remove(gs);
		}

		//public Dictionary<int, ColorStop> getMap() {
		public List<GradientStop> getMap() {
			return map;
		}

		public void setMap(List<GradientStop> m) {
			// Removing all previously defined stops:
			foreach (Control ctrl in Controls) {
				removeColorStop((GradientStop) ctrl);
				_removeStopEventHandlers(ctrl);
				//Controls.Remove(ctrl);
				ctrl.Dispose();
			}
			Controls.Clear();
			map = new List<GradientStop>();
			// Setting new stops:
			foreach (GradientStop gs in m) {
				//Console.WriteLine("color: {0}, pos: {1}", gs.color, gs.position);
				setColorStop(new GradientStop(gs.color, gs.position));
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

