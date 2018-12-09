using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	internal class ApplyRecord : IApplyRecord
	{
		public EditObject AppliedEdit { get; }
		public EditMode Mode { get; }
		public object ParmsOld { get; }
		public object ParmsNew { get; }

		public ApplyRecord(EditObject appliedEdit, EditMode mode, object parmsOld, object parmsNew)
		{
			AppliedEdit = appliedEdit;
			Mode = mode;
			ParmsOld = parmsOld;
			ParmsNew = parmsNew;
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
