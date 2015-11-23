using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using CA.Gfx;

namespace CA.Engine
{
	public class Calc : INotifyPropertyChanged
	{
		public Color cAlive;
		public Color cDead;
		public Color[] palette;
		public bool senescence = true;
		public bool GradientTest = false;
		public int ageToDie = 50;
		public Space Space;
		// integer representation of the colors
		private int ca;
		private int cd;
		//private Gfx.Palette.Map paletteMap;
		//private double step;
		public bool[,] sm = {{true, true, true}, {true, false, true}, {true, true, true}};
		//private int[] liv = {1, 3}; // {0,1,4..8}        {Conway: 3}
		//private int[] dth = {0, 2, 4, 5, 6, 7, 8}; // {0..2,4..8} {Conway: 0,1,4..8}
		public int[] liv = {1, 2, 4, 6, 7, 8};
		public int[] dth = {};
		public bool[,] rcb = {{true, true, true},
								{true, true, true},
								{true, true, true}};
		private uint generations = 0;
		public bool Overflow = false;

		public Calc (Space s)
		{
			Space = s;
			init();
		}
		
		public Calc (Space s, Color alive, Color dead)
		{
			Space = s;
			cAlive = alive;
			cDead = dead;
			init();
		}
		
		public Calc (Space s, Color[] p)
		{
			Space = s;
			palette = p;
			init();
		}
		
		//public Calc (Space s, Gfx.Palette.Map m)
		public Calc (Space s, List<Gfx.Palette.GradientEditor.GradientStop> m)
		{
			Space = s;
			initPalette(m);
			init();
		}

		public void setSpace (Space s) {
			Space = s;
			init();
		}

		private void initPalette(List<Gfx.Palette.GradientEditor.GradientStop> m) {
			palette = new Color[ageToDie];
			double ratio = 100.0f / ageToDie;
			//int stops = m.getMap().Count;
			//int stops = m.Count;
			int stop = 0;
			//int age = (int) ageToDie / (stops - 1);
			int p = 0;
			//Gfx.Palette.ColorStop first = null;
			//Gfx.Palette.ColorStop last = null;
			Gfx.Palette.GradientEditor.GradientStop first = null;
			Gfx.Palette.GradientEditor.GradientStop last = null;
			//foreach (var pair in m.getMap()) {
			foreach (Gfx.Palette.GradientEditor.GradientStop gs in m) {
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
					int start = (int)(first.getPosition() / ratio);
					int end = (int)(last.getPosition() / ratio);
					int i = start;
					int walk = end - start;
					foreach (Color c in ColorInterpolator.GetGradients(first.getColor(), last.getColor(), walk)) {
						palette[i] = c;
						i++;
					}
				}
			}
		}

		private void init() {
			cAlive = palette[0];
			cDead = Color.Black;
			generateIntColors();
			initializeSenescence();
		}
		
		private void generateIntColors() {
			// Colors to ints for faster calculation:
			ca = cAlive.ToArgb();
			cd = cDead.ToArgb();
		}
		
		protected void initializeSenescence() {
			// Create gradients for senescence
			if (senescence) {
				int newBorn = Color.White.ToArgb();
				int dead = Color.Black.ToArgb();
				int scale = newBorn - dead;
				//Console.WriteLine("newborn: {0}, dead: {1}, scale: {2}", newBorn, dead, scale);
				int year = scale / ageToDie;
				//Console.WriteLine("year: {0}", year);
				//int white = (int)Math.Abs((Color.White.ToArgb()) / year) +1;
				//int black = (int)Math.Abs((Color.Black.ToArgb()) / year) +1;
				//Console.WriteLine("white: {0}, black: {1}", white, black);
				//int yellow = (int)Math.Abs((Color.Yellow.ToArgb()) / year) +1;
				//int brown = (int)Math.Abs((Color.SaddleBrown.ToArgb()) / year) +1;
				//Console.WriteLine("yellow: {0}, brown: {1}", yellow, brown);
				int pos = 0;
				for(int  a = 0; a < (Space.Width); a++) {
					for(int d = 0; d < (Space.Height); d++) {
						pos = a + Space.Width * d;
						Space.Ages[pos] = (int)Math.Abs(Space.Cells[pos] / year) + 1;
					}
				}
				if (senescence && GradientTest) {
					for (int i = 0; i < ageToDie; i++) {
						Space.Cells[i] = palette[i].ToArgb();
						Space.Cells[Space.Width + i] = palette[i].ToArgb();
						Space.Cells[2 * Space.Width + i] = palette[i].ToArgb();
						Space.Cells[3 * Space.Width + i] = palette[i].ToArgb();
						Space.Cells[4 * Space.Width + i] = palette[i].ToArgb();
						Space.Cells[5 * Space.Width + i] = palette[i].ToArgb();
						Space.Cells[6 * Space.Width + i] = palette[i].ToArgb();
						Space.Cells[7 * Space.Width + i] = palette[i].ToArgb();
						Space.Cells[8 * Space.Width + i] = palette[i].ToArgb();
						Space.Cells[9 * Space.Width + i] = palette[i].ToArgb();
					}
				}
			}
		}

		public uint Generations {
			get { return generations; }
			set {
				generations = value;
				OnPropertyChanged("Generations");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string info) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(info));
			}
		}
		
		public void Reset()
		{
			for(int  i = 0; i < 3; i++) {
				for(int j = 0; j < 3; j++) {
					sm[i, j] = true;
					rcb[i, j] = true;
				}
			}
			sm[1, 1] = false;
			Generations = 0;
			initializeSenescence();
		}
		
		public void Randomize()
		{
			Random rnd = new Random();
		    for(int i = 0; i < (Space.Cells.Length / 8); i++) {
		        Space.Cells[rnd.Next(Space.Cells.Length)] = cAlive.ToArgb(); //~0xF1DFB8 | (int)Filter.alpha;
		    }
			initializeSenescence();
		}

		public void proceed() {
			try {
				Generations++;
			} catch (OverflowException) {
				Generations = 0;
				Overflow = true;
			}
		    int w = Space.Width;
			int h = Space.Height;
		    // Create an array to store image data
			int[] idCopy = new int[w * h];
			//int r = 0;
			int k = 0;
		    idCopy = (int[])Space.Cells.Clone();
			for(int  a = 0; a < (w); a++) {
				for(int b = 0; b < (h); b++) {
					k = 0;
					//System.Console.WriteLine("w = " + w.ToString() + ", h = " + h.ToString() + "; a = " + a.ToString() + ", b = " + b.ToString() + "; eval = " + ((a+1)*(b+1)).ToString() + "; length = " + Space.Cells.Length.ToString());
					try {
					if(sm[0,0] && rcb[0,0]) { if(idCopy[(a-1) + w * (b-1)] != cd) { k++; } }
					if(sm[0,1] && rcb[0,1]) { if(idCopy[a + w * (b-1)] != cd) { k++; } }
					if(sm[0,2] && rcb[0,2]) { if(idCopy[(a+1) + w * (b-1)] != cd) { k++; } }
					if(sm[1,0] && rcb[1,0]) { if(idCopy[(a-1) + w * b] != cd) { k++; } }
					if(sm[1,2] && rcb[1,2]) { if(idCopy[(a+1) + w * b] != cd) { k++; } }
					if(sm[2,0] && rcb[2,0]) { if(idCopy[(a-1) + w * (b+1)] != cd) { k++; } }
					if(sm[2,1] && rcb[2,1]) { if(idCopy[a + w * (b+1)] != cd) { k++; } }
					if(sm[2,2] && rcb[2,2]) { if(idCopy[(a+1) + w * (b+1)] != cd) { k++; } }
					int pos = a + w * b;
					if (idCopy[pos] == cd) {
						if(Contains(liv, k)) {
							Space.Cells[pos] = ca;
							Space.Ages[pos]++;	
						} else {
							Space.Cells[pos] = cd;
							Space.Ages[pos] = 0;
						}
					} else if (idCopy[pos] == ca) {
						if(!Contains(dth, k)) {
							Space.Cells[pos] = ca;
							Space.Ages[pos]++;
						} else {
							Space.Cells[pos] = cd;
							Space.Ages[pos] = 0;
						}
					}
					if(senescence && (Space.Ages[pos] != 0)) {
						Space.Ages[pos]++;
						
						if(Space.Ages[pos] == ageToDie) {
							Space.Cells[pos] = cd;
							Space.Ages[pos] = 0;
						} else {
							//double lambda = (float)Space.Ages[pos] * step;
							//Color clr = ColorInterpolator.InterpolateBetween(cAlive, cOld, lambda);
							//Space.Cells[pos] = clr.R<<16 + clr.G<<8 + clr.B;
							Space.Cells[pos] = palette[Space.Ages[pos] - 1].ToArgb();
						}
					}
					} catch(IndexOutOfRangeException) {
					}
				}
			}
		}
	
		private static bool Contains(int[] arr, int val) {
			foreach(int i in arr) {
				if(i == val) return true;
			}
			return false;
		}

		public void RefreshPalette(List<Gfx.Palette.GradientEditor.GradientStop> m) {
			initPalette(m);
		}
		
	}
}

