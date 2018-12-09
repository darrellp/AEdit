using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	class ZOrderRecord : EditRecord
	{
		public int OldPos { get; }
		public int NewPos { get; }

		public ZOrderRecord(int oldPos, int newPos)
		{
			OldPos = oldPos;
			NewPos = newPos;
		}
	}
}
