using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SadConsole.Input;
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
		private List<int> _lineMemory;
		#endregion

		#region IHandler methods
		public void Reset()
		{
			_fDragging = false;
		}
		public void Exit() { }

		public bool Mouse(MouseConsoleState state, Console console)
		{
			if (!state.IsOnConsole)
			{
				return true;
			}
			if (state.Mouse.LeftButtonDown && !_fDragging)
			{
				// Create an undo point before we make any changes
				Program.Undos.CreateUndo();

				_ptStart = _ptEnd = state.CellPosition;
				_fDragging = true;
				_lineMemory = DrawLineMemory(_ptStart, _ptEnd, Program.MainDisplay);
			}
			else if (state.Mouse.LeftButtonDown)
			{
				if (state.CellPosition != _ptEnd)
				{
					DrawLineFromMemory(_ptStart, _ptEnd, console, _lineMemory);
					_ptEnd = state.CellPosition;
					_lineMemory = DrawLineMemory(_ptStart, _ptEnd, Program.MainDisplay);
				}
			}
			else if (_fDragging)
			{
				_fDragging = false;
			}

			return false;
		}

		public bool Keyboard(Keyboard info, Console console)
		{
			return false;
		}
		#endregion
	}
}
