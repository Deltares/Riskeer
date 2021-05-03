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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.Helpers;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsSoilLayer1D"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MacroStabilityInwardsSoilLayer1DProperties : ObjectProperties<MacroStabilityInwardsSoilLayer1D>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer1DProperties"/>.
        /// </summary>
        /// <param name="soilLayer">The 1D soil layer for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilLayer1DProperties(MacroStabilityInwardsSoilLayer1D soilLayer)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            Data = soilLayer;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SoilLayer_Name_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SoilLayer_Name_Description))]
        public string Name
        {
            get
            {
                return SoilLayerDataHelper.GetValidName(data.Data.MaterialName);
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SoilLayer_TopLevel_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SoilLayer_TopLevel_Description))]
        public RoundedDouble TopLevel
        {
            get
            {
                return new RoundedDouble(2, data.Top);
            }
        }

        [PropertyOrder(3)]
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

        public override string ToString()
        {
            return Name;
        }
    }
}