using AEdit.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
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

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A drawing console. </summary>
	///
	/// <remarks>	The drawing console is what the user draws on (duh).  When the user finishes a
	/// 			drawing operation the handler will call MainWindow.SetObject() with a bounding
	/// 			rectangle for the drawn element.  MainWindow will turn it into an EditObject
	/// 			Console, add that as a child, clear the DrawingConsole and ensure it's the
	/// 			top console in MainDisplay.
	/// 			Darrell Plank, 11/26/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
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
			if (mode == Mode)
			{
				return;
			}
			Handler?.Exit();
			Handler = _handlerTable[(int)mode];
			Handler.Reset();
		}

		// TODO: Ctl down and dragging and drawing across empty screen "catches" first object
		public override bool ProcessMouse(MouseConsoleState state)
		{
			if (!state.IsOnConsole)
			{
				return false;
			}

			if (Program.DraggedObject != null)
			{
				Program.DraggedObject.MouseMoveFromParent(state.CellPosition);
				return true;
			}

			if (
					(Global.KeyboardState.IsKeyDown(Keys.LeftControl) ||
					Global.KeyboardState.IsKeyDown(Keys.RightControl)))
			{
				return false;
			}

			Handler?.Mouse(state, this);
			return true;
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			Handler?.Keyboard(info, this);
			return true;
		}
		#endregion
	}
}
