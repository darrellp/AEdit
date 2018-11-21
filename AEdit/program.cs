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
	public class Program
    {
	    private const int DefaultWidth = 120;
	    private const int DefaultHeight = 40;
	    private const int DefaultControlWidth = 20;

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

	    internal static Console startingConsole;

        private static void Init()
        {
	        Instance.Window.Title = "AEdit";

			startingConsole = new Console(DefaultWidth, DefaultHeight);
            CurrentScreen = startingConsole;

            // Set our new console as the thing to render and process
	        SetupConsoles();
		}

	    private static void SetupConsoles()
	    {
		    var controls = new ControlPanel(DefaultControlWidth, DefaultHeight);
		    controls.Fill(Color.White, Color.Wheat, 0);

		    var display = new MainDisplay(DefaultWidth - DefaultControlWidth, DefaultHeight);
		    display.Fill(Color.White, Color.MidnightBlue, 0);
			display.Position = new Point(DefaultControlWidth, 0);
			display.Mode = EditMode.Line;

		    CurrentScreen.Children.Add(display);
		    CurrentScreen.Children.Add(controls);
	    }
	}
}
