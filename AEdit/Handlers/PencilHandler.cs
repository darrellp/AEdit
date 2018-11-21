using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Input;
using Console = SadConsole.Console;
using Keyboard = SadConsole.Input.Keyboard;


namespace AEdit.Handlers
{
	class PencilHandler : IHandler
	{
		private char _drawChar = 'X';

		public void Reset() { }
		public void Exit() { }

		public bool Mouse(MouseConsoleState state, Console console)
		{
			if (state.Mouse.LeftButtonDown)
			{
				var pt = state.CellPosition;
				console.SetGlyph(pt.X, pt.Y, _drawChar);
				return false;
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
	}
}
