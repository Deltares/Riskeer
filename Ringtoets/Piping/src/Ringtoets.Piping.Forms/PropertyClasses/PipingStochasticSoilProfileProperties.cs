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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingStochasticSoilProfile"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfileProperties_DisplayName))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PipingStochasticSoilProfileProperties : ObjectProperties<PipingStochasticSoilProfile>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilProfileProperties"/>.
        /// </summary>
        /// <param name="stochasticSoilProfile">The stochastic soil profile for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stochasticSoilProfile"/>
        /// is <c>null</c>.</exception>
        public PipingStochasticSoilProfileProperties(PipingStochasticSoilProfile stochasticSoilProfile)
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
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Layers_DisplayName))]
        public PipingSoilLayerProperties[] Layers
        {
            get
            {
                IEnumerable<PipingSoilLayer> pipingSoilLayers = data.SoilProfile.Layers;
                return pipingSoilLayers.Select(layer => new PipingSoilLayerProperties(layer)).ToArray();
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Bottom_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Bottom_Description))]
        public RoundedDouble Bottom
        {
            get
            {
                return new RoundedDouble(2, data.SoilProfile.Bottom);
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Type_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfile_Type_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public SoilProfileType Type
        {
            get
            {
                return data.SoilProfile.SoilProfileSourceType;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}