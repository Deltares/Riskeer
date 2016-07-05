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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInput.ForeshoreGeometry"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsInputContextForeshoreProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>
    {
        private const int useForeshorePropertyIndex = 1;
        private const int coordinatesPropertyIndex = 2;

        [DynamicReadOnly]
        [PropertyOrder(useForeshorePropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "Foreshore_UseForeshore_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Foreshore_UseForeshore_Description")]
        public bool UseForeshore
        {
            get
            {
                return data.WrappedData.UseForeshore;
            }
            set
            {
                data.WrappedData.UseForeshore = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(coordinatesPropertyIndex)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), "Geometry_Coordinates_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Geometry_Coordinates_Description")]
        public Point2D[] Coordinates
        {
            get
            {
                return data.WrappedData.ForeshoreGeometry.ToArray();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return data.WrappedData.DikeProfile == null || data.WrappedData.ForeshoreGeometry.Count() < 2;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}