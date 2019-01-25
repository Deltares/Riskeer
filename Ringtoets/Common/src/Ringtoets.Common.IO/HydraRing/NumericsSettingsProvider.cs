// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.ComponentModel;
using Core.Common.Base.IO;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Defaults;
using Riskeer.HydraRing.Calculation.Data.Settings;
using Riskeer.HydraRing.Calculation.Providers;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// Provider of <see cref="NumericsSetting"/> by reading from the settings database or by resorting
    /// to defaults if no settings could be found.
    /// </summary>
    public class NumericsSettingsProvider : IDisposable
    {
        private readonly HydraRingSettingsDatabaseReader numericsSettingsReader;
        private IDictionary<HydraRingFailureMechanismType, IDictionary<int, NumericsSetting>> defaultNumericsSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="NumericsSettingsProvider"/> class.
        /// </summary>
        /// <param name="databaseFilePath">The full path to the database file to use when reading settings.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// <item>The opened database doesn't have the expected schema.</item>
        /// </list>
        /// </exception>
        public NumericsSettingsProvider(string databaseFilePath)
        {
            InitializeDefaultNumericsSettings();

            numericsSettingsReader = new HydraRingSettingsDatabaseReader(databaseFilePath);
        }

        /// <summary>
        /// Returns <see cref="NumericsSetting"/> based on the provided combination of failure mechanism type, sub mechanism id and location id.
        /// </summary>
        /// <param name="locationId">The location id to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <returns>A new <see cref="Dictionary{T, T}"/> where the key is the sub mechanism id, and the value is
        /// the <see cref="NumericsSetting"/> containing values corresponding to the provided failure mechanism type and location id.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="failureMechanismType"/> is not a valid
        /// <see cref="HydraRingFailureMechanismType"/> value.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public Dictionary<int, NumericsSetting> GetNumericsSettings(long locationId, HydraRingFailureMechanismType failureMechanismType)
        {
            FailureMechanismDefaults failureMechanismDefaults = new FailureMechanismDefaultsProvider().GetFailureMechanismDefaults(failureMechanismType);
            IEnumerable<int> subMechanismIds = failureMechanismDefaults.SubMechanismIds;
            int mechanismId = failureMechanismDefaults.MechanismId;

            var numericsSettings = new Dictionary<int, NumericsSetting>();

            foreach (int subMechanismId in subMechanismIds)
            {
                numericsSettings[subMechanismId] = numericsSettingsReader.ReadNumericsSetting(locationId, mechanismId, subMechanismId) ??
                                                   defaultNumericsSettings[failureMechanismType][subMechanismId];
            }

            return numericsSettings;
        }

        /// <summary>
        /// Returns <see cref="NumericsSetting"/> based on the provided location id.
        /// </summary>
        /// <param name="locationId">The location id to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <returns>A new <see cref="NumericsSetting"/> containing values corresponding to the provided location id.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public NumericsSetting GetNumericsSettingForPreprocessor(long locationId)
        {
            FailureMechanismDefaults failureMechanismDefaults = new FailureMechanismDefaultsProvider().GetFailureMechanismDefaults(HydraRingFailureMechanismType.AssessmentLevel);
            int mechanismId = failureMechanismDefaults.MechanismId;
            const int preprocessorSubMechanismId = 7;
            const int defaultSubMechanismId = 1;

            return numericsSettingsReader.ReadNumericsSetting(locationId, mechanismId, preprocessorSubMechanismId)
                   ?? numericsSettingsReader.ReadNumericsSetting(locationId, mechanismId, defaultSubMechanismId)
                   ?? defaultNumericsSettings[HydraRingFailureMechanismType.AssessmentLevel][defaultSubMechanismId];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                numericsSettingsReader.Dispose();
            }
        }

        private void InitializeDefaultNumericsSettings()
        {
            NumericsSetting numericsSettingForm = CreateDefaultNumericsSetting(1, 1); // Settings for a FORM calculation
            NumericsSetting numericsSettingFDir = CreateDefaultNumericsSetting(11, 4); // Settings for a hybrid calculation; FORM in first instance, DIRS in case of no convergence
            NumericsSetting numericsSettingDunes = CreateDefaultNumericsSetting(1, 4);
            var numericsSettingQVariant = new NumericsSetting(4, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 3000, 10000, 0.1, -6.0, 6.0, 25);

            defaultNumericsSettings = new Dictionary<HydraRingFailureMechanismType, IDictionary<int, NumericsSetting>>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new Dictionary<int, NumericsSetting>
                    {
                        {
                            1, numericsSettingFDir
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new Dictionary<int, NumericsSetting>
                    {
                        {
                            11, numericsSettingFDir
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new Dictionary<int, NumericsSetting>
                    {
                        {
                            14, numericsSettingFDir
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new Dictionary<int, NumericsSetting>
                    {
                        {
                            16, numericsSettingFDir
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<int, NumericsSetting>
                    {
                        {
                            5, numericsSettingQVariant
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikeHeight, GetOvertoppingDefaults(numericsSettingFDir)
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, GetOvertoppingDefaults(numericsSettingFDir)
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<int, NumericsSetting>
                    {
                        {
                            421, numericsSettingFDir
                        },
                        {
                            422, numericsSettingFDir
                        },
                        {
                            423, numericsSettingFDir
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new Dictionary<int, NumericsSetting>
                    {
                        {
                            422, numericsSettingForm
                        },
                        {
                            424, numericsSettingFDir
                        },
                        {
                            425, numericsSettingFDir
                        },
                        {
                            426, numericsSettingForm
                        },
                        {
                            427, numericsSettingForm
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new Dictionary<int, NumericsSetting>
                    {
                        {
                            422, numericsSettingForm
                        },
                        {
                            424, numericsSettingFDir
                        },
                        {
                            425, numericsSettingFDir
                        },
                        {
                            430, numericsSettingFDir
                        },
                        {
                            431, numericsSettingForm
                        },
                        {
                            432, numericsSettingForm
                        },
                        {
                            433, numericsSettingForm
                        },
                        {
                            434, numericsSettingFDir
                        },
                        {
                            435, numericsSettingFDir
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DunesBoundaryConditions, new Dictionary<int, NumericsSetting>
                    {
                        {
                            6, numericsSettingDunes
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.OvertoppingRate, GetOvertoppingDefaults(numericsSettingFDir)
                }
            };
        }

        private static NumericsSetting CreateDefaultNumericsSetting(int calculationTechniqueId, int formStartMethod)
        {
            return new NumericsSetting(calculationTechniqueId, formStartMethod, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6.0, 6.0, 25);
        }

        private static Dictionary<int, NumericsSetting> GetOvertoppingDefaults(NumericsSetting numericsSetting)
        {
            return new Dictionary<int, NumericsSetting>
            {
                {
                    102, numericsSetting
                },
                {
                    103, numericsSetting
                }
            };
        }
    }
}