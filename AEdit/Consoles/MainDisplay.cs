using System;
using AEdit.Undo;
using Microsoft.Xna.Framework;
using SadConsole;
using static AEdit.AEGlobals;
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
				if (_selected != null)
				{
					_selected.DisplayAsSelected(false);
					DoRaiseEditEvent(_selected, EditAction.Deselect);
				}
				_selected = value;
				if (value != null)
				{
					_selected.DisplayAsSelected(true);
					Mode = _selected.Mode;
					Ctrls.EditControls.SetParameters(_selected.Parms);
					Ctrls.UpdateHandler();
					DoRaiseEditEvent(_selected, EditAction.Select);
				}
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
			var editObject = new EditObject(Drawing, Drawing.Mode, Ctrls.EditControls.GetParameterInfo(), rect);
			var insertRecord = new InsertRecord(editObject);
			AddUndoRecord(insertRecord);
			// The last child is "on top".  We want the new object to be above
			// the top EditObject but still below the drawing console.
			Children.Insert(Children.Count - 1, editObject);
			Drawing.Clear();
			DoRaiseEditEvent(editObject, EditAction.Add);
			Selected = editObject;
		}
		#endregion

		#region Operations
		public void Clear()
		{
			Children.Clear();
			Children.Add(Main.Drawing);
		}

		public void DeleteSelected()
		{
			if (Selected == null)
			{
				return;
			}
			var edit = Selected;
			var childIndex = Children.IndexOf(edit);
			AddUndoRecord(new DeleteRecord(childIndex, edit));
			Children.Remove(edit);
			DoRaiseEditEvent(edit, EditAction.Remove, childIndex);
			if (Main.Children.Count > 1)
			{
				Selected = (EditObject)Main.Children[Math.Min(childIndex, Main.Children.Count - 2)];
			}
		}
		#endregion
	}
}
