using AEdit.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Input;
using static AEdit.AEGlobals;
using Console = SadConsole.Console;
using Keyboard = SadConsole.Input.Keyboard;
using static AEdit.Undo.Undo;

namespace AEdit.Consoles
{
	internal enum EditMode
	{
		Null = -1,
		Brush,
		Line,
		Canvas,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A drawing console. </summary>
	///
	/// <remarks>	The drawing console is what the user draws on (duh).  When the user finishes a
	/// 			drawing operation the handler will call MainWindow.SetObject() with a bounding
	/// 			rectangle for the drawn element.  MainWindow will turn it into an EditObject
	/// 			Console, add that as a child, clear the DrawingConsole and ensure it's the
	/// 			top console in Main.
	/// 			Darrell Plank, 11/26/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	class DrawingConsole : Console
	{
		#region Private variables
		private static readonly IHandler[] _handlerTable =
		{
			new PaintHandler(),
			new LineHandler(),
			new FlatHandler(), 
		};

		private EditMode _mode = EditMode.Null;
		private bool _ctlCheck = true;
		private IHandler _handler;
		#endregion

		#region Properties
		public IHandler Handler => _handler;

		public EditMode Mode
		{
			get => _mode;
			set
			{
				SetMode(value);
				_mode = value;
			}
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
			if (mode == EditMode.Null || mode == Mode)
			{
				return;
			}
			Handler?.Exit();
			_handler = _handlerTable[(int)mode];
			_handler.Reset();
			Ctrls.InstallModeSpecificControls(mode);
		}

		public override bool ProcessMouse(MouseConsoleState state)
		{
			if (!state.IsOnConsole)
			{
				return false;
			}

			if (DraggedObject != null)
			{
				DraggedObject.MouseMoveFromParent(state.CellPosition);
				return true;
			}

			if (state.Mouse.LeftButtonDown &&
			    (Global.KeyboardState.IsKeyDown(Keys.LeftControl) ||
				Global.KeyboardState.IsKeyDown(Keys.RightControl)))
			{
				var doCheck = !_ctlCheck;
				_ctlCheck = false;
				return doCheck;
			}

			_ctlCheck = true;
			Handler?.Mouse(state, this);
			return true;
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			if (info.IsKeyDown(Keys.LeftControl) || info.IsKeyDown(Keys.RightControl))
			{
				if (info.IsKeyPressed(Keys.Z))
				{
					PerformUndo();
					return true;
				}

				if (info.IsKeyPressed(Keys.Y))
				{
					PerformRedo();
					return true;
				}
			}
			Handler?.Keyboard(info, this);
			return false;
		}
		#endregion
	}
}
