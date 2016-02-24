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
using Ringtoets.HydraRing.Calculation.Types;

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
                            1, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            11, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            14, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            16, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            3, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            4, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            5, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 4,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            102, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            103, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesPiping, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            311, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            313, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            314, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            421, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            422, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            423, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            422, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 1,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            424, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            425, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            426, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 1,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            427, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 1,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new Dictionary<int, SubMechanismSettings>
                    {
                        {
                            422, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 1,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            424, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 1,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            425, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            430, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            431, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 1,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            432, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 1,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            434, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
                        },
                        {
                            435, new SubMechanismSettings
                            {
                                CalculationTechniqueId = 1,
                                FormStartMethod = 4,
                                FormNumberOfIterations = 50,
                                FormRelaxationFactor = 0.15,
                                FormEpsBeta = 0.01,
                                FormEpsHOH = 0.01,
                                FormEpsZFunc = 0.01,
                                DsStartMethod = 2,
                                DsMinNumberOfIterations = 10000,
                                DsMaxNumberOfIterations = 20000,
                                DsVarCoefficient = 0.1,
                                NiUMin = -6.0,
                                NiUMax = 6.0,
                                NiNumberSteps = 25
                            }
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