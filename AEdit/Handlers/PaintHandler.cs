using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole.Input;
using Console = SadConsole.Console;
using Keyboard = SadConsole.Input.Keyboard;


namespace AEdit.Handlers
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A pencil handler. </summary>
	///
	/// <remarks>	Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	class PaintHandler : IHandler
	{
		#region Private Variables
		private char _drawChar = 'X';
		private bool _fDragging;
		private Rectangle _bounds;
		private Point _ptLast = new Point(-1, -1);
		#endregion

		#region IHandler members
		public void Reset() { }
		public void Exit() { }

		public bool Mouse(MouseConsoleState state, Console console)
		{
			if (!state.IsOnConsole)
			{
				return true;
			}
			if (state.Mouse.LeftButtonDown)
			{
				if (!_fDragging)
				{
					_bounds = new Rectangle(state.CellPosition, new Point(1, 1));
					_fDragging = true;
				}
				var pt = state.CellPosition;
				if (pt == _ptLast)
				{
					return false;
				}

				_ptLast = pt;
				Program.MainDisplay.Drawing.SetGlyph(pt.X, pt.Y, _drawChar);
				Program.MainDisplay.Drawing.SetForeground(pt.X, pt.Y, Color.White);
				if (_bounds.Contains(pt))
				{
					return false;
				}

				UpdateBounds(pt);
				return false;
			}
			else if (_fDragging)
			{
				Program.MainDisplay.SetObject(_bounds);
				_fDragging = false;
			}

			return true;
		}

		private void UpdateBounds(Point pt)
		{
			if (pt.X < _bounds.X)
			{
				_bounds.Width = _bounds.Right - pt.X;
				_bounds.X = pt.X;
			}
			else if (pt.X >= _bounds.Right)
			{
				_bounds.Width = pt.X - _bounds.X + 1;
			}

			if (pt.Y < _bounds.Y)
			{
				_bounds.Height = _bounds.Bottom - pt.Y;
				_bounds.Y = pt.Y;
			}
			else if (pt.Y >= _bounds.Bottom)
			{
				_bounds.Height = pt.Y - _bounds.Y + 1;
			}
		}

		public bool Keyboard(Keyboard info, Console console)
		{
			if (!info.IsKeyDown(Keys.LeftControl) &&
			    !info.IsKeyDown(Keys.LeftAlt) &&
			    !info.IsKeyDown(Keys.RightAlt) &&
			    !info.IsKeyDown(Keys.RightControl))
			{
				var input = info.KeysPressed.Select(c => c.Character).FirstOrDefault(c => c != '\0');
				if (input != '\0')
				{
					_drawChar = input;
				}
			}

			return false;
		}
		#endregion
	}
}
