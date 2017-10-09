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

using System;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of water stresses properties in <see cref="MacroStabilityInwardsLocationInputExtreme"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsLocationExtremeProperties : MacroStabilityInwardsLocationProperties<MacroStabilityInwardsLocationInputExtreme>
    {
        private const int penetrationLengthPropertyIndex = 3;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsLocationExtremeProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsLocationExtremeProperties(MacroStabilityInwardsLocationInputExtreme data, IObservablePropertyChangeHandler handler) 
            : base(data, handler) {}

        [ResourcesDescription(typeof(Resources), nameof(Resources.WaterLevelPolder_Extreme_Description))]
        public override RoundedDouble WaterLevelPolder
        {
            get
            {
                return base.WaterLevelPolder;
            }
            set
            {
                base.WaterLevelPolder = value;
            }
        }

        [PropertyOrder(penetrationLengthPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PenetrationLength_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PenetrationLength_Extreme_Description))]
        public RoundedDouble PenetrationLength
        {
            get
            {
                return data.PenetrationLength;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.PenetrationLength = value, PropertyChangeHandler);
            }
        }
    }
}