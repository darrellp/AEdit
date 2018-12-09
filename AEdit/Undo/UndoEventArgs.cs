using System;

namespace AEdit.Undo
{
	class UndoEventArgs : EventArgs
	{
		public EditRecord UndoRecord { get; }
		public bool IsUndo { get; }

		public UndoEventArgs(EditRecord undoRecord, bool isUndo)
		{
			UndoRecord = undoRecord;
			IsUndo = isUndo;
		}
	}
}
