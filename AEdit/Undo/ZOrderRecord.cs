using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	class ZOrderRecord : IUndoRecord
	{
		private bool _isUp;

		public ZOrderRecord(bool isUp)
		{
			_isUp = isUp;
		}

		public void Undo()
		{
			DoRaiseEditEvent(Selected, _isUp ? EditAction.MoveDown : EditAction.MoveUp);
		}

		public void Redo()
		{
			DoRaiseEditEvent(Selected, _isUp ? EditAction.MoveUp : EditAction.MoveDown);
		}
	}
}
