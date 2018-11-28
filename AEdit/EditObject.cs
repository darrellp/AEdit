using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using SadConsole.Surfaces;
using Console = SadConsole.Console;

namespace AEdit
{
	class EditObject : Console
	{
		private Point _startPoint;

		public EditObject(SurfaceBase surface, Rectangle rect) : base(rect.Width, rect.Height)
		{
			surface.Copy(rect.X, rect.Y, rect.Width, rect.Height, this, 0, 0);
			Position = new Point(rect.X, rect.Y);
		}

		public void MouseMoveFromParent(Point pt)
		{
			if (!Global.MouseState.LeftButtonDown)
			{
				Program.DraggedObject = null;
			}
			else if (pt != _startPoint)
			{
				Position += pt - _startPoint;
				_startPoint = pt;
			}
		}
		public override bool ProcessMouse(MouseConsoleState state)
		{
			if (!state.IsOnConsole)
			{
				return false;
			}
			if (state.Mouse.LeftButtonDown && this[state.CellPosition.X, state.CellPosition.Y].Glyph != 0)
			{
				Program.DraggedObject = this;
				// Get position relative to parent
				_startPoint = state.CellPosition + Position;
				
				return true;
			}

			return false;
		}
	}
}
