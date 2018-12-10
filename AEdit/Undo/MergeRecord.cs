using AEdit.Consoles;

namespace AEdit.Undo
{
	class MergeRecord : EditRecord
	{
		public EditObject Above { get; }
		public EditObject Below { get; }
		public EditObject Merge { get; }
		public int IndexAbove { get; }

		public MergeRecord(EditObject above, EditObject below, EditObject merge, int indexAbove)
		{
			Above = above;
			Below = below;
			IndexAbove = indexAbove;
			Merge = merge;
		}
	}
}
