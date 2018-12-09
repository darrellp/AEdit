using System.Collections.Generic;
using System.Linq;
using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	class ClearRecord : IApplyRecord
	{
		public List<EditObject> ClearedEdits { get; }
		public EditObject SelectedObject { get; }

		public ClearRecord()
		{
			ClearedEdits = Main.Children.Take(Main.EditCount).Cast<EditObject>().ToList();
			SelectedObject = Selected;
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
