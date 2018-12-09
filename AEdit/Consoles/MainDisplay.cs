using System;
using System.Diagnostics;
using System.Windows.Forms;
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
		public int EditCount => Children.Count - 1;
		#endregion

		#region Constructor
		public MainDisplay(int width, int height)
		{
			Drawing = new DrawingConsole(width, height);
			Children.Add(Drawing);
			RaiseEditEvent += OnRaiseEditEvent;
			RaiseUndoEvent += MainDisplay_RaiseUndoEvent;
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
			Drawing.Clear();
			ApplyInsert(editObject);
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
			if (Main.EditCount > 0)
			{
				Selected = (EditObject)Main.Children[Math.Min(childIndex, Main.EditCount - 1)];
			}
		}

		public void CopySelected()
		{
			if (Selected != null)
			{
				Clipboard.SetText(Selected.ToAscii());
			}
		}

		public void CopyScreen()
		{
			// No sense in keeping Drawing console in for this since it will not have anything on it
			Children.Remove(Drawing);
			try
			{
				Clipboard.SetText(this.Flatten().ToAscii());
			}
			finally
			{
				Children.Add(Drawing);
			}
		}

		public void CopyHtml()
		{
			Children.Remove(Drawing);
			try
			{
				Clipboard.SetText(this.Flatten().ToHtml());
			}
			finally
			{
				Children.Add(Drawing);
			}
		}
		#endregion

		#region Handlers
		private void OnRaiseEditEvent(object sender, EditObjectEventArgs e)
		{
			switch (e.Action)
			{
				case EditAction.MoveUp:
					MoveSelected(true);
					break;

				case EditAction.MoveDown:
					MoveSelected(false);
					break;
			}
		}

		private void MoveSelected(bool isUp)
		{
			if (Selected == null)
			{
				return;
			}
			var iSelected = Children.IndexOf(Selected);
			if (isUp && iSelected == 0 || !isUp && iSelected == EditCount)
			{
				return;
			}
			Children.Remove(Selected);
			Children.Insert(iSelected + (isUp ? -1 : 1), Selected);
		}

		private void MainDisplay_RaiseUndoEvent(object sender, UndoEventArgs e)
		{
			switch (e.UndoRecord)
			{
				case InsertRecord ir:
					HandleInsert(ir.Edit, e.IsUndo);
					break;
			}
		}

		private void HandleInsert(EditObject edit, bool IsUndo)
		{
			if (IsUndo)
			{
				UndoInsert(edit);
			}
			else
			{
				ApplyInsert(edit);
			}
			// Don't count the drawing console
		}

		private void UndoInsert(EditObject edit)
		{
			var cUndos = Main.EditCount;
			Debug.Assert(cUndos != 0, "Trying to undo when there's no edit in the picture");


			DoRaiseEditEvent(edit, EditAction.Remove, cUndos - 1);
			Main.Children.Remove(edit);
			if ((Selected == null || Selected == edit) && Main.Children.Count > 1)
			{
				Selected = (EditObject)Main.Children[cUndos - 2];
			}
		}

		private void ApplyInsert(EditObject edit)
		{
			Children.Insert(EditCount, edit);
			DoRaiseEditEvent(edit, EditAction.Add, EditCount - 1);
			Selected = edit;
		}
		#endregion
	}
}
