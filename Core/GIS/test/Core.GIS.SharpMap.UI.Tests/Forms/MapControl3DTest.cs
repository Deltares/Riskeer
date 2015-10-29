using System.IO;
using System.Windows.Forms;
using DelftTools.TestUtils;
using log4net.Config;
using NUnit.Framework;
using SharpMap.UI.Forms;

namespace SharpMap.UI.Tests.Forms
{
    [TestFixture]
    public class MapControl3DTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            LogHelper.ConfigureLogging();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogHelper.ResetLogging();
        }

        [Test]
        [Category("Windows.Forms")]
        [Ignore("memory access problems on build server")]
        public void Create()
        {
            var form = new Form {Width = 800, Height = 600};

            // go to the directory containing resources
            const string sharpMapTestPath = @"..\..\..\..\..\test-data\DeltaShell\DeltaShell.Plugins.SharpMapGis.Tests\";
            Directory.SetCurrentDirectory(sharpMapTestPath + "3d");

            var control = new MapControl3D {Dock = DockStyle.Fill};

            form.Controls.Add(control);

            control.Refresh();
            
            WindowsFormsTestHelper.ShowModal(form);
        }
    }
}