using System;
using System.Diagnostics;
using SadConsole;
using SadConsole.Controls;
using static AEdit.AEGlobals;
using Console = System.Console;

namespace AEdit.Consoles
{
	internal class LayersControl : ListBox
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
					AddEdit(e.Edit);
					break;

				case EditAction.Remove:
					RemoveEdit(e.Edit);
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
					// TODO: eliminate the try when SadConsole gets the fix mentioned below.
					try
					{
						Items.Clear();
					}
					catch (ArgumentOutOfRangeException)
					{
					}
					break;

				default:
					Debug.Assert(false, "Unhandled Edit event");
					break;
			}
		}

		private void AddEdit(EditObject edit)
		{
			Items.Add(edit);
		}

		private void RemoveEdit(EditObject edit)
		{
			// TODO: eliminate the try when SadConsole gets the fix mentioned below.
			try
			{
				Items.Remove(edit);
			}
			catch (ArgumentOutOfRangeException)
			{
				// SadConsole throws an exception here because we're deleting the selected item and it tries to
				// set the SelectedItem to null.  Sadly, when setting a selected item it looks it up and if it
				// doesn't find it throws it's own exception.  Of course, it doesn't find null and hence throws
				// the exception which we catch here.  We need to remove this catch when that's fixed in SadConsole.
				// I think this is also what causes the last item in the list to not get deleted when the last edit
				// is undone from existence.
			}
		}

		private void ListOnSelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
		{
			var selectedEdit = (EditObject)e.Item;
			if (Selected != selectedEdit)
			{
				Selected = selectedEdit;
			}
		}
	}
}
