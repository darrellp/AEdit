using System.Diagnostics;
using AEdit.Consoles;
using Microsoft.Xna.Framework;

namespace AEdit.Undo
{
	class MoveRecord : IUndoRecord
	{
		private readonly Point _start;
		private readonly Point _end;
		private readonly EditObject _edit;

		public MoveRecord(Point start, Point end, EditObject edit)
		{
			_start = start;
			Debug.Assert(start != end, "Start and end identical in MoveRecord constructor");
			_end = end;
			_edit = edit;
		}
		public void Undo()
		{
			_edit.Position = _start;
			Program.MainDisplay.Selected = _edit;
		}

		public void Redo()
		{
			_edit.Position = _end;
			Program.MainDisplay.Selected = _edit;
		}
	}
}
