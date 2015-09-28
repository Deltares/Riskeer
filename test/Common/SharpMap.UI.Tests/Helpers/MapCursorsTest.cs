using System.Windows.Forms;
using DelftTools.TestUtils;
using NUnit.Framework;
using SharpMap.UI.Helpers;

namespace SharpMap.UI.Tests.Helpers
{
    [TestFixture]
    public class MapCursorsTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowAddPointCursor()
        {
            WindowsFormsTestHelper.ShowModal(new Form {Cursor = MapCursors.AddPoint});
        }

        [Test]
        public void CheckNoCopiesAreReturned()
        {
            var c1 = MapCursors.AddPoint;
            var c2 = MapCursors.AddPoint;
            Assert.AreSame(c1, c2);
        }
    }
}