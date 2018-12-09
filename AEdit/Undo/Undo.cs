using System.Collections.Generic;

namespace AEdit.Undo
{
	internal static class Undo
	{
		#region Private Variables
		// Stack to pop from when undoing
		private static readonly Stack<IApplyRecord> History = new Stack<IApplyRecord>();

		// Stack to pop from when redoing
		private static readonly Stack<IApplyRecord> Future = new Stack<IApplyRecord>();
		#endregion

		#region Undo operations
		public static void AddUndoRecord(IApplyRecord record)
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

			undoRecord.Apply();
			History.Push(undoRecord);
		}
		#endregion
	}
}
