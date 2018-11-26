using Microsoft.Xna.Framework;
using SadConsole;

namespace AEdit.Consoles
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	The main display for the Ascii art. </summary>
	///
	/// <remarks>	Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	internal class MainDisplay : ScreenObject
	{
		#region Public properties
		public EditMode Mode
		{
			set => Drawing.Mode = value;
		}

		public DrawingConsole Drawing { get; }
		#endregion

		#region Constructor
		public MainDisplay(int width, int height)
		{
			Drawing = new DrawingConsole(width, height);
			Children.Add(Drawing);
		}
		#endregion

		#region EditObject Handling
		public void SetObject(Rectangle rect)
		{
			var editObject = new EditObject(Drawing, rect);
			// The last child is "on top".  We want the new object to be beneath
			// the top EditObject but still below the drawing console.
			Children.Insert(Children.Count - 1, editObject);
			Drawing.Clear();
			Program.Undos.ClearRedos();
		}
		#endregion

		#region Operations
		public void Clear()
		{
			Children.Clear();
			Children.Add(Program.MainDisplay.Drawing);
		}
		#endregion
	}
}
