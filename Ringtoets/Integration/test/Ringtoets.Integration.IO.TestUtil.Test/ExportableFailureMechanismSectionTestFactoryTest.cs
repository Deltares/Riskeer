using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil.Test
{
    [TestFixture]
    public class ExportableFailureMechanismSectionTestFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSection_Always_ReturnsFailureMechanismSection()
        {
            // Call
            ExportableFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, section.Geometry);
            Assert.AreEqual(1, section.StartDistance);
            Assert.AreEqual(2, section.EndDistance);
        }
    }
}