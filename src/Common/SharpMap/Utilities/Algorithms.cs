// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

namespace SharpMap.Utilities
{
	class Algorithms
	{

		// METHOD IsCCW() IS MODIFIED FROM ANOTHER WORK AND IS ORIGINALLY BASED ON GeoTools.NET:
		/*
		 *  Copyright (C) 2002 Urban Science Applications, Inc. 
		 *
		 *  This library is free software; you can redistribute it and/or
		 *  modify it under the terms of the GNU Lesser General Public
		 *  License as published by the Free Software Foundation; either
		 *  version 2.1 of the License, or (at your option) any later version.
		 *
		 *  This library is distributed in the hope that it will be useful,
		 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
		 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
		 *  Lesser General Public License for more details.
		 *
		 *  You should have received a copy of the GNU Lesser General Public
		 *  License along with this library; if not, write to the Free Software
		 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
		 *
		 */

		/// <summary>
		/// Tests whether a ring is oriented counter-clockwise.
		/// </summary>
		/// <param name="ring">Ring to test.</param>
		/// <returns>Returns true if ring is oriented counter-clockwise.</returns>
//		public static bool IsCCW(ILinearRing ring)
//		{
//			IPoint PrevPoint, NextPoint;
//			IPoint p;
//			
//			// Check if the ring has enough vertices to be a ring
//			if (ring..Count < 3) throw(new ArgumentException("Invalid LinearRing"));
//
//			// find the point with the largest Y coordinate
//			Point hip = ring.Vertices[0];
//			int hii = 0;
//			for (int i = 1; i < ring.Vertices.Count; i++)
//			{
//				p = ring.Vertices[i];
//				if (p.Y > hip.Y)
//				{
//					hip = p;
//					hii = i;
//				}
//			}
//			// Point left to Hip
//			int iPrev = hii - 1;
//			if (iPrev < 0) iPrev = ring.Vertices.Count - 2; 
//			// Point right to Hip
//			int iNext = hii + 1;
//			if (iNext >= ring.Vertices.Count) iNext = 1;
//			PrevPoint = ring.Vertices[iPrev];
//			NextPoint = ring.Vertices[iNext];
//
//			// translate so that hip is at the origin.
//			// This will not affect the area calculation, and will avoid
//			// finite-accuracy errors (i.e very small vectors with very large coordinates)
//			// This also simplifies the discriminant calculation.
//			double prev2x = PrevPoint.X - hip.X;
//			double prev2y = PrevPoint.Y - hip.Y;
//			double next2x = NextPoint.X - hip.X;
//			double next2y = NextPoint.Y - hip.Y;
//			// compute cross-product of vectors hip->next and hip->prev
//			// (e.g. area of parallelogram they enclose)
//			double disc = next2x * prev2y - next2y * prev2x;
//			// If disc is exactly 0, lines are collinear.  There are two possible cases:
//			//	(1) the lines lie along the x axis in opposite directions
//			//	(2) the line lie on top of one another
//			//  (2) should never happen, so we're going to ignore it!
//			//	(Might want to assert this)
//			//  (1) is handled by checking if next is left of prev ==> CCW
//
//			if (disc == 0.0)
//			{
//				// poly is CCW if prev x is right of next x
//				return (PrevPoint.X > NextPoint.X);
//			}
//			else
//			{
//				// if area is positive, points are ordered CCW
//				return (disc > 0.0);
//			}
//		} 
	}
}
