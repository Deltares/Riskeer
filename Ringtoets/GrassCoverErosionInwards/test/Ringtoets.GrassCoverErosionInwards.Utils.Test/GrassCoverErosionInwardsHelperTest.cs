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
        public void CollectCalculationsPerSegment_SectionResultsAndCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(null, null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void CollectCalculationsPerSegment_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculations = new GrassCoverErosionInwardsCalculation[0];

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(null, calculations);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);
        }

        [Test]
        public void CollectCalculationsPerSegment_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(oneSectionResult, null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculations", paramName);
        }

        [Test]
        public void CollectCalculationsPerSegment_ValidEmptyData_EmptyDictionary()
        {
            // Setup
            var calculations = new GrassCoverErosionInwardsCalculation[0];

            // Call
            var collectCalculationsPerSegment = GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(oneSectionResult, calculations);

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_MultipleCalculationsInSegment_OneSegmentHasAllCalculations()
        {
            // Setup
            var calculations = new[]
            {
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

            // Call
            var collectCalculationsPerSegment = GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(twoSectionResults, calculations);

            // Assert
            Assert.AreEqual(1, collectCalculationsPerSegment.Count);
            Assert.IsTrue(collectCalculationsPerSegment.ContainsKey("firstSection"));
            Assert.AreEqual(2, collectCalculationsPerSegment["firstSection"].Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_SingleCalculationPerSegment_OneCalculationPerSegment()
        {
            // Setup
            var calculations = new[]
            {
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
            var collectCalculationsPerSegment = GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(twoSectionResults, calculations);

            // Assert
            Assert.AreEqual(2, collectCalculationsPerSegment.Count);
            Assert.AreEqual(1, collectCalculationsPerSegment["firstSection"].Count);
            Assert.AreEqual(1, collectCalculationsPerSegment["secondSection"].Count);
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
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(oneSectionResult, null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SectionResultsEmpty_ReturnsNull()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            var failureMechanismSection = GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(oneSectionResult, calculation);

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
            var failureMechanismSection = GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(twoSectionResults, calculation);

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
            var failureMechanismSection = GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(twoSectionResults, calculation);

            // Assert
            Assert.AreSame(twoSectionResults[1].Section, failureMechanismSection);
        }

        private readonly GrassCoverErosionInwardsFailureMechanismSectionResult[] oneSectionResult = new[]
        {
            new GrassCoverErosionInwardsFailureMechanismSectionResult(
                new FailureMechanismSection("testFailureMechanismSection", new List<Point2D>
                {
                    new Point2D(0.0, 0.0)
                }))
        };

        private readonly GrassCoverErosionInwardsFailureMechanismSectionResult[] twoSectionResults = new[]
        {
            new GrassCoverErosionInwardsFailureMechanismSectionResult(
                new FailureMechanismSection("firstSection", new List<Point2D>
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(10.0, 10.0),
                })),
            new GrassCoverErosionInwardsFailureMechanismSectionResult(
                new FailureMechanismSection("secondSection", new List<Point2D>
                {
                    new Point2D(11.0, 11.0),
                    new Point2D(100.0, 100.0),
                }))
        };
    }
}