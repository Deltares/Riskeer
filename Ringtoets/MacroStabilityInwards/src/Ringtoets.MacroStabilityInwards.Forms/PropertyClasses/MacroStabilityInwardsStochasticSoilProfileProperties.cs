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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsStochasticSoilProfile"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfileProperties_DisplayName))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MacroStabilityInwardsStochasticSoilProfileProperties : ObjectProperties<MacroStabilityInwardsStochasticSoilProfile>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="stochasticSoilProfile">The stochastic soil profile for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stochasticSoilProfile"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsStochasticSoilProfileProperties(MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile)
        {
            if (stochasticSoilProfile == null)
            {
                throw new ArgumentNullException(nameof(stochasticSoilProfile));
            }

            Data = stochasticSoilProfile;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Name_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Name_Description))]
        public string Name
        {
            get
            {
                return data.SoilProfile.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Probability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Probability_Description))]
        public RoundedDouble Probability
        {
            get
            {
                return new RoundedDouble(2, data.Probability * 100);
            }
        }

        [PropertyOrder(3)]
        [DynamicVisible]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Layers_DisplayName))]
        public MacroStabilityInwardsSoilLayer1DProperties[] Layers1D
        {
            get
            {
                IEnumerable<MacroStabilityInwardsSoilLayer1D> macroStabilityInwardsSoilLayers1D = (data.SoilProfile as MacroStabilityInwardsSoilProfile1D)?.Layers;
                return macroStabilityInwardsSoilLayers1D?.Select(layer => new MacroStabilityInwardsSoilLayer1DProperties(layer)).ToArray() ??
                       new MacroStabilityInwardsSoilLayer1DProperties[0];
            }
        }

        [PropertyOrder(4)]
        [DynamicVisible]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Layers_DisplayName))]
        public MacroStabilityInwardsSoilLayer2DTopLevelProperties[] Layers2D
        {
            get
            {
                IEnumerable<MacroStabilityInwardsSoilLayer2D> layers = (data.SoilProfile as MacroStabilityInwardsSoilProfile2D)?.Layers;
                IEnumerable<MacroStabilityInwardsSoilLayer2D> macroStabilityInwardsSoilLayers2D = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(layers);
                return macroStabilityInwardsSoilLayers2D.Select(layer => new MacroStabilityInwardsSoilLayer2DTopLevelProperties(layer)).ToArray();
            }
        }

        [PropertyOrder(5)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Bottom_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Bottom_Description))]
        public RoundedDouble Bottom
        {
            get
            {
                double bottomValue = (data.SoilProfile as MacroStabilityInwardsSoilProfile1D)?.Bottom ?? double.NaN;
                return new RoundedDouble(2, bottomValue);
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Type_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Type_Description))]
        public string Type
        {
            get
            {
                if (data.SoilProfile is MacroStabilityInwardsSoilProfile1D)
                {
                    return RingtoetsCommonFormsResources.StochasticSoilProfile_Type_1D;
                }

                if (data.SoilProfile is MacroStabilityInwardsSoilProfile2D)
                {
                    return RingtoetsCommonFormsResources.StochasticSoilProfile_Type_2D;
                }

                string exceptionMessage = $"{data.SoilProfile.GetType()} is not supported." +
                                          $" Supported types: {nameof(MacroStabilityInwardsSoilProfile1D)} and {nameof(MacroStabilityInwardsSoilProfile2D)}.";
                throw new NotSupportedException(exceptionMessage);
            }
        }

        [PropertyOrder(7)]
        [DynamicVisible]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PreconsolidationStresses_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PreconsolidationStresses_Description))]
        public MacroStabilityInwardsPreconsolidationStressProperties[] PreconsolidationStresses
        {
            get
            {
                IEnumerable<MacroStabilityInwardsPreconsolidationStress> preconsolidationStresses =
                    (data.SoilProfile as MacroStabilityInwardsSoilProfile2D)?.PreconsolidationStresses;
                return preconsolidationStresses?.Select(stress => new MacroStabilityInwardsPreconsolidationStressProperties(stress)).ToArray() ??
                       new MacroStabilityInwardsPreconsolidationStressProperties[0];
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName.Equals(nameof(Bottom))
                || propertyName.Equals(nameof(Layers1D)))
            {
                return data.SoilProfile is MacroStabilityInwardsSoilProfile1D;
            }

            if (propertyName.Equals(nameof(Layers2D))
                || propertyName.Equals(nameof(PreconsolidationStresses)))
            {
                return data.SoilProfile is MacroStabilityInwardsSoilProfile2D;
            }

            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}