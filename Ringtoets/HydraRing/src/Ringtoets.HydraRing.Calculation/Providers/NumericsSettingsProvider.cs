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

using System.Collections.Generic;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.IO;
using Ringtoets.HydraRing.Calculation.Properties;

namespace Ringtoets.HydraRing.Calculation.Providers
{
    /// <summary>
    /// Provider of <see cref="NumericsSetting"/>.
    /// </summary>
    internal class NumericsSettingsProvider
    {
        private readonly IDictionary<int, IDictionary<int, IDictionary<string, NumericsSetting>>> fileNumericsSettings;
        private IDictionary<HydraRingFailureMechanismType, IDictionary<int, NumericsSetting>> defaultNumericsSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="NumericsSettingsProvider"/> class.
        /// </summary>
        public NumericsSettingsProvider()
        {
            InitializeDefaultNumericsSettings();

            fileNumericsSettings = new NumericsSettingsCsvReader(Resources.NumericsSettings).ReadSettings();
        }

        /// <summary>
        /// Returns <see cref="NumericsSetting"/> based on the provided combination of failure mechanism type, sub mechanism id and ring id.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <param name="subMechanismId">The sub mechanism id to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <param name="ringId">The ring id to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <returns>The <see cref="NumericsSetting"/> corresponding to the provided failure mechanism type, sub mechanism id and ring id.</returns>
        public NumericsSetting GetNumericsSetting(HydraRingFailureMechanismType failureMechanismType, int subMechanismId, string ringId)
        {
            var mechanismId = new FailureMechanismDefaultsProvider().GetFailureMechanismDefaults(failureMechanismType).MechanismId;

            if (fileNumericsSettings.ContainsKey(mechanismId) &&
                fileNumericsSettings[mechanismId].ContainsKey(subMechanismId) &&
                ringId != null &&
                fileNumericsSettings[mechanismId][subMechanismId].ContainsKey(ringId))
            {
                return fileNumericsSettings[mechanismId][subMechanismId][ringId];
            }

            return defaultNumericsSettings[failureMechanismType][subMechanismId];
        }

        private void InitializeDefaultNumericsSettings()
        {
            var numericsSettingForm = new NumericsSetting(1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6.0, 6.0, 25);
            var numericsSettingDirs = new NumericsSetting(11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6.0, 6.0, 25);

            defaultNumericsSettings = new Dictionary<HydraRingFailureMechanismType, IDictionary<int, NumericsSetting>>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new Dictionary<int, NumericsSetting>
                    {
                        {
                            1, numericsSettingDirs
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new Dictionary<int, NumericsSetting>
                    {
                        {
                            11, numericsSettingDirs
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new Dictionary<int, NumericsSetting>
                    {
                        {
                            14, numericsSettingDirs
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new Dictionary<int, NumericsSetting>
                    {
                        {
                            16, numericsSettingDirs
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<int, NumericsSetting>
                    {
                        {
                            5, new NumericsSetting(4, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 3000, 10000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new Dictionary<int, NumericsSetting>
                    {
                        {
                            102, numericsSettingDirs
                        },
                        {
                            103, numericsSettingDirs
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesHeight, new Dictionary<int, NumericsSetting>
                    {
                        {
                            102, numericsSettingDirs
                        },
                        {
                            103, numericsSettingDirs
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesPiping, new Dictionary<int, NumericsSetting>
                    {
                        {
                            311, numericsSettingDirs
                        },
                        {
                            313, numericsSettingDirs
                        },
                        {
                            314, numericsSettingDirs
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<int, NumericsSetting>
                    {
                        {
                            421, numericsSettingDirs
                        },
                        {
                            422, numericsSettingDirs
                        },
                        {
                            423, numericsSettingDirs
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
                            424, numericsSettingDirs
                        },
                        {
                            425, numericsSettingDirs
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
                            424, numericsSettingDirs
                        },
                        {
                            425, numericsSettingDirs
                        },
                        {
                            430, numericsSettingDirs
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
                            434, numericsSettingDirs
                        },
                        {
                            435, numericsSettingDirs
                        }
                    }
                }
            };
        }
    }
}