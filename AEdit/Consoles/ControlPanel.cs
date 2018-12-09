using System;
using AEdit.Undo;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;
using static AEdit.AEGlobals;
using static AEdit.Program;
using static AEdit.Undo.Undo;

namespace AEdit.Consoles
{
	internal class ControlPanel : ControlsConsole
	{
		#region Private variables
		private const int ButtonWidth = 10;
		private const int LayerHeight = 19;
		private const int BelowLayers = 0;

		private static readonly (string, EventHandler)[] ButtonInfo = new (string, EventHandler)[]
		{
			("Undo", (s, a) => PerformUndo()),
			("Redo", (s, a) => PerformRedo()),
			("Clear", (s, a) =>
			{
				AddUndoRecord(new ClearRecord());
				Main.ApplyClear();
			}),
			("Delete", (s, a) => Main.DeleteSelected()),
			("Copy", (s, a) => Main.CopySelected()),
			("SCopy", (s, a) => Main.CopyScreen()),
			("HCopy", (s, a) => Main.CopyHtml()),
			("Apply", (s, a) =>
			{
				if (Selected != null)
				{
					var oldParms = Selected.Parms;
					var record = new ApplyRecord(Selected, Main.Mode, oldParms, _modeSpecificControls.GetParameterInfo());
					if (Main.ApplyApply(record))
					{
						AddUndoRecord(record);
					}
				}
			}),
			("Line", (s, a) => Main.Mode = EditMode.Line),
			("Paint", (s, a) => Main.Mode = EditMode.Brush),
		};

		private static readonly int ButtonRowCount = (ButtonInfo.Length + 1) / 2;
		private static readonly int ModeControlHeight = DefaultHeight - ButtonRowCount - LayerHeight - BelowLayers;
		private static readonly EditControl[] ModeControlPanels =
		{
			new PaintControls(DefaultControlWidth, ModeControlHeight),
			new LineControls(DefaultControlWidth, ModeControlHeight)
		};
		private static EditControl _modeSpecificControls;
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

			var layers = new LayersControl(width - 1, LayerHeight)
			{
				Position = new Point(0, height - LayerHeight - BelowLayers)
			};
			Add(layers);

			var btnUp = new Button(1, 1)
			{
				Text = "\x1E",
				Position = new Point(width - 1, height - LayerHeight - BelowLayers)
			};
			btnUp.Click += BtnUp_Click;
			Add(btnUp);

			var btnDown = new Button(1, 1)
			{
				Text = "\x1F",
				Position = new Point(width - 1, height - LayerHeight - BelowLayers + 1)
			};
			btnDown.Click += BtnDown_Click;
			Add(btnDown);
			
		}

		private void BtnUp_Click(object sender, EventArgs e) => DoRaiseEditEvent(Selected, EditAction.MoveUp);
		private void BtnDown_Click(object sender, EventArgs e) => DoRaiseEditEvent(Selected, EditAction.MoveDown);

		public void InstallModeSpecificControls(EditMode mode)
		{
			if (mode == EditMode.Null || _modeSpecificControls != null && mode == _modeSpecificControls.Mode)
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
				var btn = new Button(ButtonWidth, 1)
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
		public void Enable(bool doEnable)
		{
			foreach (var control in Controls)
			{
				control.IsEnabled = doEnable;
			}

			_modeSpecificControls.Enable(false);
		}

		public void UpdateHandler()
		{
			Drawing.Handler.Update(_modeSpecificControls);
		}
		#endregion
	}
}
