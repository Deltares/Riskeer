using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Ringtoets.Piping.Data.TestUtil
{
    /// <summary>
    /// The class helps to construct collections of points by reading a string (from file or directly) and
    /// transforming this in objects that can be used in tests.
    /// </summary>
    public class PointCollectionHelper
    {
        public static HashSet<Point3D> CreateFromString(string s)
        {
            var points = new SortedDictionary<int, Point3D>();
            var lines = s.Split(new [] { Environment.NewLine }, StringSplitOptions.None);
            var height = int.Parse(lines[0]);
            var lineIndex = 1;
            for (int z = height - 1; z >= 0; z--, lineIndex++)
            {
                foreach (var tuple in AllIndexesOfDigit(lines[lineIndex]))
                {
                    points.Add(tuple.Item1,new Point3D
                    {
                        X = tuple.Item2, Z = z
                    });
                }
            }
            return new HashSet<Point3D>(points.Values);
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
        private static IEnumerable<Tuple<int,int>> AllIndexesOfDigit(string line)
        {
            var guess = @"\d";
            var matches = Regex.Matches(line, guess);
            foreach (Match match in matches)
            {
                int digit;
                try
                {
                    digit = int.Parse(match.Value);
                }
                catch (ArgumentNullException e)
                {
                    throw new Exception(e.Message,e);
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
