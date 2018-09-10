// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
            const int notApplicableCorrelationLength = 999999;

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
                    HydraRingFailureMechanismType.DikeHeight, GetOvertoppingDefaults()
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, GetOvertoppingDefaults()
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<int, VariableDefaults>
                    {
                        {
                            58, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            59, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            60, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            61, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            62, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            94, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            95, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            96, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            97, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            103, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            104, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            105, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            106, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            107, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            108, new VariableDefaults(notApplicableCorrelationLength)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new Dictionary<int, VariableDefaults>
                    {
                        {
                            58, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            59, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            61, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            62, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            63, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            65, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            66, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            67, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            68, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            69, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            71, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            72, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            93, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            94, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            95, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            96, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            97, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            103, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            104, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            105, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            106, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            107, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            108, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            125, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            129, new VariableDefaults(notApplicableCorrelationLength)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new Dictionary<int, VariableDefaults>
                    {
                        {
                            43, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            58, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            60, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            61, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            63, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            65, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            66, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            67, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            80, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            81, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            82, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            83, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            84, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            85, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            86, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            87, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            88, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            89, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            90, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            91, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            92, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            93, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            94, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            95, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            96, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            97, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            103, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            104, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            105, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            106, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            108, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            125, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            130, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            131, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            132, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            133, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            134, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            135, new VariableDefaults(notApplicableCorrelationLength)
                        },
                        {
                            136, new VariableDefaults(notApplicableCorrelationLength)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DunesBoundaryConditions, new Dictionary<int, VariableDefaults>
                    {
                        {
                            26, new VariableDefaults(300)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.OvertoppingRate, GetOvertoppingDefaults()
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

        private static Dictionary<int, VariableDefaults> GetOvertoppingDefaults()
        {
            return new Dictionary<int, VariableDefaults>
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
            };
        }
    }
}