﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of dike geometry of <see cref="GrassCoverErosionInwardsInputContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsInputContextDikeGeometryProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>
    {
        [PropertyOrder(1)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), "Geometry_Coordinates_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Geometry_Coordinates_Description")]
        public Point2D[] Coordinates
        {
            get
            {
                var startingPoint = data.WrappedData.DikeGeometry.FirstOrDefault();
                if (startingPoint == null)
                {
                    return new Point2D[0];
                }
                var coordinates = new List<Point2D>
                {
                    startingPoint.StartingPoint
                };
                coordinates.AddRange(data.WrappedData.DikeGeometry.Select(d => d.EndingPoint));
                return coordinates.ToArray();
            }
        }

        [PropertyOrder(2)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), "DikeGeometry_Roughness_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeGeometry_Roughness_Description")]
        public RoundedDouble[] Roughness
        {
            get
            {
                var roughnesses = data.WrappedData.DikeGeometry.Select(d => d.Roughness);
                return roughnesses.Select(roughness => new RoundedDouble(2, roughness)).ToArray();
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}