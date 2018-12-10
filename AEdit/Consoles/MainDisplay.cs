using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using AEdit.Undo;
using Microsoft.Xna.Framework;
using SadConsole;
using static AEdit.AEGlobals;
using static AEdit.Undo.Undo;
using static System.Math;

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
			TakeAction(insertRecord);
		}
		#endregion

		#region Operations
		private void RemoveChild(EditObject child)
		{
			var iChild = Children.IndexOf(child);
			Children.Remove(child);
			DoRaiseEditEvent(child, EditAction.Remove, iChild);
		}

		private void AddChild(EditObject child, int iPosition)
		{
			Children.Insert(iPosition, child);
			DoRaiseEditEvent(child, EditAction.Add, iPosition);
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

		public void MergeDown()
		{
			if (Selected == null || EditCount < 2)
			{
				return;
			}

			var iSelected = Children.IndexOf(Selected);
			if (iSelected == EditCount - 1)
			{
				return;
			}

			var above = Selected;
			var below = (EditObject)Children[iSelected + 1];
			var mergeUL = new Point(
				Min(above.Position.X, below.Position.X),
				Min(above.Position.Y, below.Position.Y));
			var mergeLR = new Point(
				Max(above.Position.X + above.Width, below.Position.X + below.Width),
				Max(above.Position.Y + above.Height, below.Position.Y + below.Height));
			var mergeSize = mergeLR - mergeUL;

			var merge = new EditObject(Drawing, EditMode.Canvas, null, new Rectangle(mergeUL, mergeSize));
			merge.Merge(above);
			merge.Merge(below);
			var mergeRecord = new MergeRecord(above, below, merge, iSelected);
			TakeAction(mergeRecord);
			AddUndoRecord(mergeRecord);
			Selected = merge;
		}

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
		#endregion

		#region Handlers
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

				case MergeRecord mr:
					HandleMerge(mr, isUndo);
					break;
			}
			DoRaiseUndoEvent(record, isUndo);
		}

		#region Merge
		private void HandleMerge(MergeRecord mr, bool isUndo)
		{
			if (isUndo)
			{
				UndoMerge(mr);
			}
			else
			{
				PerformMerge(mr);
			}
		}

		private void PerformMerge(MergeRecord mr)
		{
			RemoveChild((EditObject) Children[mr.IndexAbove]);
			RemoveChild((EditObject)Children[mr.IndexAbove]);
			AddChild(mr.Merge, mr.IndexAbove);
			Selected = mr.Merge;
			Mode = EditMode.Canvas;
		}

		private void UndoMerge(MergeRecord mr)
		{
			RemoveChild((EditObject) Children[mr.IndexAbove]);
			AddChild(mr.Below, mr.IndexAbove);
			AddChild(mr.Above, mr.IndexAbove);
			Selected = mr.Above;
			Mode = Selected.Mode;
		}
		#endregion

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
			RemoveChild(movedEdit);
			AddChild(movedEdit, newPos);
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
			AddChild(dr.Edit, dr.Index);
			Selected = dr.Edit;
		}

		private void ApplyDelete(DeleteRecord dr)
		{
			var needNewSelection = Children[dr.Index] == Selected;

			RemoveChild((EditObject)Children[dr.Index]);
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
				// Since we took out the Drawing layer above we don't want to use EditCount
				// here but instead use Children.Count.
				AddChild(edit, Children.Count);
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
