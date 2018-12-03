using AEdit.Consoles;

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
	}
}