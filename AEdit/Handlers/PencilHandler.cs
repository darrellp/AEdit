using System.Linq;
using Microsoft.Xna.Framework.Input;
using SadConsole.Input;
using Console = SadConsole.Console;
using Keyboard = SadConsole.Input.Keyboard;


namespace AEdit.Handlers
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A pencil handler. </summary>
	///
	/// <remarks>	Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	class PencilHandler : IHandler
	{
		#region Private Variables
		private char _drawChar = 'X';
		private bool _fDragging;
		#endregion

		#region IHandler members
		public void Reset() { }
		public void Exit() { }

		public bool Mouse(MouseConsoleState state, Console console)
		{
			if (!state.IsOnConsole)
			{
				return true;
			}
			if (state.Mouse.LeftButtonDown)
			{
				if (!_fDragging)
				{
					Program.Undos.CreateUndo();
					_fDragging = true;
				}
				var pt = state.CellPosition;
				Program.MainDisplay.SetGlyph(pt.X, pt.Y, _drawChar);
				return false;
			}
			else if (_fDragging)
			{
				_fDragging = false;
			}

			return true;
		}

		public bool Keyboard(Keyboard info, Console console)
		{
			if (!info.IsKeyDown(Keys.LeftControl) &&
			    !info.IsKeyDown(Keys.LeftAlt) &&
			    !info.IsKeyDown(Keys.RightAlt) &&
			    !info.IsKeyDown(Keys.RightControl))
			{
				var input = info.KeysPressed.Select(c => c.Character).FirstOrDefault(c => c != '\0');
				if (input != '\0')
				{
					_drawChar = input;
				}
			}

			return false;
		}
		#endregion
	}
}
