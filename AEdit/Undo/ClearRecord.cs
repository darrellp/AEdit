using System.Collections.Generic;
using System.Linq;
using AEdit.Consoles;

namespace AEdit.Undo
{
	class ClearRecord : IUndoRecord
	{
		private readonly List<EditObject> _clearedEdits;
		private EditObject _selected;

		public ClearRecord()
		{
			_clearedEdits = Program.MainDisplay.Children.Take(Program.MainDisplay.Children.Count - 1).Cast<EditObject>().ToList();
			_selected = Program.MainDisplay.Selected;
		}

		public void Undo()
		{
			Program.MainDisplay.Children.Clear();
			foreach (var edit in _clearedEdits)
			{
				Program.MainDisplay.Children.Add(edit);
			}
			Program.MainDisplay.Children.Add(Program.MainDisplay.Drawing);
			Program.MainDisplay.Selected = _selected;
		}

		public void Redo()
		{
			Program.MainDisplay.Children.Clear();
			Program.MainDisplay.Children.Add(Program.MainDisplay.Drawing);
		}
	}
}
