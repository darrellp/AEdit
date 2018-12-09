using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	class MergeRecord : IUndoRecord
	{
		public EditObject Above { get; }
		public EditObject Below { get; }
		public EditObject Merge { get; }
		public int IndexOfAbove { get; }

		public MergeRecord(EditObject above, EditObject below, EditObject merge, int index)
		{
			Above = above;
			Below = below;
			IndexOfAbove = index;
			Merge = merge;
		}

		public void Undo()
		{
			DoRaiseUndoEvent(this, true);
		}

		public void Redo()
		{
			DoRaiseUndoEvent(this, false);
		}
	}
}
