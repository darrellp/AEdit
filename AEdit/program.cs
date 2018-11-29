using System.Diagnostics;
using AEdit.Consoles;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Themes;
using static SadConsole.Game;
using static SadConsole.Global;
using static SadConsole.Settings;

namespace AEdit
{
	// TODO: Don't "catch" objects when ctl dragging
	// Right now we only look at whether CTL is down and the mouse button is
	// down to decide when to drag.  This means if we have control and start
	// "dragging" over open space then as soon as we encounter an EditObject
	// we'll pick it up and start moving it.
	// TODO: Individual control panels for different handlers
	// Different modes should have mode specific controls
	// TODO: Layers
	// TODO: Object selection
	// TODO: Z Order - probably depends on layering UI
	// TODO: Rectangles
	// TODO: Circles
	// TODO: Exporting/Importing
	// TODO: Importing from web site
	// TODO: Figlet fonts
	// TODO: Copy/Paste
	// TODO: Connect paint gaps
	// TODO: Offer different line types and potentially other shapes also
	// ReSharper disable once ClassNeverInstantiated.Global
	internal class Program
    {

		#region Private variables
		private static Console _debugConsole;
	    private static Console _startingConsole;
	    private static ControlPanel _controlPanel;
		#endregion

		#region Public variables
	    public static ControlPanel ControlPanel => _controlPanel;
	    public static EditObject DraggedObject;
	    public static MainDisplay MainDisplay { get; private set; }
		#endregion

		#region Formatting constants
	    public const int DefaultWidth = 140;
	    public const int DefaultHeight = 40;
	    public const int DefaultControlWidth = 23;
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

			_startingConsole = new Console(DefaultWidth, DefaultHeight);
            CurrentScreen = _startingConsole;

            // Set our new console as the thing to render and process
	        SetupConsoles();
		}

	    private static void SetupConsoles()
	    {
			_controlPanel = new ControlPanel(DefaultControlWidth, DefaultHeight);
		    ControlPanel.Fill(Color.White, Colors.BlueDark, 0);
			MainDisplay = new MainDisplay(DefaultWidth - DefaultControlWidth, DefaultHeight);
		    MainDisplay.Drawing.IsFocused = true;
		    MainDisplay.Drawing.Mode = EditMode.Brush;
			MainDisplay.Position = new Point(ControlPanel.Width, 0);

		    CurrentScreen.Children.Add(MainDisplay);
		    CurrentScreen.Children.Add(ControlPanel);
		    SetupDebug();
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
