using AEdit.Consoles;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using static SadConsole.Game;
using static SadConsole.Global;
using static SadConsole.Settings;

namespace AEdit
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class Program
    {
	    internal static Console StartingConsole;
		internal static Console MainDisplay => _undos.CurrentDisplay;
	    private static ControlPanel ControlPanel => _controlPanel;
		internal static Point MainDisplayPosition => new Point(DefaultControlWidth, 0);
	    private static ControlPanel _controlPanel;
	    private static Undo _undos;

	    private const int DefaultWidth = 120;
	    private const int DefaultHeight = 40;
	    private const int DefaultControlWidth = 20;

	    internal static Undo Undos => _undos;

	    private static void Main()
        {
			// Setup the engine and create the main window.
			Create("Fonts/IBM.font", DefaultWidth, DefaultHeight);

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
			_controlPanel = new ControlPanel(DefaultControlWidth, DefaultHeight);
		    ControlPanel.Fill(Color.White, Color.Wheat, 0);
		    _undos = new Undo(EditMode.Pencil, DefaultWidth - DefaultControlWidth, DefaultHeight);
		    MainDisplay.IsFocused = true;

		    CurrentScreen.Children.Add(MainDisplay);
		    CurrentScreen.Children.Add(ControlPanel);
	    }
	}
}
