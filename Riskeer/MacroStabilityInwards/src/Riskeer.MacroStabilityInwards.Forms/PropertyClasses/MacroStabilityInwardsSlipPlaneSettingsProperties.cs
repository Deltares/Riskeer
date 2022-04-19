// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of slip plane settings properties in <see cref="MacroStabilityInwardsInput"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsSlipPlaneSettingsProperties : ObjectProperties<MacroStabilityInwardsInput>
    {
        private const int createZonesPropertyIndex = 1;
        private const int zoningBoundariesDeterminationTypePropertyIndex = 2;
        private const int zoneBoundaryLeftPropertyIndex = 3;
        private const int zoneBoundaryRightPropertyIndex = 4;

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

        [DynamicReadOnly]
        [PropertyOrder(zoningBoundariesDeterminationTypePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.SlipPlaneSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ZoningBoundariesDeterminationType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ZoningBoundariesDeterminationType_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public MacroStabilityInwardsZoningBoundariesDeterminationType ZoningBoundariesDeterminationType
        {
            get
            {
                return data.ZoningBoundariesDeterminationType;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.ZoningBoundariesDeterminationType = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(zoneBoundaryLeftPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.SlipPlaneSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ZoneBoundaryLeft_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ZoneBoundaryLeft_Description))]
        public RoundedDouble ZoneBoundaryLeft
        {
            get
            {
                return data.ZoneBoundaryLeft;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.ZoneBoundaryLeft = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(zoneBoundaryRightPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.SlipPlaneSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ZoneBoundaryRight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ZoneBoundaryRight_Description))]
        public RoundedDouble ZoneBoundaryRight
        {
            get
            {
                return data.ZoneBoundaryRight;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.ZoneBoundaryRight = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == nameof(ZoningBoundariesDeterminationType))
            {
                return !CreateZones;
            }

            if (propertyName == nameof(ZoneBoundaryLeft) || propertyName == nameof(ZoneBoundaryRight))
            {
                return !CreateZones || ZoningBoundariesDeterminationType == MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}