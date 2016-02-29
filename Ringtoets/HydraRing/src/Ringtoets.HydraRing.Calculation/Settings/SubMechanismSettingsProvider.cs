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

namespace Ringtoets.HydraRing.Calculation.Settings
{
    /// <summary>
    /// Provider of <see cref="SubMechanismSettings"/>.
    /// </summary>
    public class SubMechanismSettingsProvider
    {
        private readonly IDictionary<HydraRingFailureMechanismType, IDictionary<int, SubMechanismSettings>> defaultSubMechanismSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="SubMechanismSettingsProvider"/> class.
        /// </summary>
        public SubMechanismSettingsProvider()
        {
            defaultSubMechanismSettings = new Dictionary<HydraRingFailureMechanismType, IDictionary<int, SubMechanismSettings>>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            1, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            11, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            14, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            16, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            3, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            4, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            5, new SubMechanismSettings(4, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            102, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            103, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesPiping, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            311, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            313, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            314, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            421, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            422, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            423, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            422, new SubMechanismSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            424, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            425, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            426, new SubMechanismSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            427, new SubMechanismSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            422, new SubMechanismSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            424, new SubMechanismSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            425, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            430, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            431, new SubMechanismSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            432, new SubMechanismSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            433, new SubMechanismSettings(1, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            434, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        },
                        {
                            435, new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 10000, 20000, 0.1, -6.0, 6.0, 25)
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Returns <see cref="SubMechanismSettings"/> based on the provided <see cref="HydraRingFailureMechanismType"/> and sub mechanism id.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="FailureMechanismSettings"/> for.</param>
        /// <param name="subMechanismId">The sub mechanism id to obtain the <see cref="FailureMechanismSettings"/> for.</param>
        /// <returns>The <see cref="FailureMechanismSettings"/> corresponding to the provided <see cref="HydraRingFailureMechanismType"/> and sub mechanism id.</returns>
        public SubMechanismSettings GetSubMechanismSettings(HydraRingFailureMechanismType failureMechanismType, int subMechanismId)
        {
            return defaultSubMechanismSettings[failureMechanismType][subMechanismId];
        }
    }
}