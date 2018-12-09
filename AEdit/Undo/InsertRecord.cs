using System.Diagnostics;
using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	// Undoes/Redoes the last EditObject insertion
	class InsertRecord : IApplyRecord
	{
		public EditObject Edit { get; }

		public InsertRecord(EditObject editObject)
		{
			Edit = editObject;
		}

		public void Undo()
		{
			DoRaiseUndoEvent(this, true);
		}

		public void Apply()
		{
			DoRaiseUndoEvent(this, false);
		}
	}
}
