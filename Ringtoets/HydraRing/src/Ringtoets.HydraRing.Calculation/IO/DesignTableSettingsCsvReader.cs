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
using System.Linq;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.HydraRing.Calculation.IO
{
    /// <summary>
    /// The reader for the <see cref="DesignTableSettings"/> in csv format.
    /// </summary>
    internal class DesignTableSettingsCsvReader
    {
        private const char separator = ';';

        private readonly string fileContents;

        private readonly IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTableSettings>> settings = new Dictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTableSettings>>();

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
        /// Creates a new instance of <see cref="DesignTableSettingsCsvReader"/>.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="file"/> is not set.</exception>
        public DesignTableSettingsCsvReader(string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file", @"A file must be set.");
            }

            fileContents = file;
        }

        /// <summary>
        /// Reads the settings from the file.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> with the settings.</returns>
        public IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTableSettings>> ReadSettings()
        {
            string[] lines = fileContents.Split('\n');

            foreach (string line in lines.Skip(1).Where(s => !string.IsNullOrEmpty(s)))
            {
                CreateSetting(TokenizeString(line));
            }

            return settings;
        }

        private void CreateSetting(IList<string> line)
        {
            // Get failure mechanism
            HydraRingFailureMechanismType failureMechanismType = GetFailureMechanismType(line);

            if (!settings.ContainsKey(failureMechanismType))
            {
                settings.Add(failureMechanismType, new Dictionary<string, DesignTableSettings>());
            }

            // Get TrajectId
            string ringId = GetRingId(line);
            if (!settings[failureMechanismType].ContainsKey(ringId))
            {
                settings[failureMechanismType].Add(ringId, GetDesignTableSettings(line));
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

        private DesignTableSettings GetDesignTableSettings(IList<string> line)
        {
            return new DesignTableSettings(GetIntValueFromElement(line[columns[minKey]]),
                                           GetIntValueFromElement(line[columns[maxKey]]));
        }

        private string GetStringValueFromElement(string element)
        {
            return element.Trim().Replace("\"", "");
        }

        private int GetIntValueFromElement(string element)
        {
            return int.Parse(GetStringValueFromElement(element));
        }

        private string[] TokenizeString(string readText)
        {
            if (!readText.Contains(separator))
            {
                return new string[]
                {};
            }
            return readText.Split(separator)
                           .TakeWhile(text => !string.IsNullOrEmpty(text))
                           .ToArray();
        }

        #region Csv column names

        private const string ringIdKey = "TrajectID";
        private const string variableKey = "Variable";
        private const string minKey = "Min";
        private const string maxKey = "Max";

        #endregion

        #region Failure mechanism type names

        private const string assessmentLevelKey = "Toetspeil";
        private const string waveHeightKey = "Hs";
        private const string wavePeakPeriodKey = "Tp";
        private const string waveSpectralPeriodKey = "Tm-1,0";
        private const string qVariantKey = "Q";
        private const string dikeHeightKey = "HBN";

        #endregion
    }
}