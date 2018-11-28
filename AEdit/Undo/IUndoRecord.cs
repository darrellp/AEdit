namespace AEdit.Undo
{
	interface IUndoRecord
	{
		void Undo();
		void Redo();
	}
}
