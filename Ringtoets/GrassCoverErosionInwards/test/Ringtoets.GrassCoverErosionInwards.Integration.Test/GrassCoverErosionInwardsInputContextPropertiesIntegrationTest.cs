using System.Collections.Generic;
using System.Linq;

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextPropertiesIntegrationTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void DikeProfile_CalculationDikeProfileChangedToMatchOtherSection_FirstSectionResultCalculationNullSecondSectionResultCalculationSet()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var dikeProfile1 = new DikeProfile(new Point2D(0.51, 0.51), new RoughnessPoint[0], new Point2D[0],
                                               null, new DikeProfile.ConstructionProperties());
            var dikeProfile2 = new DikeProfile(new Point2D(1.51, 1.51), new RoughnessPoint[0], new Point2D[0],
                                               null, new DikeProfile.ConstructionProperties());

            var calculation1 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile1
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            failureMechanism.DikeProfiles.Add(dikeProfile1);
            failureMechanism.DikeProfiles.Add(dikeProfile2);

            failureMechanism.CalculationsGroup.Children.Add(calculation1);

            failureMechanism.DikeProfiles.Add(dikeProfile1);
            failureMechanism.DikeProfiles.Add(dikeProfile2);

            failureMechanism.AddSection(new FailureMechanismSection("firstSection", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("secondSection", new List<Point2D>
            {
                new Point2D(1.1, 1.1), new Point2D(2.2, 2.2)
            }));

            var sectionResults = failureMechanism.SectionResults.ToArray();
            sectionResults[0].Calculation = calculation1;

            var inputContext = new GrassCoverErosionInwardsInputContext(calculation1.InputParameters, calculation1, failureMechanism, assessmentSectionMock);

            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = inputContext
            };

            // Call
            properties.DikeProfile = dikeProfile2;

            // Assert
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation1, sectionResults[1].Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DikeProfile_CalculationDikeProfileChangedToMatchOtherSection_FirstSectionResultCalculationNullSecondSectionResultCalculationUnchanged()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var dikeProfile1 = new DikeProfile(new Point2D(0.51, 0.51), new RoughnessPoint[0], new Point2D[0],
                                               null, new DikeProfile.ConstructionProperties());
            var dikeProfile2 = new DikeProfile(new Point2D(1.51, 1.51), new RoughnessPoint[0], new Point2D[0],
                                               null, new DikeProfile.ConstructionProperties());

            var calculation1 = new GrassCoverErosionInwardsCalculation
            {
                Name = "firstCalculation",
                InputParameters =
                {
                    DikeProfile = dikeProfile1
                }
            };
            var calculation2 = new GrassCoverErosionInwardsCalculation
            {
                Name = "secondCalculation",
                InputParameters =
                {
                    DikeProfile = dikeProfile2
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            failureMechanism.DikeProfiles.Add(dikeProfile1);
            failureMechanism.DikeProfiles.Add(dikeProfile2);

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);

            failureMechanism.DikeProfiles.Add(dikeProfile1);
            failureMechanism.DikeProfiles.Add(dikeProfile2);

            failureMechanism.AddSection(new FailureMechanismSection("firstSection", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("secondSection", new List<Point2D>
            {
                new Point2D(1.1, 1.1), new Point2D(2.2, 2.2)
            }));

            var sectionResults = failureMechanism.SectionResults.ToArray();
            sectionResults[0].Calculation = calculation1;
            sectionResults[1].Calculation = calculation2;

            var inputContext = new GrassCoverErosionInwardsInputContext(calculation1.InputParameters, calculation1, failureMechanism, assessmentSectionMock);

            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = inputContext
            };

            // Call
            properties.DikeProfile = dikeProfile2;

            // Assert
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation2, sectionResults[1].Calculation);
            mockRepository.VerifyAll();
        }
    }
}