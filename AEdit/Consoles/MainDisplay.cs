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
			Children.Add(Drawing);
		}

		public void DeleteSelected()
		{
			if (Selected == null)
			{
				return;
			}
			var edit = Selected;
			var childIndex = Children.IndexOf(edit);
			var record = new DeleteRecord(childIndex, edit);
			AddUndoRecord(record);
			ApplyDelete(record);
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
		//private void OnRaiseEditEvent(object sender, EditObjectEventArgs e)
		//{
		//	switch (e.Action)
		//	{
		//		case EditAction.MoveUp:
		//			ChangeZOrder(true);
		//			break;

		//		case EditAction.MoveDown:
		//			ChangeZOrder(false);
		//			break;
		//	}
		//}

		public void ChangeZOrder(bool isUp)
		{
			if (Selected == null)
			{
				return;
			}
			var iSelected = Children.IndexOf(Selected);
			var iTo = iSelected + (isUp ? -1 : 1);
			if (iTo < 0 || iTo >= EditCount)
			{
				return;
			}
			var record = new ZOrderRecord(iSelected, iTo);
			TakeAction(record);
			AddUndoRecord(record);
			DoRaiseEditEvent((EditObject)Children[iTo], isUp ? EditAction.MoveUp : EditAction.MoveDown, iSelected);
		}

		public void TakeAction(EditRecord record, bool isUndo = false)
		{
			switch (record)
			{
				case InsertRecord ir:
					HandleInsert(ir.Edit, isUndo);
					break;

				case ClearRecord cr:
					HandleClear(cr, isUndo);
					break;

				case MoveRecord mr:
					HandleMove(mr, isUndo);
					break;

				case DeleteRecord dr:
					HandleDelete(dr, isUndo);
					break;

				case ApplyRecord ar:
					HandleApply(ar, isUndo);
					break;

				case ZOrderRecord zr:
					HandleZOrder(zr, isUndo);
					break;
			}
			DoRaiseUndoEvent(record, isUndo);
		}

		#region Z Order
		private void HandleZOrder(ZOrderRecord zr, bool isUndo)
		{
			if (isUndo)
			{
				UndoZOrder(zr);
			}
			else
			{
				ApplyZOrder(zr);
			}
		}

		private void ApplyZOrder(ZOrderRecord zr)
		{
			DoZOrderSwap(zr.NewPos, zr.OldPos);
		}

		private void UndoZOrder(ZOrderRecord zr)
		{
			DoZOrderSwap(zr.OldPos, zr.NewPos);
		}

		private void DoZOrderSwap(int newPos, int oldPos)
		{
			if (oldPos < 0 || oldPos > EditCount - 1 || newPos < 0 || newPos > EditCount - 1)
			{
				return;
			}
			var isMovingSelection = Children[oldPos] == Selected;
			var movedEdit = (EditObject)Children[oldPos];
			Children.Remove(movedEdit);
			DoRaiseEditEvent(movedEdit, EditAction.Remove, oldPos);

			Children.Insert(newPos, movedEdit);
			DoRaiseEditEvent(movedEdit, EditAction.Add, newPos);
			if (isMovingSelection)
			{
				Selected = movedEdit;
			}
		}
		#endregion

		#region Apply
		private void HandleApply(ApplyRecord ar, bool isUndo)
		{
			if (isUndo)
			{
				UndoApply(ar);
			}
			else
			{
				ApplyApply(ar);
			}
		}

		public bool ApplyApply(ApplyRecord ar)
		{
			var old = Ctrls.EditControls.GetParameterInfo();
			Selected = ar.AppliedEdit;
			Mode = ar.Mode;
			Ctrls.EditControls.SetParameters(ar.ParmsNew);
			bool ret = Ctrls.EditControls.Apply(ar.AppliedEdit);
			if (!ret)
			{
				Ctrls.EditControls.SetParameters(old);
			}
			return ret;
		}

		private void UndoApply(ApplyRecord ar)
		{
			Selected = ar.AppliedEdit;
			Mode = ar.Mode;
			Ctrls.EditControls.SetParameters(ar.ParmsOld);
			Ctrls.EditControls.Apply(Selected);
		}
		#endregion

		#region Delete
		private void HandleDelete(DeleteRecord dr, bool isUndo)
		{
			if (isUndo)
			{
				UndoDelete(dr);
			}
			else
			{
				ApplyDelete(dr);
			}
		}

		private void UndoDelete(DeleteRecord dr)
		{
			Children.Insert(dr.Index, dr.Edit);
			DoRaiseEditEvent(dr.Edit, EditAction.Add, dr.Index);
			Selected = dr.Edit;
		}

		private void ApplyDelete(DeleteRecord dr)
		{
			var needNewSelection = Children[dr.Index] == Selected;

			Children.Remove(Children[dr.Index]);
			DoRaiseEditEvent(dr.Edit, EditAction.Remove, dr.Index);
			if (EditCount > 0 && needNewSelection)
			{
				Selected = (EditObject)Children[Math.Min(dr.Index, EditCount - 1)];
			}
		}
		#endregion

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

		private void ApplyMove(MoveRecord mr)
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

			Children.Remove(edit);
			DoRaiseEditEvent(edit, EditAction.Remove, EditCount);
			if ((Selected == null || Selected == edit) && EditCount > 0)
			{
				Selected = (EditObject)Children[EditCount - 1];
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
