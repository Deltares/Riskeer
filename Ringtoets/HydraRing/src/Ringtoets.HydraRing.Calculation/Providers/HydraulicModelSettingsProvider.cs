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
    /// Provider of <see cref="HydraulicModelSettings"/>.
    /// </summary>
    internal class HydraulicModelSettingsProvider
    {
        private readonly IDictionary<int, IDictionary<int, IDictionary<string, HydraulicModelSettings>>> fileHydraulicModelSettings;
        private IDictionary<HydraRingFailureMechanismType, IDictionary<int, HydraulicModelSettings>> defaultHydraulicModelSettings;
        
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicModelSettingsProvider"/>.
        /// </summary>
        public HydraulicModelSettingsProvider()
        {
            InitializeDefaultHydraulicModelSettings();

            fileHydraulicModelSettings = new HydraulicModelSettingsCsvReader(Resources.HydraulicModelSettings).ReadSettings();
        }

        /// <summary>
        /// Returns <see cref="HydraulicModelSettings"/> based on the provided <see cref="HydraRingFailureMechanismType"/> and sub mechanism id.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="HydraulicModelSettings"/> for.</param>
        /// <param name="subMechanismId">The sub mechanism id to obtain the <see cref="HydraulicModelSettings"/> for.</param>
        /// <param name="ringId">The ring id to obtain the <see cref="HydraulicModelSettings"/> for.</param>
        /// <returns>The <see cref="HydraulicModelSettings"/> corresponding to the provided <see cref="HydraRingFailureMechanismType"/> and sub mechanism id.</returns>
        public HydraulicModelSettings GetHydraulicModelSettings(HydraRingFailureMechanismType failureMechanismType, int subMechanismId, string ringId)
        {
            var mechanismId = new FailureMechanismDefaultsProvider().GetFailureMechanismDefaults(failureMechanismType).MechanismId;

            if (fileHydraulicModelSettings.ContainsKey(mechanismId) &&
                fileHydraulicModelSettings[mechanismId].ContainsKey(subMechanismId) &&
                ringId != null &&
                fileHydraulicModelSettings[mechanismId][subMechanismId].ContainsKey(ringId))
            {
                return fileHydraulicModelSettings[mechanismId][subMechanismId][ringId];
            }

            return defaultHydraulicModelSettings[failureMechanismType][subMechanismId];
        }

        private void InitializeDefaultHydraulicModelSettings()
        {
            defaultHydraulicModelSettings = new Dictionary<HydraRingFailureMechanismType, IDictionary<int, HydraulicModelSettings>>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            1, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            11, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            14, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            16, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            3, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            4, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            5, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            102, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            103, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesHeight, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            102, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            103, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesPiping, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            311, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            313, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            314, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            421, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            422, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            423, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            422, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            424, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            425, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            426, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            427, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new Dictionary<int, HydraulicModelSettings>
                    {
                        {
                            422, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            424, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            425, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            430, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            431, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            432, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            433, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            434, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        },
                        {
                            435, new HydraulicModelSettings(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)
                        }
                    }
                }
            };
        }
    }
}