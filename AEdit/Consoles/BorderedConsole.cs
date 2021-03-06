﻿
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using Console = SadConsole.Console;

namespace AEdit
{
	internal class BorderedConsole : Console
	{
		#region Private variables
		private Basic BorderSurface { get; }
		#endregion

		#region Constructors
		public BorderedConsole(int width, int height, Color fore, Color back) : base(width, height)
		{
			BorderSurface = new Basic(width + 2, height + 2, Font);
			BorderSurface.DrawBox(new Rectangle(0, 0, BorderSurface.Width, BorderSurface.Height),
				new Cell(fore, back), null, ConnectedLineThick);
			BorderSurface.Position = new Point(-1, -1);

			Children.Add(BorderSurface);
		}

		public BorderedConsole(int width, int height) : this(width, height, Color.White, Color.Black)
		{
		}
		#endregion
	}
}