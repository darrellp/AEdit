using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AEdit
{
	interface IStepper<T>
	{
		IEnumerable<(Point pt, T info)> Locations();
	}
}
