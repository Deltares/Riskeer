// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
        public void AddDuneLocations_DuneLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.AddDuneLocations(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("duneLocations", paramName);
        }

        [Test]
        public void AddDuneLocations_MultipleDuneLocations_SetsExpectedLocationsAndCalculations()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var duneLocation1 = new TestDuneLocation();
            var duneLocation2 = new TestDuneLocation();
            TestDuneLocation[] duneLocations =
            {
                duneLocation1,
                duneLocation2
            };

            // Call
            failureMechanism.AddDuneLocations(duneLocations);

            // Assert
            CollectionAssert.AreEqual(duneLocations, failureMechanism.DuneLocations);

            AssertNumberOfDuneLocationCalculations(failureMechanism, duneLocations.Length);
            AssertDuneLocationCalculations(failureMechanism, 0, duneLocation1);
            AssertDuneLocationCalculations(failureMechanism, 1, duneLocation2);
        }

        private static void AssertNumberOfDuneLocationCalculations(DuneErosionFailureMechanism failureMechanism, int expectedNumberOfCalculations)
        {
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.CalculationsForMechanismSpecificSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.CalculationsForLowerLimitNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.CalculationsForFactorizedLowerLimitNorm.Count());
        }

        private static void AssertDuneLocationCalculations(DuneErosionFailureMechanism failureMechanism, int index, DuneLocation expectedDuneLocation)
        {
            AssertDefaultDuneLocationCalculation(expectedDuneLocation, failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.ElementAt(index));
            AssertDefaultDuneLocationCalculation(expectedDuneLocation, failureMechanism.CalculationsForMechanismSpecificSignalingNorm.ElementAt(index));
            AssertDefaultDuneLocationCalculation(expectedDuneLocation, failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.ElementAt(index));
            AssertDefaultDuneLocationCalculation(expectedDuneLocation, failureMechanism.CalculationsForLowerLimitNorm.ElementAt(index));
            AssertDefaultDuneLocationCalculation(expectedDuneLocation, failureMechanism.CalculationsForFactorizedLowerLimitNorm.ElementAt(index));
        }

        private static void AssertDefaultDuneLocationCalculation(DuneLocation expectedDuneLocation, DuneLocationCalculation calculation)
        {
            Assert.AreSame(expectedDuneLocation, calculation.DuneLocation);
            Assert.IsNull(calculation.Output);
        }
    }
}