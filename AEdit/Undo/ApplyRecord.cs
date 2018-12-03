using AEdit.Consoles;

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
			Program.MainDisplay.Selected = _appliedEdit;
			Program.MainDisplay.Mode = _mode;
			Program.ControlPanel.EditControls.SetParameters(_parmsOld);
			Program.ControlPanel.EditControls.Apply(Program.MainDisplay.Selected);
		}

		public void Redo()
		{
			Program.MainDisplay.Selected = _appliedEdit;
			Program.MainDisplay.Mode = _mode;
			Program.ControlPanel.EditControls.SetParameters(_parmsNew);
			Program.ControlPanel.EditControls.Apply(Program.MainDisplay.Selected);
		}
	}
}
