using Microsoft.Xna.Framework;

namespace AEdit.Undo
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Information about a figlet. </summary>
	///
	/// <remarks>	Currently we only have a foreground color with a transparent background color.
	/// 			Darrell Plank, 12/9/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	class FigletRecord : EditRecord
	{
		public string FontName { get; }
		public string Text { get; }
		public Color Foreground { get; }

		public FigletRecord(string fontName, string text, Color foreground)
		{
			FontName = fontName;
			Text = text;
			Foreground = foreground;
		}
	}
}
