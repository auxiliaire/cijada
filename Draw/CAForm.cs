using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using CA.Engine;
using CA.Gfx;
using CA.Gfx.Palette.GradientEditor;

namespace CA {

	class CAForm: Form {
		private Controller caController = null;
		private Timer tmr;
		private const int WIDTH = 800;
		private const int HEIGHT = 542;
		private int SP_WIDTH = 640;
		private int SP_HEIGHT = 480;
		//private int PANEL_SPACE = 8; // git teszt
		private StatusBar sb;
		private string generations = "";
		private Button startButton;
		private TextBox livTB;
		private TextBox dthTB;
		private CheckBox ageingCB;
		private GroupBox crmGB;
		private	CellRelationCheckBox[,] crmCB;
		private GradientEditor gedit;
	
		//static FastLoop _fastLoop = new FastLoop(AnimLoop);
		
		public static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
	        Application.Run(new CAForm());
	    }
	
		public CAForm() { // CONSTRUCTOR
			DoubleBuffered = true;
	        Text = "CA 2011";
	        Size = new Size(WIDTH, HEIGHT);
	        ToolTip tooltip = new ToolTip();
	        tooltip.SetToolTip(this, "Click or draw to add cells!");
	        Bitmap bm;
	        try {
			    bm = LoadBitmap("bgr.jpg");
	        } catch (Exception) {
			    bm = CreateBitmap(SP_WIDTH, SP_HEIGHT);
	        }
			// Creating palette
			//Gfx.Palette.Map paletteMap = new Gfx.Palette.Map();
	
			//paletteMap.setColorStop(0, new Gfx.Palette.ColorStop(0, Color.White));
			//paletteMap.setColorStop(3, new Gfx.Palette.ColorStop(3, Color.LightSteelBlue)); // Color.Yellow));
			//paletteMap.setColorStop(10, new Gfx.Palette.ColorStop(10, Color.SteelBlue)); // Color.Lime));
			//paletteMap.setColorStop(95, new Gfx.Palette.ColorStop(99, Color.DarkBlue)); //.SaddleBrown));
			//paletteMap.setColorStop(100, new Gfx.Palette.ColorStop(100, Color.DarkTurquoise));
	
			//paletteMap.setColorStop(0, new Gfx.Palette.GradientEditor.GradientStop(Color.White, 0));
			//paletteMap.setColorStop(3, new Gfx.Palette.GradientEditor.GradientStop(Color.LightSteelBlue, 3)); // Color.Yellow));
			//paletteMap.setColorStop(10, new Gfx.Palette.GradientEditor.GradientStop(Color.SteelBlue, 10)); // Color.Lime));
			//paletteMap.setColorStop(95, new Gfx.Palette.GradientEditor.GradientStop(Color.DarkBlue, 99)); //.SaddleBrown));
			//paletteMap.setColorStop(100, new Gfx.Palette.GradientEditor.GradientStop(Color.DarkTurquoise, 100));
	
			//paletteMap.setColorStop(0, new ColorStop(0, Color.Yellow));
			//paletteMap.setColorStop(10, new ColorStop(10, Color.Lime));
			//paletteMap.setColorStop(80, new ColorStop(85, Color.DarkRed));
			//paletteMap.setColorStop(100, new ColorStop(100, Color.SaddleBrown));
	
			Panel p = new TableLayoutPanel();
			p.Width = ClientSize.Width - bm.Width;
			p.Padding = new Padding(5);
			p.Dock = DockStyle.Right;
			p.Parent = this;
	
			//GradientEditor gedit = new GradientEditor(paletteMap);
			gedit = new GradientEditor();
			gedit.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			gedit.setColorStop(new Gfx.Palette.GradientEditor.GradientStop(Color.White, 0));
			gedit.setColorStop(new Gfx.Palette.GradientEditor.GradientStop(Color.LightSteelBlue, 3)); // Color.Yellow));
			gedit.setColorStop(new Gfx.Palette.GradientEditor.GradientStop(Color.SteelBlue, 10)); // Color.Lime));
			gedit.setColorStop(new Gfx.Palette.GradientEditor.GradientStop(Color.DarkBlue, 95)); //.SaddleBrown));
			gedit.setColorStop(new Gfx.Palette.GradientEditor.GradientStop(Color.DarkTurquoise, 100));
			gedit.MapChanged += new GradientEditor.MapChangedEventHandler(GeditMapChangedHandler);
	
			//caController = new Controller(bm, bm.Width, bm.Height, paletteMap); // ClientSize.Width, ClientSize.Height);
			caController = new Controller(bm, bm.Width, bm.Height, gedit.getMap());
	        ResizeRedraw = true;
			Graphics gf = CreateGraphics();
			gf.DrawImageUnscaled(caController.Surface, 0, 0);

			//Bitmap shade = new Bitmap(ClientSize.Width, 50);
			//Graphics g = Graphics.FromImage(shade);
			//Brush b = new SolidBrush(Color.FromArgb(20, 200, 200, 200));
			//gf.FillRectangle(b, new Rectangle(0, 0, ClientSize.Width, 50));
			//g.DrawImageUnscaled(shade, 0, 0);
			//g.Dispose();
			//Graphics gs = CreateGraphics();
			//gf.DrawImageUnscaled(shade, 0, 0);
	
			startButton = new Button();
	        startButton.Text = "Start";
			startButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
	        startButton.Click += new EventHandler(OnStartClick);
			startButton.Parent = p;
	
			Button randomButton = new Button();
	        randomButton.Text = "Randomize";
			randomButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
	        randomButton.Click += new EventHandler(OnRandomClick);
			randomButton.Parent = p;
	
			Button resetButton = new Button();
	        resetButton.Text = "Reset";
			resetButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
	        resetButton.Click += new EventHandler(OnResetClick);
			resetButton.Parent = p;
	
			crmGB = new GroupBox();
			crmGB.Text = "Cell relations";
			crmGB.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			crmGB.Width = p.Width - 20;
			crmGB.Parent = p;
	
			crmCB = new CellRelationCheckBox[3, 3];
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					if (!(i == 1 && j == 1)) {
						crmCB[i, j] = new CellRelationCheckBox(i, j);
						crmCB[i, j].Location = new Point(j * 20 + 20, i * 20 + 20);
						crmCB[i, j].Width = 20;
						crmCB[i, j].Checked = caController.Calc.sm[i, j];
						crmCB[i, j].CheckedChanged += new EventHandler(OnCrmCBChanged);
						crmCB[i, j].Parent = crmGB;
					}
				}
			}

			Label livLabel = new Label();
	        livLabel.Text = "Create cells on:";
	        livLabel.AutoSize = true;
	        livLabel.Parent = p;
	
	        livTB = new TextBox();
			livTB.Anchor = AnchorStyles.Left | AnchorStyles.Right;
	        livTB.KeyUp += new KeyEventHandler(OnKeyUp);
			livTB.Name = "livTB";
			RefreshLivDthTB(livTB);
			livTB.Validating += OnLivDthTextValidating;
	        livTB.Parent = p;

			ToolTip livTT = new ToolTip();
	        livTT.SetToolTip(livTB, "Comma separated numbers of neighboring cells that makes an empty space filled (eg: 0,3,5,8).");

			Label dthLabel = new Label();
	        dthLabel.Text = "Destroy cells on:";
	        dthLabel.AutoSize = true;
	        dthLabel.Parent = p;

	        dthTB = new TextBox();
			dthTB.Anchor = AnchorStyles.Left | AnchorStyles.Right;
	        dthTB.KeyUp += new KeyEventHandler(OnKeyUp);
			dthTB.Name = "dthTB";
			RefreshLivDthTB(dthTB);
			dthTB.Validating += OnLivDthTextValidating;
	        dthTB.Parent = p;
	
			ToolTip dthTT = new ToolTip();
	        dthTT.SetToolTip(dthTB, "Comma separated numbers of neighboring cells that makes a filled space empty (eg: 0,1,4,8).");

			ageingCB = new CheckBox();
	        ageingCB.Location = new Point(10, 40);
	        ageingCB.Text = "Ageing";
	        ageingCB.Checked = true;
	        ageingCB.Parent = p;
			ageingCB.CheckedChanged += new EventHandler(OnAgeingCBChanged);
	
			Label geditLabel = new Label();
	        geditLabel.Text = "Colors of ageing:";
	        geditLabel.Location = new Point(10, 280);
	        geditLabel.AutoSize = true;
	        geditLabel.Parent = p;
	
			gedit.Parent = p;
	
			Button saveImg = new Button();
			saveImg.Text = "Export image...";
			saveImg.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			saveImg.Click += new EventHandler(OnSaveImgClick);
			saveImg.Parent = p;

			Button loadImg = new Button();
			loadImg.Text = "Import image...";
			loadImg.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			loadImg.Click += new EventHandler(OnLoadImgClick);
			loadImg.Parent = p;

			Button saveSettings = new Button();
			saveSettings.Text = "Save config...";
			saveSettings.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			saveSettings.Click += new EventHandler(OnSaveSettingsClick);
			saveSettings.Parent = p;

			Button loadSettings = new Button();
			loadSettings.Text = "Load config...";
			loadSettings.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			loadSettings.Click += new EventHandler(OnLoadSettingsClick);
			loadSettings.Parent = p;

			tmr = new Timer();
			tmr.Enabled = false;
			tmr.Interval = 50;
			tmr.Tick += new EventHandler(HandleTick);
			if (!caController.Calc.GradientTest) {
				//tmr.Start();
			}
	
			Button quitButton = new Button();
	        quitButton.Text = "Quit";
			quitButton.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			//quitButton.Dock = DockStyle.Top;
	        quitButton.Click += new EventHandler(OnQuitClick);
			quitButton.Parent = p;
	
			sb = new StatusBar();
			sb.Text = "Generations: 0";
			sb.Parent = this;
			caController.Calc.PropertyChanged += new PropertyChangedEventHandler(OnGenerationsChanged);
	
	        //Controls.Add();
	        CenterToScreen();
	    }
	
		public void OnLivDthTextValidating(object sender, CancelEventArgs e) {
			TextBox tb = (TextBox) sender;
			if (IsValidLivDthText(tb.Text)) {
				// skip
			} else {
				RefreshLivDthTB(tb);
			}
		}
	
		public bool IsValidLivDthText(string s) {
			Match match = Regex.Match(s, "^([0-8](,[0-8]){0,8})?$");
			return match.Success;
		}

		public void OnKeyUp(object sender, KeyEventArgs e) {
			TextBox tb = (TextBox) sender;
			if (IsValidLivDthText(tb.Text)) {
				int[] numbers;
				if (tb.Text.Length > 0) {
					string[] chars = tb.Text.Split(',');
					numbers = new int[chars.Length];
					int i = 0;
					foreach (string c in chars) {
						numbers[i] = Int32.Parse(c);
						i++;
					}
				} else {
					numbers = new int[0];
				}
				switch (tb.Name) {
					case "livTB":
						caController.Calc.liv = numbers;
					break;
					case "dthTB":
						caController.Calc.dth = numbers;
					break;
				}
			}
		}
	
		public void RefreshLivDthTB(TextBox t) {
			int[] numbers;
			switch (t.Name) {
				case "livTB":
					numbers = caController.Calc.liv;
				break;
				case "dthTB":
					numbers = caController.Calc.dth;
				break;
				default:
					numbers = new int[0];
				break;
			}
			if (numbers.Length > 0) {
				string[] s = new string[numbers.Length];
				int i = 0;
				foreach (int n in numbers) {
					s[i] = n.ToString();
					i++;
				}
				t.Text = String.Join(",", s);
			} else {
				t.Text = "";
			}
		}
	
		public void OnResetClick(object sender, EventArgs e) {
			_resetCA();
		}

		protected void _resetCA() {
			caController.Reset();
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					if (!(i == 1 && j == 1)) {
						crmCB[i, j].Checked = caController.Calc.sm[i, j];
					}
				}
			}
			Graphics gf = CreateGraphics();
			//gf.DrawImageUnscaled(blurred, 0, 0);
			gf.DrawImageUnscaled(caController.Surface, 0, 0);
			sb.Text = "Generations: " + generations;
		}
	
		public void OnRandomClick(object sender, EventArgs e) {
			caController.Randomize();
			Graphics gf = CreateGraphics();
			//gf.DrawImageUnscaled(blurred, 0, 0);
			gf.DrawImageUnscaled(caController.Surface, 0, 0);
			sb.Text = "Generations: " + generations;
		}
	
		public void OnSaveImgClick(object sender, EventArgs e) {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Save file as...";
			dialog.Filter = "JPeg Image|*.jpg|Png Image|*.png|Bitmap Image|*.bmp|Gif Image|*.gif";
			dialog.RestoreDirectory = true;
			dialog.ShowDialog();

			if (dialog.FileName != "") {
				// Saves the Image via a FileStream created by the OpenFile method.
	      		System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();
				//MessageBox.Show(dialog.FileName);
				switch (dialog.FilterIndex) {
					case 1: // jpg
						caController.Surface.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
					break;
					case 2: // png
						caController.Surface.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
					break;
					case 3: // bmp
						caController.Surface.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
					break;
					case 4: // gif
						caController.Surface.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
					break;
				}
				fs.Close();
			}
		}

		public void OnSaveSettingsClick(object sender, EventArgs e) {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Save config as...";
			dialog.Filter = "CA configuration|*.cac";
			dialog.RestoreDirectory = true;

			if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != "") {
				CA.Model.Settings config = new CA.Model.Settings();
				config.Senescence = caController.Calc.senescence;
				config.CellRelation = caController.Calc.sm;
				config.RuleLife = caController.Calc.liv;
				config.RuleDeath = caController.Calc.dth;
				config.GradientMap = gedit.getMap();

				CA.Model.Serializer.SSettings serializer = new CA.Model.Serializer.SSettings();
				serializer.SerializeObject(dialog.FileName, config);
			}
		}

		public void OnLoadSettingsClick(object sender, EventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Select config to load";
			dialog.Filter = "CA configuration|*.cac";
			dialog.RestoreDirectory = true;

			if (dialog.ShowDialog() == DialogResult.OK) {
				startButton.Text = "Start";
	            tmr.Stop();
				gedit.Enabled = true;
				try {
					CA.Model.Serializer.SSettings serializer = new CA.Model.Serializer.SSettings();
					CA.Model.Settings config = serializer.DeSerializeObject(dialog.FileName);
					_resetCA();
					ageingCB.Checked = config.Senescence;
					for (int i = 0; i < 3; i++) {
						for (int j = 0; j < 3; j++) {
							crmCB[i, j].Checked = config.CellRelation[i, j];
						}
					}
					caController.Calc.liv = config.RuleLife;
					caController.Calc.dth = config.RuleDeath;
					RefreshLivDthTB(livTB);
					RefreshLivDthTB(dthTB);
					gedit.setMap(config.GradientMap);
				} catch (Exception ex) {
					MessageBox.Show("Could not load config (corrupted file?).");
					_resetCA();
				}
			}
		}

		public void OnLoadImgClick(object sender, EventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Choose image file to import";
			dialog.Filter = "JPeg Image|*.jpg|Png Image|*.png|Bitmap Image|*.bmp|Gif Image|*.gif";
			dialog.RestoreDirectory = true;

			if (dialog.ShowDialog() == DialogResult.OK) {
				Bitmap bm = LoadBitmap(dialog.FileName);
				caController.loadSurface(bm);
				Graphics gf = CreateGraphics();
				gf.DrawImageUnscaled(caController.Surface, 0, 0);
				gf.Dispose();
			}
		}

		public void OnAgeingCBChanged(object sender, EventArgs e)
		{
			caController.Calc.senescence = ageingCB.Checked;
			gedit.Enabled = ageingCB.Checked;
		}

		public void OnCrmCBChanged(object sender, EventArgs e)
		{
			CellRelationCheckBox crmCB = (CellRelationCheckBox)sender;
			//Console.WriteLine("Row: {0}, Col: {1}", crmCB.Row, crmCB.Col);
			caController.Calc.sm[crmCB.Row, crmCB.Col] = crmCB.Checked;
		}
	
		public void OnGenerationsChanged(object sender, PropertyChangedEventArgs args) {
			Calc c = (Calc)sender;
			generations = (c.Overflow?"!":"") + c.Generations.ToString();
		}
			
		protected Bitmap CreateBitmap(int width, int height) {
			Bitmap bm = new Bitmap(width, height);
			Graphics g = Graphics.FromImage(bm);
			Brush b = new SolidBrush(Color.Black); //Color.FromArgb(cDead));
			g.FillRectangle(b, new Rectangle(0, 0, width, height));
			g.Dispose();
			return bm;
		}
		
		public Bitmap LoadBitmap(string filename)
		{
			Bitmap bmp = null;
			//try {
				bmp = new Bitmap(filename);
			//} catch (Exception) {
			//	bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
			//}
			return bmp;
		}
	
		static void AnimLoop() {
			// Get Input
			// Process
			// Render
			//System.Console.WriteLine("loop");
		}
	
	    public void OnStartClick(object sender, EventArgs e) {
			Button b = (Button) sender;
	        if (tmr.Enabled) {
				b.Text = "Start";
	            tmr.Stop();
				gedit.Enabled = caController.Calc.senescence;
	        } else {
				b.Text = "Stop";
				gedit.Enabled = false;
	            tmr.Start();
	        }
	    }
	
		public void OnQuitClick(object sender, EventArgs e) {
			//Application.Exit();
			Close ();
		}
	
	     protected override void OnMouseDown(MouseEventArgs mea) {
	          if (mea.Button != MouseButtons.Left)
	               return;
	   
	          //ptLast = new Point(mea.X, mea.Y);
			//System.Console.WriteLine("x = " + mea.X.ToString() + ", y= " + mea.Y.ToString());
	         caController.drawing = true;
			caController.addCells(new Point(mea.X, mea.Y));
	     }
		
		protected void HandleTick (object sender, EventArgs e) {
			caController.HandleTick();
				
			Graphics gf = CreateGraphics();
			//gf.DrawImageUnscaled(blurred, 0, 0);
			gf.DrawImageUnscaled(caController.Surface, 0, 0);
			sb.Text = "Generations: " + generations;
		}
		
	     protected override void OnMouseMove(MouseEventArgs mea) {
	          if (!caController.drawing)
	               return;
			caController.addCells(new Point(mea.X, mea.Y));
			Graphics gf = CreateGraphics();
			gf.DrawImageUnscaled(caController.Surface, 0, 0);
	    }
		
	     protected override void OnMouseUp(MouseEventArgs mea) {
	          caController.drawing = false;
			caController.mX = -1;
			caController.mY = -1;
	     }
	
		protected override void OnPaint(PaintEventArgs pea) {
	          DoPage(pea.Graphics, ForeColor, ClientSize.Width, ClientSize.Height);
	    }
		
	    protected void DoPage(Graphics g, Color clr, int cx, int cy) {
			g.DrawImageUnscaled(caController.Surface, 0, 0);
	          //Pen pen = new Pen(clr);
	   
	          //g.DrawLine(pen, 0,      0, cx - 1, cy - 1);
	          //g.DrawLine(pen, cx - 1, 0, 0,      cy - 1);
			//g.DrawLine(pen, frX, frY, toX, toY);
			//frX = toX;
			//frY = toY;
	    }
		
		protected void CLive() {
			
		}
			
		private static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
		{
		    Bitmap blurred = new Bitmap(image.Width, image.Height);
		 
		    // make an exact copy of the bitmap provided
		    using(Graphics graphics = Graphics.FromImage(blurred))
		        graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
		            new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
		 
		    // look at every pixel in the blur rectangle
		    for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
		    {
		        for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
		        {
		            Int32 avgR = 0, avgG = 0, avgB = 0;
		            Int32 blurPixelCount = 0;
		 
		            // average the color of the red, green and blue for each pixel in the
		            // blur size while making sure you don't go outside the image bounds
		            for (Int32 x = xx; (x < xx + blurSize && x < image.Width); x++)
		            {
		                for (Int32 y = yy; (y < yy + blurSize && y < image.Height); y++)
		                {
		                    Color pixel = blurred.GetPixel(x, y);
		 
		                    avgR += pixel.R;
		                    avgG += pixel.G;
		                    avgB += pixel.B;
		 
		                    blurPixelCount++;
		                }
		            }
		 
		            avgR = avgR / blurPixelCount;
		            avgG = avgG / blurPixelCount;
		            avgB = avgB / blurPixelCount;
		 
		            // now that we know the average for the blur size, set each pixel to that color
		            for (Int32 x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
		                for (Int32 y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
		                    blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
		        }
		    }

		    return blurred;
		}
		
		private static Bitmap Blur(Bitmap image, Int32 blurSize)
		{
		    return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);
		}
	
		protected virtual void GeditMapChangedHandler (object sender, EventArgs e)
		{
			caController.Calc.RefreshPalette(((GradientEditor)sender).getMap());
		}
	
	} // end class

} // end namespace