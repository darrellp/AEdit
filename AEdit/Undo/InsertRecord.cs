﻿using System.Diagnostics;
using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	// Undoes/Redoes the last EditObject insertion
	class InsertRecord : IApplyRecord
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

			DoRaiseEditEvent(Edit, EditAction.Remove, cUndos - 1);
			Main.Children.Remove(Edit);
			if ((Selected == null || Selected == Edit) && Main.Children.Count > 1)
			{
				Selected = (EditObject) Main.Children[cUndos - 2];
			}

		}

		public void Apply()
		{
			Main.Children.Insert(Main.Children.Count - 1, Edit);
			DoRaiseEditEvent(Edit, EditAction.Add, Main.Children.Count - 2);
			Selected = Edit;
		}
	}
}
