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
    /// Properties for the Fault tree of Illustration Points
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SubMechanismIllustrationPointChildProperty : IllustrationPointChildProperty
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointProperty"/>.
        /// </summary>
        /// <param name="faultTreeData">The data to use for the properties. </param>
        /// <param name="windDirection">String containing the wind direction for this illustration point</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public SubMechanismIllustrationPointChildProperty(
            IllustrationPointNode faultTreeData, string windDirection) : base(faultTreeData, windDirection)
        {
            if (!(data.Data is SubMechanismIllustrationPoint))
            {
                throw new ArgumentException("IllustrationPoint data type has to be SubMechanismIllustrationPoint");
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_Realization_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_Realization_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(SubMechanismIllustrationPointStochast.Realization))]
        public SubMechanismIllustrationPointStochast[] SubMechanismStochasts
        {
            get
            {
                return ((SubMechanismIllustrationPoint) data.Data).Stochasts.ToArray();
            }
        }
    }
}