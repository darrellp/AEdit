using System;
using System.Linq;
using System.Runtime.InteropServices;
using AEdit.Handlers;
using Microsoft.Xna.Framework.Input;
using SadConsole;
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

	internal class MainDisplay : Console
	{
		private EditMode _mode;

		private static readonly IHandler[] _handlerTable =
		{
			new PencilHandler(),
			new LineHandler(), 
		};

		public IHandler Handler { get; private set; }

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
