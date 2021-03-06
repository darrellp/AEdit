﻿using System;
using AEdit.Windows;
using Microsoft.Xna.Framework;
using SadConsole.Controls;
using SadConsole.Themes;
using static AEdit.AEGlobals;

namespace AEdit.Consoles
{
	static class ControlHelpers
	{

		public static DrawingSurface SetColorButton(EditControl controls, Point btnPos, string text, Color color, Action<Color> fnAssign)
		{

			var colorSwatch = new DrawingSurface(5, 1)
			{
				Position = new Point(12, 0) + btnPos
			};
			UpdateColorSwatch(colorSwatch, color);
			controls.Add(colorSwatch);

			var button = new Button(9, 1)
			{
				Text = text,
				Position = btnPos
			};
			button.Click += (s, a) =>
			{
				var cp = new ColorPicker(p =>
				{
					var newColor = p.CurColor;
					fnAssign(newColor);
					UpdateColorSwatch(colorSwatch, newColor);
					Ctrls.UpdateHandler();
				});
				cp.CurColor = (Color) controls.GetParmValue(text);
				cp.Show(true);
			};
			controls.Add(button);
			return colorSwatch;
		}

		public static void UpdateColorSwatch(DrawingSurface swatch, Color color)
		{
			var blanks = new string(' ', swatch.Width);
			swatch.Surface.Print(0, 0, blanks, Colors.White, color);
		}

	}
}
