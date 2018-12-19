using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEdit.Undo
{
	internal class FigletRecord : EditRecord
	{
		public FigletRecord(FigletInfo newInfo, FigletInfo oldInfo)
		{
			OldInfo = oldInfo;
			NewInfo = newInfo;
		}

		public FigletInfo OldInfo { get; }
		public FigletInfo NewInfo { get; }
	}
}
