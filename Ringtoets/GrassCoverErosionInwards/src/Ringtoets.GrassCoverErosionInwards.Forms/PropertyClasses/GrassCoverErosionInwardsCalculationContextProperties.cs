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

using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsCalculationContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationContextProperties : ObjectProperties<GrassCoverErosionInwardsCalculationContext>
    {
        private const int dikeGeometryPropertyIndex = 1;
        private const int dikeHeightPropertyIndex = 2;
        private const int foreshorePropertyIndex = 3;
        private const int orientationPropertyIndex = 4;
        private const int breakWaterPropertyIndex = 5;

        [PropertyOrder(dikeGeometryPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "DikeGeometryProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeGeometryProperties_Description")]
        public DikeGeometryProperties DikeGeometry
        {
            get
            {
                return new DikeGeometryProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(dikeHeightPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "DikeHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeHeight_Description")]
        public string DikeHeight
        {
            get
            {
                return new RoundedDouble(2, data.WrappedData.InputParameters.DikeHeight).Value.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                data.WrappedData.InputParameters.DikeHeight = new RoundedDouble(2, double.Parse(value));
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ForeshoreProperties_Description")]
        public ForeshoreProperties Foreshore
        {
            get
            {
                return new ForeshoreProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "Orientation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Orientation_Description")]
        public string Orientation
        {
            get
            {
                return new RoundedDouble(2, data.WrappedData.InputParameters.Orientation).Value.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                data.WrappedData.InputParameters.Orientation = new RoundedDouble(2, double.Parse(value));
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterProperties_Description")]
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
    }
}