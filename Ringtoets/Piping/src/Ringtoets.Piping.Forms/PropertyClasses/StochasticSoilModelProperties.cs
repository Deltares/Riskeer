// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StochasticSoilModel"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StochasticSoilModelProperties : ObjectProperties<StochasticSoilModel>
    {
        /// <summary>
        /// Gets the id from the <see cref="StochasticSoilModel"/>.
        /// </summary>
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilModel_Id_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilModel_Id_Description")]
        public long Id
        {
            get
            {
                return data.Id;
            }
        }

        /// <summary>
        /// Gets the name from the <see cref="StochasticSoilModel"/>.
        /// </summary>
        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilModel_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilModel_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        /// <summary>
        /// Gets the name of the segment from the <see cref="StochasticSoilModel"/>.
        /// </summary>
        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilModel_SegmentName_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilModel_SegmentName_Description")]
        public string SegmentName
        {
            get
            {
                return data.SegmentName;
            }
        }

        /// <summary>
        /// Gets the geometry points from the <see cref="StochasticSoilModel"/>.
        /// </summary>
        [PropertyOrder(4)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilModel_Geometry_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilModel_Geometry_Description")]
        public Point2D[] Geometry
        {
            get
            {
                return data.Geometry.ToArray();
            }
        }

        /// <summary>
        /// Gets the <see cref="StochasticSoilProfiles"/> from the <see cref="StochasticSoilModel"/>.
        /// </summary>
        [PropertyOrder(5)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilModel_StochasticSoilProfiles_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilModel_StochasticSoilProfiles_Description")]
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