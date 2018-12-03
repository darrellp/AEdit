using AEdit.Consoles;
using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	internal class ApplyRecord : IUndoRecord
	{
		private readonly EditObject _appliedEdit;
		private readonly EditMode _mode;
		private readonly object _parmsOld;
		private readonly object _parmsNew;

		public ApplyRecord(EditObject appliedEdit, EditMode mode, object parmsOld, object parmsNew)
		{
			_appliedEdit = appliedEdit;
			_mode = mode;
			_parmsOld = parmsOld;
			_parmsNew = parmsNew;
		}

		public void Undo()
		{
			Selected = _appliedEdit;
			Main.Mode = _mode;
			Ctrls.EditControls.SetParameters(_parmsOld);
			Ctrls.EditControls.Apply(Selected);
		}

		public void Redo()
		{
			Selected = _appliedEdit;
			Main.Mode = _mode;
			Ctrls.EditControls.SetParameters(_parmsNew);
			Ctrls.EditControls.Apply(Selected);
		}
	}
}
