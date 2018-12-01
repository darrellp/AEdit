using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEdit.Consoles;
using Microsoft.Xna.Framework;

namespace AEdit.Undo
{
	class MoveRecord : IUndoRecord
	{
		private readonly Point _start;
		private readonly Point _end;
		private readonly EditObject _edit;

		public MoveRecord(Point start, Point end, EditObject edit)
		{
			_start = start;
			_end = end;
			_edit = edit;
		}
		public void Undo()
		{
			_edit.Position = _start;
		}

		public void Redo()
		{
			_edit.Position = _end;
		}
	}
}
