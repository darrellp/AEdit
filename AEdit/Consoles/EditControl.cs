using System;
using SadConsole;

namespace AEdit.Consoles
{
	internal abstract class EditControl : ControlsConsole
	{
		protected EditControl(int width, int height) : base(width, height) { }
		public abstract Object GetParameterInfo();
		public abstract void SetParameters(Object parms);
		public virtual EditMode Mode => EditMode.Null;
		public abstract bool Apply(EditObject edit);
		public abstract object GetParmValue(string parm);

		public virtual void Enable(bool doEnable)
		{
			foreach (var control in Controls)
			{
				control.IsEnabled = doEnable;
			}
		}
	}
}
