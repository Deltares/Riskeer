using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableFailureMechanismSectionFactoryTest
    {
        [Test]
        public void CreateExportableFailureMechanismSection_FailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSection_WithFailureMechanismSection_ReturnExportableFailureMechanismSection()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(1, 1)
            });

            // Call
            ExportableFailureMechanismSection exportableFailureMechanismSection =
                ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(failureMechanismSection);

            // Assert
            Assert.AreSame(failureMechanismSection.Points, exportableFailureMechanismSection.Geometry);
            Assert.IsNaN(exportableFailureMechanismSection.StartDistance);
            Assert.IsNaN(exportableFailureMechanismSection.EndDistance);
        }
    }
}