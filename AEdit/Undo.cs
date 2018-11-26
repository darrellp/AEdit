using System.Collections.Generic;
using SadConsole;

namespace AEdit
{
	internal class Undo
	{
		#region Public Variables
		#endregion

		#region Private Variables
		// Undo objects
		private readonly List<ScreenObject> _redoObjects = new List<ScreenObject>();
		#endregion

		#region Undo operations
		public void PerformUndo()
		{
			// Don't count the drawing console
			var cUndos = Program.MainDisplay.Children.Count - 1;
			if (cUndos == 0)
			{
				return;
			}

			var undoObject = Program.MainDisplay.Children[cUndos - 1];

			_redoObjects.Add(undoObject);
			Program.MainDisplay.Children.Remove(undoObject);
		}

		public void PerformRedo()
		{
			if (_redoObjects.Count == 0)
			{
				return;
			}

			var redoObject = _redoObjects[_redoObjects.Count - 1];
			_redoObjects.RemoveAt(_redoObjects.Count - 1);
			var children = Program.MainDisplay.Children;
			children.Insert(children.Count - 1, redoObject);
		}

		public void ClearRedos()
		{
			_redoObjects.Clear();
		}
		#endregion
	}
}
