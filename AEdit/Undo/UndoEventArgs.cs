using System;

namespace AEdit.Undo
{
	class UndoEventArgs : EventArgs
	{
		public IApplyRecord UndoRecord { get; }
		public bool IsUndo { get; }

		public UndoEventArgs(IApplyRecord undoRecord, bool isUndo)
		{
			UndoRecord = undoRecord;
			IsUndo = isUndo;
		}
	}
}
