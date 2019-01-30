// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Base.Geometry;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// The class helps to construct collections of points by reading a string (from file or directly) and
    /// transforming this in objects that can be used in tests.
    /// </summary>
    public static class Segment2DLoopCollectionHelper
    {
        /// <summary>
        /// Creates a loop from a given string. The string needs to be formatted as follows.
        /// <list type="bullet">
        ///   <item>First line contains the number of lines (<c>n</c>).</item>
        ///   <item>The <c>n</c> following lines contain a number of digits.</item>
        ///   <item>Each digit is a point and should be unique.</item>
        /// </list> 
        /// Example string:
        /// <c>
        /// "3" + Environment.NewLine +
        /// "..1.." + Environment.NewLine +
        /// "3...." + Environment.NewLine +
        /// "...2."
        /// </c>
        /// </summary>
        /// <param name="s">The (correctly formatted) string representing a grid with points.</param>
        /// <returns>The <see cref="List{T}"/> of <see cref="Segment2D"/> which describe a loop.</returns>
        public static List<Segment2D> CreateFromString(string s)
        {
            var points = new SortedDictionary<int, Point2D>();
            string[] lines = s.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None);
            int height = int.Parse(lines[0]);
            var lineIndex = 1;
            for (int y = height - 1; y >= 0; y--, lineIndex++)
            {
                foreach (Tuple<int, int> tuple in AllIndexesOfDigit(lines[lineIndex]))
                {
                    points.Add(tuple.Item1, new Point2D(tuple.Item2, y));
                }
            }

            return CreateLoop(points);
        }

        private static List<Segment2D> CreateLoop(SortedDictionary<int, Point2D> points)
        {
            var loop = new List<Segment2D>(points.Count);
            int count = points.Values.Count;
            for (var i = 0; i < count; i++)
            {
                Point2D firstPoint = points.Values.ElementAt(i);
                Point2D secondPoint = points.Values.ElementAt((i + 1) % count);
                loop.Add(new Segment2D(firstPoint, secondPoint));
            }

            return loop;
        }

        /// <summary>
        /// Returns a <see cref="IEnumerable{T}"/> of <see cref="Tuple{T,T}"/> if <see cref="int"/>.
        /// The first item in the tuple contains the digit and the second item contains its index.
        /// </summary>
        /// <param name="line">The line which contains digits.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Tuple{T,T}"/> of <see cref="int"/>
        /// which contains the digits and the index of those digits.</returns>
        /// <exception cref="Exception">Thrown when the regex magically matches more or less than it should
        /// have (1 digit).</exception>
        private static IEnumerable<Tuple<int, int>> AllIndexesOfDigit(string line)
        {
            const string guess = @"\d";
            MatchCollection matches = Regex.Matches(line, guess);
            foreach (Match match in matches)
            {
                int digit;
                try
                {
                    digit = int.Parse(match.Value);
                }
                catch (ArgumentNullException e)
                {
                    throw new Exception(e.Message, e);
                }
                catch (FormatException e)
                {
                    throw new Exception(e.Message, e);
                }
                catch (OverflowException e)
                {
                    throw new Exception(e.Message, e);
                }

                yield return Tuple.Create(digit, match.Index);
            }
        }
    }
}