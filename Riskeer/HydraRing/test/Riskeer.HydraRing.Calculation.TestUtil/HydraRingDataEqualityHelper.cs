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

using System.Linq;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.TestUtil
{
    /// <summary>
    /// Assert methods for <see cref="HydraRingCalculationInput"/>.
    /// </summary>
    public static class HydraRingDataEqualityHelper
    {
        private const double accuracy = 1e-6;

        /// <summary>
        /// Asserts whether or not <paramref name="expectedInput"/> and <paramref name="actualInput"/> are equal to each other.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="HydraRingCalculationInput"/> to compare.</typeparam>
        /// <param name="expectedInput">The expected calculation input.</param>
        /// <param name="actualInput">The actual calculation input.</param>
        public static void AreEqual<T>(T expectedInput, T actualInput) where T : HydraRingCalculationInput
        {
            Assert.AreEqual(expectedInput.FailureMechanismType, actualInput.FailureMechanismType);
            Assert.AreEqual(expectedInput.CalculationTypeId, actualInput.CalculationTypeId);
            Assert.AreEqual(expectedInput.VariableId, actualInput.VariableId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            AreEqual(expectedInput.Section, actualInput.Section);
            AreEqual(expectedInput.Variables.ToArray(), actualInput.Variables.ToArray());
            AreEqual(expectedInput.ProfilePoints.ToArray(), actualInput.ProfilePoints.ToArray());
            AreEqual(expectedInput.ForelandsPoints.ToArray(), actualInput.ForelandsPoints.ToArray());
            AreEqual(expectedInput.BreakWater, actualInput.BreakWater);
            Assert.AreEqual(expectedInput.Beta, actualInput.Beta, accuracy);
        }

        /// <summary>
        /// Asserts whether or not <paramref name="expectedVariables"/> and <paramref name="actualVariables"/> are equal to each other.
        /// </summary>
        /// <param name="expectedVariables">The array of expected <see cref="HydraRingVariable"/>.</param>
        /// <param name="actualVariables">The array of actual <see cref="HydraRingVariable"/>.</param>
        public static void AreEqual(HydraRingVariable[] expectedVariables, HydraRingVariable[] actualVariables)
        {
            Assert.AreEqual(expectedVariables.Length, actualVariables.Length);

            for (var i = 0; i < expectedVariables.Length; i++)
            {
                AreEqual(expectedVariables[i], actualVariables[i]);
            }
        }

        private static void AreEqual(HydraRingSection expectedSection, HydraRingSection actualSection)
        {
            Assert.AreEqual(expectedSection.SectionId, actualSection.SectionId);
            Assert.AreEqual(expectedSection.SectionLength, actualSection.SectionLength, accuracy);
            Assert.AreEqual(expectedSection.CrossSectionNormal, actualSection.CrossSectionNormal, accuracy);
        }

        private static void AreEqual(HydraRingVariable expectedVariable, HydraRingVariable actualVariable)
        {
            Assert.AreEqual(expectedVariable.DeviationType, actualVariable.DeviationType);
            Assert.AreEqual(expectedVariable.DistributionType, actualVariable.DistributionType);
            Assert.AreEqual(expectedVariable.Value, actualVariable.Value, accuracy);
            Assert.AreEqual(expectedVariable.Parameter1, actualVariable.Parameter1, accuracy);
            Assert.AreEqual(expectedVariable.Parameter2, actualVariable.Parameter2);
            Assert.AreEqual(expectedVariable.Parameter3, actualVariable.Parameter3);
            Assert.AreEqual(expectedVariable.Parameter4, actualVariable.Parameter4);
            Assert.AreEqual(expectedVariable.VariableId, actualVariable.VariableId, accuracy);
            Assert.AreEqual(expectedVariable.CoefficientOfVariation, actualVariable.CoefficientOfVariation, accuracy);
        }

        private static void AreEqual(HydraRingProfilePoint[] expectedProfilePoints, HydraRingProfilePoint[] actualProfilePoints)
        {
            Assert.AreEqual(expectedProfilePoints.Length, actualProfilePoints.Length);

            for (var i = 0; i < expectedProfilePoints.Length; i++)
            {
                Assert.AreEqual(expectedProfilePoints[i].X, actualProfilePoints[i].X, accuracy);
                Assert.AreEqual(expectedProfilePoints[i].Z, actualProfilePoints[i].Z, accuracy);
                Assert.AreEqual(expectedProfilePoints[i].Roughness, actualProfilePoints[i].Roughness, accuracy);
            }
        }

        private static void AreEqual(HydraRingForelandPoint[] expectedForelandPoints, HydraRingForelandPoint[] actualForelandPoints)
        {
            Assert.AreEqual(expectedForelandPoints.Length, actualForelandPoints.Length);

            for (var i = 0; i < expectedForelandPoints.Length; i++)
            {
                Assert.AreEqual(expectedForelandPoints[i].X, actualForelandPoints[i].X, accuracy);
                Assert.AreEqual(expectedForelandPoints[i].Z, actualForelandPoints[i].Z, accuracy);
            }
        }

        private static void AreEqual(HydraRingBreakWater expectedBreakWater, HydraRingBreakWater actualBreakWater)
        {
            if (expectedBreakWater == null)
            {
                Assert.IsNull(actualBreakWater);
            }
            else
            {
                Assert.AreEqual(expectedBreakWater.Height, actualBreakWater.Height, accuracy);
                Assert.AreEqual(expectedBreakWater.Type, actualBreakWater.Type);
            }
        }
    }
}