namespace CA.Engine
{
    public class Space
    {
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public int[] Cells { get; set; }
        public int[] Ages { get; set; }

        public Space()
        {
        }

        public Space(int w, int h)
        {
            Width = w;
            Height = h;
            Reset();
        }

        public void Reset()
        {
            int size = Width * Height;
            Cells = new int[size];
            Ages = new int[size];
        }
    }
}

