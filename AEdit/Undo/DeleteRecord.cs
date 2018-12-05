using System;
using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	internal class DeleteRecord : IUndoRecord
	{
		private readonly int _index;
		private readonly EditObject _edit;

		public DeleteRecord(int index, EditObject edit)
		{
			_index = index;
			_edit = edit;
		}

		public void Undo()
		{
			Main.Children.Insert(_index, _edit);
			DoRaiseEditEvent(_edit, EditAction.Add, _index);
			Selected = _edit;
		}

		public void Redo()
		{
			Main.Children.Remove(_edit);
			DoRaiseEditEvent(_edit, EditAction.Remove, _index);
			if (Main.Children.Count > 1)
			{
				Selected = (EditObject)Main.Children[Math.Min(_index, Main.Children.Count - 2)];
			}
		}
	}
}
