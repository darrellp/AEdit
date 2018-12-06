using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using static System.Char;
using static System.Math;

namespace AEdit
{
	public static class Extensions
	{
		public static bool InRange(this int value, int from, int to)
		{
			return value >= @from && value <= to;
		}

		public static bool InRange(this double value, double from, double to)
		{
			return value >= @from && value <= to;
		}

		public static int ClipTo(this int value, int from, int to)
		{
			return value > to ? to : (value < @from ? @from : value);
		}

		public static double ClipTo(this double value, double from, double to)
		{
			return value > to ? to : (value < @from ? @from : value);
		}

		private static (int width, int height) Dimensions(this ScreenObject obj)
		{
			if (obj is SurfaceBase thisSurface)
			{
				return (thisSurface.Width, thisSurface.Height);
			}

			var width = 0;
			var height = 0;

			foreach (var child in obj.Children)
			{
				var (wCur, hCur) = child.Dimensions();
				var (x, y) = child.Position;
				width = Max(width, wCur + x);
				height = Max(height, hCur + y);
			}

			return (width, height);
		}

		public static string ToAscii(this SurfaceBase surface)
		{
			var sb = new StringBuilder();
			var cellpos = 0;
			for (var iRow = 0; iRow < surface.Height; iRow++)
			{
				sb.Append(surface.GetString(cellpos, surface.Width).Replace('\0', ' '));
				sb.Append(Environment.NewLine);
				cellpos += surface.Width;
			}

			return sb.ToString();
		}

		public static string ToHtml(this SurfaceBase surface)
		{
			var sbText = new StringBuilder();
			var cellpos = 0;
			var colors = new List<(Color, Color)>();
			var colorDict = new Dictionary<(Color, Color), int>();
			var nextColor = 0;

			sbText.Append("<p>");
			for (var iRow = 0; iRow < surface.Height; iRow++)
			{
				while (true)
				{
					var (run, fg, bg, isRowEnd) = GetRun(surface, ref cellpos);
					if (!colorDict.TryGetValue((fg, bg), out int iColor))
					{
						iColor = nextColor++;
						colorDict[(fg, bg)] = iColor;
						colors.Add((fg, bg));
					}

					sbText.Append($"<span class=\"ae{iColor:000}\">");
					sbText.Append(run);
					sbText.Append($"</span>");
					if (isRowEnd)
					{
						break;
					}
				}
				sbText.Append("<br />" + Environment.NewLine);
			}

			sbText.Append("</p>" + Environment.NewLine);

			var sbStyles = new StringBuilder(
				"<html>" + Environment.NewLine +
				"<head>" + Environment.NewLine +
				"<style>" + Environment.NewLine);
			for (var i = 0; i < colors.Count; i++)
			{
				var (fg, bg) = colors[i];
				sbStyles.Append(
					$".ae{i:000} {{font-family: monospace; color: rgb({fg.R},{fg.G},{fg.B}); background-color: rgb({bg.R},{bg.G},{bg.B});}}" +
				    Environment.NewLine);
			}

			sbStyles.Append("</style>" + Environment.NewLine + "</head>" + Environment.NewLine);
			return sbStyles.ToString() + sbText + "</body>" + Environment.NewLine + "</html>";
		}

		private static (string run, Color fg, Color bg, bool isRowEnd) GetRun(SurfaceBase surface, ref int cellpos)
		{
			var row = cellpos / surface.Width;
			var cell = surface.Cells[cellpos];
			var fg = cell.Foreground;
			var bg = cell.Background;
			var nextRow = (row + 1) * surface.Width;
			var run = new StringBuilder();

			while (true)
			{
				cell = surface.Cells[cellpos];

				if (cell.Foreground != fg || cell.Background != bg)
				{
					return (run.ToString(), fg, bg, false);
				}
				var ch = cell.Glyph == 0 ? "&nbsp" : ((char)cell.Glyph).ToString();
				run.Append(ch);
				if (++cellpos == nextRow)
				{
					return (run.ToString(), fg, bg, true);
				}
			}
		}

		public static SurfaceBase Flatten(this ScreenObject obj, bool fCrop = true)
		{
			if (obj is SurfaceBase sb && obj.Children.Count == 0 && !fCrop)
			{
				// If we're a surface with no children and we don't need to be cropped
				// then we already are flattened.
				return sb;
			}
			var flattenedChildren = new List<SurfaceBase>();
			foreach (var child in obj.Children)
			{
				flattenedChildren.Add(child.Flatten(false));
			}

			var (width, height) = obj.Dimensions();
			var ptUL = new Point(width, height);
			var ptLR = new Point(0, 0);

			// Composite all our children
			SurfaceBase tmp = new Basic(width, height);
			foreach (var flattened in flattenedChildren)
			{
				for (var ix = 0; ix < flattened.Width; ix++)
				{
					for (var iy = 0; iy < flattened.Height; iy++)
					{
						var cell = flattened[ix, iy];
						if (cell.IsVisible())
						{
							var xPos = flattened.Position.X + ix;
							var yPos = flattened.Position.Y + iy;
							ptUL.X = Min(xPos, ptUL.X);
							ptUL.Y = Min(yPos, ptUL.Y);
							ptLR.X = Max(xPos, ptLR.X);
							ptLR.Y = Max(yPos, ptLR.Y);
							tmp.SetCellAppearance(xPos, yPos, cell);
							tmp.SetGlyph(xPos, yPos, cell.Glyph);
						}
					}
				}
			}

			// Composite us in as well if we've actually got data to composite
			if (obj is SurfaceBase thisSurface)
			{
				for (var ix = 0; ix < thisSurface.Width; ix++)
				{
					for (var iy = 0; iy < thisSurface.Height; iy++)
					{
						var cell = thisSurface[ix, iy];
						if (cell.IsVisible())
						{
							ptUL.X = Min(ix, ptUL.X);
							ptUL.Y = Min(iy, ptUL.Y);
							ptLR.X = Max(ix, ptLR.X);
							ptLR.Y = Max(iy, ptLR.Y);
							tmp.SetCellAppearance(ix, iy, cell);
							tmp.SetGlyph(ix, iy, cell.Glyph);
						}
					}
				}
			}

			var ret = tmp;
			if (fCrop)
			{
				// Crop unused space into a final returned SurfaceBase
				var finalWidth = ptLR.X - ptUL.X + 1;
				var finalHeight = ptLR.Y - ptUL.Y + 1;
				ret = new Basic(finalWidth, finalHeight);
				for (var ix = 0; ix < finalWidth; ix++)
				{
					for (var iy = 0; iy < finalHeight; iy++)
					{
						var cell = tmp[ix + ptUL.X, iy + ptUL.Y];
						ret.SetCellAppearance(ix, iy, cell);
						ret.SetGlyph(ix, iy, cell.Glyph);
					}
				}
			}
			ret.Position = obj.Position;

			return ret;
		}

		public static void Deconstruct(this Point source, out int X, out int Y)
		{
			X = source.X;
			Y = source.Y;
		}

		public static bool IsVisible(this Cell cell)
		{
			return cell.Background != Color.Transparent ||
			       !IsWhiteSpace((char) cell.Glyph) && cell.Foreground != Color.Transparent;
		}
	}
}
