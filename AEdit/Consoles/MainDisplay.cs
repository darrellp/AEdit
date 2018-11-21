using AEdit.Handlers;
using SadConsole.Input;
using Console = SadConsole.Console;
using Keyboard = SadConsole.Input.Keyboard;

namespace AEdit.Consoles
{
	enum EditMode
	{
		Pencil,
		Line,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	The main display for the Ascii art. </summary>
	///
	/// <remarks>	Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	internal class MainDisplay : Console
	{
		private EditMode _mode;

		private static readonly IHandler[] _handlerTable =
		{
			new PencilHandler(),
			new LineHandler(), 
		};

		private IHandler Handler { get; set; }

		public EditMode Mode
		{
			get => _mode;
			set => SetMode(_mode = value);
		}

		public MainDisplay(int width, int height) : base(width, height) { }

		private void SetMode(EditMode mode)
		{
			Handler?.Exit();
			Handler = _handlerTable[(int) mode];
			Handler.Reset();
		}

		public override bool ProcessMouse(MouseConsoleState state)
		{
			return Handler == null || Handler.Mouse(state, this) && base.ProcessMouse(state);
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			return Handler == null || Handler.Keyboard(info, this) && base.ProcessKeyboard(info);
		}
	}
}
