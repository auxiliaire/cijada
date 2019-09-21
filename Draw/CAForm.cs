using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text.RegularExpressions;
using CA.Engine;
using CA.Gfx.Palette.GradientEditor;
using System.Reflection;

[assembly: AssemblyVersion("0.0.0.1")]
namespace CA {

	class CAForm: Form {
		private readonly Controller caController = null;
		private readonly Timer timer;
		private const int WIDTH = 800;
		private const int HEIGHT = 542;
		private readonly int SP_WIDTH = 640;
		private readonly int SP_HEIGHT = 480;
		private readonly StatusBar statusBar;
		private string generations = "";
		private readonly Button startButton;
		private readonly TextBox livTB;
		private readonly TextBox dthTB;
		private readonly CheckBox ageingCB;
		private	readonly CellRelationCheckBox[,] crmCB;
		private readonly GradientEditor gradientEditor;

        [STAThread]
        public static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
	        Application.Run(new CAForm());
	    }
	
		public CAForm() {
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
            Panel p = new TableLayoutPanel
            {
                Width = ClientSize.Width - bm.Width,
                Padding = new Padding(5),
                Dock = DockStyle.Right,
                Parent = this
            };

            gradientEditor = new GradientEditor
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            gradientEditor.SetColorStop(new GradientStop(Color.White, 0));
			gradientEditor.SetColorStop(new GradientStop(Color.LightSteelBlue, 3));
			gradientEditor.SetColorStop(new GradientStop(Color.SteelBlue, 10));
			gradientEditor.SetColorStop(new GradientStop(Color.DarkBlue, 95));
			gradientEditor.SetColorStop(new GradientStop(Color.DarkTurquoise, 100));
			gradientEditor.MapChanged += new GradientEditor.MapChangedEventHandler(GeditMapChangedHandler);
	
			caController = new Controller(bm, bm.Width, bm.Height, gradientEditor.GetMap());
	        ResizeRedraw = true;
			Graphics gf = CreateGraphics();
			gf.DrawImageUnscaled(caController.Surface, 0, 0);

            startButton = new Button
            {
                Text = "Start",
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            startButton.Click += new EventHandler(OnStartClick);
			startButton.Parent = p;

            Button randomButton = new Button
            {
                Text = "Randomize",
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            randomButton.Click += new EventHandler(OnRandomClick);
			randomButton.Parent = p;

            Button resetButton = new Button
            {
                Text = "Reset",
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            resetButton.Click += new EventHandler(OnResetClick);
			resetButton.Parent = p;

            GroupBox crmGB = new GroupBox
            {
                Text = "Cell relations",
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Width = p.Width - 20,
                Parent = p
            };

            crmCB = new CellRelationCheckBox[3, 3];
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					if (!(i == 1 && j == 1)) {
                        crmCB[i, j] = new CellRelationCheckBox(i, j)
                        {
                            Location = new Point(j * 20 + 20, i * 20 + 20),
                            Width = 20,
                            Checked = caController.Calc.Vm[i, j]
                        };
                        crmCB[i, j].CheckedChanged += new EventHandler(OnCrmCBChanged);
						crmCB[i, j].Parent = crmGB;
					}
				}
			}

            p.Controls.Add(new Label
            {
                Text = "Create cells on:",
                AutoSize = true
            });

            livTB = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            livTB.KeyUp += new KeyEventHandler(OnKeyUp);
			livTB.Name = "livTB";
			RefreshLivDthTB(livTB);
			livTB.Validating += OnLivDthTextValidating;
	        livTB.Parent = p;

			ToolTip livTT = new ToolTip();
	        livTT.SetToolTip(livTB, "Comma separated numbers of neighboring cells that makes an empty space filled (eg: 0,3,5,8).");

            p.Controls.Add(new Label
            {
                Text = "Destroy cells on:",
                AutoSize = true,
                Parent = p
            });

            dthTB = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            dthTB.KeyUp += new KeyEventHandler(OnKeyUp);
			dthTB.Name = "dthTB";
			RefreshLivDthTB(dthTB);
			dthTB.Validating += OnLivDthTextValidating;
	        dthTB.Parent = p;
	
			ToolTip dthTT = new ToolTip();
	        dthTT.SetToolTip(dthTB, "Comma separated numbers of neighboring cells that makes a filled space empty (eg: 0,1,4,8).");

            ageingCB = new CheckBox
            {
                Location = new Point(10, 40),
                Text = "Ageing",
                Checked = true,
                Parent = p
            };
            ageingCB.CheckedChanged += new EventHandler(OnAgeingCBChanged);

            p.Controls.Add(new Label
            {
                Text = "Colors of ageing:",
                Location = new Point(10, 280),
                AutoSize = true,
            });

            gradientEditor.Parent = p;

            Button saveImg = new Button
            {
                Text = "Export image...",
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            saveImg.Click += new EventHandler(OnSaveImgClick);
			saveImg.Parent = p;

            Button loadImg = new Button
            {
                Text = "Import image...",
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            loadImg.Click += new EventHandler(OnLoadImgClick);
			loadImg.Parent = p;

            Button saveSettings = new Button
            {
                Text = "Save config...",
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            saveSettings.Click += new EventHandler(OnSaveSettingsClick);
			saveSettings.Parent = p;

            Button loadSettings = new Button
            {
                Text = "Load config...",
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            loadSettings.Click += new EventHandler(OnLoadSettingsClick);
			loadSettings.Parent = p;

            timer = new Timer
            {
                Enabled = false,
                Interval = 50
            };
            timer.Tick += new EventHandler(HandleTick);

            Button quitButton = new Button
            {
                Text = "Quit",
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            quitButton.Click += new EventHandler(OnQuitClick);
			quitButton.Parent = p;

            statusBar = new StatusBar
            {
                Text = "Generations: 0",
                Parent = this
            };
            caController.Calc.PropertyChanged += new PropertyChangedEventHandler(OnGenerationsChanged);
	
	        CenterToScreen();
	    }
	
		public void OnLivDthTextValidating(object sender, CancelEventArgs e) {
			TextBox tb = (TextBox) sender;
			if (!IsValidLivDthText(tb.Text)) {
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
						caController.Calc.ToLive = numbers;
					break;
					case "dthTB":
						caController.Calc.ToDie = numbers;
					break;
				}
			}
		}
	
		public void RefreshLivDthTB(TextBox t) {
			int[] numbers;
			switch (t.Name) {
				case "livTB":
					numbers = caController.Calc.ToLive;
				break;
				case "dthTB":
					numbers = caController.Calc.ToDie;
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
						crmCB[i, j].Checked = caController.Calc.Vm[i, j];
					}
				}
			}
			Graphics gf = CreateGraphics();
			gf.DrawImageUnscaled(caController.Surface, 0, 0);
			statusBar.Text = "Generations: " + generations;
		}
	
		public void OnRandomClick(object sender, EventArgs e) {
			caController.Randomize();
			Graphics gf = CreateGraphics();
			gf.DrawImageUnscaled(caController.Surface, 0, 0);
			statusBar.Text = "Generations: " + generations;
		}
	
		public void OnSaveImgClick(object sender, EventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Save file as...",
                Filter = "JPeg Image|*.jpg|Png Image|*.png|Bitmap Image|*.bmp|Gif Image|*.gif",
                RestoreDirectory = true
            };
            dialog.ShowDialog();

			if (dialog.FileName != "") {
				// Saves the Image via a FileStream created by the OpenFile method.
	      		System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();
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
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Save config as...",
                Filter = "CA configuration|*.cac",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != "") {
                Model.Settings config = new Model.Settings
                {
                    Senescence = caController.Calc.Senescence,
                    CellRelation = caController.Calc.Vm,
                    RuleLife = caController.Calc.ToLive,
                    RuleDeath = caController.Calc.ToDie,
                    GradientMap = gradientEditor.GetMap()
                };

                Model.Serializer.SSettings serializer = new Model.Serializer.SSettings();
				serializer.SerializeObject(dialog.FileName, config);
			}
		}

		public void OnLoadSettingsClick(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select config to load",
                Filter = "CA configuration|*.cac",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
				startButton.Text = "Start";
	            timer.Stop();
				gradientEditor.Enabled = true;
				try {
					Model.Serializer.SSettings serializer = new Model.Serializer.SSettings();
					Model.Settings config = serializer.DeSerializeObject(dialog.FileName);
					_resetCA();
					ageingCB.Checked = config.Senescence;
					for (int i = 0; i < 3; i++) {
						for (int j = 0; j < 3; j++) {
                            if (!(i == 1 && j == 1))
							    crmCB[i, j].Checked = config.CellRelation[i, j];
						}
					}
					caController.Calc.ToLive = config.RuleLife;
					caController.Calc.ToDie = config.RuleDeath;
					RefreshLivDthTB(livTB);
					RefreshLivDthTB(dthTB);
					gradientEditor.SetMap(config.GradientMap);
				} catch (Exception exception) {
					MessageBox.Show("Could not load config (corrupted file?).");
                    Console.WriteLine(exception.Message);
					_resetCA();
				}
			}
		}

		public void OnLoadImgClick(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Choose image file to import",
                Filter = "JPeg Image|*.jpg|Png Image|*.png|Bitmap Image|*.bmp|Gif Image|*.gif",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
				Bitmap bm = LoadBitmap(dialog.FileName);
				caController.LoadSurface(bm);
				Graphics gf = CreateGraphics();
				gf.DrawImageUnscaled(caController.Surface, 0, 0);
				gf.Dispose();
			}
		}

		public void OnAgeingCBChanged(object sender, EventArgs e)
		{
			caController.Calc.Senescence = ageingCB.Checked;
			gradientEditor.Enabled = ageingCB.Checked;
		}

		public void OnCrmCBChanged(object sender, EventArgs e)
		{
			CellRelationCheckBox cellRelationCheckBox = (CellRelationCheckBox)sender;
			caController.Calc.Vm[cellRelationCheckBox.Row, cellRelationCheckBox.Col] = cellRelationCheckBox.Checked;
		}
	
		public void OnGenerationsChanged(object sender, PropertyChangedEventArgs args) {
			Calc c = (Calc)sender;
			generations = (c.Overflow?"!":"") + c.Generations.ToString();
		}
			
		protected Bitmap CreateBitmap(int width, int height) {
			Bitmap bm = new Bitmap(width, height);
			Graphics g = Graphics.FromImage(bm);
			Brush b = new SolidBrush(Color.Black);
			g.FillRectangle(b, new Rectangle(0, 0, width, height));
			g.Dispose();
			return bm;
		}
		
		public Bitmap LoadBitmap(string filename)
		{
			Bitmap bmp = null;
			bmp = new Bitmap(filename);
			return bmp;
		}
	
	    public void OnStartClick(object sender, EventArgs e) {
			Button b = (Button) sender;
	        if (timer.Enabled) {
				b.Text = "Start";
	            timer.Stop();
				gradientEditor.Enabled = caController.Calc.Senescence;
	        } else {
				b.Text = "Stop";
				gradientEditor.Enabled = false;
	            timer.Start();
	        }
	    }
	
		public void OnQuitClick(object sender, EventArgs e) {
			Close ();
		}
	
	     protected override void OnMouseDown(MouseEventArgs e) {
	          if (e.Button != MouseButtons.Left)
	               return;
	   
	         caController.Drawing = true;
			 caController.AddCells(new Point(e.X, e.Y));
	     }
		
		protected void HandleTick (object sender, EventArgs e) {
			caController.HandleTick();
				
			Graphics gf = CreateGraphics();
			gf.DrawImageUnscaled(caController.Surface, 0, 0);
			statusBar.Text = "Generations: " + generations;
		}
		
	     protected override void OnMouseMove(MouseEventArgs e) {
	          if (!caController.Drawing)
	               return;
			caController.AddCells(new Point(e.X, e.Y));
			Graphics gf = CreateGraphics();
			gf.DrawImageUnscaled(caController.Surface, 0, 0);
	    }
		
	     protected override void OnMouseUp(MouseEventArgs e) {
	        caController.Drawing = false;
			caController.M_X = -1;
			caController.M_Y = -1;
	     }
	
		protected override void OnPaint(PaintEventArgs e) {
	          DoPage(e.Graphics, ForeColor, ClientSize.Width, ClientSize.Height);
	    }
		
	    protected void DoPage(Graphics g, Color clr, int cx, int cy) {
			g.DrawImageUnscaled(caController.Surface, 0, 0);
	    }
		
		protected virtual void GeditMapChangedHandler (object sender, EventArgs e)
		{
			caController.Calc.RefreshPalette(((GradientEditor)sender).GetMap());
		}
	
	} // end class

} // end namespace