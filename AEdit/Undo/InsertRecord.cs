using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEdit.Consoles;

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
			var cUndos = Program.MainDisplay.Children.Count - 1;
			Debug.Assert(cUndos != 0, "Trying to undo when there's no edit in the picture");

			Edit = (EditObject)Program.MainDisplay.Children[cUndos - 1];

			Program.MainDisplay.Children.Remove(Edit);
		}

		public void Redo()
		{
			Program.MainDisplay.Children.Insert(Program.MainDisplay.Children.Count - 1, Edit);
		}
	}
}
