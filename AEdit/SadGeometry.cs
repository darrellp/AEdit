using System;
using System.Collections.Generic;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;

namespace AEdit
{
	public class LineBrush
	{
		private readonly char[] _chars;
		public static readonly LineBrush EraseBrush = new LineBrush("            ");

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

		public char CharFromLineStatus(LineStatus ls)
		{
			return _chars[(int) ls];
		}
	}



	public static class SadGeometry
	{
		private static readonly LineBrush _defaultBrush = new LineBrush(".-|`,/\\,`/\\");

		private static void DrawLine(Point start, Point end, Console console, LineBrush brush = null)
		{
			if (brush == null)
			{
				brush = _defaultBrush;
			}

			var ls = new LineStepper(start, end);
			foreach (var (pt, linfo) in ls.Locations())
			{
				console.SetGlyph(pt.X, pt.Y, brush.CharFromLineStatus(linfo.LineStatus));
			}
		}

		public static List<int> DrawLineMemory(Point start, Point end, Console console, LineBrush brush = null)
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
			DrawLine(start, end, console, LineBrush.EraseBrush);
		}
	}
}
