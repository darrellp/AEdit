using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEdit.Consoles;
using Microsoft.Xna.Framework;

namespace AEdit
{
	internal class Undo
	{
		// Count of undo operations
		private const int UndoCount = 5;

		internal MainDisplay CurrentDisplay => undoStack[_iCurrent];

		// We need the undo screens plus the current one
		readonly MainDisplay[] undoStack = new MainDisplay[UndoCount + 1];

		// _iCurrent is the current screen.  _iCurrent - 1 (mod UndoCount) is the undo screen
		// unless it's equal to _iLast in which case we've "underflowed" the stack.
		private int _iLast = UndoCount, _iCurrent;

		public Undo(EditMode initMode, int width, int height)
		{
			for (var i = 0; i < UndoCount + 1; i++)
			{
				undoStack[i] = new MainDisplay(width, height)
					{ Mode = initMode, Position = Program.MainDisplayPosition};
				undoStack[i].Fill(Color.White, Color.MidnightBlue, 0);

			}
		}

		private void AdvanceScreenIndex()
		{
			_iCurrent = (_iCurrent + 1) % (UndoCount + 1);
			if (_iCurrent == _iLast)
			{
				_iLast = (_iLast + 1) % (UndoCount + 1);
			}
		}

		public void CreateUndo()
		{
			var undoScreen = CurrentDisplay;
			AdvanceScreenIndex();
			undoScreen.Copy(CurrentDisplay);
			Program.StartingConsole.Children.Remove(undoScreen);
			Program.StartingConsole.Children.Add(CurrentDisplay);
		}
	}
}
