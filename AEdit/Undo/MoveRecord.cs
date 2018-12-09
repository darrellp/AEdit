using System.Diagnostics;
using AEdit.Consoles;
using Microsoft.Xna.Framework;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	class MoveRecord : IApplyRecord
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
			Selected = _edit;
		}

		public void Apply()
		{
			_edit.Position = _end;
			Selected = _edit;
		}
	}
}
