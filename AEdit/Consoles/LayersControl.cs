using AEdit.Consoles.Controls;
using static AEdit.AEGlobals;

namespace AEdit.Consoles
{
	internal class LayersControl : Layers
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

				case EditAction.Select:
					if (SelectedItem != e.Edit)
					{
						SelectedItem = e.Edit;
					}
					break;

				case EditAction.Clear:
					Items.Clear();
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
