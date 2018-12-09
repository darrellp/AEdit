using System;

namespace AEdit.Undo
{
	class UndoEventArgs : EventArgs
	{
		public IUndoRecord UndoRecord { get; private set; }
		public bool IsUndo { get; private set; }

		public UndoEventArgs(IUndoRecord undoRecord, bool isUndo)
		{
			UndoRecord = undoRecord;
			IsUndo = isUndo;
		}
	}
}
