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
using Ringtoets.HydraRing.Calculation.Data.Defaults;

namespace Ringtoets.HydraRing.Calculation.Providers
{
    /// <summary>
    /// Provider of <see cref="VariableDefaults"/>.
    /// </summary>
    internal class VariableDefaultsProvider
    {
        private readonly IDictionary<HydraRingFailureMechanismType, IDictionary<int, VariableDefaults>> variableDefaults;

        /// <summary>
        /// Creates a new instance of the <see cref="VariableDefaultsProvider"/> class.
        /// </summary>
        public VariableDefaultsProvider()
        {
            variableDefaults = new Dictionary<HydraRingFailureMechanismType, IDictionary<int, VariableDefaults>>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new Dictionary<int, VariableDefaults>
                    {
                        {
                            26, new VariableDefaults(300)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new Dictionary<int, VariableDefaults>
                    {
                        {
                            28, new VariableDefaults(300)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new Dictionary<int, VariableDefaults>
                    {
                        {
                            29, new VariableDefaults(300)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new Dictionary<int, VariableDefaults>
                    {
                        {
                            29, new VariableDefaults(300)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<int, VariableDefaults>
                    {
                        {
                            113, new VariableDefaults(300)
                        },
                        {
                            114, new VariableDefaults(300)
                        },
                        {
                            115, new VariableDefaults(300)
                        },
                        {
                            116, new VariableDefaults(300)
                        },
                        {
                            117, new VariableDefaults(300)
                        },
                        {
                            118, new VariableDefaults(300)
                        },
                        {
                            119, new VariableDefaults(300)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new Dictionary<int, VariableDefaults>
                    {
                        {
                            1, new VariableDefaults(300)
                        },
                        {
                            8, new VariableDefaults(300)
                        },
                        {
                            10, new VariableDefaults(300)
                        },
                        {
                            11, new VariableDefaults(300)
                        },
                        {
                            12, new VariableDefaults(300)
                        },
                        {
                            17, new VariableDefaults(300)
                        },
                        {
                            120, new VariableDefaults(300)
                        },
                        {
                            123, new VariableDefaults(300)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesHeight, new Dictionary<int, VariableDefaults>
                    {
                        {
                            1, new VariableDefaults(300)
                        },
                        {
                            8, new VariableDefaults(300)
                        },
                        {
                            10, new VariableDefaults(300)
                        },
                        {
                            11, new VariableDefaults(300)
                        },
                        {
                            12, new VariableDefaults(300)
                        },
                        {
                            17, new VariableDefaults(300)
                        },
                        {
                            120, new VariableDefaults(300)
                        },
                        {
                            123, new VariableDefaults(300)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesPiping, new Dictionary<int, VariableDefaults>
                    {
                        {
                            23, new VariableDefaults(6000)
                        },
                        {
                            42, new VariableDefaults(999999)
                        },
                        {
                            43, new VariableDefaults(999999)
                        },
                        {
                            44, new VariableDefaults(200)
                        },
                        {
                            45, new VariableDefaults(300)
                        },
                        {
                            46, new VariableDefaults(999999)
                        },
                        {
                            47, new VariableDefaults(999999)
                        },
                        {
                            48, new VariableDefaults(3000)
                        },
                        {
                            49, new VariableDefaults(200)
                        },
                        {
                            50, new VariableDefaults(300)
                        },
                        {
                            51, new VariableDefaults(999999)
                        },
                        {
                            52, new VariableDefaults(600)
                        },
                        {
                            53, new VariableDefaults(999999)
                        },
                        {
                            54, new VariableDefaults(99000)
                        },
                        {
                            55, new VariableDefaults(600)
                        },
                        {
                            56, new VariableDefaults(180)
                        },
                        {
                            58, new VariableDefaults(99000)
                        },
                        {
                            124, new VariableDefaults(300)
                        },
                        {
                            127, new VariableDefaults(999999)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<int, VariableDefaults>
                    {
                        {
                            58, new VariableDefaults(99000)
                        },
                        {
                            59, new VariableDefaults(999999)
                        },
                        {
                            60, new VariableDefaults(999999)
                        },
                        {
                            61, new VariableDefaults(999999)
                        },
                        {
                            62, new VariableDefaults(999999)
                        },
                        {
                            94, new VariableDefaults(999999)
                        },
                        {
                            95, new VariableDefaults(999999)
                        },
                        {
                            96, new VariableDefaults(999999)
                        },
                        {
                            97, new VariableDefaults(999999)
                        },
                        {
                            103, new VariableDefaults(999999)
                        },
                        {
                            104, new VariableDefaults(999999)
                        },
                        {
                            105, new VariableDefaults(999999)
                        },
                        {
                            106, new VariableDefaults(999999)
                        },
                        {
                            107, new VariableDefaults(99000)
                        },
                        {
                            108, new VariableDefaults(999999)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new Dictionary<int, VariableDefaults>
                    {
                        {
                            58, new VariableDefaults(99000)
                        },
                        {
                            59, new VariableDefaults(999999)
                        },
                        {
                            61, new VariableDefaults(999999)
                        },
                        {
                            62, new VariableDefaults(999999)
                        },
                        {
                            63, new VariableDefaults(999999)
                        },
                        {
                            64, new VariableDefaults(999999)
                        },
                        {
                            65, new VariableDefaults(999999)
                        },
                        {
                            66, new VariableDefaults(999999)
                        },
                        {
                            67, new VariableDefaults(999999)
                        },
                        {
                            68, new VariableDefaults(999999)
                        },
                        {
                            69, new VariableDefaults(999999)
                        },
                        {
                            71, new VariableDefaults(999999)
                        },
                        {
                            72, new VariableDefaults(999999)
                        },
                        {
                            93, new VariableDefaults(999999)
                        },
                        {
                            94, new VariableDefaults(999999)
                        },
                        {
                            95, new VariableDefaults(999999)
                        },
                        {
                            96, new VariableDefaults(999999)
                        },
                        {
                            97, new VariableDefaults(999999)
                        },
                        {
                            103, new VariableDefaults(999999)
                        },
                        {
                            104, new VariableDefaults(999999)
                        },
                        {
                            105, new VariableDefaults(999999)
                        },
                        {
                            106, new VariableDefaults(999999)
                        },
                        {
                            107, new VariableDefaults(999999)
                        },
                        {
                            108, new VariableDefaults(999999)
                        },
                        {
                            129, new VariableDefaults(50)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new Dictionary<int, VariableDefaults>
                    {
                        {
                            43, new VariableDefaults(99000)
                        },
                        {
                            58, new VariableDefaults(99000)
                        },
                        {
                            60, new VariableDefaults(50)
                        },
                        {
                            61, new VariableDefaults(99000)
                        },
                        {
                            62, new VariableDefaults(50)
                        },
                        {
                            63, new VariableDefaults(50)
                        },
                        {
                            64, new VariableDefaults(50)
                        },
                        {
                            65, new VariableDefaults(50)
                        },
                        {
                            66, new VariableDefaults(50)
                        },
                        {
                            67, new VariableDefaults(50)
                        },
                        {
                            80, new VariableDefaults(50)
                        },
                        {
                            81, new VariableDefaults(50)
                        },
                        {
                            82, new VariableDefaults(50)
                        },
                        {
                            83, new VariableDefaults(50)
                        },
                        {
                            84, new VariableDefaults(50)
                        },
                        {
                            85, new VariableDefaults(50)
                        },
                        {
                            86, new VariableDefaults(50)
                        },
                        {
                            87, new VariableDefaults(50)
                        },
                        {
                            88, new VariableDefaults(50)
                        },
                        {
                            89, new VariableDefaults(50)
                        },
                        {
                            90, new VariableDefaults(50)
                        },
                        {
                            91, new VariableDefaults(50)
                        },
                        {
                            92, new VariableDefaults(50)
                        },
                        {
                            93, new VariableDefaults(50)
                        },
                        {
                            94, new VariableDefaults(50)
                        },
                        {
                            95, new VariableDefaults(50)
                        },
                        {
                            96, new VariableDefaults(50)
                        },
                        {
                            97, new VariableDefaults(50)
                        },
                        {
                            103, new VariableDefaults(50)
                        },
                        {
                            104, new VariableDefaults(50)
                        },
                        {
                            105, new VariableDefaults(50)
                        },
                        {
                            106, new VariableDefaults(50)
                        },
                        {
                            108, new VariableDefaults(99000)
                        },
                        {
                            130, new VariableDefaults(6000)
                        },
                        {
                            131, new VariableDefaults(50)
                        },
                        {
                            132, new VariableDefaults(50)
                        },
                        {
                            133, new VariableDefaults(50)
                        },
                        {
                            134, new VariableDefaults(50)
                        },
                        {
                            135, new VariableDefaults(99000)
                        },
                        {
                            136, new VariableDefaults(99000)
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Returns <see cref="VariableDefaults"/> based on the provided <see cref="HydraRingFailureMechanismType"/> and variable id.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="VariableDefaults"/> for.</param>
        /// <param name="variableId">The variable id to obtain the <see cref="VariableDefaults"/> for.</param>
        /// <returns>The <see cref="VariableDefaults"/> corresponding to the provided <see cref="HydraRingFailureMechanismType"/> and variable id.</returns>
        public VariableDefaults GetVariableDefaults(HydraRingFailureMechanismType failureMechanismType, int variableId)
        {
            return variableDefaults[failureMechanismType][variableId];
        }
    }
}