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

using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Utils;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsViewHelperTest
    {
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

        [Test]
        public void CollectCalculationsPerSegment_NullParameters_EmptyDictionary()
        {
            // Call
            var collectCalculationsPerSegment = GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(null, null);

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_FirstParameterNull_EmptyDictionary()
        {
            // Setup
            var calculations = new GrassCoverErosionInwardsCalculation[0];

            // Call
            var collectCalculationsPerSegment = GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(null, calculations);

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_SecondParameterNull_EmptyDictionary()
        {
            // Call
            var collectCalculationsPerSegment = GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(oneSectionResult, null);

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_ValidEmptyParameters_EmptyDictionary()
        {
            // Setup
            var calculations = new GrassCoverErosionInwardsCalculation[0];

            // Call
            var collectCalculationsPerSegment = GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(oneSectionResult, calculations);

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_ValidParameters_OneSegmentHasAllCalculations()
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
        public void CollectCalculationsPerSegment_ValidParameters_OneCalculationPerSegment()
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
        public void FailureMechanismSectionForCalculation_NullParameters_Null()
        {
            // Call
            var failureMechanismSection = GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(null, null);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_FirstParameterNull_Null()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            var failureMechanismSection = GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(null, calculation);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SecondParameterNull_Null()
        {
            // Call
            var failureMechanismSection = GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(oneSectionResult, null);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_ValidEmptyFirstParameter_Null()
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
    }
}