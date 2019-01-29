// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.IsInstanceOf<ICalculatableFailureMechanism>(failureMechanism);
            Assert.IsInstanceOf<IHasSectionResults<GrassCoverErosionInwardsFailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Grasbekleding erosie kruin en binnentalud", failureMechanism.Name);
            Assert.AreEqual("GEKB", failureMechanism.Code);
            Assert.AreEqual(1, failureMechanism.Group);

            Assert.IsInstanceOf<GeneralGrassCoverErosionInwardsInput>(failureMechanism.GeneralInput);

            Assert.AreEqual("Berekeningen", failureMechanism.CalculationsGroup.Name);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.DikeProfiles);
        }

        [Test]
        public void SetSections_WithSection_SetsSectionResults()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.AreSame(section, failureMechanism.SectionResults.First().Section);
        }

        [Test]
        public void ClearAllSections_WithSectionResults_SectionResultsCleared()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(2, 1)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(2, 1)
                })
            });

            // Precondition
            Assert.AreEqual(2, failureMechanism.SectionResults.Count());

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
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
                        new GrassCoverErosionInwardsCalculation(),
                        mocks.StrictMock<ICalculation>(),
                        new GrassCoverErosionInwardsCalculation()
                    }
                }
            };

            mocks.ReplayAll();

            // Call
            List<ICalculation> calculations = failureMechanism.Calculations.ToList();

            // Assert
            Assert.AreEqual(2, calculations.Count);
            Assert.IsTrue(calculations.All(c => c is GrassCoverErosionInwardsCalculation));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismWithDikeProfiles_WhenAddingAndRemovingElements_ThenCollectionIsUpdated()
        {
            // Scenario
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            const string filePath = "path";

            DikeProfile dikeProfile1 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0, 0), "id1");
            DikeProfile dikeProfile2 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1, 1), "id2");
            DikeProfile dikeProfile3 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(2, 2), "id3");

            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile1
            }, filePath);

            // Event
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile3,
                dikeProfile2
            }, filePath);
            failureMechanism.DikeProfiles.Remove(dikeProfile1);

            // Result
            DikeProfile[] expectedDikeProfiles =
            {
                dikeProfile3,
                dikeProfile2
            };
            CollectionAssert.AreEqual(expectedDikeProfiles, failureMechanism.DikeProfiles);
        }

        [Test]
        public void GivenObserverAttachedToDikeProfiles_WhenNotifyObservers_ThenObserverIsNotified()
        {
            // Scenario
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.Attach(observer);

            // Event
            failureMechanism.DikeProfiles.NotifyObservers();

            // Result
            mocks.VerifyAll(); // Expect observer to be notified.
        }
    }
}