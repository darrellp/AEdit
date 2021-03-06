﻿using System.Linq;
using AEdit.Undo;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.Surfaces;
using static AEdit.AEGlobals;
using Console = SadConsole.Console;

namespace AEdit.Consoles
{
	internal class EditObject : Console
	{
		#region Private variables
		private Point _startPoint;
		private Point _initialPosition;
		private static int NameCount = 0;
		#endregion

		#region Public properties
		public EditMode Mode { get; }
		public object Parms { get; set; }
		public string Name { get; set; }
		#endregion

		#region Constructor
		public EditObject(SurfaceBase surface, EditMode mode, object parms, Rectangle rect) : base(rect.Width, rect.Height)
		{
			surface.Copy(rect.X, rect.Y, rect.Width, rect.Height, this, 0, 0);
			Position = new Point(rect.X, rect.Y);
			Mode = mode;
			Parms = parms;
			Name = PickName();
		}

		private static string PickName()
		{
			return $"Edit{NameCount++:000}";
		}
		#endregion

		#region Handlers
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Mouse moves passed down from DrawingConsole. </summary>
		///
		/// <remarks>	Darrell Plank, 11/30/2018. </remarks>
		///
		/// <param name="pt">	Where the mouse is. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void MouseMoveFromParent(Point pt)
		{
			if (!Global.MouseState.LeftButtonDown)
			{
				DraggedObject = null;
				if (Position != _initialPosition)
				{
					var moveRecord = new MoveRecord(_initialPosition, Position, this);
					Undo.Undo.AddUndoRecord(moveRecord);
				}

				Selected = this;
			}
			else if (pt != _startPoint)
			{
				Position += pt - _startPoint;
				_startPoint = pt;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Processes direct mouse calls. </summary>
		///
		/// <remarks>	Darrell Plank, 11/30/2018. </remarks>
		///
		/// <param name="state">	The mouse state related to this console. </param>
		///
		/// <returns>	True when the mouse is over this console and processing should stop. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public override bool ProcessMouse(MouseConsoleState state)
		{
			if (!state.IsOnConsole)
			{
				return false;
			}
			if (state.Mouse.LeftButtonDown && this[state.CellPosition.X, state.CellPosition.Y].Glyph != 0)
			{
				DraggedObject = this;
				// Get position relative to parent
				_startPoint = state.CellPosition + Position;
				_initialPosition = Position;
				return true;
			}

			return false;
		}
		#endregion

		#region Operations
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Shows/Hides selected mode display </summary>
		///
		/// <remarks>	Darrell Plank, 11/30/2018. </remarks>
		///
		/// <param name="isSelected">	True if this object is selected. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void DisplayAsSelected(bool isSelected)
		{
			if (isSelected)
			{
				SetEffect(Cells, new Blink() {BlinkSpeed = 0.1, BlinkCount = 4, RemoveOnFinished = true});
			}
			else
			{
				Effects.RemoveAll();
			}
		}

		public void ApplyColors(Color fore, Color back)
		{
			foreach (var cell in Cells.Where(c => c.Glyph != 0 || c.Background != Color.Transparent))
			{
				cell.Foreground = fore;
				cell.Background = back;
			}
			IsDirty = true;
			SetRenderCells();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Merges one editObject onto another. </summary>
		///
		/// <remarks>	Doesn't check for the merged one being interior to this.  Caller needs to ensure
		/// 			that this is large enough before making the call.
		/// 			Darrell Plank, 12/8/2018. </remarks>
		///
		/// <param name="edit">	The edit to be merged. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Merge(EditObject edit)
		{
			var offset = edit.Position - Position;

			for (var iRow = 0; iRow < edit.Height; iRow++)
			{
				for (var iCol = 0; iCol < edit.Width; iCol++)
				{
					var cell = edit[iCol, iRow];
					if (cell.IsVisible)
					{
						cell.CopyAppearanceTo(this[iCol + offset.X, iRow + offset.Y]);
					}
				}
			}
		}
		#endregion

		#region Overrides
		public override string ToString() => Name;
		#endregion
	}
}
