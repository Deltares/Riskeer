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
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Overtopping
{
    [TestFixture]
    public class OvertoppingCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int expectedCalculationTypeId = 1;
            const int expectedVariableId = 1;
            int hydraulicBoundaryLocationId = 1000;
            HydraRingSection expectedHydraRingSection = new HydraRingSection(expectedVariableId, "1000", double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            double dikeHeight = 11.11;
            double criticalOvertoppingMean = 22.22;
            double criticalOvertoppingStandardDeviation = 33.33;
            var expectedRingProfilePoints = new List<HydraRingProfilePoint>
            {
                new HydraRingProfilePoint(1.1, 2.2)
            };
            var expectedRingForelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(2.2, 3.3)
            };
            var expectedRingBreakwaters = new List<HydraRingBreakwater>
            {
                new HydraRingBreakwater(2, 3.3)
            };

            // Call
            OvertoppingCalculationInput overtoppingCalculationInput = new OvertoppingCalculationInput(hydraulicBoundaryLocationId, expectedHydraRingSection, dikeHeight,
                                                                                                      criticalOvertoppingMean, criticalOvertoppingStandardDeviation,
                                                                                                      expectedRingProfilePoints, expectedRingForelandPoints, expectedRingBreakwaters);

            // Assert
            Assert.AreEqual(expectedCalculationTypeId, overtoppingCalculationInput.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, overtoppingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesOvertopping, overtoppingCalculationInput.FailureMechanismType);
            Assert.AreEqual(expectedVariableId, overtoppingCalculationInput.VariableId);
            Assert.IsNotNull(overtoppingCalculationInput.Section);
            CheckOvertoppingVariables(GetDefaultOvertoppingVariables().ToArray(), overtoppingCalculationInput.Variables.ToArray());
            CollectionAssert.AreEqual(expectedRingProfilePoints, overtoppingCalculationInput.ProfilePoints);
            CollectionAssert.AreEqual(expectedRingForelandPoints, overtoppingCalculationInput.ForelandsPoints);
            CollectionAssert.AreEqual(expectedRingBreakwaters, overtoppingCalculationInput.BreakWaters);
            Assert.IsNaN(overtoppingCalculationInput.Beta);

            var hydraRingSection = overtoppingCalculationInput.Section;
            Assert.AreEqual(expectedHydraRingSection, hydraRingSection);
        }

        [Test]
        [TestCase(101, null)]
        [TestCase(102, 94)]
        [TestCase(103, 95)]
        [TestCase(104, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Setup 
            HydraRingSection hydraRingSection = new HydraRingSection(1, "1000", double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            OvertoppingCalculationInput overtoppingCalculationInput = new OvertoppingCalculationInput(1, hydraRingSection, 2, 3, 4,
                                                                                                      new List<HydraRingProfilePoint>(),
                                                                                                      new List<HydraRingForelandPoint>(),
                                                                                                      new List<HydraRingBreakwater>());

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, overtoppingCalculationInput.GetSubMechanismModelId(subMechanismModelId));
        }

        private void CheckOvertoppingVariables(HydraRingVariable[] expected, HydraRingVariable[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].DeviationType, actual[i].DeviationType);
                Assert.AreEqual(expected[i].DistributionType, actual[i].DistributionType);
                Assert.AreEqual(expected[i].Mean, actual[i].Mean);
                Assert.AreEqual(expected[i].Shift, actual[i].Shift);
                Assert.AreEqual(expected[i].Variability, actual[i].Variability);
                Assert.AreEqual(expected[i].VariableId, actual[i].VariableId);
            }
        }

        private IEnumerable<HydraRingVariable> GetDefaultOvertoppingVariables()
        {
            yield return new HydraRingVariableImplementation(1, HydraRingDistributionType.Deterministic, 11.11, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariableImplementation(8, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariableImplementation(10, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 4.75, 0.5, double.NaN);
            yield return new HydraRingVariableImplementation(11, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 2.6, 0.35, double.NaN);
            yield return new HydraRingVariableImplementation(12, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariableImplementation(17, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 22.22, 33.33, double.NaN);
            yield return new HydraRingVariableImplementation(120, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 1, 0.07, double.NaN);
            yield return new HydraRingVariableImplementation(123, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 0.92, 0.24, double.NaN);
        }

        private class HydraRingVariableImplementation : HydraRingVariable
        {
            public HydraRingVariableImplementation(int variableId, HydraRingDistributionType distributionType, double value, HydraRingDeviationType deviationType, double mean, double variability, double shift) :
                base(variableId, distributionType, value, deviationType, mean, variability, shift) {}
        }
    }
}