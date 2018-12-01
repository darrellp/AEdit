using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;

namespace AEdit.Consoles
{
	internal class PaintControls : ControlsConsole
	{
		public Color Foreground { get; private set; }
		public Color Background { get; private set; }

		public PaintControls(int width, int height) : this(width, height, Color.White, Color.Black) { }

		public PaintControls(int width, int height, Color fgnd, Color bgnd) : base(width, height)

		{
			DefaultBackground = Color.Transparent;
			Clear();

			Foreground = fgnd;
			Background = bgnd;

			ControlHelpers.SetColorButton(this, new Point(1, 3), "Foregnd", Foreground, c => Foreground = c);
			ControlHelpers.SetColorButton(this, new Point(1, 4), "Backgrnd", Background, c => Background = c);

			var label = new DrawingSurface(20, 1)
			{
				Position = new Point(1, 1),

			};
			label.Surface.Print(0, 0, "PAINT", Colors.Yellow, Color.Transparent);
			Add(label);
		}
	}
}
