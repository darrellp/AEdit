using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole;

namespace AEdit.Consoles
{
	class ControlPanel : ControlsConsole
	{
		readonly int _buttonWidth = 11;

		public ControlPanel(int width, int height) : base(width, height)
		{
			Fill(Color.White, Color.Wheat, 0);
			CreateButtons(width, height);
		}

		private void CreateButtons(int width, int height)
		{
			var btnAbout = new SadConsole.Controls.Button(_buttonWidth, 1)
			{
				Text = "About",
				Position = new Point((width - _buttonWidth) / 2, 3)
			};

			btnAbout.Click += (s, a) => Window.Message("AEdit by Darrell Plank", "Close");
			Add(btnAbout);

			var btnUndo = new SadConsole.Controls.Button(_buttonWidth, 1)
			{
				Text = "Undo",
				Position = new Point((width - _buttonWidth) / 2, 4)
			};

			btnUndo.Click += (s, a) => Program.Undos.PerformUndo();
			Add(btnUndo);
		}

	}
}
