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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Utils.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsHelperTest
    {
        [Test]
        public void CollectCalculationsPerSegment_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculations = new GrassCoverErosionInwardsCalculation[0];

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(null, calculations);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);
        }

        [Test]
        public void CollectCalculationsPerSegment_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(oneSectionResult, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculations", paramName);
        }

        [Test]
        public void CollectCalculationsPerSegment_ValidEmptySectionResults_EmptyDictionary()
        {
            // Setup
            var emptySectionResults = new GrassCoverErosionInwardsFailureMechanismSectionResult[0];

            // Call
            Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> collectCalculationsPerSegment = 
                GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(emptySectionResults, twoCalculations);

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_ValidEmptyCalculations_EmptyDictionary()
        {
            // Setup
            var calculations = new GrassCoverErosionInwardsCalculation[0];

            // Call
            Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> collectCalculationsPerSegment = 
                GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(oneSectionResult, calculations);

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_CalculationsWithoutDikeProfiles_EmptyDictionary()
        {
            // Setup
            GrassCoverErosionInwardsCalculation[] calculations = {
                new GrassCoverErosionInwardsCalculation(),
                new GrassCoverErosionInwardsCalculation()
            };

            // Call
            Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> collectCalculationsPerSegment = 
                GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(oneSectionResult, calculations);

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_MultipleCalculationsInSegment_OneSegmentHasAllCalculations()
        {
            // Call
            Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> collectCalculationsPerSegment = 
                GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(twoSectionResults, twoCalculations);

            // Assert
            Assert.AreEqual(1, collectCalculationsPerSegment.Count);
            Assert.IsTrue(collectCalculationsPerSegment.ContainsKey(firstSectionName));
            CollectionAssert.AreEqual(twoCalculations, collectCalculationsPerSegment[firstSectionName]);
        }

        [Test]
        public void CollectCalculationsPerSegment_SingleCalculationPerSegment_OneCalculationPerSegment()
        {
            // Setup
            GrassCoverErosionInwardsCalculation[] calculations = {
                new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = new DikeProfile(new Point2D(1.1, 2.2), new RoughnessPoint[0], new Point2D[0])
                    }
                },
                new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = new DikeProfile(new Point2D(50.0, 66.0), new RoughnessPoint[0], new Point2D[0])
                    }
                }
            };

            // Call
            Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> collectCalculationsPerSegment = 
                GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(twoSectionResults, calculations);

            // Assert
            Assert.AreEqual(2, collectCalculationsPerSegment.Count);
            Assert.AreEqual(1, collectCalculationsPerSegment[firstSectionName].Count);
            Assert.AreSame(calculations[0], collectCalculationsPerSegment[firstSectionName][0]);
            Assert.AreEqual(1, collectCalculationsPerSegment[secondSectionName].Count);
            Assert.AreSame(calculations[1], collectCalculationsPerSegment[secondSectionName][0]);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SectionResultAndCalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(null, null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(null, calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(oneSectionResult, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_CalculationWithoutDikeProfile_ReturnsNull()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            FailureMechanismSection failureMechanismSection = 
                GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(oneSectionResult, calculation);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SectionResultsEmpty_ReturnsNull()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            FailureMechanismSection failureMechanismSection = 
                GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(oneSectionResult, calculation);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_ValidEmptySectionResults_ReturnsNull()
        {
            // Setup
            var emptySectionResults = new GrassCoverErosionInwardsFailureMechanismSectionResult[0];
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            FailureMechanismSection failureMechanismSection = 
                GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(emptySectionResults, calculation);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_FirstSectionResultContainsCalculation_FailureMechanismSectionOfFirstSectionResult()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = new DikeProfile(new Point2D(1.1, 2.2), new RoughnessPoint[0], new Point2D[0])
                }
            };

            // Call
            FailureMechanismSection failureMechanismSection = 
                GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(twoSectionResults, calculation);

            // Assert
            Assert.AreSame(twoSectionResults[0].Section, failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SecondSectionResultContainsCalculation_FailureMechanismSectionOfSecondSectionResult()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = new DikeProfile(new Point2D(50.0, 66.0), new RoughnessPoint[0], new Point2D[0])
                }
            };

            // Call
            FailureMechanismSection failureMechanismSection = 
                GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(twoSectionResults, calculation);

            // Assert
            Assert.AreSame(twoSectionResults[1].Section, failureMechanismSection);
        }

        private const string firstSectionName = "firstSection";
        private const string secondSectionName = "secondSection";

        private readonly GrassCoverErosionInwardsFailureMechanismSectionResult[] oneSectionResult = {
            new GrassCoverErosionInwardsFailureMechanismSectionResult(
                new FailureMechanismSection("testFailureMechanismSection", new List<Point2D>
                {
                    new Point2D(0.0, 0.0)
                }))
        };

        private readonly GrassCoverErosionInwardsFailureMechanismSectionResult[] twoSectionResults = {
            new GrassCoverErosionInwardsFailureMechanismSectionResult(
                new FailureMechanismSection(firstSectionName, new List<Point2D>
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(10.0, 10.0),
                })),
            new GrassCoverErosionInwardsFailureMechanismSectionResult(
                new FailureMechanismSection(secondSectionName, new List<Point2D>
                {
                    new Point2D(11.0, 11.0),
                    new Point2D(100.0, 100.0),
                }))
        };

        private readonly GrassCoverErosionInwardsCalculation[] twoCalculations = {
            new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = new DikeProfile(new Point2D(1.1, 2.2), new RoughnessPoint[0], new Point2D[0])
                }
            },
            new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = new DikeProfile(new Point2D(3.3, 4.4), new RoughnessPoint[0], new Point2D[0])
                }
            }
        };
    }
}