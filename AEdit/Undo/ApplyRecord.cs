using AEdit.Consoles;

namespace AEdit.Undo
{
	internal class ApplyRecord : EditRecord
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
	}
}
