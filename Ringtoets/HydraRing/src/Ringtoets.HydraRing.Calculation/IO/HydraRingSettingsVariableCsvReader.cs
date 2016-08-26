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

using System;
using System.Collections.Generic;
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.HydraRing.Calculation.IO
{
    /// <summary>
    /// An abstract base for Hydra-Ring settings readers with variable properties.
    /// </summary>
    /// <typeparam name="TOutput">The output format of the read settings.</typeparam>
    internal abstract class HydraRingSettingsVariableCsvReader<TOutput> : HydraRingSettingsCsvReader<TOutput>
    {
        private readonly Dictionary<string, HydraRingFailureMechanismType> failureMechanismTypes = new Dictionary<string, HydraRingFailureMechanismType>
        {
            {
                assessmentLevelKey, HydraRingFailureMechanismType.AssessmentLevel
            },
            {
                waveHeightKey, HydraRingFailureMechanismType.WaveHeight
            },
            {
                wavePeakPeriodKey, HydraRingFailureMechanismType.WavePeakPeriod
            },
            {
                waveSpectralPeriodKey, HydraRingFailureMechanismType.WaveSpectralPeriod
            },
            {
                qVariantKey, HydraRingFailureMechanismType.QVariant
            },
            {
                dikeHeightKey, HydraRingFailureMechanismType.DikesHeight
            },
            {
                grassKey, HydraRingFailureMechanismType.DikesOvertopping
            },
            {
                heightStructuresKey, HydraRingFailureMechanismType.StructuresOvertopping
            },
            {
                closingStructuresKey, HydraRingFailureMechanismType.StructuresClosure
            },
            {
                structuresStructuralFailureKey, HydraRingFailureMechanismType.StructuresStructuralFailure
            }
        };

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingSettingsVariableCsvReader{TOutput}"/>.
        /// </summary>
        /// <param name="fileContents">The file contents to read.</param>
        /// <param name="settings">The provided settings object to add the read settings to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileContents"/> or <paramref name="settings"/> is <c>null</c>.</exception>
        protected HydraRingSettingsVariableCsvReader(string fileContents, TOutput settings)
            : base(fileContents, settings) {}

        protected abstract override void CreateSetting(IList<string> line);

        /// <summary>
        /// Gets the <see cref="HydraRingFailureMechanismType"/> from the variable.
        /// </summary>
        /// <param name="variable">The variable to get the <see cref="HydraRingFailureMechanismType"/> for.</param>
        /// <returns>The <see cref="HydraRingFailureMechanismType"/>.</returns>
        protected HydraRingFailureMechanismType GetFailureMechanismType(string variable)
        {
            return failureMechanismTypes[variable];
        }

        #region Variable names

        private const string assessmentLevelKey = "Toetspeil";
        private const string waveHeightKey = "Hs";
        private const string wavePeakPeriodKey = "Tp";
        private const string waveSpectralPeriodKey = "Tm-1,0";
        private const string qVariantKey = "Q";
        private const string dikeHeightKey = "HBN";
        private const string grassKey = "Gras";
        private const string heightStructuresKey = "KwHoogte";
        private const string closingStructuresKey = "KwSluiten";
        private const string structuresStructuralFailureKey = "KwPuntconstructies";

        #endregion
    }
}