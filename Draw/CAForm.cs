using CA.Engine;
using CA.Gfx.Palette.GradientEditor;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

[assembly: AssemblyVersion("0.0.0.1")]
namespace CA
{

	class CAForm : Form
	{
		private readonly Controller caController = null;
		private readonly Timer timer;
		private const int WIDTH = 880;
		private const int HEIGHT = 560;
		private readonly int SP_WIDTH = 640;
		private readonly int SP_HEIGHT = 480;
		private readonly Panel panel;
		private readonly Button startButton;
		private readonly StatusBar statusBar;
		private string generations = "";
		private readonly TextBox liveTextBox;
		private readonly TextBox deathTextBox;
		private readonly CheckBox ageingCheckBox;
		private readonly CellRelationCheckBox[,] cellRelationCheckboxes;
		private readonly GradientEditor gradientEditor;
		private static readonly string DefaultTitle = "Cijada";

		[STAThread]
		public static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new CAForm());
		}

		public CAForm()
		{
			DoubleBuffered = true;
			SetTitle(DefaultTitle);
			Size = new Size(WIDTH, HEIGHT);
			MinimumSize = Size;
			statusBar = CreateStatusbar();

			InitTooltip();

			panel = CreateControlPanel();
			startButton = CreateStartButton();
			InitBasicButtons();

			// Creating palette
			gradientEditor = CreateGradientEditor();
			caController = CreateController();
			InitSurface();

			cellRelationCheckboxes = CreateCellRelationCheckboxes();
			liveTextBox = CreateLiveTextBox();
			deathTextBox = CreateDeathTextBox();

			ageingCheckBox = CreateAgeingCheckbox();

			panel.Controls.Add(gradientEditor);

			InitIOButtons();

			timer = CreateTimer();

			InitQuitButton();

			this.Controls.Add(statusBar);

			CenterToScreen();
		}

		private static StatusBar CreateStatusbar()
		{
			return new StatusBar
			{
				Text = "Generations: 0"
			};
		}

		private void InitTooltip()
		{
			ToolTip tooltip = new ToolTip();
			tooltip.SetToolTip(this, "Click or draw to add cells!");
		}

		private Panel CreateControlPanel()
		{
			Panel panel = new TableLayoutPanel
			{
				Padding = new Padding(5),
				Dock = DockStyle.Right,
				Parent = this
			};

			return panel;
		}

		private Button CreateStartButton()
		{
			Button startButton = new Button
			{
				Text = "Start",
				Anchor = AnchorStyles.Left | AnchorStyles.Right
			};
			startButton.Click += new EventHandler(OnStartClick);
			startButton.Parent = panel;

			return startButton;
		}

		public void OnStartClick(object sender, EventArgs e)
		{
			Button b = (Button)sender;
			if (timer.Enabled)
			{
				b.Text = "Start";
				timer.Stop();
				gradientEditor.Enabled = caController.Calc.Senescence;
			}
			else
			{
				b.Text = "Stop";
				gradientEditor.Enabled = false;
				timer.Start();
			}
		}

		private void InitBasicButtons()
		{
			InitRandomButton();
			InitResetButton();
		}

		private void InitRandomButton()
		{
			Button randomButton = new Button
			{
				Text = "Randomize",
				Anchor = AnchorStyles.Left | AnchorStyles.Right
			};
			randomButton.Click += new EventHandler(OnRandomClick);
			randomButton.Parent = panel;
		}

		public void OnRandomClick(object sender, EventArgs e)
		{
			caController.Randomize();
			Graphics gf = CreateGraphics();
			DrawSurface(gf);
			statusBar.Text = "Generations: " + generations;
		}

		private void InitResetButton()
		{
			Button resetButton = new Button
			{
				Text = "Reset",
				Anchor = AnchorStyles.Left | AnchorStyles.Right
			};
			resetButton.Click += new EventHandler(OnResetClick);
			resetButton.Parent = panel;
		}

		public void OnResetClick(object sender, EventArgs e)
		{
			_resetCA();
		}

		private GradientEditor CreateGradientEditor()
		{
			panel.Controls.Add(new Label
			{
				Text = "Colors of ageing:",
				Location = new Point(10, 280),
				AutoSize = true,
			});

			GradientEditor gradientEditor = new GradientEditor
			{
				Anchor = AnchorStyles.Left | AnchorStyles.Right
			};
			gradientEditor.SetColorStop(new GradientStop(Color.White, 0));
			gradientEditor.SetColorStop(new GradientStop(Color.LightSteelBlue, 3));
			gradientEditor.SetColorStop(new GradientStop(Color.SteelBlue, 10));
			gradientEditor.SetColorStop(new GradientStop(Color.DarkBlue, 95));
			gradientEditor.SetColorStop(new GradientStop(Color.DarkTurquoise, 100));
			gradientEditor.MapChanged += new GradientEditor.MapChangedEventHandler(GeditMapChangedHandler);
			return gradientEditor;
		}

		protected virtual void GeditMapChangedHandler(object sender, EventArgs e)
		{
			caController.Calc.RefreshPalette(((GradientEditor)sender).GetMap());
		}

		private Controller CreateController()
		{
			Bitmap bm = CreateBackground();

			Controller caController = new Controller(bm, bm.Width, bm.Height, gradientEditor.GetMap());
			caController.Calc.PropertyChanged += new PropertyChangedEventHandler(OnGenerationsChanged);

			return caController;
		}

		private Bitmap CreateBackground()
		{
			Bitmap bm;
			try
			{
				bm = LoadBitmap("bgr.jpg");
			}
			catch (Exception)
			{
				bm = CreateBitmap(SP_WIDTH, SP_HEIGHT);
			}
			return bm;
		}

		protected Bitmap CreateBitmap(int width, int height)
		{
			Bitmap bm = new Bitmap(width, height);
			Graphics g = Graphics.FromImage(bm);
			Brush b = new SolidBrush(Color.Black);
			g.FillRectangle(b, new Rectangle(0, 0, width, height));
			g.Dispose();
			return bm;
		}

		public void OnGenerationsChanged(object sender, PropertyChangedEventArgs args)
		{
			Calc c = (Calc)sender;
			generations = (c.Overflow ? "!" : "") + c.Generations.ToString();
		}

		private void InitSurface()
		{
			ResizeRedraw = true;
			Graphics gf = CreateGraphics();
			DrawSurface(gf);
		}

		private CellRelationCheckBox[,] CreateCellRelationCheckboxes()
		{
			GroupBox crmGB = CreateCellRelationGroupBox();

			CellRelationCheckBox[,] crmCB = new CellRelationCheckBox[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (!(i == 1 && j == 1))
					{
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
			return crmCB;
		}

		private GroupBox CreateCellRelationGroupBox()
		{
			return new GroupBox
			{
				Text = "Cell relations",
				Anchor = AnchorStyles.Left | AnchorStyles.Right,
				Width = panel.Width - 20,
				Parent = panel
			};
		}

		public void OnCrmCBChanged(object sender, EventArgs e)
		{
			CellRelationCheckBox cellRelationCheckBox = (CellRelationCheckBox)sender;
			caController.Calc.Vm[cellRelationCheckBox.Row, cellRelationCheckBox.Col] = cellRelationCheckBox.Checked;
		}

		private TextBox CreateLiveTextBox()
		{
			panel.Controls.Add(new Label
			{
				Text = "Create cells on:",
				AutoSize = true
			});

			TextBox livTB = new TextBox
			{
				Anchor = AnchorStyles.Left | AnchorStyles.Right
			};
			livTB.KeyUp += new KeyEventHandler(OnKeyUp);
			livTB.Name = "livTB";
			RefreshLivDthTB(livTB);
			livTB.Validating += OnLivDthTextValidating;
			livTB.Parent = panel;

			ToolTip livTT = new ToolTip();
			livTT.SetToolTip(livTB, "Comma separated numbers of neighboring cells that makes an empty space filled (eg: 0,3,5,8).");

			return livTB;
		}

		private TextBox CreateDeathTextBox()
		{
			panel.Controls.Add(new Label
			{
				Text = "Destroy cells on:",
				AutoSize = true,
				Parent = panel
			});

			TextBox dthTB = new TextBox
			{
				Anchor = AnchorStyles.Left | AnchorStyles.Right
			};
			dthTB.KeyUp += new KeyEventHandler(OnKeyUp);
			dthTB.Name = "dthTB";
			RefreshLivDthTB(dthTB);
			dthTB.Validating += OnLivDthTextValidating;
			dthTB.Parent = panel;

			ToolTip dthTT = new ToolTip();
			dthTT.SetToolTip(dthTB, "Comma separated numbers of neighboring cells that makes a filled space empty (eg: 0,1,4,8).");

			return dthTB;

		}

		public void OnKeyUp(object sender, KeyEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			if (IsValidLivDthText(tb.Text))
			{
				int[] numbers;
				if (tb.Text.Length > 0)
				{
					string[] chars = tb.Text.Split(',');
					numbers = new int[chars.Length];
					int i = 0;
					foreach (string c in chars)
					{
						numbers[i] = Int32.Parse(c);
						i++;
					}
				}
				else
				{
					numbers = new int[0];
				}
				switch (tb.Name)
				{
					case "livTB":
						caController.Calc.ToLive = numbers;
						break;
					case "dthTB":
						caController.Calc.ToDie = numbers;
						break;
				}
			}
		}

		public void OnLivDthTextValidating(object sender, CancelEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			if (!IsValidLivDthText(tb.Text))
			{
				RefreshLivDthTB(tb);
			}
		}

		public bool IsValidLivDthText(string s)
		{
			Match match = Regex.Match(s, "^([0-8](,[0-8]){0,8})?$");
			return match.Success;
		}

		private CheckBox CreateAgeingCheckbox()
		{
			CheckBox ageingCB = new CheckBox
			{
				Location = new Point(10, 40),
				Text = "Ageing",
				Checked = true,
				Parent = panel
			};
			ageingCB.CheckedChanged += new EventHandler(OnAgeingCBChanged);

			return ageingCB;
		}

		public void OnAgeingCBChanged(object sender, EventArgs e)
		{
			caController.Calc.Senescence = ageingCheckBox.Checked;
			gradientEditor.Enabled = ageingCheckBox.Checked;
		}

		private void InitIOButtons()
		{
			InitExportImageButton();
			InitLoadImageButton();

			InitSaveSettingsButton();
			InitLoadSettingsButton();
		}

		private Button InitExportImageButton()
		{
			Button saveImg = new Button
			{
				Text = "Export image...",
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
			};
			saveImg.Click += new EventHandler(OnSaveImgClick);
			saveImg.Parent = panel;

			return saveImg;
		}

		public void OnSaveImgClick(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog
			{
				Title = "Save file as...",
				Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp|GIF Image|*.gif",
				RestoreDirectory = true
			};
			dialog.ShowDialog();

			if (dialog.FileName != "")
			{
				// Saves the Image via a FileStream created by the OpenFile method.
				System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();
				switch (dialog.FilterIndex)
				{
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

		private Button InitLoadImageButton()
		{
			Button loadImg = new Button
			{
				Text = "Import image...",
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
			};
			loadImg.Click += new EventHandler(OnLoadImgClick);
			loadImg.Parent = panel;

			return loadImg;
		}

		public void OnLoadImgClick(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog
			{
				Title = "Choose image file to import",
				Filter = "JPeg Image|*.jpg|Png Image|*.png|Bitmap Image|*.bmp|Gif Image|*.gif",
				RestoreDirectory = true
			};

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				Bitmap bm = LoadBitmap(dialog.FileName);
				caController.LoadSurface(bm);
				Graphics gf = CreateGraphics();
				DrawSurface(gf);
				gf.Dispose();
			}
		}

		public Bitmap LoadBitmap(string filename)
		{
			Bitmap bmp = new Bitmap(filename);
			return bmp;
		}

		private Button InitSaveSettingsButton()
		{
			Button saveSettings = new Button
			{
				Text = "Save config...",
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
			};
			saveSettings.Click += new EventHandler(OnSaveSettingsClick);
			saveSettings.Parent = panel;

			return saveSettings;
		}

		public void OnSaveSettingsClick(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog
			{
				Title = "Save config as...",
				Filter = "CA configuration|*.cac",
				RestoreDirectory = true
			};

			if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != "")
			{
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

		private Button InitLoadSettingsButton()
		{
			Button loadSettings = new Button
			{
				Text = "Load config...",
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
			};
			loadSettings.Click += new EventHandler(OnLoadSettingsClick);
			loadSettings.Parent = panel;

			return loadSettings;
		}

		public void OnLoadSettingsClick(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog
			{
				Title = "Select config to load",
				Filter = "CA configuration|*.cac",
				RestoreDirectory = true
			};

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				startButton.Text = "Start";
				timer.Stop();
				gradientEditor.Enabled = true;
				try
				{
					Model.Serializer.SSettings serializer = new Model.Serializer.SSettings();
					Model.Settings config = serializer.DeSerializeObject(dialog.FileName);
					_resetCA();
					ageingCheckBox.Checked = config.Senescence;
					for (int i = 0; i < 3; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							if (!(i == 1 && j == 1))
								cellRelationCheckboxes[i, j].Checked = config.CellRelation[i, j];
						}
					}
					caController.Calc.ToLive = config.RuleLife;
					caController.Calc.ToDie = config.RuleDeath;
					RefreshLivDthTB(liveTextBox);
					RefreshLivDthTB(deathTextBox);
					gradientEditor.SetMap(config.GradientMap);
					SetTitle(WithDefaultTitle(ConfigNameToRuleName(dialog.FileName)));
				}
				catch (Exception exception)
				{
					MessageBox.Show("Could not load config (corrupted file?).");
					Console.WriteLine(exception.Message);
					_resetCA();
				}
			}
		}

		private Timer CreateTimer()
		{
			Timer timer = new Timer
			{
				Enabled = false,
				Interval = 50
			};
			timer.Tick += new EventHandler(HandleTick);

			return timer;
		}

		protected void HandleTick(object sender, EventArgs e)
		{
			caController.HandleTick();

			Graphics gf = CreateGraphics();
			DrawSurface(gf);
			statusBar.Text = "Generations: " + generations;
		}

		private void InitQuitButton()
		{
			Button quitButton = new Button
			{
				Text = "Quit",
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
			};
			quitButton.Click += new EventHandler(OnQuitClick);
			quitButton.Parent = panel;
		}

		public void OnQuitClick(object sender, EventArgs e)
		{
			Close();
		}

		protected void _resetCA()
		{
			caController.Reset();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (!(i == 1 && j == 1))
					{
						cellRelationCheckboxes[i, j].Checked = caController.Calc.Vm[i, j];
					}
				}
			}
			Graphics gf = CreateGraphics();
			DrawSurface(gf);
			statusBar.Text = "Generations: " + generations;
		}

		public void RefreshLivDthTB(TextBox t)
		{
			int[] numbers;
			switch (t.Name)
			{
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
			if (numbers.Length > 0)
			{
				string[] s = new string[numbers.Length];
				int i = 0;
				foreach (int n in numbers)
				{
					s[i] = n.ToString();
					i++;
				}
				t.Text = String.Join(",", s);
			}
			else
			{
				t.Text = "";
			}
		}

		private static string WithDefaultTitle(string extension)
		{
			return DefaultTitle + " - " + extension;
		}

		private static string ConfigNameToRuleName(string configName)
		{
			string[] path = configName.Split(System.IO.Path.DirectorySeparatorChar);
			path = path[path.Length - 1].Split('.');
			string rule = path[0];
			return rule.Substring(0, 1).ToUpper() + rule.Substring(1).ToLower();
		}

		private void SetTitle(string title)
		{
			Text = title;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			caController.Drawing = true;
			caController.AddCells(new Point(e.X - GetGraphX(), e.Y - GetGraphY()));
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!caController.Drawing)
				return;
			caController.AddCells(new Point(e.X - GetGraphX(), e.Y - GetGraphY()));
			Graphics gf = CreateGraphics();
			DrawSurface(gf);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			caController.Drawing = false;
			caController.X = -1;
			caController.Y = -1;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			DoPage(e.Graphics);
		}

		protected void DoPage(Graphics g)
		{
			DrawSurface(g);
		}

		private void DrawSurface(Graphics g)
		{
			g.DrawImageUnscaled(caController.Surface, GetGraphX(), GetGraphY());
		}

		private int GetGraphX()
		{
			return (ClientSize.Width - caController.Surface.Width - panel.Width) / 2;
		}

		private int GetGraphY()
		{
			return (ClientSize.Height - caController.Surface.Height - statusBar.Height) / 2;
		}

	} // end class

} // end namespace