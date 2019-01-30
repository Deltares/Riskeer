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

namespace Riskeer.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsInputStepSizeTest : EnumWithResourcesDisplayNameTestFixture<WaveConditionsInputStepSize>
    {
        protected override IDictionary<WaveConditionsInputStepSize, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<WaveConditionsInputStepSize, string>
                {
                    {
                        WaveConditionsInputStepSize.Half, "0.5"
                    },
                    {
                        WaveConditionsInputStepSize.One, "1.0"
                    },
                    {
                        WaveConditionsInputStepSize.Two, "2.0"
                    }
                };
            }
        }

        protected override IDictionary<WaveConditionsInputStepSize, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<WaveConditionsInputStepSize, int>
                {
                    {
                        WaveConditionsInputStepSize.Half, 1
                    },
                    {
                        WaveConditionsInputStepSize.One, 2
                    },
                    {
                        WaveConditionsInputStepSize.Two, 3
                    }
                };
            }
        }
    }
}