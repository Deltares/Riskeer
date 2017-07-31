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

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties for the fault tree illustration points
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FaultTreeIllustrationPointChildProperties : IllustrationPointChildProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPointChildProperties"/>.
        /// </summary>
        /// <param name="illustrationPointNode">The data to use for the properties. </param>
        /// <param name="windDirection">String containing the wind direction for this illustration point</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c> or when illustrationPointNode is of the wrong type.</exception>
        /// <exception cref="ArgumentException">Thrown when the illustration point node is not of type <see cref="FaultTreeIllustrationPoint"/></exception>
        public FaultTreeIllustrationPointChildProperties(
            IllustrationPointNode illustrationPointNode, string windDirection) : base(illustrationPointNode, windDirection)
        {
            if (!(data.Data is FaultTreeIllustrationPoint))
            {
                throw new ArgumentException("illustrationPointNode type has to be FaultTreeIllustrationPoint");
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Alpha))]
        public Stochast[] AlphaValues
        {
            get
            {
                return ((FaultTreeIllustrationPoint) data.Data).Stochasts.ToArray();
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Durations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Duration))]
        public Stochast[] Durations
        {
            get
            {
                return ((FaultTreeIllustrationPoint) data.Data).Stochasts.ToArray();
            }
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}