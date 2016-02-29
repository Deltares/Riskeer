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
using System.Globalization;
using System.Linq;
using Ringtoets.HydraRing.Calculation.Settings;

namespace Ringtoets.HydraRing.Calculation.IO
{
    /// <summary>
    /// The reader for the HydraRingSettings in csv format.
    /// </summary>
    public class HydraRingSettingsCsvReader
    {
        private const char separator = ';';

        private readonly string filePath;

        private readonly IDictionary<int, IDictionary<int, IDictionary<string, SubMechanismSettings>>> settings = new Dictionary<int, IDictionary<int, IDictionary<string, SubMechanismSettings>>>();

        private readonly Dictionary<string, int> columns = new Dictionary<string, int>
        {
            {ringIdKey, 0},
            {mechanismIdKey, 1},
            {subMechanismIdKey, 2},
            {calculationMethodKey, 3},
            {formStartMethodKey, 4},
            {formIterationsKey, 5},
            {formRelaxationFactorKey, 6},
            {formEpsBetaKey, 7},
            {formEpsHohKey, 8},
            {formEpsZFunc, 9},
            {dsStartMethod, 10},
            {dsMinNumberOfIterations, 11},
            {dsMaxNumberOfIterations, 12},
            {dsVarCoefficient, 13},
            {niUMin, 14},
            {niUMax, 15},
            {niNumberSteps, 16}
        };

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingSettingsCsvReader"/>.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="file"/> is not set.</exception>
        public HydraRingSettingsCsvReader(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException("file", "A file must be set.");
            }

            filePath = file;
        }

        /// <summary>
        /// Reads the settings from the file.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> with the settings.</returns>
        public IDictionary<int, IDictionary<int, IDictionary<string, SubMechanismSettings>>> ReadSettings()
        {
            string[] lines = filePath.Split('\n');

            foreach (string line in lines.Skip(1).Where(s => !string.IsNullOrEmpty(s)))
            {
                CreateSetting(TokenizeString(line));
            }

            return settings;
        }


        private void CreateSetting(string[] line)
        {
            // Get failure mechanism
            int failureMechanismType = GetFailureMechanismType(line);

            if (!settings.ContainsKey(failureMechanismType))
            {
                settings.Add(failureMechanismType, new Dictionary<int, IDictionary<string, SubMechanismSettings>>());
            }

            // Get sub mechanism
            int subMechanism = GetSubMechanismType(line);

            if (!settings[failureMechanismType].ContainsKey(subMechanism))
            {
                settings[failureMechanismType].Add(subMechanism, new Dictionary<string, SubMechanismSettings>());
            }

            // Get TrajectId
            string ringId = GetRingId(line);

            if (!settings[failureMechanismType][subMechanism].ContainsKey(ringId))
            {
                settings[failureMechanismType][subMechanism].Add(ringId, GetSubMechanismSetting(line));
            }
        }

        private int GetFailureMechanismType(IList<string> line)
        {
            return GetIntValueFromElement(line[columns[mechanismIdKey]]);
        }

        private int GetSubMechanismType(string[] line)
        {
            return GetIntValueFromElement(line[columns[subMechanismIdKey]]);
        }

        private string GetRingId(IList<string> line)
        {
            return line[columns[ringIdKey]].Trim();
        }

        private SubMechanismSettings GetSubMechanismSetting(IList<string> line)
        {
            return new SubMechanismSettings(GetIntValueFromElement(line[columns[calculationMethodKey]]),
                                            GetIntValueFromElement(line[columns[formStartMethodKey]]),
                                            GetIntValueFromElement(line[columns[formIterationsKey]]),
                                            GetDoubleValueFromElement(line[columns[formRelaxationFactorKey]]),
                                            GetDoubleValueFromElement(line[columns[formEpsBetaKey]]),
                                            GetDoubleValueFromElement(line[columns[formEpsHohKey]]),
                                            GetDoubleValueFromElement(line[columns[formEpsZFunc]]),
                                            GetIntValueFromElement(line[columns[dsStartMethod]]),
                                            GetIntValueFromElement(line[columns[dsMinNumberOfIterations]]),
                                            GetIntValueFromElement(line[columns[dsMaxNumberOfIterations]]),
                                            GetDoubleValueFromElement(line[columns[dsVarCoefficient]]),
                                            GetDoubleValueFromElement(line[columns[niUMin]]),
                                            GetDoubleValueFromElement(line[columns[niUMax]]),
                                            GetIntValueFromElement(line[columns[niNumberSteps]]));
        }

        private static int GetIntValueFromElement(string element)
        {
            return int.Parse(element.Trim());
        }

        private static double GetDoubleValueFromElement(string element)
        {
            return double.Parse(element.Trim(), CultureInfo.InvariantCulture);
        }

        private string[] TokenizeString(string readText)
        {
            if (!readText.Contains(separator))
            {
                return new string[]{};
            }
            return readText.Split(separator)
                           .TakeWhile(text => !string.IsNullOrEmpty(text))
                           .ToArray();
        }

        #region csv column names

        private const string ringIdKey = "TrajectID";
        private const string mechanismIdKey = "MechanismID";
        private const string subMechanismIdKey = "SubMechanismID";
        private const string calculationMethodKey = "Rekenmethode";
        private const string formStartMethodKey = "FORM_StartMethod";
        private const string formIterationsKey = "FORM_NrIterations";
        private const string formRelaxationFactorKey = "FORM_RelaxationFactor";
        private const string formEpsBetaKey = "FORM_EpsBeta";
        private const string formEpsHohKey = "FORM_EpsHOH";
        private const string formEpsZFunc = "FORM_EpsZFunc";
        private const string dsStartMethod = "Ds_StartMethod";
        private const string dsMinNumberOfIterations = "Ds_Min";
        private const string dsMaxNumberOfIterations = "Ds_Max";
        private const string dsVarCoefficient = "Ds_VarCoefficient";
        private const string niUMin = "NI_UMin";
        private const string niUMax = "NI_Umax";
        private const string niNumberSteps = "NI_NumberSteps";

        #endregion
    }
}
