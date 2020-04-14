using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using CA.Gfx;

namespace CA.Engine
{
	public class Calc : INotifyPropertyChanged
	{
		public Color CellAlive { get; set; }
		public Color CellDead { get; set; }
		public Color[] Palette { get; set; }
		public bool Senescence { get; set; } = true;
		public bool GradientTest { get; set; } = false;
		public int AgeToDie { get; set; } = 50;
		public Space Space { get; set; }
        // integer representation of the colors
        private int ca;
		private int cd;
		public bool[,] Vm { get; set; } = {{true, true, true}, {true, false, true}, {true, true, true}};
		public int[] ToLive { get; set; } = {1, 2, 4, 6, 7, 8};
		public int[] ToDie { get; set; } = {};
		private uint generations = 0;
		public bool Overflow { get; set; } = false;

		public Calc (Space s)
		{
			Space = s;
			Init();
		}
		
		public Calc (Space s, Color alive, Color dead)
		{
			Space = s;
			CellAlive = alive;
			CellDead = dead;
			Init();
		}
		
		public Calc (Space s, Color[] p)
		{
			Space = s;
			Palette = p;
			Init();
		}
		
		//public Calc (Space s, Gfx.Palette.Map m)
		public Calc (Space s, List<Gfx.Palette.GradientEditor.GradientStop> m)
		{
			Space = s;
			InitPalette(m);
			Init();
		}

		public void SetSpace (Space s) {
			Space = s;
			Init();
		}

		private void InitPalette(List<Gfx.Palette.GradientEditor.GradientStop> gradientStops) {
			Palette = new Color[AgeToDie];
			double ratio = 100.0f / AgeToDie;
			int stop = 0;
			int p = 0;
			Gfx.Palette.GradientEditor.GradientStop first = null;
			Gfx.Palette.GradientEditor.GradientStop last = null;
			foreach (Gfx.Palette.GradientEditor.GradientStop gradientStop in gradientStops) {
				p++;
				if (p == 1) {
					if (first == null) {
						first = gradientStop;
						last = null;
					} else if (last != null) {
						first = last;
						last = gradientStop;
						p = 0;
						stop++;
					}
				} else { // 2
					last = gradientStop;
					p = 0;
					stop++;
				}
				if ((first != null) && (last != null)) {
					int start = (int)(first.GetPosition() / ratio);
					int end = (int)(last.GetPosition() / ratio);
					int i = start;
					int walk = end - start;
					foreach (Color c in ColorInterpolator.GetGradients(first.Color, last.Color, walk)) {
						Palette[i] = c;
						i++;
					}
				}
			}
		}

		private void Init() {
			CellAlive = Palette[0];
			CellDead = Color.Black;
			GenerateIntColors();
			InitializeSenescence();
		}
		
		private void GenerateIntColors() {
			// Colors to ints for faster calculation:
			ca = CellAlive.ToArgb();
			cd = CellDead.ToArgb();
		}
		
		protected void InitializeSenescence() {
			// Create gradients for senescence
			if (Senescence) {
				int newBorn = Color.White.ToArgb();
				int dead = Color.Black.ToArgb();
				int scale = newBorn - dead;
				int year = scale / AgeToDie;
				int pos = 0;
				for(int  a = 0; a < (Space.Width); a++) {
					for(int d = 0; d < (Space.Height); d++) {
						pos = a + Space.Width * d;
						Space.Ages[pos] = Math.Abs(Space.Cells[pos] / year) + 1;
					}
				}
				if (Senescence && GradientTest) {
					for (int i = 0; i < AgeToDie; i++) {
						Space.Cells[i] = Palette[i].ToArgb();
						Space.Cells[Space.Width + i] = Palette[i].ToArgb();
						Space.Cells[2 * Space.Width + i] = Palette[i].ToArgb();
						Space.Cells[3 * Space.Width + i] = Palette[i].ToArgb();
						Space.Cells[4 * Space.Width + i] = Palette[i].ToArgb();
						Space.Cells[5 * Space.Width + i] = Palette[i].ToArgb();
						Space.Cells[6 * Space.Width + i] = Palette[i].ToArgb();
						Space.Cells[7 * Space.Width + i] = Palette[i].ToArgb();
						Space.Cells[8 * Space.Width + i] = Palette[i].ToArgb();
						Space.Cells[9 * Space.Width + i] = Palette[i].ToArgb();
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
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
		
		public void Reset()
		{
			for(int  i = 0; i < 3; i++) {
				for(int j = 0; j < 3; j++) {
					Vm[i, j] = true;
				}
			}
			Vm[1, 1] = false;
			Generations = 0;
			InitializeSenescence();
		}
		
		public void Randomize()
		{
			Random rnd = new Random();
		    for(int i = 0; i < (Space.Cells.Length / 8); i++) {
		        Space.Cells[rnd.Next(Space.Cells.Length)] = CellAlive.ToArgb();
		    }
			InitializeSenescence();
		}

		public void Proceed() {
			if (Generations < uint.MaxValue) {
				Generations++;
			} else {
				Generations = 0;
				Overflow = true;
			}
		    int w = Space.Width;
			int h = Space.Height;
            int maxW = w - 1;
            int maxH = h - 1;
		    // Create an array to store image data
			int[] idCopy;
			int k = 0;
		    idCopy = (int[])Space.Cells.Clone();
			for(int  a = 0; a < w; a++) {
				for(int b = 0; b < h; b++) {
					k = 0;
					if(Vm[0,0] && (a > 0 && b > 0 && idCopy[(a-1) + w * (b-1)] != cd)) { k++; }
					if(Vm[0,1] && (b > 0 && idCopy[a + w * (b-1)] != cd)) { k++; }
					if(Vm[0,2] && (b > 0 && a < maxW && idCopy[(a+1) + w * (b-1)] != cd)) { k++; }
					if(Vm[1,0] && (a > 0 && idCopy[(a-1) + w * b] != cd)) { k++; }
					if(Vm[1,2] && (a < maxW && idCopy[(a+1) + w * b] != cd)) { k++; }
					if(Vm[2,0] && (a > 0 && b < maxH && idCopy[(a-1) + w * (b+1)] != cd)) { k++; }
					if(Vm[2,1] && (b < maxH && idCopy[a + w * (b+1)] != cd)) { k++; }
					if(Vm[2,2] && (a < maxW && b < maxH && idCopy[(a+1) + w * (b+1)] != cd)) { k++; }
					int pos = a + w * b;
					if (idCopy[pos] == cd) {
						if(Contains(ToLive, k)) {
							Space.Cells[pos] = ca;
							Space.Ages[pos]++;	
						} else {
							Space.Cells[pos] = cd;
							Space.Ages[pos] = 0;
						}
					} else if (idCopy[pos] == ca) {
						if(!Contains(ToDie, k)) {
							Space.Cells[pos] = ca;
							Space.Ages[pos]++;
						} else {
							Space.Cells[pos] = cd;
							Space.Ages[pos] = 0;
						}
					}
					if(Senescence && (Space.Ages[pos] != 0)) {
						Space.Ages[pos]++;
						
						if(Space.Ages[pos] == AgeToDie) {
							Space.Cells[pos] = cd;
							Space.Ages[pos] = 0;
						} else {
                            var age = Space.Ages[pos] - 1;
                            if (age >= 0 && age < Palette.Length)
                                Space.Cells[pos] = Palette[age].ToArgb();
						}
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
			InitPalette(m);
		}
		
	}
}

