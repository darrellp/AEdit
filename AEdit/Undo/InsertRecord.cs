using System.Diagnostics;
using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	// Undoes/Redoes the last EditObject insertion
	class InsertRecord : IUndoRecord
	{
		private EditObject Edit { get; set; }

		public InsertRecord(EditObject editObject)
		{
			Edit = editObject;
		}

		public void Undo()
		{
			// Don't count the drawing console
			var cUndos = Main.Children.Count - 1;
			Debug.Assert(cUndos != 0, "Trying to undo when there's no edit in the picture");

			Edit = (EditObject)Main.Children[cUndos - 1];

			Main.Children.Remove(Edit);
			if (Selected == Edit && Main.Children.Count > 1)
			{
				Selected = (EditObject) Main.Children[cUndos - 2];
			}
		}

		public void Redo()
		{
			Main.Children.Insert(Main.Children.Count - 1, Edit);
			Selected = Edit;
		}
	}
}
