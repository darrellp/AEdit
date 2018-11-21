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
	interface IHandler
	{
		void Reset();
		void Exit();

		bool Mouse(MouseConsoleState state, Console console);
		bool Keyboard(Keyboard info, Console console);
	}
}
