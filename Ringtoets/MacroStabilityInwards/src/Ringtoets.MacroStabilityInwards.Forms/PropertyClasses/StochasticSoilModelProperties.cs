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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StochasticSoilModel"/> for properties panel.
    /// </summary>
    public class StochasticSoilModelProperties : ObjectProperties<StochasticSoilModel>
    {
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilModel_Id_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilModel_Id_Description))]
        public long Id
        {
            get
            {
                return data.Id;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilModel_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilModel_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilModel_SegmentName_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilModel_SegmentName_Description))]
        public string SegmentName
        {
            get
            {
                return data.SegmentName;
            }
        }

        [PropertyOrder(4)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilModel_Geometry_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilModel_Geometry_Description))]
        public Point2D[] Geometry
        {
            get
            {
                return data.Geometry.ToArray();
            }
        }

        [PropertyOrder(5)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StochasticSoilModel_StochasticSoilProfiles_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StochasticSoilModel_StochasticSoilProfiles_Description))]
        public StochasticSoilProfileProperties[] StochasticSoilProfiles
        {
            get
            {
                return data.StochasticSoilProfiles.Count > 0
                           ? data.StochasticSoilProfiles.Select(ssp => new StochasticSoilProfileProperties
                           {
                               Data = ssp
                           }).ToArray()
                           : new StochasticSoilProfileProperties[0];
            }
        }
    }
}