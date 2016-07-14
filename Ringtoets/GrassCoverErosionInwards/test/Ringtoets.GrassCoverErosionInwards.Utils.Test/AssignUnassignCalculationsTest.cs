// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
        public void Update_NullSectionResults_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(null, calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);
        }

        [Test]
        public void Update_NullCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(failureMechanism.SectionResults, null);

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

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile1
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            failureMechanism.DikeProfiles.Add(dikeProfile1);
            failureMechanism.DikeProfiles.Add(dikeProfile2);

            failureMechanism.CalculationsGroup.Children.Add(calculation);

            failureMechanism.AddSection(new FailureMechanismSection("firstSection", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("secondSection", new List<Point2D>
            {
                new Point2D(1.1, 1.1), new Point2D(2.2, 2.2)
            }));

            var sectionResults = failureMechanism.SectionResults.ToArray();
            sectionResults[0].Calculation = calculation;

            // Call
            calculation.InputParameters.DikeProfile = dikeProfile2;
            AssignUnassignCalculations.Update(failureMechanism.SectionResults, calculation);

            // Assert
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation, sectionResults[1].Calculation);
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
            AssignUnassignCalculations.Update(failureMechanism.SectionResults, calculation1);

            // Assert
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation2, sectionResults[1].Calculation);
        }
        
        [Test]
        public void Delete_NullSectionResults_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(null, calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);
        }

        [Test]
        public void Delete_NullCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(failureMechanism.SectionResults, null);

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

            failureMechanism.AddSection(new FailureMechanismSection("section", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            }));

            // Call
            AssignUnassignCalculations.Delete(failureMechanism.SectionResults, calculation, Enumerable.Empty<GrassCoverErosionInwardsCalculation>());

            // Assert
            Assert.IsNull(failureMechanism.SectionResults.First().Calculation);
        }

        [Test]
        public void Delete_RemoveCalculationAssignedToSectionResult_SingleRemainingCalculationAssignedToSectionResult()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0.51, 0.51), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());

            var calculation1 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };
            var calculation2 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            failureMechanism.DikeProfiles.Add(dikeProfile);

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);

            failureMechanism.AddSection(new FailureMechanismSection("section", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            }));

            failureMechanism.SectionResults.First().Calculation = calculation1;

            var remainingCalculations = new[]
            {
                calculation2
            };

            // Call
            AssignUnassignCalculations.Delete(failureMechanism.SectionResults, calculation1, remainingCalculations);

            // Assert
            Assert.AreSame(calculation2, failureMechanism.SectionResults.First().Calculation);
        }
    }
}