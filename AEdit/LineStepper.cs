using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static System.Math;

namespace AEdit
{
	public enum LineStatus
	{
		Degenerate,		// Beginning and end of line are identical
		Horizontal,		// Line is horizontal
		Vertical,		// Line is vertical
		SUp,			// Starting up - one char later we will have moved up a line
		SDown,			// Starting down
		SLeft,			// Starting left
		SRight,			// Starting right
		AUp,			// We've arrived up a char and won't be going up on next char (hence, not SUp)
		ADown,			// Arriving down
		ALeft,			// Arriving left
		ARight,			// Arriving right
	}

	#region Internal classes
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Information about the line drawing process. </summary>
	///
	/// <remarks>	As we step through the line this is the information we return.
	/// 			Aliasing information will only be supplied if it's asked for in
	/// 			the LineStepper constructor.  If it is supplied it is a double
	/// 			between 0 and 1 representing the portion of the way we are to the
	/// 			next line.  
	/// 			Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	internal struct LineInfo
	{
		public LineStatus LineStatus { get; }
		private double Alias { get; }

		public LineInfo(LineStatus lineStatus, double alias = 0)
		{
			LineStatus = lineStatus;
			Alias = alias;
		}
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A line stepper using Bresenham's algorithm </summary>
	///
	/// <remarks>	Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	internal class LineStepper : IStepper<LineInfo>
	{
		#region Private variables
		private int _bresenhamSum;					// Sum for Bresenham's thus far
		private readonly int _bresenhamAdd;			// How much to add to the sum each time
		private readonly int _bresenhamSpan;		// How big the span is
		private readonly Point _dvShort;			// Amount to add along short length
		private readonly Point _dvLong;				// Amount to add along long length
		private Point _current;						// Next point to return
		private LineStatus _lineStatus;				// Next lineInfo to return
		private readonly LineStatus _sShortMove;	// LineStatus for when we start the short move
		private readonly LineStatus _aShortMove;	// LineStatus for when we arrive on the short move
		private readonly LineStatus _longMove;		// LineStatus if not doing short move
		private readonly bool _doAliasing;			// Whether we should bother with aliasing info.
		private readonly Point _final;              // Last point we'll produce
		#endregion

		#region Constructor
		////////////////////////////////////////////////////////////////////////////////////////////////////
		///  <summary>	Constructor. </summary>
		/// 
		///  <remarks>	Darrell Plank, 11/17/2018. </remarks>
		/// 
		///  <param name="start">	The starting point. </param>
		///  <param name="end">  	The ending point. </param>
		///  <param name="doAliasing"> Return aliasing information </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public LineStepper(Point start, Point end, bool doAliasing = false)
		{
			_doAliasing = doAliasing;

			if (start == end)
			{
				_current = start;
				_lineStatus = LineStatus.Degenerate;
				return;
			}

			var dx = Abs(start.X - end.X);
			var dy = Abs(start.Y - end.Y);
			// ReSharper disable once AssignmentInConditionalExpression
			if (dy > dx)
			{
				if (start.Y < end.Y)
				{
					_current = start;
					_final = end;
				}
				else
				{
					_current = end;
					_final = start;
				}
				_bresenhamSpan = dy;
				_bresenhamAdd = dx;
				_longMove = LineStatus.Vertical;
				_dvLong = new Point(0, 1);
				if (_current.X > _final.X)
				{
					_dvShort = new Point(-1, 0);
					_sShortMove = LineStatus.SLeft;
					_aShortMove = LineStatus.ALeft;
				}
				else
				{
					_dvShort = new Point(1, 0);
					_sShortMove = LineStatus.SRight;
					_aShortMove = LineStatus.ARight;
				}
			}
			else
			{
				if (start.X < end.X)
				{
					_current = start;
					_final = end;
				}
				else
				{
					_current = end;
					_final = start;
				}
				_bresenhamSpan = dx;
				_bresenhamAdd = dy;
				_longMove = LineStatus.Horizontal;
				_dvLong = new Point(1, 0);
				_lineStatus = LineStatus.Horizontal;
				if (_current.Y < _final.Y)
				{
					_dvShort = new Point(0, 1);
					_sShortMove = LineStatus.SDown;
					_aShortMove = LineStatus.ADown;
				}
				else
				{
					_dvShort = new Point(0, -1);
					_sShortMove = LineStatus.SUp;
					_aShortMove = LineStatus.AUp;
				}
			}
			_bresenhamSum = (_bresenhamSpan + 1) / 2;
			_lineStatus = _bresenhamAdd + _bresenhamSum > _bresenhamSpan ? _sShortMove : _longMove;
		}
		#endregion

		#region Actual stepper
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Enumerates locations for this line and information about each point. </summary>
		///
		/// <remarks>	Darrell Plank, 11/17/2018. </remarks>
		///
		/// <returns>
		/// An enumerator that allows foreach to be used to process locations in this line.
		/// </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public IEnumerable<(Point pt, LineInfo info)> Locations()
		{
			if (_lineStatus == LineStatus.Degenerate)
			{
				yield return (_current, new LineInfo(LineStatus.Degenerate));
				yield break;
			}
			while (true)
			{
				yield return (_current, new LineInfo(_lineStatus, _doAliasing ? (double)_bresenhamSum / _bresenhamSpan : 0));
				if (_current == _final)
				{
					yield break;
				}
				_current += _dvLong;
				_bresenhamSum += _bresenhamAdd;
				if (_bresenhamSum > _bresenhamSpan)
				{
					_bresenhamSum -= _bresenhamSpan;
					_current += _dvShort;
				}

				if (_bresenhamSum > _bresenhamSpan - _bresenhamAdd)
				{
					_lineStatus = _sShortMove;
				}
				else if (_lineStatus == _sShortMove)
				{
					_lineStatus = _aShortMove;
				}
				else
				{
					_lineStatus = _longMove;
				}
			}
		}
		#endregion
	}
}
