using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole.Input;
using Console = SadConsole.Console;
using static AEdit.SadGeometry;

namespace AEdit.Handlers
{
	class LineHandler : IHandler
	{
		private bool _fDragging;
		private Point _ptStart;
		private Point _ptEnd;
		private List<int> _lineMemory;

		public void Reset()
		{
			_fDragging = false;
		}
		public void Exit() { }

		public bool Mouse(MouseConsoleState state, Console console)
		{
			if (state.Mouse.LeftButtonDown && !_fDragging)
			{
				_ptStart = _ptEnd = state.CellPosition;
				_fDragging = true;
				_lineMemory = DrawLineMemory(_ptStart, _ptEnd, console);
			}
			else if (state.Mouse.LeftButtonDown)
			{
				if (state.CellPosition != _ptEnd)
				{
					//EraseLine(_ptStart, _ptEnd, console);
					DrawLineFromMemory(_ptStart, _ptEnd, console, _lineMemory);
					_ptEnd = state.CellPosition;
					_lineMemory = DrawLineMemory(_ptStart, _ptEnd, console);
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
	}
}
