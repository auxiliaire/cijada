using System.Windows.Forms;

namespace CA.Engine
{
    public class CellRelationCheckBox : CheckBox
    {
        public CellRelationCheckBox()
        {
        }

        public CellRelationCheckBox(int r, int c)
        {
            Row = r;
            Col = c;
        }

        public int Row { get; set; }

        public int Col { get; set; }
    }
}

