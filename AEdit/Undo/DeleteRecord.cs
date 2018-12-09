using System;
using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	internal class DeleteRecord : IApplyRecord
	{
		public int Index { get; }
		public EditObject Edit { get; }

		public DeleteRecord(int index, EditObject edit)
		{
			Index = index;
			Edit = edit;
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
