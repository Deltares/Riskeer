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
    /// Provider of <see cref="FailureMechanismSettings"/>.
    /// </summary>
    public class FailureMechanismSettingsProvider
    {
        private readonly IDictionary<HydraRingFailureMechanismType, FailureMechanismSettings> defaultFailureMechanismSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismSettingsProvider"/> class.
        /// </summary>
        public FailureMechanismSettingsProvider()
        {
            defaultFailureMechanismSettings = new Dictionary<HydraRingFailureMechanismType, FailureMechanismSettings>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel,
                    new FailureMechanismSettings
                    {
                        ValueMin = 0,
                        ValueMax = 50
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight,
                    new FailureMechanismSettings
                    {
                        ValueMin = 0,
                        ValueMax = 50
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod,
                    new FailureMechanismSettings
                    {
                        ValueMin = 0,
                        ValueMax = 50
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod,
                    new FailureMechanismSettings
                    {
                        ValueMin = 0,
                        ValueMax = 50
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant,
                    new FailureMechanismSettings
                    {
                        ValueMin = 0,
                        ValueMax = 50
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping,
                    new FailureMechanismSettings
                    {
                        ValueMin = double.NaN,
                        ValueMax = double.NaN
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesPiping,
                    new FailureMechanismSettings
                    {
                        ValueMin = double.NaN,
                        ValueMax = double.NaN
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping,
                    new FailureMechanismSettings
                    {
                        ValueMin = double.NaN,
                        ValueMax = double.NaN
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure,
                    new FailureMechanismSettings
                    {
                        ValueMin = double.NaN,
                        ValueMax = double.NaN
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure,
                    new FailureMechanismSettings
                    {
                        ValueMin = double.NaN,
                        ValueMax = double.NaN
                    }
                }
            };
        }

        /// <summary>
        /// Returns <see cref="FailureMechanismSettings"/> based on the provided <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="FailureMechanismSettings"/> for.</param>
        /// <returns>The <see cref="FailureMechanismSettings"/> corresponding to the provided <see cref="HydraRingFailureMechanismType"/>.</returns>
        public FailureMechanismSettings GetFailureMechanismSettings(HydraRingFailureMechanismType failureMechanismType)
        {
            return defaultFailureMechanismSettings[failureMechanismType];
        }
    }
}
