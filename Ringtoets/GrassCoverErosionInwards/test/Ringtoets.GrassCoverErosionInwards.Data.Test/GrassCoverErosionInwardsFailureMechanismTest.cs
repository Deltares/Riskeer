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

using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.IsInstanceOf<ICalculatableFailureMechanism>(failureMechanism);
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayName, failureMechanism.Name);
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayCode, failureMechanism.Code);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            Assert.IsInstanceOf<GeneralGrassCoverErosionInwardsInput>(failureMechanism.GeneralInput);
            Assert.IsInstanceOf<GeneralNormProbabilityInput>(failureMechanism.NormProbabilityInput);
            Assert.AreEqual("Berekeningen", failureMechanism.CalculationsGroup.Name);
            Assert.IsEmpty(failureMechanism.CalculationsGroup.Children);
        }

        [Test]
        public void AddSection_WithSection_AddedGrassCoverErosionInwardsFailureMechanismSectionResult()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));

            // Assert
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.IsInstanceOf<GrassCoverErosionInwardsFailureMechanismSectionResult>(failureMechanism.SectionResults.ElementAt(0));
        }

        [Test]
        public void ClearAllSections_WithSectionsAndSectionResults_SectionsAndSectionResultsCleared()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));

            // Precondition
            Assert.AreEqual(2, failureMechanism.Sections.Count());
            Assert.AreEqual(2, failureMechanism.SectionResults.Count());

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            Assert.AreEqual(0, failureMechanism.Sections.Count());
            Assert.AreEqual(0, failureMechanism.SectionResults.Count());
        }

        [Test]
        public void Calculations_MultipleChildrenAdded_ReturnGrassCoverErosionInwardsCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new CalculationGroup(),
                        new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput()),
                        mocks.StrictMock<ICalculation>(),
                        new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput())
                    }
                }
            };

            mocks.ReplayAll();

            // Call
            var calculations = failureMechanism.Calculations.ToList();

            // Assert
            Assert.AreEqual(2, calculations.Count);
            Assert.IsTrue(calculations.All(c => c is GrassCoverErosionInwardsCalculation));
            mocks.VerifyAll();
        }
    }
}