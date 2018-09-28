// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of water stresses properties in <see cref="MacroStabilityInwardsLocationInputDaily"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsLocationInputDailyProperties : MacroStabilityInwardsLocationInputBaseProperties<MacroStabilityInwardsLocationInputDaily>
    {
        private const int penetrationLengthPropertyIndex = 3;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsLocationInputDailyProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsLocationInputDailyProperties(MacroStabilityInwardsLocationInputDaily data, IObservablePropertyChangeHandler handler)
            : base(data, handler) {}

        [PropertyOrder(penetrationLengthPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PenetrationLength_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PenetrationLength_Description))]
        public RoundedDouble PenetrationLength
        {
            get
            {
                return data.PenetrationLength;
            }
        }
    }
}