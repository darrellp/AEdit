using System.Diagnostics;
using AEdit.Consoles;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using SadConsole;
using static SadConsole.Game;
using static SadConsole.Global;
using static SadConsole.Settings;

namespace AEdit
{
	// ReSharper disable once ClassNeverInstantiated.Global
	internal class Program
    {
		#region Private variables
		private static ControlPanel _controlPanel;
	    private static Console _debugConsole;
	    private static Undo _undos;
	    private static ControlPanel ControlPanel => _controlPanel;
	    private static MainDisplay _mainDisplay;
		#endregion

		#region Public variables
	    private static Console StartingConsole;
	    public static MainDisplay MainDisplay => _mainDisplay;
	    public static Undo Undos => _undos;
		#endregion

		#region Formatting constants
		private const int DefaultWidth = 140;
	    private const int DefaultHeight = 40;
	    private const int DefaultControlWidth = 23;
#if DEBUG
		private const int DebugConsoleHeight = 10;
#else
		private const int DebugConsoleHeight = 0;
#endif
		#endregion

		#region Main
		private static void Main()
        {
			// Setup the engine and create the main window.
			Create("Fonts/IBM.font", DefaultWidth, DefaultHeight + DebugConsoleHeight);

            // Hook the start event so we can add consoles to the system.
            OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            OnUpdate = Update;
			
            // Start the editor.
            Instance.Run();

            //
            // Code here will not run until the game window closes.
            //
            
            Instance.Dispose();
        }
#endregion

#region Handlers
		private static void Update(GameTime time)
        {
            // Called each logic update.

            // As an example, we'll use the F5 key to make the game full screen
            if (KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                ToggleFullScreen();
            }
        }

        private static void Init()
        {
	        Instance.Window.Title = "AEdit";

			StartingConsole = new Console(DefaultWidth, DefaultHeight);
            CurrentScreen = StartingConsole;

            // Set our new console as the thing to render and process
	        SetupConsoles();
		}

	    private static void SetupConsoles()
	    {
		    SetupDebug();

			_controlPanel = new ControlPanel(DefaultControlWidth, DefaultHeight);
		    ControlPanel.Fill(Color.White, Color.Wheat, 0);
		    _undos = new Undo();
			_mainDisplay = new MainDisplay(DefaultWidth - DefaultControlWidth, DefaultHeight);
		    MainDisplay.Drawing.IsFocused = true;
		    MainDisplay.Drawing.Mode = EditMode.Brush;
			_mainDisplay.Position = new Point(ControlPanel.Width, 0);

		    CurrentScreen.Children.Add(MainDisplay);
		    CurrentScreen.Children.Add(ControlPanel);
	    }

	    [Conditional("DEBUG")]
		private static void SetupDebug()
	    {
		    _debugConsole = new Console(DefaultWidth, DebugConsoleHeight)
		    {
			    Position = new Point(0, DefaultHeight),
			    DefaultBackground = Color.MidnightBlue,
			    Cursor = {PrintAppearance = new Cell(Color.White, Color.MidnightBlue)}
		    };
		    CurrentScreen.Children.Add(_debugConsole);
		}

	    [Conditional("DEBUG")]
	    // ReSharper disable once UnusedMember.Global
	    // ReSharper disable once InconsistentNaming
	    public static void AETrace(string printString)
	    {
			_debugConsole.Cursor.Print(printString);
	    }

	    [Conditional("DEBUG")]
	    // ReSharper disable once UnusedMember.Global
	    // ReSharper disable once InconsistentNaming
		public static void AETraceLine(string printString)
	    {
		    _debugConsole.Cursor.Print(printString + "\r\n");
	    }
		#endregion
	}
}
