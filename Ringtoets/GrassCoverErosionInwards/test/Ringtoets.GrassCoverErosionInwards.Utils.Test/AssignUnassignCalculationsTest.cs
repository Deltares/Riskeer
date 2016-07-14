using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Utils.Test
{
    [TestFixture]
    public class AssignUnassignCalculationsTest
    {
        [Test]
        public void Update_NullFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(null, calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Update_NullCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void Update_CalculationDikeProfileChangedToMatchOtherSection_FirstSectionResultCalculationNullSecondSectionResultCalculationSet()
        {
            // Setup
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

            // Call
            calculation1.InputParameters.DikeProfile = dikeProfile2;
            AssignUnassignCalculations.Update(failureMechanism, calculation1);

            // Assert
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation1, sectionResults[1].Calculation);
        }

        [Test]
        public void Update_CalculationDikeProfileChangedToMatchOtherSection_FirstSectionResultCalculationNullSecondSectionResultCalculationUnchanged()
        {
            // Setup
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

            // Call
            calculation1.InputParameters.DikeProfile = dikeProfile2;
            AssignUnassignCalculations.Update(failureMechanism, calculation1);

            // Assert
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation2, sectionResults[1].Calculation);
        }
        
        [Test]
        public void Delete_NullFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(null, calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Delete_NullCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void Delete_RemoveCalculationAssignedToSectionResult_SectionResultCalculationNull()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0.51, 0.51), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            failureMechanism.DikeProfiles.Add(dikeProfile);

            failureMechanism.CalculationsGroup.Children.Add(calculation);

            failureMechanism.DikeProfiles.Add(dikeProfile);

            failureMechanism.AddSection(new FailureMechanismSection("section", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            }));

            // Call
            AssignUnassignCalculations.Delete(failureMechanism, calculation);

            // Assert
            Assert.IsNull(failureMechanism.SectionResults.First().Calculation);
        }
    }
}