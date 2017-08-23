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

using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsSoilLayer2D"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StochasticSoilProfileProperties_DisplayName))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MacroStabilityInwardsSoilLayer2DProperties : ObjectProperties<MacroStabilityInwardsSoilLayer2D>
    {
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilProfile_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilProfile_Name_Description))]
        public string Name
        {
            get
            {
                return data.Properties.MaterialName;
            }
        }

        [PropertyOrder(2)]
        [ReadOnly(true)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SoilLayer_OuterRing_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SoilLayer_OuterRing_Description))]

        public Point2D[] OuterRing
        {
            get
            {
                return data.OuterRing.Points.ToArray();
            }
        }

        [PropertyOrder(3)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SoilLayer_Holes_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SoilLayer_Holes_Description))]
        public RingProperties[] Holes
        {
            get
            {
                return data.Holes.Select(ring => new RingProperties
                           {
                               Data = ring
                           })
                           .ToArray();
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SoilLayer_IsAquifer_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SoilLayer_IsAquifer_Description))]
        public bool IsAquifer
        {
            get
            {
                return data.Properties.IsAquifer;
            }
        }

        public override string ToString()
        {
            return data.Properties.MaterialName;
        }
    }
}