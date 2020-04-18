using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Windows.Forms;

namespace CA.Gfx.Palette.GradientEditor
{
    [Serializable]
    public sealed class GradientStop : Control, IComparable<GradientStop>, ISerializable, IEquatable<GradientStop>
    {
        public Color Color { get; set; }
        public int Position { get; set; }

        private const int STOP_WIDTH = 7;
        private const int STOP_HEIGHT = 10;

        public GradientStop()
        {
            _init();
        }

        public GradientStop(Color c)
        {
            Color = c;
            _init();
        }

        public GradientStop(Color c, int p)
        {
            Color = c;
            Position = p;
            _init();
        }

        private void _init()
        {
            Width = STOP_WIDTH;
            Height = STOP_HEIGHT;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Pen pen = new Pen(Color.Black);
            int bLeft = 0;
            int bRight = STOP_WIDTH - 1;
            int center = STOP_WIDTH / 2;
            Point[] points = new Point[6];
            points[0] = new Point(center + 1, STOP_HEIGHT);
            points[1] = new Point(bLeft, STOP_HEIGHT - 4);
            points[2] = new Point(bLeft, 0);
            points[3] = new Point(bRight, 0);
            points[4] = new Point(bRight, STOP_HEIGHT - 4);
            points[5] = new Point(center, STOP_HEIGHT - 1);
            Brush c = new SolidBrush(_c(Color));
            e.Graphics.FillPolygon(c, points);
            e.Graphics.DrawPolygon(pen, points);
            c.Dispose();
            pen.Dispose();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (Parent.Controls.IndexOf(this) != 0)
            {
                this.BringToFront();
            }
            else
            {
                this.SendToBack();
            }
            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                Color = dialog.Color;
                Invalidate();
            }
        }

        public void SetPosition(int p)
        {
            if (p > 100)
            {
                Console.WriteLine("Warning: ColorStop.position cannot exceed a value of 100");
                p = 100;
            }
            Position = p;
        }

        public int GetPosition()
        {
            return Position;
        }

        public static bool operator >(GradientStop operand1, GradientStop operand2)
        {
            return operand1.CompareTo(operand2) == 1;
        }

        public static bool operator <(GradientStop operand1, GradientStop operand2)
        {
            return operand1.CompareTo(operand2) == -1;
        }

        public static bool operator >=(GradientStop operand1, GradientStop operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        public static bool operator <=(GradientStop operand1, GradientStop operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }

        public static bool operator ==(GradientStop operand1, GradientStop operand2)
        {
            if (operand1 is null)
            {
                return operand2 is null;
            }
            return operand1.CompareTo(operand2) == 0;
        }

        public static bool operator !=(GradientStop operand1, GradientStop operand2)
        {
            if (operand1 is null)
            {
                return !(operand2 is null);
            }
            return operand1.CompareTo(operand2) != 0;
        }

        public int CompareTo(GradientStop other)
        {
            if (other == null) return 1;

            return Position.CompareTo(other.Position);
        }

        // Converts all colors to grayscale if Disabled
        private Color _c(Color c)
        {
            if (Enabled) return c;
            // Else (Disabled):
            int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
            return Color.FromArgb(luma, luma, luma);
        }

        // SERIALIZATION RELATED:

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public GradientStop(SerializationInfo info, StreamingContext ctxt)
        {
            Color = (Color)info.GetValue("Color", typeof(Color));
            Position = (int)info.GetValue("Position", typeof(int));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("Color", Color, typeof(Color));
            info.AddValue("Position", Position, typeof(int));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GradientStop);
        }

        public bool Equals(GradientStop other)
        {
            return other != null &&
                   EqualityComparer<Color>.Default.Equals(Color, other.Color) &&
                   Position == other.Position;
        }

        public override int GetHashCode()
        {
            var hashCode = -1695330334;
            hashCode = hashCode * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(Color);
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            return hashCode;
        }
    }
}

