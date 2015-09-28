using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DelftTools.TestUtils;
using GeoAPI.CoordinateSystems;
using NUnit.Framework;
using SharpMap.Extensions.CoordinateSystems;
using SharpMap.UI.Forms;

namespace SharpMap.UI.Tests.Forms
{
    [TestFixture]
    public class CoordinateSystemTypeEditorTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Environment.SetEnvironmentVariable("GDAL_DATA", @"..\..\..\..\..\lib\Common\SharpMap.Extensions\gdal_data");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Environment.SetEnvironmentVariable("GDAL_DATA", string.Empty);
        }

        [Test]
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void PropertyGridWithCoordinateSystemTypeEditor()
        {
            Map.CoordinateSystemFactory = new OgrCoordinateSystemFactory();

            var myClass = new MyClass()/*{ CoordinateSystem = new OgrCoordinateSystem("")}*/;
            var form = new Form();
            var grid = new PropertyGrid {Dock = DockStyle.Fill};

            form.Controls.Add(grid);

            grid.SelectedObject = myClass;
            WindowsFormsTestHelper.ShowModal(form);
        }

        private class MyClass
        {
            [TypeConverter(typeof(CoordinateSystemStringTypeConverter))]
            [Editor(typeof(CoordinateSystemTypeEditor), typeof(UITypeEditor))]
            public ICoordinateSystem CoordinateSystem { get; set; } 
        }
    }
}