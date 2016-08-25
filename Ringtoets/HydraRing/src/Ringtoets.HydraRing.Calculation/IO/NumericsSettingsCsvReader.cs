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
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.HydraRing.Calculation.IO
{
    /// <summary>
    /// The reader for <see cref="NumericsSetting"/> in csv format.
    /// </summary>
    internal class NumericsSettingsCsvReader : HydraRingSettingsCsvReader<IDictionary<int, IDictionary<int, IDictionary<string, NumericsSetting>>>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="NumericsSettingsCsvReader"/>.
        /// </summary>
        /// <param name="fileContents">The file contents to read.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileContents"/> is not set.</exception>
        public NumericsSettingsCsvReader(string fileContents) : base(fileContents, new Dictionary<int, IDictionary<int, IDictionary<string, NumericsSetting>>>()) {}

        protected override void CreateSetting(IList<string> line)
        {
            int failureMechanismType = GetFailureMechanismType(line);

            if (!Settings.ContainsKey(failureMechanismType))
            {
                Settings.Add(failureMechanismType, new Dictionary<int, IDictionary<string, NumericsSetting>>());
            }

            int subMechanism = GetSubMechanismType(line);

            if (!Settings[failureMechanismType].ContainsKey(subMechanism))
            {
                Settings[failureMechanismType].Add(subMechanism, new Dictionary<string, NumericsSetting>());
            }

            string ringId = GetRingId(line);

            if (!Settings[failureMechanismType][subMechanism].ContainsKey(ringId))
            {
                Settings[failureMechanismType][subMechanism].Add(ringId, GetNumericsSetting(line));
            }
        }

        private int GetFailureMechanismType(IList<string> line)
        {
            return GetIntValueFromElement(line[(int) NumericsSettingsColumns.MechanismId]);
        }

        private int GetSubMechanismType(IList<string> line)
        {
            return GetIntValueFromElement(line[(int) NumericsSettingsColumns.SubMechanismId]);
        }

        private string GetRingId(IList<string> line)
        {
            return GetStringValueFromElement(line[(int) NumericsSettingsColumns.RingId]);
        }

        private NumericsSetting GetNumericsSetting(IList<string> line)
        {
            return new NumericsSetting(GetIntValueFromElement(line[(int) NumericsSettingsColumns.CalculationMethod]),
                                       GetIntValueFromElement(line[(int) NumericsSettingsColumns.FormStartMethod]),
                                       GetIntValueFromElement(line[(int) NumericsSettingsColumns.FormIterations]),
                                       GetDoubleValueFromElement(line[(int) NumericsSettingsColumns.FormRelaxationFactor]),
                                       GetDoubleValueFromElement(line[(int) NumericsSettingsColumns.FormEpsBeta]),
                                       GetDoubleValueFromElement(line[(int) NumericsSettingsColumns.FormEpsHoh]),
                                       GetDoubleValueFromElement(line[(int) NumericsSettingsColumns.FormEpsZFunc]),
                                       GetIntValueFromElement(line[(int) NumericsSettingsColumns.DsStartMethod]),
                                       GetIntValueFromElement(line[(int) NumericsSettingsColumns.DsMinNumberOfIterations]),
                                       GetIntValueFromElement(line[(int) NumericsSettingsColumns.DsMaxNumberOfIterations]),
                                       GetDoubleValueFromElement(line[(int) NumericsSettingsColumns.DsVarCoefficient]),
                                       GetDoubleValueFromElement(line[(int) NumericsSettingsColumns.NiUMin]),
                                       GetDoubleValueFromElement(line[(int) NumericsSettingsColumns.NiUMax]),
                                       GetIntValueFromElement(line[(int) NumericsSettingsColumns.NiNumberSteps]));
        }
    }
}