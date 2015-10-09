using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SharpMap.Extensions.Tests.Properties;

namespace SharpMap.Extensions.Tests.CoordinateSystems
{
    [TestFixture]
    public class OgrCoordinateSystemTest
    {
        [Test]
        [Ignore("run to convert coordinate systems from GDAL files to ids")]
        public void ExtractCoordinateSystemIds()
        {
            var gcsIds = GetCoordinateSystemIds(Resources.gcs);
            var pcsIds = GetCoordinateSystemIds(Resources.pcs);
            var esriIds = GetCoordinateSystemIds(Resources.esri_extra);

            File.WriteAllText("cs.ids.txt",
                              "# geographical\n" + gcsIds.Select(id => id.ToString()).Aggregate((i1, i2) => i1 + "\n" + i2) + "\n" +
                              "# projected\n" + pcsIds.Select(id => id.ToString()).Aggregate((i1, i2) => i1 + "\n" + i2) + "\n" +
                              "# esri\n" + esriIds.Select(id => id.ToString()).Aggregate((i1, i2) => i1 + "\n" + i2));
        }

        private static IEnumerable<int> GetCoordinateSystemIds(string resource)
        {
            return resource.Split('\n').Where(s => !string.IsNullOrEmpty(s) && !s.StartsWith("\"") && !s.StartsWith("#"))
                           .Select(id => int.Parse(id.Split(',')[0]))
                           .ToArray();
        }
    }
}