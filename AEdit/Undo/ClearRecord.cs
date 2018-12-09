using System.Collections.Generic;
using System.Linq;
using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	internal class ClearRecord : EditRecord
	{
		public List<EditObject> ClearedEdits { get; }
		public EditObject SelectedObject { get; }

		public ClearRecord()
		{
			ClearedEdits = Main.Children.Take(Main.EditCount).Cast<EditObject>().ToList();
			SelectedObject = Selected;
		}
	}
}
