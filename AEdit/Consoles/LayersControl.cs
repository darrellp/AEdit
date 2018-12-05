using System;
using System.Diagnostics;
using SadConsole;
using SadConsole.Controls;
using static AEdit.AEGlobals;
using Console = System.Console;

namespace AEdit.Consoles
{
	internal class LayersControl : MyListBox
	{
		public LayersControl(int width, int height) : base(width, height)
		{
			HideBorder = false;

			SelectedItemChanged += ListOnSelectedItemChanged;
			RaiseEditEvent += OnRaiseEditEvent;
		}

		private void OnRaiseEditEvent(object sender, EditObjectEventArgs e)
		{
			switch (e.Action)
			{
				case EditAction.Add:
					AddEdit(e.Edit, e.ChildIndex);
					break;

				case EditAction.Remove:
					RemoveEdit(e.ChildIndex);
					break;

				case EditAction.Deselect:
					break;

				case EditAction.Select:
					if (SelectedItem != e.Edit)
					{
						SelectedItem = e.Edit;
					}
					break;

				case EditAction.Clear:
					Items.Clear();
					break;

				default:
					Debug.Assert(false, "Unhandled Edit event");
					break;
			}
		}

		private void AddEdit(EditObject edit, int childIndex)
		{
			Items.Insert(childIndex, edit);
		}

		private void RemoveEdit(int childIndex)
		{
			Items.RemoveAt(childIndex);
		}

		private void ListOnSelectedItemChanged(object sender, SelectedItemEventArgs e)
		{
			var selectedEdit = (EditObject)e.Item;
			if (Selected != selectedEdit)
			{
				Selected = selectedEdit;
			}
		}
	}
}
