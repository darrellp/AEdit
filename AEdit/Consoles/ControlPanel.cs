using System;
using AEdit.Undo;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Themes;
using static AEdit.Undo.Undo;

namespace AEdit.Consoles
{
	internal class ControlPanel : ControlsConsole
	{
		#region Private variables
		private const int ButtonWidth = 10;

		private static readonly (string, EventHandler)[] ButtonInfo = new (string, EventHandler)[]
		{
			("Undo", (s, a) => PerformUndo()),
			("Redo", (s, a) => PerformRedo()),
			("Clear", (s, a) =>
			{
				AddRecord(new ClearRecord());
				Program.MainDisplay.Clear();
			}),
			("Line", (s, a) => Program.MainDisplay.Mode = EditMode.Line),
			("Paint", (s, a) => Program.MainDisplay.Mode = EditMode.Brush),
		};

		private static readonly int ButtonRowCount = (ButtonInfo.Length + 1) / 2;

		private static readonly EditControl[] ModeControlPanels =
		{
			new PaintControls(Program.DefaultControlWidth, Program.DefaultHeight - ButtonRowCount),
			new LineControls(Program.DefaultControlWidth, Program.DefaultHeight - ButtonRowCount)
		};
		private EditControl _modeSpecificControls;
		#endregion

		#region Public Properties
		public EditControl EditControls => _modeSpecificControls;
		#endregion

		#region Constructor
		public ControlPanel(int width, int height) : base(width, height)
		{
			Fill(Color.White, Colors.BlueDark, 0);
			CreateButtons(width);
			foreach (var panel in ModeControlPanels)
			{
				panel.Position = new Point(0, ButtonRowCount);
			}
		}

		public void InstallModeSpecificControls(EditMode mode)
		{
			if (mode == null || _modeSpecificControls != null && mode == _modeSpecificControls.Mode)
			{
				return;
			}
			var index = (int) mode;
			if (index >= ModeControlPanels.Length)
			{
				return;
			}

			if (_modeSpecificControls != null)
			{
				Children.Remove(_modeSpecificControls);
			}

			if (index >= 0)
			{
				_modeSpecificControls = ModeControlPanels[index];
				Children.Add(_modeSpecificControls);
			}
		}

		private void CreateButtons(int width)
		{
			var btnSpacing = (width - 2 * ButtonWidth) / 3;
			var leftCol = btnSpacing;
			var rightCol = btnSpacing * 2 + ButtonWidth;
			var row = 0;
			var col = leftCol;

			foreach (var (title, handler) in ButtonInfo)
			{
				var btn = new SadConsole.Controls.Button(ButtonWidth, 1)
				{
					Text = title,
					Position = new Point(col, row)
				};
				btn.Click += handler;
				Add(btn);
				if (col == leftCol)
				{
					col = rightCol;
				}
				else
				{
					col = leftCol;
					row++;
				}
			}
		}
		#endregion

		#region Operations
		public void UpdateHandler()
		{
			Program.Drawing.Handler.Update(_modeSpecificControls);
		}
		#endregion
	}
}
