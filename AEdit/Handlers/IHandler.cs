using SadConsole.Input;
using Console = SadConsole.Console;
using Keyboard = SadConsole.Input.Keyboard;

namespace AEdit.Handlers
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Interface all handlers for the main display have to implement </summary>
	///
	/// <remarks>	Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	interface IHandler
	{
		// Called before the handler's mode is entered to initialze things
		void Reset();

		// Called after the handler's mode is exited to tear anything down
		void Exit();

		// Mouse handler
		void Mouse(MouseConsoleState state, Console console);

		// Keyboard handler
		// ReSharper disable once UnusedParameter.Global
		void Keyboard(Keyboard info, Console console);
	}
}
