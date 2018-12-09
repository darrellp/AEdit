namespace AEdit.Consoles
{
	class FlatControl : EditControl
	{
		public FlatControl(int width, int height) : base(width, height)
		{
		}

		public override object GetParameterInfo()
		{
			// Nothing to do right now
			return null;
		}

		public override void SetParameters(object parms)
		{
			// Nothing to do right now
		}

		public override bool Apply(EditObject edit)
		{
			// Nothing to do right now
			return true;
		}

		public override object GetParmValue(string parm)
		{
			// Nothing to do right now
			return null;
		}
	}
}
