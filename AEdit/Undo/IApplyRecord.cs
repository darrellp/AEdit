namespace AEdit.Undo
{
	interface IApplyRecord
	{
		void Undo();
		void Apply();
	}
}
