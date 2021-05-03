﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.Properties;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of drainage properties in <see cref="MacroStabilityInwardsInput"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsDrainageProperties : ObjectProperties<MacroStabilityInwardsInput>
    {
        private const int drainageConstructionPresentPropertyIndex = 0;
        private const int xCoordinateDrainageConstructionPropertyIndex = 1;
        private const int zCoordinateDrainageConstructionPropertyIndex = 2;

        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsDrainageProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsDrainageProperties(MacroStabilityInwardsInput data, IObservablePropertyChangeHandler handler)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            this.data = data;
            propertyChangeHandler = handler;
        }

        [DynamicReadOnly]
        [PropertyOrder(drainageConstructionPresentPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DrainageConstruction_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DrainageConstructionPresent_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DrainageConstructionPresent_Description))]
        public bool DrainageConstructionPresent
        {
            get
            {
                return data.DrainageConstructionPresent;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.DrainageConstructionPresent = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(xCoordinateDrainageConstructionPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DrainageConstruction_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.XCoordinateDrainageConstruction_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.XCoordinateDrainageConstruction_Description))]
        public RoundedDouble XCoordinateDrainageConstruction
        {
            get
            {
                return data.XCoordinateDrainageConstruction;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.XCoordinateDrainageConstruction = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(zCoordinateDrainageConstructionPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DrainageConstruction_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ZCoordinateDrainageConstruction_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ZCoordinateDrainageConstruction_Description))]
        public RoundedDouble ZCoordinateDrainageConstruction
        {
            get
            {
                return data.ZCoordinateDrainageConstruction;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.ZCoordinateDrainageConstruction = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if ((propertyName == nameof(XCoordinateDrainageConstruction) || propertyName == nameof(ZCoordinateDrainageConstruction))
                && !DrainageConstructionPresent)
            {
                return true;
            }

            return data.DikeSoilScenario == MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay
                   || data.DikeSoilScenario == MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}