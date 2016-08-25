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
    internal class DesignTablesSettingsCsvReader : HydraRingSettingsVariableCsvReader<IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DesignTablesSettingsCsvReader"/>.
        /// </summary>
        /// <param name="fileContents">The file contents to read.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileContents"/> is not set.</exception>
        public DesignTablesSettingsCsvReader(string fileContents)
            : base(fileContents, new Dictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>>()) {}

        protected override void CreateSetting(IList<string> line)
        {
            HydraRingFailureMechanismType failureMechanismType = GetFailureMechanismType(GetStringValueFromElement(line[(int) DesignTablesSettingsColumns.Variable]));

            if (!Settings.ContainsKey(failureMechanismType))
            {
                Settings.Add(failureMechanismType, new Dictionary<string, DesignTablesSetting>());
            }

            string ringId = GetRingId(line);
            if (!Settings[failureMechanismType].ContainsKey(ringId))
            {
                Settings[failureMechanismType].Add(ringId, GetDesignTablesSetting(line));
            }
        }

        private string GetRingId(IList<string> line)
        {
            return GetStringValueFromElement(line[(int) DesignTablesSettingsColumns.RingId]);
        }

        private DesignTablesSetting GetDesignTablesSetting(IList<string> line)
        {
            return new DesignTablesSetting(GetDoubleValueFromElement(line[(int) DesignTablesSettingsColumns.Min]),
                                           GetDoubleValueFromElement(line[(int) DesignTablesSettingsColumns.Max]));
        }
    }
}