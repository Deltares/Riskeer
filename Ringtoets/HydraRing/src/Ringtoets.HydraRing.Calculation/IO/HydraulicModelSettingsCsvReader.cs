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

namespace Ringtoets.HydraRing.Calculation.IO
{
    /// <summary>
    /// The reader for the <see cref="HydraRingTimeIntegrationSchemeType"/> in csv format.
    /// </summary>
    internal class HydraulicModelSettingsCsvReader
    {
        private const char separator = ';';

        private readonly string fileContents;

        private readonly IDictionary<int, IDictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>> settings = new Dictionary<int, IDictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>>();

        private readonly Dictionary<string, int> columns = new Dictionary<string, int>
        {
            {
                ringIdKey, 0
            },
            {
                mechanismIdKey, 1
            },
            {
                subMechanismIdKey, 2
            },
            {
                timeIntegrationKey, 3
            }
        };

        /// <summary>
        /// Reads the settings from the file.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> with the settings.</returns>
        public HydraulicModelSettingsCsvReader(string file)
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
        public IDictionary<int, IDictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>> ReadSettings()
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
            int failureMechanismType = GetFailureMechanismType(line);

            if (!settings.ContainsKey(failureMechanismType))
            {
                settings.Add(failureMechanismType, new Dictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>());
            }

            // Get sub mechanism
            int subMechanism = GetSubMechanismType(line);

            if (!settings[failureMechanismType].ContainsKey(subMechanism))
            {
                settings[failureMechanismType].Add(subMechanism, new Dictionary<string, HydraRingTimeIntegrationSchemeType>());
            }

            // Get TrajectId
            string ringId = GetRingId(line);

            if (!settings[failureMechanismType][subMechanism].ContainsKey(ringId))
            {
                settings[failureMechanismType][subMechanism].Add(ringId, (HydraRingTimeIntegrationSchemeType) GetIntValueFromElement(line[columns[timeIntegrationKey]]));
            }
        }

        private int GetFailureMechanismType(IList<string> line)
        {
            return GetIntValueFromElement(line[columns[mechanismIdKey]]);
        }

        private int GetSubMechanismType(IList<string> line)
        {
            return GetIntValueFromElement(line[columns[subMechanismIdKey]]);
        }

        private string GetRingId(IList<string> line)
        {
            return line[columns[ringIdKey]].Trim().Replace("\"", "");
        }

        private static int GetIntValueFromElement(string element)
        {
            return int.Parse(element.Trim());
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
        private const string mechanismIdKey = "MechanismID";
        private const string subMechanismIdKey = "SubMechanismID";
        private const string timeIntegrationKey = "TimeIntegration";

        #endregion
    }
}