using System.Diagnostics;
using AEdit.Consoles;
using Microsoft.Xna.Framework;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	class MoveRecord : IApplyRecord
	{
		public Point Start { get; }
		public Point End { get; }
		public EditObject Edit { get; }

		public MoveRecord(Point start, Point end, EditObject edit)
		{
			Start = start;
			Debug.Assert(start != end, "Start and end identical in MoveRecord constructor");
			End = end;
			Edit = edit;
		}
		public void Undo()
		{
			DoRaiseUndoEvent(this, true);
		}

		public void Apply()
		{
			DoRaiseUndoEvent(this, false);
		}
	}
}
