﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Linq;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Riskeer.MacroStabilityInwards.Forms.Properties;
using Riskeer.MacroStabilityInwards.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a top level <see cref="MacroStabilityInwardsSoilLayer2D"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MacroStabilityInwardsSoilLayer2DTopLevelProperties : MacroStabilityInwardsSoilLayer2DBaseProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer2DTopLevelProperties"/> for
        /// the top level properties of a <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="soilLayer">The 2D soil layer for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilLayer2DTopLevelProperties(MacroStabilityInwardsSoilLayer2D soilLayer) : base(soilLayer) {}

        [PropertyOrder(3)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SoilLayer_NestedLayers_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SoilLayer_NestedLayers_Description))]
        public MacroStabilityInwardsSoilLayer2DBaseProperties[] NestedLayers
        {
            get
            {
                return GetNestedLayers();
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SoilLayer_IsAquifer_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SoilLayer_IsAquifer_Description))]
        public bool IsAquifer
        {
            get
            {
                return data.Data.IsAquifer;
            }
        }

        private MacroStabilityInwardsSoilLayer2DBaseProperties[] GetNestedLayers()
        {
            return data.NestedLayers.Select(nestedLayer => new MacroStabilityInwardsSoilLayer2DBaseProperties(nestedLayer)).ToArray();
        }
    }
}