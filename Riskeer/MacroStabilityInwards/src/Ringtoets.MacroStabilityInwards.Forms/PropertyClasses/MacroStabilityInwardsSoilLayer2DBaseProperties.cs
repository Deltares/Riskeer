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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Helpers;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// Base ViewModel of <see cref="MacroStabilityInwardsSoilLayer2D"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MacroStabilityInwardsSoilLayer2DBaseProperties : ObjectProperties<MacroStabilityInwardsSoilLayer2D>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer2DBaseProperties"/>.
        /// </summary>
        /// <param name="soilLayer">The 2D soil layer for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilLayer2DBaseProperties(MacroStabilityInwardsSoilLayer2D soilLayer)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            Data = soilLayer;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.SoilLayer_Name_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.SoilLayer_Name_Description))]
        public string Name
        {
            get
            {
                return SoilLayerDataHelper.GetValidName(data.Data.MaterialName);
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SoilLayer_Geometry_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SoilLayer_Geometry_Description))]
        public Point2D[] Geometry
        {
            get
            {
                return data.OuterRing.Points.ToArray();
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}