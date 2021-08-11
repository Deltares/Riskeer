// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data.TestUtil;

namespace Riskeer.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneErosionFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new DuneErosionFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.AreEqual("Duinwaterkering - Duinafslag", failureMechanism.Name);
            Assert.AreEqual("DA", failureMechanism.Code);
            Assert.AreEqual(3, failureMechanism.Group);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
            CollectionAssert.IsEmpty(failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities);
            Assert.IsNotNull(failureMechanism.GeneralInput);

            CollectionAssert.IsEmpty(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsForMechanismSpecificSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsForFactorizedLowerLimitNorm);
        }

        [Test]
        public void SetSections_WithSection_SetsSectionResults()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
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
            var failureMechanism = new DuneErosionFailureMechanism();

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
        public void SetDuneLocations_DuneLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            void Call() => failureMechanism.SetDuneLocations(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("duneLocations", paramName);
        }

        [Test]
        public void SetDuneLocations_Always_PreviousLocationsAndCalculationsCleared()
        {
            // Setup
            var calculationsForTargetProbability1 = new DuneLocationCalculationsForTargetProbability();
            var calculationsForTargetProbability2 = new DuneLocationCalculationsForTargetProbability();
            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    calculationsForTargetProbability1,
                    calculationsForTargetProbability2
                }
            };

            failureMechanism.SetDuneLocations(new DuneLocation[]
            {
                new TestDuneLocation()
            });

            // Precondition
            CollectionAssert.IsNotEmpty(failureMechanism.DuneLocations);
            CollectionAssert.IsNotEmpty(calculationsForTargetProbability1.DuneLocationCalculations);
            CollectionAssert.IsNotEmpty(calculationsForTargetProbability2.DuneLocationCalculations);

            // Call
            failureMechanism.SetDuneLocations(Enumerable.Empty<DuneLocation>());

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
            CollectionAssert.IsEmpty(calculationsForTargetProbability1.DuneLocationCalculations);
            CollectionAssert.IsEmpty(calculationsForTargetProbability2.DuneLocationCalculations);
        }

        [Test]
        public void SetDuneLocations_MultipleDuneLocations_SetsExpectedLocationsAndCalculations()
        {
            // Setup
            var calculationsForTargetProbability1 = new DuneLocationCalculationsForTargetProbability();
            var calculationsForTargetProbability2 = new DuneLocationCalculationsForTargetProbability();
            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    calculationsForTargetProbability1,
                    calculationsForTargetProbability2
                }
            };

            var duneLocation1 = new TestDuneLocation();
            var duneLocation2 = new TestDuneLocation();

            TestDuneLocation[] duneLocations =
            {
                duneLocation1,
                duneLocation2
            };

            // Call
            failureMechanism.SetDuneLocations(duneLocations);

            // Assert
            CollectionAssert.AreEqual(duneLocations, failureMechanism.DuneLocations);

            Assert.AreEqual(2, calculationsForTargetProbability1.DuneLocationCalculations.Count);
            Assert.AreSame(duneLocation1, calculationsForTargetProbability1.DuneLocationCalculations[0].DuneLocation);
            Assert.AreSame(duneLocation2, calculationsForTargetProbability1.DuneLocationCalculations[1].DuneLocation);

            Assert.AreEqual(2, calculationsForTargetProbability2.DuneLocationCalculations.Count);
            Assert.AreSame(duneLocation1, calculationsForTargetProbability2.DuneLocationCalculations[0].DuneLocation);
            Assert.AreSame(duneLocation2, calculationsForTargetProbability2.DuneLocationCalculations[1].DuneLocation);
        }
    }
}