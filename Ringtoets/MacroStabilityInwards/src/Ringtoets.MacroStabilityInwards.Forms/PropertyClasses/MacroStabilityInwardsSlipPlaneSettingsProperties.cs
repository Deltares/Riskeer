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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of slip plane settings properties in <see cref="MacroStabilityInwardsInputContext"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsSlipPlaneSettingsProperties : ObjectProperties<MacroStabilityInwardsInput>
    {
        private const int createZonesPropertyIndex = 1;
        private const int automaticForbiddenZonesPropertyIndex = 2;

        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsGridSettingsProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsSlipPlaneSettingsProperties(MacroStabilityInwardsInput data, IObservablePropertyChangeHandler handler)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Data = data;
            propertyChangeHandler = handler;
        }

        [PropertyOrder(createZonesPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.SlipPlaneSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CreateZones_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CreateZones_Description))]
        public bool CreateZones
        {
            get
            {
                return data.CreateZones;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.CreateZones = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(automaticForbiddenZonesPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.SlipPlaneSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AutomaticForbiddenZones_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.AutomaticForbiddenZones_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public MacroStabilityInwardsZoningBoundariesDetermination AutomaticForbiddenZones
        {
            get
            {
                return data.AutomaticForbiddenZones;
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}