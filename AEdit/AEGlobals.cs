using System;
using AEdit.Consoles;
using AEdit.Undo;

namespace AEdit
{
	internal static class AEGlobals
	{
		public static ControlPanel Ctrls { get; set; }
		public static EditObject DraggedObject;
		public static MainDisplay Main { get; set; }
		public static DrawingConsole Drawing => Main.Drawing;
		public static EditObject Selected
		{
			get => Main.Selected;
			set => Main.Selected = value;
		}

		// Events
		public static event EventHandler<EditObjectEventArgs> RaiseEditEvent;
		public static void DoRaiseEditEvent(EditObject edit, EditAction action, int childIndex = -1)
		{
			var handler = RaiseEditEvent;

			if (handler != null)
			{
				var args = new EditObjectEventArgs(edit, action, childIndex);
				handler(Main, args);
			}
		}

		public static event EventHandler<UndoEventArgs> RaiseUndoEvent;
		public static void DoRaiseUndoEvent(EditRecord undoRecord, bool isUndo)
		{
			var handler = RaiseUndoEvent;

			if (handler != null)
			{
				var args = new UndoEventArgs(undoRecord, isUndo);
				handler(Main, args);
			}
		}
	}
}