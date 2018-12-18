using Microsoft.Xna.Framework;
using SadConsole.Controls;
using SadConsole.Themes;

namespace AEdit.Consoles
{
	class CanvasControl : EditControl
	{
		public CanvasControl(int width, int height) : base(width, height)
		{
			var label = new DrawingSurface(20, 1)
			{
				Position = new Point(1, 1),

			};
			label.Surface.Print(0, 0, "CANVAS", Colors.Yellow, Color.Transparent);
			Add(label);
		}

		public override object GetParameterInfo()
		{
			// Nothing to do right now
			return null;
		}

		public override void SetParameters(object parms)
		{
			// Nothing to do right now
		}

		public override bool Apply(EditObject edit)
		{
			// Nothing to do right now
			return true;
		}

		public override object GetParmValue(string parm)
		{
			// Nothing to do right now
			return null;
		}
	}
}
