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

		public ControlPanel(int width, int height, Color fore, Color back) : base(width, height)
		{
			DrawBox(new Rectangle(0, 0, width, height),
				new Cell(Color.White, Color.Black), null, ConnectedLineThick);
			var button = new SadConsole.Controls.Button(_buttonWidth, 1)
			{
				Text = "Click",
				Position = new Point((width - _buttonWidth) /2, 3)
			};
			Fill(Color.White, Color.Wheat, 0);

			button.Click += (s, a) => Window.Message("This has been clicked!", "Close");
			Add(button);
		}

		public ControlPanel(int width, int height) : this(width, height, Color.Black, Color.Wheat) { }

	}
}
