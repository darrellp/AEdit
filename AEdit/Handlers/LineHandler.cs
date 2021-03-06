﻿using System.Diagnostics;
using AEdit.Consoles;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using static System.Math;
using static AEdit.AEGlobals;
using Console = SadConsole.Console;
using static AEdit.SadGeometry;

namespace AEdit.Handlers
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A line handler for drawing lines on the screen. </summary>
	///
	/// <remarks>	Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	internal class LineHandler : IHandler
	{
		#region Private variables
		private bool _fDragging;
		private Point _ptStart;
		private Point _ptEnd;
		private Color _foreDefault;
		private Color _backDefault;
		#endregion

		#region Constructor
		public LineHandler()
		{
			_foreDefault = Color.White;
			_backDefault = Color.Black;
		}
		#endregion

		#region IHandler methods
		public void Reset()
		{
			_fDragging = false;
		}
		public void Exit() { }

		public void Mouse(MouseConsoleState state, Console console)
		{
			if (state.Mouse.LeftButtonDown && !_fDragging)
			{
				_ptStart = _ptEnd = state.CellPosition;
				_fDragging = true;
			}
			else if (state.Mouse.LeftButtonDown)
			{
				if (state.CellPosition != _ptEnd)
				{
					EraseLine(_ptStart, _ptEnd, console);
					_ptEnd = state.CellPosition;
					DrawLine(_ptStart, _ptEnd, Main.Drawing, _foreDefault, _backDefault);
				}
			}
			else if (_fDragging)
			{
				var boundsPosition = new Point(Min(_ptStart.X, _ptEnd.X), Min(_ptStart.Y, _ptEnd.Y));
				var boundsSize = new Point(Abs(_ptStart.X - _ptEnd.X) + 1, Abs(_ptStart.Y - _ptEnd.Y) + 1);
				Main.SetObject(new Rectangle(boundsPosition, boundsSize));
				_fDragging = false;
			}
		}

		public void Keyboard(Keyboard info, Console console) { }
		public void Update(ControlsConsole ModeSpecificPanel)
		{
			var lineControls = ModeSpecificPanel as LineControls;
			Debug.Assert(lineControls != null, "Non-paint controls being passed to paint handler");
			_foreDefault = lineControls.Foreground;
			_backDefault = lineControls.Background;
		}

		#endregion
	}
}
