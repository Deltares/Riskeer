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
using Ringtoets.Common.Data.DikeProfiles;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.Common.Service.ValidationRules
{
    /// <summary>
    /// Validation rule to validate the <see cref="IUseBreakWater"/> properties.
    /// </summary>
    public class UseBreakWaterRule : ValidationRule
    {
        private readonly IUseBreakWater breakWater;

        /// <summary>
        /// Instantiates a <see cref="UseBreakWaterRule"/> to validate the properties of calculation inputs that implement 
        /// <see cref="IUseBreakWater"/>.
        /// </summary>
        /// <param name="breakWater">The <see cref="IUseBreakWater"/> that needs to be validated.</param>
        public UseBreakWaterRule(IUseBreakWater breakWater)
        {
            this.breakWater = breakWater;
        }

        public override IEnumerable<string> Validate()
        {
            var messages = new List<string>();

            if (breakWater.UseBreakWater && IsInValidNumber(breakWater.BreakWater.Height))
            {
                messages.Add(RingtoetsCommonServiceResources.Validation_Invalid_BreakWaterHeight_value);
            }

            return messages;
        }
    }
}