using System;
using System.Windows.Forms;

namespace CA.Engine
{
	public class CellRelationCheckBox : CheckBox
	{
		private int row;
		private int col;

		public CellRelationCheckBox ()
		{
		}

		public CellRelationCheckBox (int r, int c)
		{
			row = r;
			col = c;
		}

		public int Row {
			get {
				return row;
			}
			set {
				row = value;
			}
		}

		public int Col {
			get {
				return col;
			}
			set {
				col = value;
			}
		}
	}
}

