using System;
using Microsoft.Xna.Framework;
using SadConsole;

namespace AEdit.Consoles
{
	internal class ControlPanel : ControlsConsole
	{
		#region Private variables
		const int ButtonWidth = 10;
		#endregion

		#region Constructor
		public ControlPanel(int width, int height) : base(width, height)
		{
			Fill(Color.White, Color.Wheat, 0);
			CreateButtons(width);
		}

		private void CreateButtons(int width)
		{
			(string, EventHandler)[] buttonInfo = new (string, EventHandler)[]
			{
				("Undo", (s, a) => Undo.Undo.PerformUndo()),
				("Redo", (s, a) => Undo.Undo.PerformRedo()),
				("Clear", (s, a) => Program.MainDisplay.Clear()),
				("Line", (s, a) => Program.MainDisplay.Mode = EditMode.Line),
				("Paint", (s, a) => Program.MainDisplay.Mode = EditMode.Brush),
			};

			var btnSpacing = (width - 2 * ButtonWidth) / 3;
			var leftCol = btnSpacing;
			var rightCol = btnSpacing * 2 + ButtonWidth;
			var row = 0;
			var col = leftCol;

			foreach (var (title, handler) in buttonInfo)
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
	}
}
