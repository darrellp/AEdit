using System;
using AEdit.Consoles;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Input;
using SadConsole.Themes;

namespace AEdit.Windows
{
	internal class ColorPicker : Window
	{
		private readonly DrawingSurface _colorSwatch;
		private readonly ScrollBar _red;
		private readonly ScrollBar _green;
		private readonly ScrollBar _blue;
		private readonly ScrollBar _alpha;
		private Action<ColorPicker> _fnSave;
		public const int WidthConst = 130;
		public const int HeightConst = 9;

		public ColorPicker(Action<ColorPicker> fn) : base(WidthConst, HeightConst)
		{
			Title = "Color Picker";
			IsExclusiveMouse = false;
			Center();
			_fnSave = fn;

			_red = ScrollBar.Create(Orientation.Horizontal, 123);
			_red.Position = new Point(6, 1);
			_red.Maximum = 255;
			_red.ValueChanged += (s, a) => UpdateColorSwatch();
			Add(_red);
			var label = new DrawingSurface(3, 1)
			{
				Position = new Point(1, 1),

			};
			label.Surface.Print(0, 0, "Red", Colors.Yellow, Color.Transparent);
			Add(label);

			_green = ScrollBar.Create(Orientation.Horizontal, 123);
			_green.Position = new Point(6, 2);
			_green.Maximum = 255;
			_green.ValueChanged += (s, a) => UpdateColorSwatch();
			Add(_green);
			label = new DrawingSurface(3, 1)
			{
				Position = new Point(1, 2),

			};
			label.Surface.Print(0, 0, "Green", Colors.Yellow, Color.Transparent);
			Add(label);

			_blue = ScrollBar.Create(Orientation.Horizontal, 123);
			_blue.Position = new Point(6, 3);
			_blue.Maximum = 255;
			_blue.ValueChanged += (s, a) => UpdateColorSwatch();
			Add(_blue);
			label = new DrawingSurface(3, 1)
			{
				Position = new Point(1, 3),

			};
			label.Surface.Print(0, 0, "Blue", Colors.Yellow, Color.Transparent);
			Add(label);

			_alpha = ScrollBar.Create(Orientation.Horizontal, 123);
			_alpha.Position = new Point(6, 4);
			_alpha.Maximum = 255;
			_alpha.ValueChanged += (s, a) => UpdateColorSwatch();
			Add(_alpha);
			label = new DrawingSurface(5, 1)
			{
				Position = new Point(1, 4),

			};
			label.Surface.Print(0, 0, "Alpha", Colors.Yellow, Color.Transparent);
			Add(label);

			var button = new Button(9, 1)
			{
				Position = new Point(4, Height - 2),
				Text = "Save"
			};
			Add(button);

			button.Click += (btn, args) => { SaveColor(); };

			_colorSwatch = new DrawingSurface(Width - 2, 1)
			{
				Position = new Point(1, Height - 4)
			};
			UpdateColorSwatch();
			Add(_colorSwatch);
			
			CPPalette.Singleton.Position = new Point(1, Height - 3);
			CPPalette.Singleton.ColorPicker = this;
			Children.Add(CPPalette.Singleton);
		}

		public void SaveColor()
		{
			Hide();
			CPPalette.Singleton.InsertMyPaletteColor(CurColor);
			_fnSave(this);
		}

		public Color CurColor
		{
			get
			{
				var r = _red.Value;
				var g = _green.Value;
				var b = _blue.Value;
				var a = _alpha.Value;
				var ret =  new Color(r, g, b, a);
				return ret;
			}
			set
			{
				_red.Value = value.R;
				_green.Value = value.G;
				_blue.Value = value.B;
				_alpha.Value = value.A;
			}
		}

		private void UpdateColorSwatch()
		{
			_colorSwatch.Surface.Print(0, 0, new string(' ', Width - 2), Colors.White, CurColor);
		}

		public override bool ProcessMouse(MouseConsoleState state)
		{
			if (CPPalette.Singleton.ProcessMouse(state))
			{
				return true;
			}
			if (base.ProcessMouse(state) || IsExclusiveMouse)
			{
				if (CapturedControl != null)
					CapturedControl.ProcessMouse(state);

				else
				{
					foreach (var control in _controls)
					{
						if (control.IsVisible && control.ProcessMouse(state))
							break;
					}
				}

				return true;
			}

			return false;
		}
	}
}
