using System;

namespace AEdit.Consoles
{
	internal enum EditAction
	{
		Add,
		Remove,
		Select,
		Deselect,
		Clear
	}

	internal class EditObjectEventArgs : EventArgs
	{
		public EditObjectEventArgs(EditObject edit, EditAction action)
		{
			Edit = edit;
			Action = action;
		}

		public EditObject Edit { get; private set; }
		public EditAction Action { get; private set; }
	}
}
