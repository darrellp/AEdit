using System.Collections.Generic;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	internal static class Undo
	{
		#region Private Variables
		// Stack to pop from when undoing
		private static readonly Stack<EditRecord> History = new Stack<EditRecord>();

		// Stack to pop from when redoing
		private static readonly Stack<EditRecord> Future = new Stack<EditRecord>();
		#endregion

		#region Undo operations
		public static void AddUndoRecord(EditRecord record)
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
			var redoRecord = Future.Pop();
			redoRecord.Apply();
			History.Push(redoRecord);
		}
		#endregion
	}
}
