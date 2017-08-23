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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
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
    public class StochasticSoilProfileProperties : ObjectProperties<MacroStabilityInwardsStochasticSoilProfile>
    {
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilProfile_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilProfile_Name_Description))]
        public string Name
        {
            get
            {
                return data.SoilProfile != null ? data.SoilProfile.Name : string.Empty;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilProfile_Probability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilProfile_Probability_Description))]
        public string Probability
        {
            get
            {
                return new RoundedDouble(3, data.Probability * 100).Value.ToString(CultureInfo.CurrentCulture);
            }
        }

        [PropertyOrder(3)]
        [DynamicVisible]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilProfile_Layers_DisplayName))]
        public MacroStabilityInwardsSoilLayer1DProperties[] Layers1D
        {
            get
            {
                IEnumerable<MacroStabilityInwardsSoilLayer1D> macroStabilityInwardsSoilLayers1D = (data.SoilProfile as MacroStabilityInwardsSoilProfile1D)?.Layers;
                if (macroStabilityInwardsSoilLayers1D != null)
                {
                    return macroStabilityInwardsSoilLayers1D.Select(layer => new MacroStabilityInwardsSoilLayer1DProperties
                    {
                        Data = layer
                    }).ToArray();
                }
                return new MacroStabilityInwardsSoilLayer1DProperties[0];
            }
        }

        [PropertyOrder(4)]
        [DynamicVisible]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilProfile_Layers_DisplayName))]
        public MacroStabilityInwardsSoilLayer2DProperties[] Layers2D
        {
            get
            {
                IEnumerable<MacroStabilityInwardsSoilLayer2D> macroStabilityInwardsSoilLayers2D = (data.SoilProfile as MacroStabilityInwardsSoilProfile2D)?.Layers;
                if (macroStabilityInwardsSoilLayers2D != null)
                {
                    return macroStabilityInwardsSoilLayers2D.Select(layer => new MacroStabilityInwardsSoilLayer2DProperties
                    {
                        Data = layer
                    }).ToArray();
                }
                return new MacroStabilityInwardsSoilLayer2DProperties[0];
            }
        }

        [PropertyOrder(5)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilProfile_Bottom_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilProfile_Bottom_Description))]
        public string Bottom
        {
            get
            {
                return new RoundedDouble(2, (data.SoilProfile as MacroStabilityInwardsSoilProfile1D)?.Bottom ?? double.NaN).Value.ToString(CultureInfo.CurrentCulture);
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilProfile_Type_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilProfile_Type_Description))]
        public string Type
        {
            get
            {
                if (data.SoilProfile is MacroStabilityInwardsSoilProfile1D)
                {
                    return "1D profiel";
                }
                if (data.SoilProfile is MacroStabilityInwardsSoilProfile2D)
                {
                    return "2D profiel";
                }

                // If type is not supported, throw exception (currently not possible, safeguard for future)
                throw new NotSupportedException($"Type of {nameof(data.SoilProfile)} is not supported. Supported types: {nameof(MacroStabilityInwardsSoilProfile1D)} and {nameof(MacroStabilityInwardsSoilProfile2D)}");
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName.Equals(nameof(Bottom)) ||
                propertyName.Equals(nameof(Layers1D)))
            {
                return data.SoilProfile is MacroStabilityInwardsSoilProfile1D;
            }

            if (propertyName.Equals(nameof(Layers2D)))
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