using System.Collections.Generic;
using System.Linq;

namespace AEdit.Undo
{
	class ClearRecord : IUndoRecord
	{
		private List<EditObject> _clearedEdits;

		public ClearRecord()
		{
			_clearedEdits = Program.MainDisplay.Children.Take(Program.MainDisplay.Children.Count - 1).Cast<EditObject>().ToList();
		}

		public void Undo()
		{
			Program.MainDisplay.Children.Clear();
			foreach (var edit in _clearedEdits)
			{
				Program.MainDisplay.Children.Add(edit);
			}
			Program.MainDisplay.Children.Add(Program.MainDisplay.Drawing);
		}

		public void Redo()
		{
			Program.MainDisplay.Children.Clear();
			Program.MainDisplay.Children.Add(Program.MainDisplay.Drawing);
		}
	}
}
