using Microsoft.Xna.Framework;
using SadConsole.Surfaces;
using Console = SadConsole.Console;

namespace AEdit
{
	class EditObject : Console
	{
		public EditObject(SurfaceBase surface, Rectangle rect) : base(rect.Width, rect.Height)
		{
			surface.Copy(rect.X, rect.Y, rect.Width, rect.Height, this, 0, 0);
			Position = new Point(rect.X, rect.Y);
		}
	}
}
