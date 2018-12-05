using System.Diagnostics;
using Microsoft.Xna.Framework;
using SadConsole.Controls;
using SadConsole.Themes;

namespace AEdit.Consoles
{
	internal class PaintControls : EditControl
	{
		private readonly DrawingSurface _foreSwatch;
		private readonly DrawingSurface _backSwatch;

		public Color Foreground { get; private set; }
		public Color Background { get; private set; }
		public override EditMode Mode => EditMode.Brush;

		public PaintControls(int width, int height) : this(width, height, Color.White, Color.Black) { }

		private PaintControls(int width, int height, Color fgnd, Color bgnd) : base(width, height)

		{
			DefaultBackground = Color.Transparent;
			Clear();

			Foreground = fgnd;
			Background = bgnd;

			_foreSwatch = ControlHelpers.SetColorButton(this, new Point(1, 3), "Foregnd", Foreground, c => Foreground = c);
			_backSwatch = ControlHelpers.SetColorButton(this, new Point(1, 4), "Backgnd", Background, c => Background = c);

			var label = new DrawingSurface(20, 1)
			{
				Position = new Point(1, 1),

			};
			label.Surface.Print(0, 0, "PAINT", Colors.Yellow, Color.Transparent);
			Add(label);
		}

		public override object GetParameterInfo()
		{
			return (Foreground, Background);
		}

		public override void SetParameters(object parms)
		{
			(Foreground, Background) = ((Color, Color))parms;
			ControlHelpers.UpdateColorSwatch(_foreSwatch, Foreground);
			ControlHelpers.UpdateColorSwatch(_backSwatch, Background);
		}

		public override bool Apply(EditObject edit)
		{
			if (edit.Mode != EditMode.Brush)
			{
				return false;
			}
			var (foreground, background) = ((Color, Color))edit.Parms;
			if (foreground == Foreground && background == Background)
			{
				return false;
			}
			edit.ApplyColors(Foreground, Background);
			edit.Parms = GetParameterInfo();
			return true;
		}

		public override object GetParmValue(string parm)
		{
			switch (parm)
			{
				case "Foregnd":
					return Foreground;

				case "Backgnd":
					return Background;

				default:
					Debug.Assert(false, "Bad parm type");
					break;
			}

			return null;
		}
	}
}
