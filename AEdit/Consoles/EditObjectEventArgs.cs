using System;
using static AEdit.AEGlobals;

namespace AEdit.Consoles
{
	internal enum EditAction
	{
		Add,
		Remove,
		Select,
		Deselect,
		Clear,
		MoveUp,
		MoveDown
	}

	internal class EditObjectEventArgs : EventArgs
	{
		public EditObjectEventArgs(EditObject edit, EditAction action, int childIndex = -1)
		{
			Edit = edit;
			Action = action;
			if (childIndex == -1)
			{
				childIndex = Main.Children.IndexOf(edit);
			}
			ChildIndex = childIndex;
		}

		public EditObject Edit { get; private set; }
		public EditAction Action { get; private set; }
		public int ChildIndex { get; private set; }
	}
}
