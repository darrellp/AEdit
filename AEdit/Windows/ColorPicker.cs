using System;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;

namespace AEdit.Windows
{
	internal class ColorPicker : Window
	{
		private readonly DrawingSurface _colorSwatch;
		private readonly TextBox _red;
		private readonly TextBox _green;
		private readonly TextBox _blue;

		public ColorPicker(Action<ColorPicker> fn) : base(20, 7)
		{
			Title = "Color Picker";
			Center();

			_red = new TextBox(5)
			{
				Text = "255",
				Position = new Point(2, 1),
				IsNumeric = true
			};
			Add(_red);
			_red.TextChangedPreview += (s, a) => UpdateColorSwatch();

			_green = new TextBox(5)
			{
				Text = "255",
				Position = new Point(2, 2),
				IsNumeric = true
			};
			Add(_green);
			_green.TextChangedPreview += (s, a) => UpdateColorSwatch();

			_blue = new TextBox(5)
			{
				Text = "255",
				Position = new Point(2, 3),
				IsNumeric = true
			};
			Add(_blue);
			_blue.TextChangedPreview += (s, a) => UpdateColorSwatch();

			var button = new Button(9, 1)
			{
				Position = new Point(4, 5),
				Text = "Close"
			};
			Add(button);

			button.Click += (btn, args) =>
			{
				Hide();
				fn(this);
			};

			_colorSwatch = new DrawingSurface(5, 1)
			{
				Position = new Point(8, 1)

			};
			UpdateColorSwatch();
			Add(_colorSwatch);
		}

		public Color GetColor
		{
			get
			{
				var r = (byte)(_red.EditingText == "" ? 0 : int.Parse(_red.EditingText).ClipTo(0, 255));
				var g = (byte)(_green.EditingText == "" ? 0 : int.Parse(_green.EditingText).ClipTo(0, 255));
				var b = (byte)(_blue.EditingText == "" ? 0 : int.Parse(_blue.EditingText).ClipTo(0, 255));
				var ret =  new Color(r, g, b);
				return ret;
			}
		}

		private void UpdateColorSwatch()
		{
			_colorSwatch.Surface.Print(0, 0, "     ", Colors.White, GetColor);
		}
	}
}
