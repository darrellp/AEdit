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
		#endregion

		#region Public Properties
		public string Text
		{
			get => _textbox.EditingText;
			set => _textbox.Text = value;
		}

		public Color Foreground { get; private set; }
		#endregion

		#region Constructor
		public FigletControls(int width, int height) : base(width, height)
		{
			DefaultBackground = Color.Transparent;
			Clear();
			Foreground = Color.White;

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
			return new FigletRecord(_figletFont, Text, Foreground);
		}

		public override void SetParameters(object parms)
		{
			var fr = parms as FigletRecord;
			Debug.Assert(fr != null, "Bad record in SetParameters");
			Text = fr.Text;
			_figletFont = fr.FontName;
			Foreground = fr.Foreground;
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

		private void FillEdit(EditObject edit)
		{
			var (figlet, width, height) = FigletFromParms();
			edit.Resize(Math.Max(1, width), Math.Max(1, height), true);
			edit.DefaultBackground = Color.Transparent;
			edit.DefaultForeground = Foreground;
			edit.Clear();
			var iCells = 0;
			foreach (var line in figlet.Split('\n'))
			{
				foreach (var ch in line)
				{
					if (ch != ' ')
					{
						edit.Cells[iCells].Glyph = ch;
					}

					iCells++;
				}

				iCells = width * ((iCells + width - 1) / width);
			}
		}

		private (string figlet, int width, int height) FigletFromParms()
		{
			var font = FigletFont.FigletFromName(_figletFont);
			var arranger = new Arranger(font, 100) {Text = Text == string.Empty ? " " : Text};
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
