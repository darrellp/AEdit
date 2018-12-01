using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEdit
{
	public static class Extensions
	{
		public static bool InRange(this int value, int from, int to)
		{
			return value >= @from && value <= to;
		}

		public static bool InRange(this double value, double from, double to)
		{
			return value >= @from && value <= to;
		}

		public static int ClipTo(this int value, int from, int to)
		{
			return value > to ? to : (value < @from ? @from : value);
		}

		public static double ClipTo(this double value, double from, double to)
		{
			return value > to ? to : (value < @from ? @from : value);
		}
	}
}
