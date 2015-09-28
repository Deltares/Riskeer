using System.Collections.Generic;
using System.IO;
using System.Linq;
using DelftTools.TestUtils;
using SharpMap.Extensions.CoordinateSystems;
using log4net;
using log4net.Core;
using NUnit.Framework;
using SharpMap.UI.Forms;

namespace SharpMap.Extensions.Tests.CoordinateSystems
{
    [TestFixture]
    public class OgrCoordinateSystemTest
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OgrCoordinateSystemTest));

        [Test]
        [Ignore("run to convert coordinate systems from GDAL files to ids")]
        public void ExtractCoordinateSystemIds()
        {
            var gcsIds = GetCoordinateSystemIds(Properties.Resources.gcs);
            var pcsIds = GetCoordinateSystemIds(Properties.Resources.pcs);
            var esriIds = GetCoordinateSystemIds(Properties.Resources.esri_extra);
            
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

        [Test]
        [Category(TestCategory.WindowsForms)]
        [Category(TestCategory.WorkInProgress)] // can't find env variable on build server, in progress ...
        [Category("CoordinateSystem")]
        public void AvailableCoordinateSystems()
        {
            LogHelper.ConfigureLogging(Level.Debug);

            var control = new SelectCoordinateSystemDialog(Map.CoordinateSystemFactory.SupportedCoordinateSystems, Map.CoordinateSystemFactory.CustomCoordinateSystems);
            WindowsFormsTestHelper.ShowModal(control);
        }
    }
}
