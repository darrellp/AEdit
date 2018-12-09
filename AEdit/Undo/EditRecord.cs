﻿using static AEdit.AEGlobals;

namespace AEdit.Undo
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Interface for apply record. </summary>
	///
	/// <remarks>	Just a dummy interface for type checking's sake
	/// 			Darrell Plank, 12/9/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	class EditRecord
	{
		public void Undo()
		{
			DoRaiseUndoEvent(this, true);
		}

		public void Apply()
		{
			DoRaiseUndoEvent(this, false);
		}
	}
}
