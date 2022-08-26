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
using Core.Common.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.Data.TestUtil
{
    /// <summary>
    /// Creates <see cref="CombinedFailureMechanismSectionAssemblyResult"/> instances for test purposes.
    /// </summary>
    public static class CombinedFailureMechanismSectionAssemblyResultTestFactory
    {
        /// <summary>
        /// Creates a default instance of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <returns>A <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</returns>
        public static CombinedFailureMechanismSectionAssemblyResult Create()
        {
            var random = new Random(21);
            return Create(random.NextDouble(), random.NextDouble());
        }

        /// <summary>
        /// Creates an instance of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="sectionStart">The start of the section from the beginning of the reference line
        /// in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of the reference line
        /// in meters.</param>
        /// <returns>A configured <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</returns>
        public static CombinedFailureMechanismSectionAssemblyResult Create(double sectionStart, double sectionEnd)
        {
            var random = new Random(21);
            var totalResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var commonSectionAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var failureMechanismResultsAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var combinedSectionResultAssemblyMethod = random.NextEnumValue<AssemblyMethod>();

            return new CombinedFailureMechanismSectionAssemblyResult(
                sectionStart, sectionEnd, totalResult, commonSectionAssemblyMethod,
                failureMechanismResultsAssemblyMethod, combinedSectionResultAssemblyMethod,
                new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties());
        }

        /// <summary>
        /// Creates an instance of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="seed">The seed to randomize the result.</param>
        /// <param name="hasAssemblyGroupResults">Indicator whether the result has assembly group results.</param>
        /// <returns>A configured <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</returns>
        public static CombinedFailureMechanismSectionAssemblyResult Create(int seed, bool hasAssemblyGroupResults)
        {
            var random = new Random(seed);
            return new CombinedFailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                random.NextEnumValue<AssemblyMethod>(), random.NextEnumValue<AssemblyMethod>(), random.NextEnumValue<AssemblyMethod>(),
                new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                {
                    Piping = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    GrassCoverErosionInwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    MacroStabilityInwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    Microstability = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    StabilityStoneCover = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    WaveImpactAsphaltCover = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    WaterPressureAsphaltCover = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    GrassCoverErosionOutwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    GrassCoverSlipOffOutwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    GrassCoverSlipOffInwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    HeightStructures = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    ClosingStructures = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    PipingStructure = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    StabilityPointStructures = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    DuneErosion = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    SpecificFailureMechanisms = new[]
                    {
                        GetAssemblyGroup(random, hasAssemblyGroupResults),
                        GetAssemblyGroup(random, hasAssemblyGroupResults)
                    }
                });
        }

        private static FailureMechanismSectionAssemblyGroup? GetAssemblyGroup(Random random, bool hasAssemblyGroupResults)
        {
            return hasAssemblyGroupResults
                       ? random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()
                       : (FailureMechanismSectionAssemblyGroup?) null;
        }
    }
}