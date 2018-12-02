﻿using AEdit.Undo;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.Surfaces;
using Console = SadConsole.Console;

namespace AEdit.Consoles
{
	internal class EditObject : Console
	{
		#region Private variables
		private Point _startPoint;
		private Point _initialPosition;
		private EditMode _mode;
		private object _parms;
		#endregion

		#region Public properties
		public EditMode Mode => _mode;
		public object Parms => _parms;
		#endregion

		#region Constructor
		public EditObject(SurfaceBase surface, EditMode mode, object parms, Rectangle rect) : base(rect.Width, rect.Height)
		{
			surface.Copy(rect.X, rect.Y, rect.Width, rect.Height, this, 0, 0);
			Position = new Point(rect.X, rect.Y);
			_mode = mode;
			_parms = parms;
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
				Program.DraggedObject = null;
				if (Position != _initialPosition)
				{
					var moveRecord = new MoveRecord(_initialPosition, Position, this);
					Undo.Undo.AddRecord(moveRecord);
				}

				Program.MainDisplay.Selected = this;
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
				Program.DraggedObject = this;
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
		#endregion
	}
}
