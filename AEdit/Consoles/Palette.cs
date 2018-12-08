using System.Linq;
using AEdit.Windows;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace AEdit.Consoles
{
	class MyPalette : Console
	{
		private static MyPalette _singleton;
		public static MyPalette Singleton
		{
			get
			{
				if (_singleton == null)
				{
					_singleton = new MyPalette(ColorPicker.WidthConst - 2, 1);
				}

				return _singleton;
			}
		}

		public ColorPicker ColorPicker { get; set; }

		private MyPalette(int width, int height) : base(width, height)
		{
			Fill(Color.Transparent, Color.Transparent, ' ');
		}

		public override bool ProcessMouse(MouseConsoleState state)
		{
			if (!state.IsOnConsole || !state.Mouse.LeftClicked)
			{
				return false;
			}

			var localPos = state.CellPosition - Position;
			if (localPos.Y != 0)
			{
				return false;
			}

			var cell = Cells[localPos.X];

			if (cell.Background == Color.Transparent)
			{
				return false;
			}

			ColorPicker.CurColor = cell.Background;

			return true;
		}

		public void InsertMyPaletteColor(Color color)
		{
			foreach (var cell in Cells.Where(c => c.Background != Color.Transparent))
			{
				if (cell.Background == color)
				{
					return;
				}
			}
			for (var i = Cells.Length - 2; i >= 0; i--)
			{
				Cells[i + 1].CopyAppearanceFrom(Cells[i]);
			}
			this[0].Background = color;
			IsDirty = true;
		}
	}
}
