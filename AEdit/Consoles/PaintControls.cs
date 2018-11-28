using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;

namespace AEdit.Consoles
{
	internal class PaintControls : ControlsConsole
	{
		public PaintControls(int width, int height) : base(width, height)
		{
			DefaultBackground = Color.Transparent;
			DefaultForeground = Color.White;
			Clear();

			var button = new Button(9, 1)
			{
				Text = "Foregnd ",
				Position = new Point(1, 2),
			};
			button.Click += (s, a) => Window.Message("This has been clicked!", "Close");
			Add(button);

			button = new Button(10, 1)
			{
				Text = "Backgrnd",
				Position = new Point(1, 4),
			};
			button.Click += (s, a) => Window.Message("This has been clicked!", "Close");
			Add(button);

			PrintLabels();
		}

		public override void Invalidate()
		{
			base.Invalidate();

			PrintLabels();
		}

		private void PrintLabels()
		{
			Print(1, 1, "PAINT", Colors.BlueDark);
		}
	}
}
