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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ForeshoreProfile"/> for properties panel.
    /// </summary>
    public class ForeshoreProfileProperties : ObjectProperties<ForeshoreProfile>
    {
        private const int idPropertyIndex = 1;
        private const int namePropertyIndex = 2;
        private const int worldReferencePointPropertyIndex = 3;
        private const int orientationPropertyIndex = 4;
        private const int breakWaterPropertyIndex = 5;
        private const int foreshorePropertyIndex = 6;

        [PropertyOrder(idPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Id_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ForeshoreProfile_Id_Description))]
        public string Id
        {
            get
            {
                return data.Id;
            }
        }

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Profile_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ForeshoreProfile_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WorldReferencePoint_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WorldReferencePoint_ForeshoreProfile_Description))]
        public Point2D WorldReferencePoint
        {
            get
            {
                return new Point2D(
                    new RoundedDouble(0, data.WorldReferencePoint.X),
                    new RoundedDouble(0, data.WorldReferencePoint.Y));
            }
        }

        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Orientation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Orientation_ForeshoreProfile_Description))]
        public RoundedDouble Orientation
        {
            get
            {
                return data.Orientation;
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BreakWaterProperties_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BreakWaterProperties_Description))]
        public BreakWaterProperties BreakWater
        {
            get
            {
                return new BreakWaterProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ForeshoreProperties_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ForeshoreProperties_Description))]
        public ForeshoreGeometryProperties Foreshore
        {
            get
            {
                return new ForeshoreGeometryProperties
                {
                    Data = data
                };
            }
        }
    }
}