// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationTypeTest : EnumWithResourcesDisplayNameTestFixture<GrassCoverErosionOutwardsWaveConditionsCalculationType>
    {
        protected override IDictionary<GrassCoverErosionOutwardsWaveConditionsCalculationType, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<GrassCoverErosionOutwardsWaveConditionsCalculationType, int>
                {
                    {
                        GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp, 1
                    },
                    {
                        GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact, 2
                    },
                    {
                        GrassCoverErosionOutwardsWaveConditionsCalculationType.Both, 3
                    },
                    {
                    GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact, 4
                },
                {
                    GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact, 5
                },
                {
                    GrassCoverErosionOutwardsWaveConditionsCalculationType.All, 6
                }
                };
            }
        }

        protected override IDictionary<GrassCoverErosionOutwardsWaveConditionsCalculationType, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<GrassCoverErosionOutwardsWaveConditionsCalculationType, string>
                {
                    {
                        GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp, "Gras (golfoploop)"
                    },
                    {
                        GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact, "Gras (golfklap)"
                    },
                    {
                        GrassCoverErosionOutwardsWaveConditionsCalculationType.Both, "Gras (golfoploop en golfklap)"
                    },
                    {
                        GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact, "Gras (golfklap voor toets op maat)"
                    },
                    {
                        GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact, "Gras (golfoploop en golfklap voor toets op maat)"
                    },
                    {
                        GrassCoverErosionOutwardsWaveConditionsCalculationType.All, "Gras (golfoploop, golfklap en golfklap voor toets op maat)"
                    }
                };
            }
        }
    }
}