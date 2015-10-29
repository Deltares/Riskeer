using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Data.TestUtil
{
    public class PointCollectionHelper
    {
        public static HashSet<Point3D> CreateFromFile(string url)
        {
            return CreateFromString(File.ReadAllText(url));
        } 

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

        private static IEnumerable<Tuple<int,int>> AllIndexesOfDigit(string word)
        {
            var guess = @"\d";
            var matches = Regex.Matches(word, guess);
            foreach (Match match in matches)
            {
                var digit = int.Parse(match.Value);
                yield return Tuple.Create<int, int>(digit, match.Index);
            }
        }
    }
}
