using System.Collections.Generic;

namespace AEdit.Undo
{
	internal static class Undo
	{
		#region Private Variables
		// Stack to pop from when undoing
		private static readonly Stack<IUndoRecord> History = new Stack<IUndoRecord>();

		// Stack to pop from when redoing
		private static readonly Stack<IUndoRecord> Future = new Stack<IUndoRecord>();
		#endregion

		#region Undo operations
		public static void AddUndoRecord(IUndoRecord record)
		{
			History.Push(record);
			Future.Clear();
		}

		public static void PerformUndo()
		{
			if (History.Count == 0)
			{
				return;
			}
			var undoRecord = History.Pop();

			undoRecord.Undo();
			Future.Push(undoRecord);
		}

		public static void PerformRedo()
		{
			if (Future.Count == 0)
			{
				return;
			}
			var undoRecord = Future.Pop();

			undoRecord.Redo();
			History.Push(undoRecord);

		}
		#endregion
	}
}
