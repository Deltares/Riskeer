﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
                calculationMethodKey, 3
            },
            {
                formStartMethodKey, 4
            },
            {
                formIterationsKey, 5
            },
            {
                formRelaxationFactorKey, 6
            },
            {
                formEpsBetaKey, 7
            },
            {
                formEpsHohKey, 8
            },
            {
                formEpsZFuncKey, 9
            },
            {
                dsStartMethodKey, 10
            },
            {
                dsMinNumberOfIterationsKey, 11
            },
            {
                dsMaxNumberOfIterationsKey, 12
            },
            {
                dsVarCoefficientKey, 13
            },
            {
                niUMinKey, 14
            },
            {
                niUMaxKey, 15
            },
            {
                niNumberStepsKey, 16
            }
        };

        /// <summary>
        /// Creates a new instance of <see cref="NumericsSettingsCsvReader"/>.
        /// </summary>
        /// <param name="fileContents">The fileContents to read.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileContents"/> is not set.</exception>
        public NumericsSettingsCsvReader(string fileContents) : base(fileContents, new Dictionary<int, IDictionary<int, IDictionary<string, NumericsSetting>>>()) {}

        protected override void CreateSetting(IList<string> line)
        {
            // Get failure mechanism
            var failureMechanismType = GetFailureMechanismType(line);

            if (!Settings.ContainsKey(failureMechanismType))
            {
                Settings.Add(failureMechanismType, new Dictionary<int, IDictionary<string, NumericsSetting>>());
            }

            // Get sub mechanism
            var subMechanism = GetSubMechanismType(line);

            if (!Settings[failureMechanismType].ContainsKey(subMechanism))
            {
                Settings[failureMechanismType].Add(subMechanism, new Dictionary<string, NumericsSetting>());
            }

            // Get TrajectId
            var ringId = GetRingId(line);

            if (!Settings[failureMechanismType][subMechanism].ContainsKey(ringId))
            {
                Settings[failureMechanismType][subMechanism].Add(ringId, GetNumericSettings(line));
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
            return GetStringValueFromElement(line[columns[ringIdKey]]);
        }

        private NumericsSetting GetNumericSettings(IList<string> line)
        {
            return new NumericsSetting(GetIntValueFromElement(line[columns[calculationMethodKey]]),
                                        GetIntValueFromElement(line[columns[formStartMethodKey]]),
                                        GetIntValueFromElement(line[columns[formIterationsKey]]),
                                        GetDoubleValueFromElement(line[columns[formRelaxationFactorKey]]),
                                        GetDoubleValueFromElement(line[columns[formEpsBetaKey]]),
                                        GetDoubleValueFromElement(line[columns[formEpsHohKey]]),
                                        GetDoubleValueFromElement(line[columns[formEpsZFuncKey]]),
                                        GetIntValueFromElement(line[columns[dsStartMethodKey]]),
                                        GetIntValueFromElement(line[columns[dsMinNumberOfIterationsKey]]),
                                        GetIntValueFromElement(line[columns[dsMaxNumberOfIterationsKey]]),
                                        GetDoubleValueFromElement(line[columns[dsVarCoefficientKey]]),
                                        GetDoubleValueFromElement(line[columns[niUMinKey]]),
                                        GetDoubleValueFromElement(line[columns[niUMaxKey]]),
                                        GetIntValueFromElement(line[columns[niNumberStepsKey]]));
        }

        #region Csv column names

        private const string ringIdKey = "TrajectID";
        private const string mechanismIdKey = "MechanismID";
        private const string subMechanismIdKey = "SubMechanismID";
        private const string calculationMethodKey = "Rekenmethode";
        private const string formStartMethodKey = "FORM_StartMethod";
        private const string formIterationsKey = "FORM_NrIterations";
        private const string formRelaxationFactorKey = "FORM_RelaxationFactor";
        private const string formEpsBetaKey = "FORM_EpsBeta";
        private const string formEpsHohKey = "FORM_EpsHOH";
        private const string formEpsZFuncKey = "FORM_EpsZFunc";
        private const string dsStartMethodKey = "Ds_StartMethod";
        private const string dsMinNumberOfIterationsKey = "Ds_Min";
        private const string dsMaxNumberOfIterationsKey = "Ds_Max";
        private const string dsVarCoefficientKey = "Ds_VarCoefficient";
        private const string niUMinKey = "NI_UMin";
        private const string niUMaxKey = "NI_Umax";
        private const string niNumberStepsKey = "NI_NumberSteps";

        #endregion
    }
}