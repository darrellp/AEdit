using System.Collections.Generic;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using SadConsole.Themes;

namespace AEdit
{
	internal class LineBrush
	{
		#region Private variables
		private readonly char[] _chars;
		internal static readonly LineBrush EraseBrush = new LineBrush("            ");
		#endregion

		#region Constructor
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor. </summary>
		///
		/// <remarks>	
		/// 			chars in string are in the following order (which matches 
		/// 			the order in LineStatus):
		/// 			
		/// 			chDegenerate,
		/// 			chHorizontal,
		/// 			chVertical,
		/// 			chSUp,
		/// 			chSDown,
		/// 			chSLeft,
		/// 			chSRight,
		/// 			chAUp,
		/// 			chADown,
		/// 			chALeft,
		/// 			chARight)
		/// 			
		/// 			Darrell Plank, 11/18/2018. </remarks>
		///
		/// <param name="chars">	The characters to draw for the various LineStatus cases. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public LineBrush(string chars)
		{
			_chars = chars.ToCharArray();
		}
		#endregion

		#region Queries
		public char CharFromLineStatus(LineStatus ls)
		{
			return _chars[(int) ls];
		}
		#endregion
	}

	internal static class SadGeometry
	{
		#region Private variables
		private static readonly LineBrush _defaultBrush = new LineBrush(".-|`,/\\,`/\\");
		private static readonly LineBrush _eraseBrush = new LineBrush("\0\0\0\0\0\0\0\0\0\0\0");
		#endregion

		#region Operations

		public static void DrawLine(Point start, Point end, Console console, LineBrush brush = null)
		{
			DrawLine(start, end, console, Color.White, Color.Black, brush);
		}

		public static void DrawLine(Point start, Point end, Console console, Color fore, Color back, LineBrush brush = null)
		{
			if (brush == null)
			{
				brush = _defaultBrush;
			}

			var ls = new LineStepper(start, end);
			foreach (var (pt, linfo) in ls.Locations())
			{
				console.SetGlyph(pt.X, pt.Y, brush.CharFromLineStatus(linfo.LineStatus));
				console.SetForeground(pt.X, pt.Y, fore);
				console.SetBackground(pt.X, pt.Y, back);
			}
		}

		public static List<int> DrawLineMemory(Point start, Point end, Console console, LineBrush brush = null)
		{
			return DrawLineMemory(start, end, console, Color.White, Color.Black, brush);
		}

		public static List<int> DrawLineMemory(Point start, Point end, Console console, Color fore, Color back, LineBrush brush = null)
		{
			if (brush == null)
			{
				brush = _defaultBrush;
			}

			var ret = new List<int>();
			var ls = new LineStepper(start, end);
			foreach (var (pt, linfo) in ls.Locations())
			{
				ret.Add(console.GetGlyph(pt.X, pt.Y));
				console.SetGlyph(pt.X, pt.Y, brush.CharFromLineStatus(linfo.LineStatus));
				console.SetForeground(pt.X, pt.Y, fore);
				console.SetBackground(pt.X, pt.Y, back);
			}

			return ret;
		}

		public static void DrawLineFromMemory(Point start, Point end, Console console, List<int> memory)
		{
			var ls = new LineStepper(start, end);
			var i = 0;
			foreach (var (pt, _) in ls.Locations())
			{
				console.SetGlyph(pt.X, pt.Y, i >= memory.Count ? 0 :memory[i++]);
			}
		}

		public static void EraseLine(Point start, Point end, Console console)
		{
			DrawLine(start, end, console, Color.Transparent, Color.Transparent, _eraseBrush);
		}
		#endregion
	}
}
