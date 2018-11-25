using System.Diagnostics;
using AEdit.Consoles;

namespace AEdit
{
	internal class Undo
	{
		#region Public Variables
		public MainDisplay CurrentDisplay => _undoStack[_iCurrent];
		#endregion

		#region Private Variables
		// Count of undo operations
		private const int UndoCount = 5;

		// We need the undo screens plus the current one
		private readonly MainDisplay[] _undoStack = new MainDisplay[UndoCount + 1];

		// _iCurrent is the current screen.  _iCurrent - 1 (mod UndoCount) is the undo screen
		// unless it's equal to _iLast in which case we've "underflowed" the stack.
		private int _iLast, _iCurrent, _iRedoLimit;
		#endregion

		#region Constructor
		public Undo(EditMode initMode, int width, int height)
		{
			for (var i = 0; i < UndoCount + 1; i++)
			{
				_undoStack[i] = new MainDisplay(width, height)
					{ Mode = initMode, Position = Program.MainDisplayPosition};
			}
		}
		#endregion

		#region Undo operations
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

		public void PerformRedo()
		{
			var oldDisplay = CurrentDisplay;
			if (_iCurrent != _iRedoLimit)
			{
				_iCurrent = (_iCurrent + 1) % (UndoCount + 1);
			}

			Program.StartingConsole.Children.Remove(oldDisplay);
			CurrentDisplay.Mode = oldDisplay.Mode;
			Program.StartingConsole.Children.Add(CurrentDisplay);
			CurrentDisplay.IsFocused = true;
		}

		private void AdvanceScreenIndex()
		{
			var next = (_iCurrent + 1) % (UndoCount + 1);
			if (_iCurrent == _iRedoLimit)
			{
				_iRedoLimit = next;
			}
	
			_iCurrent = next;
			if (_iCurrent == _iLast)
			{
				_iLast = (_iLast + 1) % (UndoCount + 1);
			}
		}
		private bool RetreatScreenIndex()
		{
			if (_iCurrent == _iLast)
			{
				return false;
			}
			var iRetreat = (_iCurrent + UndoCount) % (UndoCount + 1);

			_iCurrent = iRetreat;
			return true;
		}

		[Conditional("DEBUG")]
		// ReSharper disable once UnusedMember.Local
		private void PrintIndices()
		{
			Program.AETraceLine($"_iLast = {_iLast} : _iCurrent = {_iCurrent} : _iRedoLimit = {_iRedoLimit}");
		}
		#endregion
	}
}
