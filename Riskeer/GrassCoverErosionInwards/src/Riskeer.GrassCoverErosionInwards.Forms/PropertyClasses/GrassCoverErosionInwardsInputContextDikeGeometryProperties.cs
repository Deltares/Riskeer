// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInput.DikeGeometry"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsInputContextDikeGeometryProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>
    {
        [PropertyOrder(1)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Geometry_Coordinates_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Geometry_Coordinates_Description))]
        public Point2D[] Coordinates
        {
            get
            {
                return data.WrappedData.DikeGeometry.Select(rp => rp.Point).ToArray();
            }
        }

        [PropertyOrder(2)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeGeometry_Roughness_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeGeometry_Roughness_Description))]
        public RoundedDouble[] Roughnesses
        {
            get
            {
                return DikeGeometryHelper.GetRoughnesses(data.WrappedData.DikeGeometry).ToArray();
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}