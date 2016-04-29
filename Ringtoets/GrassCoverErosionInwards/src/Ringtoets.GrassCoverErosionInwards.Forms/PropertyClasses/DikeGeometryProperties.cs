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

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInput.DikeGeometry"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DikeGeometryProperties : ObjectProperties<GrassCoverErosionInwardsCalculationContext>
    {
        [PropertyOrder(1)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), "DikeGeometry_Coordinates_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeGeometry_Coordinates_Description")]
        public string[] Coordinates
        {
            get
            {
                var startingPoint = data.WrappedData.InputParameters.DikeGeometry.FirstOrDefault();
                if (startingPoint == null)
                {
                    return new string[0];
                }
                var coordinates = new List<string>
                {
                    new RoundedDouble(2, startingPoint.StartingPoint.X).Value.ToString(CultureInfo.InvariantCulture)
                };
                coordinates.AddRange(data.WrappedData.InputParameters.DikeGeometry.Select(d => new RoundedDouble(2, d.EndingPoint.X).Value.ToString(CultureInfo.InvariantCulture)));
                return coordinates.ToArray();
            }
        }

        [PropertyOrder(2)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), "DikeGeometry_Roughness_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeGeometry_Roughness_Description")]
        public string[] Roughness
        {
            get
            {
                var roughnesses = data.WrappedData.InputParameters.DikeGeometry.Select(d => d.Roughness);
                return roughnesses.Select(roughness => new RoundedDouble(2, roughness).Value.ToString(CultureInfo.InvariantCulture)).ToArray();
            }
        }

        public override string ToString()
        {
            return Resources.DikeGeometryProperties_DisplayName;
        }
    }
}