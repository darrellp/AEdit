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

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Creates an undo. </summary>
		///
		/// <remarks>	This should be thought of as "making a snapshot so new changes can be made" - i.e.,
		/// 			it should be done immediately BEFORE new changes are made, not after they are made.
		/// 			Darrell Plank, 11/20/2018. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void CreateUndo()
		{
			var oldDisplay = CurrentDisplay;
			AdvanceScreenIndex();
			oldDisplay.Copy(CurrentDisplay);
			Program.StartingConsole.Children.Remove(oldDisplay);
			CurrentDisplay.Mode = oldDisplay.Mode;
			Program.StartingConsole.Children.Add(CurrentDisplay);
			CurrentDisplay.IsFocused = true;
		}

		public void PerformUndo()
		{
			var oldDisplay = CurrentDisplay;
			if (!RetreatScreenIndex())
			{
				// Off the end of the stack
				return;
			}

			Program.StartingConsole.Children.Remove(oldDisplay);
			CurrentDisplay.Mode = oldDisplay.Mode;
			Program.StartingConsole.Children.Add(CurrentDisplay);
			CurrentDisplay.IsFocused = true;
		}

		private void AdvanceScreenIndex()
		{
			_iCurrent = (_iCurrent + 1) % (UndoCount + 1);
			if (_iCurrent == _iLast)
			{
				_iLast = (_iLast + 1) % (UndoCount + 1);
			}
		}

		private bool RetreatScreenIndex()
		{
			var iRetreat = (_iCurrent + UndoCount) % (UndoCount + 1);
			if (iRetreat == _iLast)
			{
				return false;
			}

			_iCurrent = iRetreat;
			return true;
		}
	}
}
