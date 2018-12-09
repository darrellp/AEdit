using AEdit.Consoles;

namespace AEdit.Undo
{
	// Undoes/Redoes the last EditObject insertion
	internal class InsertRecord : EditRecord
	{
		public EditObject Edit { get; }

		public InsertRecord(EditObject editObject)
		{
			Edit = editObject;
		}
	}
}
