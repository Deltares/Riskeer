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
    /// Provider of <see cref="NumericsSettings"/>.
    /// </summary>
    internal class NumericsSettingsProvider
    {
        private readonly IDictionary<int, IDictionary<int, IDictionary<string, NumericsSettings>>> fileNumericsSettings;
        private IDictionary<HydraRingFailureMechanismType, IDictionary<int, NumericsSettings>> defaultNumericsSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="NumericsSettingsProvider"/> class.
        /// </summary>
        public NumericsSettingsProvider()
        {
            InitializeDefaultNumericsSettings();

            fileNumericsSettings = new NumericsSettingsCsvReader(Resources.NumericsSettings).ReadSettings();
        }

        /// <summary>
        /// Returns <see cref="NumericsSettings"/> based on the provided <see cref="HydraRingFailureMechanismType"/> and sub mechanism id.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="NumericsSettings"/> for.</param>
        /// <param name="subMechanismId">The sub mechanism id to obtain the <see cref="NumericsSettings"/> for.</param>
        /// <param name="ringId">The ring id to obtain the <see cref="NumericsSettings"/> for.</param>
        /// <returns>The <see cref="NumericsSettings"/> corresponding to the provided <see cref="HydraRingFailureMechanismType"/> and sub mechanism id.</returns>
        public NumericsSettings GetNumericsSettings(HydraRingFailureMechanismType failureMechanismType, int subMechanismId, string ringId)
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
            defaultNumericsSettings = new Dictionary<HydraRingFailureMechanismType, IDictionary<int, NumericsSettings>>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new Dictionary<int, NumericsSettings>
                    {
                        {
                            1, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new Dictionary<int, NumericsSettings>
                    {
                        {
                            11, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new Dictionary<int, NumericsSettings>
                    {
                        {
                            14, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new Dictionary<int, NumericsSettings>
                    {
                        {
                            16, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<int, NumericsSettings>
                    {
                        {
                            3, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            4, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            5, new NumericsSettings(4, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new Dictionary<int, NumericsSettings>
                    {
                        {
                            102, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            103, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesHeight, new Dictionary<int, NumericsSettings>
                    {
                        {
                            102, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            103, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesPiping, new Dictionary<int, NumericsSettings>
                    {
                        {
                            311, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            313, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            314, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<int, NumericsSettings>
                    {
                        {
                            421, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            422, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            423, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new Dictionary<int, NumericsSettings>
                    {
                        {
                            422, new NumericsSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            424, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            425, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            426, new NumericsSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            427, new NumericsSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new Dictionary<int, NumericsSettings>
                    {
                        {
                            422, new NumericsSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            424, new NumericsSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            425, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            430, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            431, new NumericsSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            432, new NumericsSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            433, new NumericsSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            434, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            435, new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                }
            };
        }
    }
}