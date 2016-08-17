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
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.HydraRing.Calculation.IO
{
    /// <summary>
    /// The reader for <see cref="DesignTablesSetting"/> in csv format.
    /// </summary>
    internal class DesignTablesSettingsCsvReader : HydraRingSettingsCsvReader<IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>>>
    {
        private readonly Dictionary<string, int> columns = new Dictionary<string, int>
        {
            {
                ringIdKey, 0
            },
            {
                variableKey, 1
            },
            {
                minKey, 2
            },
            {
                maxKey, 3
            }
        };

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
            }
        };

        /// <summary>
        /// Creates a new instance of <see cref="DesignTablesSettingsCsvReader"/>.
        /// </summary>
        /// <param name="fileContents">The file contents to read.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileContents"/> is not set.</exception>
        public DesignTablesSettingsCsvReader(string fileContents) 
            : base(fileContents, new Dictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>>()) {}

        protected override void CreateSetting(IList<string> line)
        {
            // Get failure mechanism
            var failureMechanismType = GetFailureMechanismType(line);

            if (!Settings.ContainsKey(failureMechanismType))
            {
                Settings.Add(failureMechanismType, new Dictionary<string, DesignTablesSetting>());
            }

            // Get TrajectId
            var ringId = GetRingId(line);
            if (!Settings[failureMechanismType].ContainsKey(ringId))
            {
                Settings[failureMechanismType].Add(ringId, GetDesignTablesSetting(line));
            }
        }

        private HydraRingFailureMechanismType GetFailureMechanismType(IList<string> line)
        {
            return failureMechanismTypes[GetStringValueFromElement(line[columns[variableKey]])];
        }

        private string GetRingId(IList<string> line)
        {
            return GetStringValueFromElement(line[columns[ringIdKey]]);
        }

        private DesignTablesSetting GetDesignTablesSetting(IList<string> line)
        {
            return new DesignTablesSetting(GetDoubleValueFromElement(line[columns[minKey]]),
                                           GetDoubleValueFromElement(line[columns[maxKey]]));
        }

        #region Csv column names

        private const string ringIdKey = "TrajectID";
        private const string variableKey = "Variable";
        private const string minKey = "Min";
        private const string maxKey = "Max";

        #endregion

        #region Variable names

        private const string assessmentLevelKey = "Toetspeil";
        private const string waveHeightKey = "Hs";
        private const string wavePeakPeriodKey = "Tp";
        private const string waveSpectralPeriodKey = "Tm-1,0";
        private const string qVariantKey = "Q";
        private const string dikeHeightKey = "HBN";

        #endregion
    }
}