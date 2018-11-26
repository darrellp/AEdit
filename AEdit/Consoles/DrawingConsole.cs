using AEdit.Handlers;
using Microsoft.Xna.Framework;
using SadConsole.Input;
using Console = SadConsole.Console;
using Keyboard = SadConsole.Input.Keyboard;

namespace AEdit.Consoles
{
	enum EditMode
	{
		Brush,
		Line,
	}

	class DrawingConsole : Console
	{
		#region Private variables
		private static readonly IHandler[] _handlerTable =
		{
			new PaintHandler(),
			new LineHandler(),
		};

		private IHandler Handler { get; set; }
		private EditMode _mode;
		#endregion

		#region Properties
		public EditMode Mode
		{
			get => _mode;
			set => SetMode(_mode = value);
		}

		#endregion

		#region Constructor
		public DrawingConsole(int width, int height) : base(width, height)
		{
			DefaultBackground = Color.Transparent;
			DefaultForeground = Color.Transparent;
			Clear();
		}
		#endregion

		#region Event handling
		private void SetMode(EditMode mode)
		{
			Handler?.Exit();
			Handler = _handlerTable[(int)mode];
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
		#endregion
	}
}
