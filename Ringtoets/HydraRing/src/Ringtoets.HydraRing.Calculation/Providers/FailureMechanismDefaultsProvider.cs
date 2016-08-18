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

using System.Collections.Generic;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Defaults;

namespace Ringtoets.HydraRing.Calculation.Providers
{
    /// <summary>
    /// Provider of <see cref="FailureMechanismDefaults"/>.
    /// </summary>
    internal class FailureMechanismDefaultsProvider
    {
        private readonly IDictionary<HydraRingFailureMechanismType, FailureMechanismDefaults> failureMechanismDefaults;

        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismDefaultsProvider"/> class.
        /// </summary>
        /// <remarks>
        /// The default settings should not be overruled and just reflect:
        /// - some supported ids within Hydra-Ring;
        /// - a WTI 2017 specific configuration of Hydra-Ring.
        /// </remarks>
        public FailureMechanismDefaultsProvider()
        {
            failureMechanismDefaults = new Dictionary<HydraRingFailureMechanismType, FailureMechanismDefaults>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new FailureMechanismDefaults(1, new[]
                    {
                        1
                    }, 1)
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new FailureMechanismDefaults(11, new[]
                    {
                        11
                    }, 11)
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new FailureMechanismDefaults(11, new[]
                    {
                        14
                    }, 14)
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new FailureMechanismDefaults(11, new[]
                    {
                        16
                    }, 16)
                },
                {
                    HydraRingFailureMechanismType.QVariant, new FailureMechanismDefaults(3, new[]
                    {
                        3,
                        4,
                        5
                    }, 7)
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new FailureMechanismDefaults(101, new[]
                    {
                        102,
                        103
                    }, 1017)
                },
                {
                    HydraRingFailureMechanismType.DikesHeight, new FailureMechanismDefaults(101, new[]
                    {
                        102,
                        103
                    }, 1017)
                },
                {
                    HydraRingFailureMechanismType.DikesPiping, new FailureMechanismDefaults(103, new[]
                    {
                        311,
                        313,
                        314
                    }, 3015)
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new FailureMechanismDefaults(110, new[]
                    {
                        421,
                        422,
                        423
                    }, 4404)
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new FailureMechanismDefaults(111, new[]
                    {
                        422,
                        424,
                        425,
                        426,
                        427
                    }, 4505)
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new FailureMechanismDefaults(112, new[]
                    {
                        422,
                        424,
                        425,
                        430,
                        431,
                        432,
                        433,
                        434,
                        435
                    }, 4607)
                }
            };
        }

        /// <summary>
        /// Returns <see cref="FailureMechanismDefaults"/> based on the provided <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="FailureMechanismDefaults"/> for.</param>
        /// <returns>The <see cref="FailureMechanismDefaults"/> corresponding to the provided <see cref="HydraRingFailureMechanismType"/>.</returns>
        public FailureMechanismDefaults GetFailureMechanismDefaults(HydraRingFailureMechanismType failureMechanismType)
        {
            return failureMechanismDefaults[failureMechanismType];
        }
    }
}