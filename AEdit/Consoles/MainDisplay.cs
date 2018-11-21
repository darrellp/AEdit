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
		#region Private variables
		private EditMode _mode;

		private static readonly IHandler[] _handlerTable =
		{
			new PencilHandler(),
			new LineHandler(), 
		};

		private IHandler Handler { get; set; }
		#endregion

		#region Public properties
		public EditMode Mode
		{
			get => _mode;
			set => SetMode(_mode = value);
		}
		#endregion

		#region Constructor
		public MainDisplay(int width, int height) : base(width, height) { }
		#endregion

		#region Event handling
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
		#endregion

		#region Operations
		private void BaseClear()
		{
			base.Clear();
		}

		public new void Clear()
		{
			Program.Undos.CreateUndo();
			// The displaying window is no longer us so we have to tell that window to do a real clear
			Program.MainDisplay.BaseClear();
		}
		#endregion
	}
}
