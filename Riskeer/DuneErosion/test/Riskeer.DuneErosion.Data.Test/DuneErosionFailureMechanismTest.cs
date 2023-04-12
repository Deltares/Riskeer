// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.DuneErosion.Data.TestUtil;

namespace Riskeer.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneErosionFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new DuneErosionFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase<NonAdoptableFailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual("Duinafslag", failureMechanism.Name);
            Assert.AreEqual("DA", failureMechanism.Code);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.DuneLocations);
            CollectionAssert.IsEmpty(failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities);
            Assert.IsNotNull(failureMechanism.GeneralInput);

            CollectionAssert.IsEmpty(failureMechanism.SectionResults);

            Assert.IsNotNull(failureMechanism.CalculationsInputComments);
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
        public void SetDuneLocations_MultipleDuneLocations_SetsExpectedLocationsAndCalculations()
        {
            // Setup
            var calculationsForTargetProbability1 = new DuneLocationCalculationsForTargetProbability(0.1);
            var calculationsForTargetProbability2 = new DuneLocationCalculationsForTargetProbability(0.01);
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

        [Test]
        public void GivenFailureMechanismWithDuneLocationCalculations_WhenSetDuneLocations_ThenLocationsAndCalculationsAdded()
        {
            // Setup
            var calculationsForTargetProbability1 = new DuneLocationCalculationsForTargetProbability(0.1);
            var calculationsForTargetProbability2 = new DuneLocationCalculationsForTargetProbability(0.01);
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
            Assert.AreEqual(1, failureMechanism.DuneLocations.Count());
            Assert.AreEqual(1, calculationsForTargetProbability1.DuneLocationCalculations.Count);
            Assert.AreEqual(1, calculationsForTargetProbability2.DuneLocationCalculations.Count);

            // Call
            failureMechanism.SetDuneLocations(new DuneLocation[]
            {
                new TestDuneLocation()
            });

            // Assert
            Assert.AreEqual(2, failureMechanism.DuneLocations.Count());
            Assert.AreEqual(2, calculationsForTargetProbability1.DuneLocationCalculations.Count);
            Assert.AreEqual(2, calculationsForTargetProbability2.DuneLocationCalculations.Count);
        }
    }
}