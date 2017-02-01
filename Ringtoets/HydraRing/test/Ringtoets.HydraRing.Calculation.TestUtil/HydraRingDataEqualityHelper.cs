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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Data.Variables;

namespace Ringtoets.HydraRing.Calculation.TestUtil
{
    /// <summary>
    /// Assert methods for <see cref="HydraRingVariable"/>.
    /// </summary>
    public static class HydraRingDataEqualityHelper
    {
        private const double accuracy = 1e-6;

        /// <summary>
        /// Asserts whether or not <paramref name="expectedInput"/> and <paramref name="actualInput"/> are equal to each other.
        /// </summary>
        /// <param name="expectedInput">The expected calculation input.</param>
        /// <param name="actualInput">The actual calculation input.</param>
        public static void AreEqual(OvertoppingCalculationInput expectedInput, OvertoppingCalculationInput actualInput)
        {
            Assert.AreEqual(expectedInput.FailureMechanismType, actualInput.FailureMechanismType);
            Assert.AreEqual(expectedInput.CalculationTypeId, actualInput.CalculationTypeId);
            Assert.AreEqual(expectedInput.VariableId, actualInput.VariableId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Section.SectionId, actualInput.Section.SectionId);

            Assert.AreEqual(expectedInput.Beta, actualInput.Beta, accuracy);
            Assert.AreEqual(expectedInput.Section.CrossSectionNormal, actualInput.Section.CrossSectionNormal, accuracy);

            AreEqual(expectedInput.BreakWater, actualInput.BreakWater);
            AreEqual(expectedInput.Section, actualInput.Section);
            AreEqual(expectedInput.ProfilePoints.ToArray(), actualInput.ProfilePoints.ToArray());
            AreEqual(expectedInput.ForelandsPoints.ToArray(), actualInput.ForelandsPoints.ToArray());
            AreEqual(expectedInput.Variables.ToArray(), actualInput.Variables.ToArray());
        }

        /// <summary>
        /// Asserts whether or not <paramref name="expectedInput"/> and <paramref name="actualInput"/> are equal to each other.
        /// </summary>
        /// <param name="expectedInput">The expected calculation input.</param>
        /// <param name="actualInput">The actual calculation input.</param>
        public static void AreEqual(DikeHeightCalculationInput expectedInput, DikeHeightCalculationInput actualInput)
        {
            Assert.AreEqual(expectedInput.FailureMechanismType, actualInput.FailureMechanismType);
            Assert.AreEqual(expectedInput.CalculationTypeId, actualInput.CalculationTypeId);
            Assert.AreEqual(expectedInput.VariableId, actualInput.VariableId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Section.SectionId, actualInput.Section.SectionId);

            Assert.AreEqual(expectedInput.Beta, actualInput.Beta, accuracy);
            Assert.AreEqual(expectedInput.Section.CrossSectionNormal, actualInput.Section.CrossSectionNormal, accuracy);

            AreEqual(expectedInput.BreakWater, actualInput.BreakWater);
            AreEqual(expectedInput.Section, actualInput.Section);
            AreEqual(expectedInput.ProfilePoints.ToArray(), actualInput.ProfilePoints.ToArray());
            AreEqual(expectedInput.ForelandsPoints.ToArray(), actualInput.ForelandsPoints.ToArray());
            AreEqual(expectedInput.Variables.ToArray(), actualInput.Variables.ToArray());
        }

        /// <summary>
        /// Asserts whether or not <paramref name="expectedInput"/> and <paramref name="actualInput"/> are equal to each other.
        /// </summary>
        /// <param name="expectedInput">The expected calculation input.</param>
        /// <param name="actualInput">The actual calculation input.</param>
        public static void AreEqual(WaveConditionsCosineCalculationInput expectedInput, HydraRingCalculationInput actualInput)
        {
            Assert.AreEqual(expectedInput.FailureMechanismType, actualInput.FailureMechanismType);
            Assert.AreEqual(expectedInput.CalculationTypeId, actualInput.CalculationTypeId);
            Assert.AreEqual(expectedInput.VariableId, actualInput.VariableId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Section.SectionId, actualInput.Section.SectionId);

            Assert.AreEqual(expectedInput.Beta, actualInput.Beta, accuracy);
            Assert.AreEqual(expectedInput.Section.CrossSectionNormal, actualInput.Section.CrossSectionNormal, accuracy);

            AreEqual(expectedInput.BreakWater, actualInput.BreakWater);
            AreEqual(expectedInput.Section, actualInput.Section);
            AreEqual(expectedInput.ForelandsPoints.ToArray(), actualInput.ForelandsPoints.ToArray());
            AreEqual(expectedInput.Variables.ToArray(), actualInput.Variables.ToArray());
        }

        /// <summary>
        /// Asserts whether or not <paramref name="expectedInput"/> and <paramref name="actualInput"/> are equal to each other.
        /// </summary>
        /// <param name="expectedInput">The expected calculation input.</param>
        /// <param name="actualInput">The actual calculation input.</param>
        public static void AreEqual(StructuresOvertoppingCalculationInput expectedInput, StructuresOvertoppingCalculationInput actualInput)
        {
            Assert.AreEqual(expectedInput.FailureMechanismType, actualInput.FailureMechanismType);
            Assert.AreEqual(expectedInput.CalculationTypeId, actualInput.CalculationTypeId);
            Assert.AreEqual(expectedInput.VariableId, actualInput.VariableId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Section.SectionId, actualInput.Section.SectionId);

            Assert.AreEqual(expectedInput.Beta, actualInput.Beta, accuracy);
            Assert.AreEqual(expectedInput.Section.CrossSectionNormal, actualInput.Section.CrossSectionNormal, accuracy);

            AreEqual(expectedInput.BreakWater, actualInput.BreakWater);
            AreEqual(expectedInput.Section, actualInput.Section);
            AreEqual(expectedInput.ForelandsPoints.ToArray(), actualInput.ForelandsPoints.ToArray());
            AreEqual(expectedInput.Variables.ToArray(), actualInput.Variables.ToArray());
        }

        /// <summary>
        /// Asserts whether or not <paramref name="expectedInput"/> and <paramref name="actualInput"/> are equal to each other.
        /// </summary>
        /// <param name="expectedInput">The expected calculation input.</param>
        /// <param name="actualInput">The actual calculation input.</param>
        public static void AreEqual(StructuresClosureCalculationInput expectedInput, StructuresClosureCalculationInput actualInput)
        {
            Assert.AreEqual(expectedInput.FailureMechanismType, actualInput.FailureMechanismType);
            Assert.AreEqual(expectedInput.CalculationTypeId, actualInput.CalculationTypeId);
            Assert.AreEqual(expectedInput.VariableId, actualInput.VariableId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Section.SectionId, actualInput.Section.SectionId);

            Assert.AreEqual(expectedInput.Beta, actualInput.Beta, accuracy);
            Assert.AreEqual(expectedInput.Section.CrossSectionNormal, actualInput.Section.CrossSectionNormal, accuracy);

            AreEqual(expectedInput.BreakWater, actualInput.BreakWater);
            AreEqual(expectedInput.Section, actualInput.Section);
            AreEqual(expectedInput.ForelandsPoints.ToArray(), actualInput.ForelandsPoints.ToArray());
            AreEqual(expectedInput.Variables.ToArray(), actualInput.Variables.ToArray());
        }

        /// <summary>
        /// Asserts whether or not <paramref name="expectedInput"/> and <paramref name="actualInput"/> are equal to each other.
        /// </summary>
        /// <param name="expectedInput">The expected calculation input.</param>
        /// <param name="actualInput">The actual calculation input.</param>
        public static void AreEqual(StructuresStabilityPointCalculationInput expectedInput, StructuresStabilityPointCalculationInput actualInput)
        {
            Assert.AreEqual(expectedInput.FailureMechanismType, actualInput.FailureMechanismType);
            Assert.AreEqual(expectedInput.CalculationTypeId, actualInput.CalculationTypeId);
            Assert.AreEqual(expectedInput.VariableId, actualInput.VariableId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Section.SectionId, actualInput.Section.SectionId);

            Assert.AreEqual(expectedInput.Beta, actualInput.Beta, accuracy);
            Assert.AreEqual(expectedInput.Section.CrossSectionNormal, actualInput.Section.CrossSectionNormal, accuracy);

            AreEqual(expectedInput.BreakWater, actualInput.BreakWater);
            AreEqual(expectedInput.Section, actualInput.Section);
            AreEqual(expectedInput.ForelandsPoints.ToArray(), actualInput.ForelandsPoints.ToArray());
            AreEqual(expectedInput.Variables.ToArray(), actualInput.Variables.ToArray());
        }

        /// <summary>
        /// Asserts whether or not <paramref name="expected"/> and <paramref name="actual"/> are equal to each other.
        /// </summary>
        /// <param name="expected">The array of expected <see cref="HydraRingVariable"/>.</param>
        /// <param name="actual">The array of actual <see cref="HydraRingVariable"/>.</param>
        public static void AreEqual(HydraRingVariable[] expected, HydraRingVariable[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                AreEqual(expected[i], actual[i]);
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

        private static void AreEqual(HydraRingSection expectedSection, HydraRingSection actualSection)
        {
            Assert.AreEqual(expectedSection.SectionLength, actualSection.SectionLength, accuracy);
            Assert.AreEqual(expectedSection.CrossSectionNormal, actualSection.CrossSectionNormal, accuracy);
            Assert.AreEqual(expectedSection.SectionId, actualSection.SectionId);
        }

        private static void AreEqual(HydraRingProfilePoint[] expectedProfilePoints, HydraRingProfilePoint[] actualProfilePoints)
        {
            Assert.AreEqual(expectedProfilePoints.Length, actualProfilePoints.Length);

            for (int i = 0; i < expectedProfilePoints.Length; i++)
            {
                Assert.AreEqual(expectedProfilePoints[i].X, actualProfilePoints[i].X, accuracy);
                Assert.AreEqual(expectedProfilePoints[i].Z, actualProfilePoints[i].Z, accuracy);
                Assert.AreEqual(expectedProfilePoints[i].Roughness, actualProfilePoints[i].Roughness, accuracy);
            }
        }

        private static void AreEqual(HydraRingForelandPoint[] expectedForelandPoints, HydraRingForelandPoint[] actualForelandPoints)
        {
            Assert.AreEqual(expectedForelandPoints.Length, actualForelandPoints.Length);

            for (int i = 0; i < expectedForelandPoints.Length; i++)
            {
                Assert.AreEqual(expectedForelandPoints[i].X, actualForelandPoints[i].X, accuracy);
                Assert.AreEqual(expectedForelandPoints[i].Z, actualForelandPoints[i].Z, accuracy);
            }
        }

        private static void AreEqual(HydraRingVariable expected, HydraRingVariable actual)
        {
            Assert.AreEqual(expected.DeviationType, actual.DeviationType);
            Assert.AreEqual(expected.DistributionType, actual.DistributionType);
            Assert.AreEqual(expected.Value, actual.Value, accuracy);
            Assert.AreEqual(expected.Parameter1, actual.Parameter1, accuracy);
            Assert.AreEqual(expected.Parameter2, actual.Parameter2);
            Assert.AreEqual(expected.Parameter3, actual.Parameter3);
            Assert.AreEqual(expected.Parameter4, actual.Parameter4);
            Assert.AreEqual(expected.VariableId, actual.VariableId, accuracy);
            Assert.AreEqual(expected.CoefficientOfVariation, actual.CoefficientOfVariation, accuracy);
        }
    }
}