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
using Core.Common.IO.Exceptions;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.Providers;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// Provider of <see cref="NumericsSetting"/>.
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
        /// </list>
        /// </exception>
        public NumericsSettingsProvider(string databaseFilePath)
        {
            InitializeDefaultNumericsSettings();

            numericsSettingsReader = new HydraRingSettingsDatabaseReader(databaseFilePath);
        }

        /// <summary>
        /// Returns <see cref="NumericsSetting"/> based on the provided combination of failure mechanism type, sub mechanism id and ring id.
        /// </summary>
        /// <param name="locationId">The location id to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <returns>A new <see cref="Dictionary{T, T}"/> where the key is the submechanism id, and the value is
        /// the <see cref="NumericsSetting"/> containing values corresponding to the provided failure mechanism type and location id.</returns>
        public Dictionary<long, NumericsSetting> GetNumericsSettings(long locationId, HydraRingFailureMechanismType failureMechanismType)
        {
            var failureMechanismDefaults = new FailureMechanismDefaultsProvider().GetFailureMechanismDefaults(failureMechanismType);
            var subMechanismIds = failureMechanismDefaults.SubMechanismIds;
            var mechanismId = failureMechanismDefaults.MechanismId;

            var numericsSettings = new Dictionary<long, NumericsSetting>();

            foreach (var subMechanismId in subMechanismIds)
            {
                numericsSettings[subMechanismId] = numericsSettingsReader.ReadNumericsSetting(locationId, mechanismId, subMechanismId) ??
                                                   defaultNumericsSettings[failureMechanismType][subMechanismId];
            }

            return numericsSettings;
        }

        public void Dispose()
        {
            numericsSettingsReader.Dispose();
        }

        private void InitializeDefaultNumericsSettings()
        {
            defaultNumericsSettings = new Dictionary<HydraRingFailureMechanismType, IDictionary<int, NumericsSetting>>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new Dictionary<int, NumericsSetting>
                    {
                        {
                            1, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new Dictionary<int, NumericsSetting>
                    {
                        {
                            11, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new Dictionary<int, NumericsSetting>
                    {
                        {
                            14, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new Dictionary<int, NumericsSetting>
                    {
                        {
                            16, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<int, NumericsSetting>
                    {
                        {
                            5, new NumericsSetting(4, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new Dictionary<int, NumericsSetting>
                    {
                        {
                            102, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            103, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesHeight, new Dictionary<int, NumericsSetting>
                    {
                        {
                            102, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            103, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesPiping, new Dictionary<int, NumericsSetting>
                    {
                        {
                            311, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            313, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            314, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<int, NumericsSetting>
                    {
                        {
                            421, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            422, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            423, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new Dictionary<int, NumericsSetting>
                    {
                        {
                            422, new NumericsSetting(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            424, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            425, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            426, new NumericsSetting(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            427, new NumericsSetting(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new Dictionary<int, NumericsSetting>
                    {
                        {
                            422, new NumericsSetting(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            424, new NumericsSetting(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            425, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            430, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            431, new NumericsSetting(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            432, new NumericsSetting(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            433, new NumericsSetting(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            434, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            435, new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                }
            };
        }
    }
}