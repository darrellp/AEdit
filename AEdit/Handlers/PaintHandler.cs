using System;
using System.Diagnostics;
using System.Linq;
using AEdit.Consoles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Input;
using Console = SadConsole.Console;
using Keyboard = SadConsole.Input.Keyboard;
using static AEdit.SadGeometry;



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
		private Point _ptLast;
		private Color _foreDefault;
		private Color _backDefault;
		private LineBrush _lineBrush;
		#endregion

		#region Constructor
		public PaintHandler()
		{
			_foreDefault = Color.White;
			_backDefault = Color.Transparent;
			_lineBrush = new LineBrush(new string(_drawChar, 11));
		}
		#endregion

		#region IHandler members
		public void Reset() { }
		public void Exit() { }

		public void Mouse(MouseConsoleState state, Console console)
		{
			if (state.Mouse.LeftButtonDown)
			{
				if (!_fDragging)
				{
					_bounds = new Rectangle(state.CellPosition, new Point(1, 1));
					_ptLast = new Point(-1, -1);
					_fDragging = true;
				}
				var pt = state.CellPosition;
				if (pt == _ptLast)
				{
					return;
				}

				if (_ptLast.X >= 0 && (Math.Abs(pt.X - _ptLast.X) > 1 || Math.Abs(pt.Y - _ptLast.Y) > 1))
				{
					DrawLine(_ptLast, pt, Program.MainDisplay.Drawing, _foreDefault, _backDefault, _lineBrush);
				}
				_ptLast = pt;
				Program.MainDisplay.Drawing.SetGlyph(pt.X, pt.Y, _drawChar);
				Program.MainDisplay.Drawing.SetForeground(pt.X, pt.Y, _foreDefault);
				Program.MainDisplay.Drawing.SetBackground(pt.X, pt.Y, _backDefault);
				if (_bounds.Contains(pt))
				{
					return;
				}

				UpdateBounds(pt);
				return;
			}

			if (_fDragging)
			{
				Program.MainDisplay.SetObject(_bounds);
				_fDragging = false;
			}
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

		public void Keyboard(Keyboard info, Console console)
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
					_lineBrush = new LineBrush(new string(_drawChar, 11));
				}
			}
		}

		public void Update(ControlsConsole ModeSpecificPanel)
		{
			// ReSharper disable once UsePatternMatching
			var paintControls = ModeSpecificPanel as PaintControls;
			Debug.Assert(paintControls != null, "Non-paint controls being passed to paint handler");
			_foreDefault = paintControls.Foreground;
			_backDefault = paintControls.Background;
		}
		#endregion
	}
}
