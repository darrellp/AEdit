using Microsoft.Xna.Framework;

namespace AEdit.Undo
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Information about a figlet. </summary>
	///
	/// <remarks>	Currently we only have a foreground color with a transparent background color.
	/// 			Darrell Plank, 12/9/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	class FigletInfo
	{
		public string FontName { get; }
		public string Text { get; }
		public Color Foreground { get; }

		public FigletInfo(string fontName, string text, Color foreground)
		{
			FontName = fontName;
			Text = text;
			Foreground = foreground;
		}
	}
}
