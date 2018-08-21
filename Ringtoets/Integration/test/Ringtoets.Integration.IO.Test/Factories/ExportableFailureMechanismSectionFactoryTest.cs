using System;
using System.Collections.Generic;
using System.Linq;
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
        public void CreateExportableFailureMechanismSections_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSections(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSections", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSections_WithEmptyFailureMechanismSections_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections =
                ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSections(Enumerable.Empty<FailureMechanismSection>());

            // Assert
            CollectionAssert.IsEmpty(exportableFailureMechanismSections);
        }

        [Test]
        public void CreateExportableFailureMechanismSections_WithFailureMechanismSections_ReturnsExportableFailureMechanismSections()
        {
            // Setup
            FailureMechanismSection[] failureMechanismSections =
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(0, 10)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 10),
                    new Point2D(0, 20)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 20),
                    new Point2D(0, 40)
                })
            };

            // Call
            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections =
                ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSections(failureMechanismSections);

            // Assert
            Assert.AreEqual(3, exportableFailureMechanismSections.Count());

            ExportableFailureMechanismSection firstExportableSection = exportableFailureMechanismSections.First();
            Assert.AreSame(failureMechanismSections[0].Points, firstExportableSection.Geometry);
            Assert.AreEqual(0, firstExportableSection.StartDistance);
            Assert.AreEqual(10, firstExportableSection.EndDistance);

            ExportableFailureMechanismSection secondExportableSection = exportableFailureMechanismSections.ElementAt(1);
            Assert.AreSame(failureMechanismSections[1].Points, secondExportableSection.Geometry);
            Assert.AreEqual(10, secondExportableSection.StartDistance);
            Assert.AreEqual(20, secondExportableSection.EndDistance);

            ExportableFailureMechanismSection thirdExportableSection = exportableFailureMechanismSections.ElementAt(2);
            Assert.AreSame(failureMechanismSections[2].Points, thirdExportableSection.Geometry);
            Assert.AreEqual(20, thirdExportableSection.StartDistance);
            Assert.AreEqual(40, thirdExportableSection.EndDistance);
        }

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