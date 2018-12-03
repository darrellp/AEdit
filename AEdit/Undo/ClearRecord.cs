using System.Collections.Generic;
using System.Linq;
using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	class ClearRecord : IUndoRecord
	{
		private readonly List<EditObject> _clearedEdits;
		private EditObject _selected;

		public ClearRecord()
		{
			_clearedEdits = Main.Children.Take(Main.Children.Count - 1).Cast<EditObject>().ToList();
			_selected = Selected;
		}

		public void Undo()
		{
			Main.Children.Clear();
			foreach (var edit in _clearedEdits)
			{
				Main.Children.Add(edit);
			}
			Main.Children.Add(Main.Drawing);
			Selected = _selected;
		}

		public void Redo()
		{
			Main.Children.Clear();
			Main.Children.Add(Main.Drawing);
		}
	}
}
