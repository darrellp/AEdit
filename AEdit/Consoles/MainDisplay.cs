using System;
using System.Collections.Generic;
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

				case ClearRecord cr:
					HandleClear(cr, e.IsUndo);
					break;

				case MoveRecord mr:
					HandleMove(mr, e.IsUndo);
					break;
			}
		}

		#region Move
		private void HandleMove(MoveRecord mr, bool isUndo)
		{
			if (isUndo)
			{
				UndoMove(mr);
			}
			else
			{
				ApplyMove(mr);
			}
		}

		public void ApplyMove(MoveRecord mr)
		{
			mr.Edit.Position = mr.End;
			Selected = mr.Edit;
		}

		private void UndoMove(MoveRecord mr)
		{
			mr.Edit.Position = mr.Start;
			Selected = mr.Edit;
		}
		#endregion

		#region Clear
		private void HandleClear(ClearRecord cr, bool isUndo)
		{
			if (isUndo)
			{
				UndoClear(cr.ClearedEdits, cr.SelectedObject);
			}
			else
			{
				ApplyClear();
			}
		}

		public void ApplyClear()
		{

			Children.Clear();
			Children.Add(Drawing);
			DoRaiseEditEvent(null, EditAction.Clear);
		}

		private void UndoClear(List<EditObject> clearedEdits, EditObject selectedObject)
		{
			Children.Clear();
			foreach (var edit in clearedEdits)
			{
				Children.Add(edit);
				DoRaiseEditEvent(edit, EditAction.Add);
			}
			Children.Add(Drawing);
			Selected = selectedObject;
		}
		#endregion

		#region Insert
		private void HandleInsert(EditObject edit, bool isUndo)
		{
			if (isUndo)
			{
				UndoInsert(edit);
			}
			else
			{
				ApplyInsert(edit);
			}
		}

		private void UndoInsert(EditObject edit)
		{
			Debug.Assert(EditCount != 0, "Trying to undo when there's no edit in the picture");


			DoRaiseEditEvent(edit, EditAction.Remove, EditCount - 1);
			Main.Children.Remove(edit);
			if ((Selected == null || Selected == edit) && EditCount > 0)
			{
				Selected = (EditObject)Main.Children[EditCount - 1];
			}
		}

		private void ApplyInsert(EditObject edit)
		{
			Children.Insert(EditCount, edit);
			DoRaiseEditEvent(edit, EditAction.Add, EditCount - 1);
			Selected = edit;
		}
		#endregion
		#endregion
	}
}
