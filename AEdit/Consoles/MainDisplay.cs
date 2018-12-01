using AEdit.Undo;
using Microsoft.Xna.Framework;
using SadConsole;
using static AEdit.Undo.Undo;

namespace AEdit.Consoles
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	The main display for the Ascii art. </summary>
	///
	/// <remarks>	Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	internal class MainDisplay : ScreenObject
	{
		#region Private Variables
		private EditObject _selected;
		#endregion

		#region Public properties
		public EditObject Selected
		{
			get => _selected;
			set
			{
				_selected?.DisplayAsSelected(false);
				_selected = value;
				_selected.DisplayAsSelected(true);
				Mode = _selected.Mode;
				Program.ControlPanel.EditControls.SetParameters(_selected.Parms);
				Program.ControlPanel.UpdateHandler();
			}
		}

		public EditMode Mode
		{
			set => Drawing.Mode = value;
			get => Drawing.Mode;
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
			var editObject = new EditObject(Drawing, Drawing.Mode, Program.ControlPanel.EditControls.GetParameterInfo(), rect);
			var insertRecord = new InsertRecord(editObject);
			AddRecord(insertRecord);
			// The last child is "on top".  We want the new object to be above
			// the top EditObject but still below the drawing console.
			Children.Insert(Children.Count - 1, editObject);
			Drawing.Clear();
			Selected = editObject;
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
