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

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsStochasticSoilModel"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsStochasticSoilModelProperties : ObjectProperties<MacroStabilityInwardsStochasticSoilModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
        /// </summary>
        /// <param name="stochasticSoilModel">The stochastic soil model for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stochasticSoilModel"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsStochasticSoilModelProperties(MacroStabilityInwardsStochasticSoilModel stochasticSoilModel)
        {
            if (stochasticSoilModel == null)
            {
                throw new ArgumentNullException(nameof(stochasticSoilModel));
            }

            Data = stochasticSoilModel;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StochasticSoilModel_Name_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StochasticSoilModel_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(2)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StochasticSoilModel_Geometry_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StochasticSoilModel_Geometry_Description))]
        public Point2D[] Geometry
        {
            get
            {
                return GetGeometry();
            }
        }

        private Point2D[] GetGeometry()
        {
            return data.Geometry.ToArray();
        }

        [PropertyOrder(3)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StochasticSoilModel_StochasticSoilProfiles_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StochasticSoilModel_StochasticSoilProfiles_Description))]
        public MacroStabilityInwardsStochasticSoilProfileProperties[] MacroStabilityInwardsStochasticSoilProfiles
        {
            get
            {
                return data.StochasticSoilProfiles.Any()
                           ? GetMacroStabilityInwardsStochasticSoilProfiles()
                           : new MacroStabilityInwardsStochasticSoilProfileProperties[0];
            }
        }

        private MacroStabilityInwardsStochasticSoilProfileProperties[] GetMacroStabilityInwardsStochasticSoilProfiles()
        {
            return data.StochasticSoilProfiles.Select(ssp => new MacroStabilityInwardsStochasticSoilProfileProperties(ssp)).ToArray();
        }
    }
}