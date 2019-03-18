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
using Riskeer.GrassCoverErosionOutwards.IO.Configurations;

namespace Riskeer.GrassCoverErosionOutwards.IO.Test.Configurations
{
    [TestFixture]
    public class ConfigurationGrassCoverErosionOutwardsCalculationTypeTest : EnumValuesTestFixture<ConfigurationGrassCoverErosionOutwardsCalculationType, int>
    {
        protected override IDictionary<ConfigurationGrassCoverErosionOutwardsCalculationType, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ConfigurationGrassCoverErosionOutwardsCalculationType, int>
                {
                    {
                        ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUp, 1
                    },
                    {
                        ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact, 2
                    },
                    {
                        ConfigurationGrassCoverErosionOutwardsCalculationType.Both,3
                    }
                };
            }
        }
    }
}