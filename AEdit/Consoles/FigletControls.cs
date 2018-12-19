using System;
using System.Diagnostics;
using System.Linq;
using AEdit.Consoles.Controls;
using AEdit.Undo;
using Microsoft.Xna.Framework;
using SadConsole.Controls;
using SadConsole.Themes;
using CSFiglet;
using SadConsole;
using static AEdit.AEGlobals;
using static AEdit.Undo.Undo;

namespace AEdit.Consoles
{
	internal class FigletControls : EditControl
	{
		#region Private variables
		private readonly DrawingSurface _foreSwatch;
		private string _figletFont;
		private TestTextBox _textbox;
		private Color _foreground;
		#endregion

		#region Public Properties
		public string Text
		{
			get => _textbox.EditingText;
			set
			{
				// ReSharper disable once RedundantCheckBeforeAssignment
				if (value != _textbox.Text)
				{
					_textbox.Text = value;
				}
			}
		}

		public Color Foreground
		{
			get => _foreground;
			private set
			{
				_foreground = value;
				if (Selected != null)
				{
					FillEdit(Selected);
				}
			}
		}
		#endregion

		#region Constructor
		public FigletControls(int width, int height) : base(width, height)
		{
			DefaultBackground = Color.Transparent;
			Clear();
			_foreground = Color.White;

			_foreSwatch = ControlHelpers.SetColorButton(this, new Point(1, 3), "Foregnd", Foreground, c => Foreground = c);

			_textbox = new TestTextBox(20)
			{
				Position = new Point(1, 5)
			};
			_textbox.IsDirtyChanged += _textbox_IsDirtyChanged;
			Add(_textbox);

			var fontlist = new ListBox(20, 9)
			{
				Position = new Point(1, 6)
			};
			foreach (var name in FigletFont.Names())
			{
				if (name != "term")
				{
					fontlist.Items.Add(name);
				}
			}
			Add(fontlist);
			fontlist.SelectedItem = _figletFont = (string)fontlist.Items[0];
			fontlist.SelectedItemChanged += Fontlist_SelectedItemChanged;
			
			var label = new DrawingSurface(20, 1)
			{
				Position = new Point(1, 1),

			};
			label.Surface.Print(0, 0, "FIGLET", Colors.Yellow, Color.Transparent);
			Add(label);
			UseKeyboard = true;
			Global.FocusedConsoles.Push(this);
		}
		#endregion

		#region EditControl abstract methods
		public override object GetParameterInfo()
		{
			return new FigletInfo(_figletFont, Text, Foreground);
		}

		public override void SetParameters(object parms)
		{
			var finfo = parms as FigletInfo;

			Debug.Assert(finfo != null, "Bad record in SetParameters");
			Text = finfo.Text;
			_figletFont = finfo.FontName;
			Foreground = finfo.Foreground;
			ControlHelpers.UpdateColorSwatch(_foreSwatch, Foreground);
		}

		public override bool Apply(EditObject edit)
		{
			if (edit.Mode != EditMode.Figlet)
			{
				return false;
			}
			SetParameters(edit.Parms);

			FillEdit(edit);
			return true;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Fills (and possibly resizes) edit with new char info. </summary>
		///
		/// <remarks>	Darrell Plank, 12/18/2018. </remarks>
		///
		/// <param name="edit">	The edit. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void FillEdit(EditObject edit)
		{
			var finfo = edit.Parms as FigletInfo;
			Debug.Assert(finfo != null);
			if (finfo.Foreground == Foreground && finfo.FontName == _figletFont && finfo.Text == Text)
			{
				return;
			}

			var fr = new FigletRecord(new FigletInfo( _figletFont, Text, Foreground), finfo);
			Main.TakeAction(fr);
			AddUndoRecord(fr);
		}

		private (string figlet, int width, int height) FigletFromParms()
		{
			var text = Text == string.Empty ? " " : Text;
			return FigletFromParms(_figletFont, text);
		}

		public static (string figlet, int width, int height) FigletFromParms(string fontName, string text)
		{
			var font = FigletFont.FigletFromName(fontName);
			var arranger = new Arranger(font, 100) { Text = text == string.Empty ? " " : text };
			var figlet = arranger.StringContents;
			var (width, height) = BoundingBoxSize(figlet);
			return (figlet, width, height);
		}

		public override object GetParmValue(string parm)
		{
			switch (parm)
			{
				case "Foregnd":
					return Foreground;

				default:
					Debug.Assert(false, "Asking for a parameter we don't have");
					break;
			}

			return null;
		}
		#endregion

		#region Helpers
		private static (int, int) BoundingBoxSize(string str)
		{
			var array = str.Split('\n');
			return (array.Max(s => s.Length), array.Length);
		}
		#endregion

		#region Handlers
		private void _textbox_IsDirtyChanged(object sender, EventArgs e)
		{
			if (Selected == null || Selected.Mode != EditMode.Figlet)
			{
				var rect = new Rectangle(new Point(), new Point(1, 1));
				var editObject = new EditObject(Drawing, EditMode.Figlet, GetParameterInfo(), rect);
				var insertRecord = new InsertRecord(editObject);
				AddUndoRecord(insertRecord);
				Main.TakeAction(insertRecord);
			}

			FillEdit(Selected);
		}

		private void Fontlist_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
		{
			_figletFont = (string)e.Item;
			FillEdit(Selected);
		}
		#endregion
	}
}
