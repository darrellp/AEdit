using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using SadConsole.Controls;
using SadConsole.Themes;
using Keyboard = SadConsole.Input.Keyboard;

namespace AEdit.Consoles.Controls
{
	class TestTextBox : TextBox
	{
		/// <summary>
		/// The theme of the control.
		/// </summary>
		[DataMember(Name = "Theme")]
		protected TestTextBoxTheme _theme;

		/// <summary>
		/// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
		/// </summary>
		public TestTextBoxTheme Theme
		{
			get => _theme;
			set
			{
				_theme = value;
				_theme.Attached(this);
				DetermineState();
				IsDirty = true;
			}
		}

		public TestTextBox(int width) : base(width)
		{
			Theme = new TestTextBoxTheme();
		}

		public override void Update(TimeSpan time)
		{
			Theme.UpdateAndDraw(this, time);
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			if (info.KeysPressed.Count != 0)
			{
				if (DisableKeyboard)
				{
					for (int i = 0; i < info.KeysPressed.Count; i++)
					{
						if (info.KeysPressed[i].Key == Keys.Enter)
						{
							this.IsDirty = true;
							DisableKeyboard = false;
							Text = EditingText;
						}
					}
					return true;
				}
				else
				{
					System.Text.StringBuilder newText = new System.Text.StringBuilder(EditingText, Width - 1);

					this.IsDirty = true;

					for (int i = 0; i < info.KeysPressed.Count; i++)
					{
						if (_isNumeric)
						{
							if (info.KeysPressed[i].Key == Keys.Back && newText.Length != 0)
								newText.Remove(newText.Length - 1, 1);

							else if (info.KeysPressed[i].Key == Keys.Enter)
							{
								DisableKeyboard = true;

								Text = EditingText;

								return true;
							}
							else if (info.KeysPressed[i].Key == Keys.Escape)
							{
								DisableKeyboard = true;
								return true;
							}

							else if (char.IsDigit(info.KeysPressed[i].Character) || (_allowDecimalPoint && info.KeysPressed[i].Character == '.'))
							{
								newText.Append(info.KeysPressed[i].Character);
							}

							PositionCursor();
						}

						else
						{
							if (info.KeysPressed[i].Key == Keys.Back && newText.Length != 0 && _caretPos != 0)
							{
								if (_caretPos == newText.Length)
									newText.Remove(newText.Length - 1, 1);
								else
									newText.Remove(_caretPos - 1, 1);

								_caretPos -= 1;

								if (_caretPos == -1)
									_caretPos = 0;
							}
							else if (info.KeysPressed[i].Key == Keys.Space && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength)))
							{
								newText.Insert(_caretPos, ' ');
								_caretPos++;

								if (_caretPos > newText.Length)
									_caretPos = newText.Length;
							}

							else if ((info.KeysPressed[i].Key == Keys.Decimal || info.KeysPressed[i].Key == Keys.Delete) && _caretPos != newText.Length)
							{
								newText.Remove(_caretPos, 1);

								if (_caretPos > newText.Length)
									_caretPos = newText.Length;
							}

							else if (info.KeysPressed[i].Key == Keys.Enter)
							{
								Text = EditingText;
								DisableKeyboard = true;
								return true;
							}
							else if (info.KeysPressed[i].Key == Keys.Escape)
							{
								DisableKeyboard = true;
								return true;
							}
							else if (info.KeysPressed[i].Key == Keys.Left)
							{
								_caretPos -= 1;

								if (_caretPos == -1)
									_caretPos = 0;
							}
							else if (info.KeysPressed[i].Key == Keys.Right)
							{
								_caretPos += 1;

								if (_caretPos > newText.Length)
									_caretPos = newText.Length;
							}

							else if (info.KeysPressed[i].Key == Keys.Home)
							{
								_caretPos = 0;
							}

							else if (info.KeysPressed[i].Key == Keys.End)
							{
								_caretPos = newText.Length;
							}

							else if (info.KeysPressed[i].Character != 0 && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength)))
							{
								newText.Insert(_caretPos, info.KeysPressed[i].Character);
								_caretPos++;

								if (_caretPos > newText.Length)
									_caretPos = newText.Length;
							}

							// Test to see if caret is off edge of box
							if (_caretPos >= Width)
							{
								LeftDrawOffset = newText.Length - Width + 1;

								if (LeftDrawOffset < 0)
									LeftDrawOffset = 0;
							}
							else
							{
								LeftDrawOffset = 0;
							}
						}

					}

					EditingText = newText.ToString();

					ValidateEdit();
				}

				return true;
			}

			return false;
		}
		protected void ValidateEdit()
		{
			PositionCursor();

			DetermineState();
			IsDirty = true;
		}

		/// <summary>
		/// Correctly positions the cursor within the text.
		/// </summary>
		protected void PositionCursor()
		{
			if (MaxLength != 0 && EditingText.Length > MaxLength)
			{
				EditingText = EditingText.Substring(0, MaxLength);

				if (EditingText.Length == MaxLength)
					_caretPos = EditingText.Length - 1;
			}

			// Test to see if caret is off edge of box
			if (_caretPos >= Width)
			{
				LeftDrawOffset = EditingText.Length - Width + 1;

				if (LeftDrawOffset < 0)
					LeftDrawOffset = 0;
			}
			else
			{
				LeftDrawOffset = 0;
			}
		}
	}
}
