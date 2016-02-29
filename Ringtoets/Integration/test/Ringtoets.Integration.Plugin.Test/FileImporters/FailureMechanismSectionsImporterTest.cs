using System.Collections.Generic;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.FileImporters;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class FailureMechanismSectionsImporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var importer = new FailureMechanismSectionsImporter();

            // Assert
            Assert.IsInstanceOf<FileImporterBase>(importer);
            Assert.AreEqual("Vakindeling", importer.Name);
            Assert.AreEqual("Algemeen", importer.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.Sections, importer.Image);
            Assert.AreEqual(typeof(FailureMechanismSectionsContext), importer.SupportedItemType);
            Assert.AreEqual("Vakindeling shapefile (*.shp)|*.shp", importer.FileFilter);
        }

        [Test]
        public void Import_ValidFileCorrespondingToReferenceLine_ImportSections()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1_vakken.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new SimpleFailureMechanism();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            var importSuccesful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            Assert.IsTrue(importSuccesful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(62, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, assessmentSection.ReferenceLine);
            mocks.VerifyAll();
        }

        private void AssertSectionsAreValidForReferenceLine(FailureMechanismSection[] sections, ReferenceLine referenceLine)
        {
            Point2D[] referenceLineGeometry = referenceLine.Points.ToArray();

            // 1. Start & End coherence:
            Assert.AreEqual(referenceLineGeometry[0], sections[0].GetStart(),
                "Start of the sections should correspond to the Start of the reference line.");
            Assert.AreEqual(referenceLineGeometry[referenceLineGeometry.Length - 1], sections[sections.Length-1].GetLast(),
                "End of the sections should correspond to the End of the reference line.");

            // 2. Total length coherence:
            var totalLengthOfSections = sections.Sum(s => GetLengthOfLine(s.Points));
            var totalLengthOfReferenceLine = GetLengthOfLine(referenceLineGeometry);
            Assert.AreEqual(totalLengthOfReferenceLine, totalLengthOfSections, 1e-6,
                "The length of all sections should sum up to the length of the reference line.");

            // 3. Section Start and End coherence
            IEnumerable<Point2D> allStartAndEndPoints = sections.Select(s => s.GetStart()).Concat(sections.Select(s => s.GetLast()));
            foreach (Point2D point in allStartAndEndPoints)
            {
                Assert.Less(GetDistanceToReferenceLine(point, referenceLine), 1e-6,
                    "All start- and end points should be on the reference line.");
            }

            // 4. Section Start and End points coherence
            FailureMechanismSection sectionTowardsEnd = null;
            foreach (FailureMechanismSection section in sections)
            {
                FailureMechanismSection sectionTowardsStart = sectionTowardsEnd;
                sectionTowardsEnd = section;

                if (sectionTowardsStart != null)
                {
                    Assert.AreEqual(sectionTowardsStart.GetLast(), sectionTowardsEnd.GetStart(),
                        "All sections should be connected and in order of connectedness.");
                }
            }
        }

        private double GetDistanceToReferenceLine(Point2D point, ReferenceLine referenceLine)
        {
            return GetLineSegments(referenceLine.Points)
                .Select(segment => segment.GetEuclideanDistanceToPoint(point))
                .Min();
        }

        private double GetLengthOfLine(IEnumerable<Point2D> linePoints)
        {
            return GetLineSegments(linePoints).Sum(segment => segment.Length);
        }

        private IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            Point2D firstPoint = null;
            foreach (Point2D linePoint in linePoints)
            {
                Point2D secondPoint = firstPoint;
                firstPoint = linePoint;

                if (secondPoint != null)
                {
                    yield return new Segment2D(firstPoint, secondPoint);
                }
            }
        }

        private class SimpleFailureMechanism : BaseFailureMechanism
        {
            public SimpleFailureMechanism() : base("Stubbed name") {}

            public override IEnumerable<ICalculationItem> CalculationItems
            {
                get
                {
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}