using AEdit.Consoles;

namespace AEdit.Undo
{
	internal class DeleteRecord : EditRecord
	{
		public int Index { get; }
		public EditObject Edit { get; }

		public DeleteRecord(int index, EditObject edit)
		{
			Index = index;
			Edit = edit;
		}
	}
}
