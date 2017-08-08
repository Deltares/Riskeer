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
    /// Properties for the fault tree illustration points.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FaultTreeIllustrationPointProperties : IllustrationPointProperties
    {
        private readonly FaultTreeIllustrationPoint faultTreeIllustrationPoint;

        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPointProperties"/>.
        /// </summary>
        /// <param name="illustrationPointNode">The data to use for the properties. </param>
        /// <param name="windDirection">String containing the wind direction for this illustration point.</param>
        /// <param name="closingSituation">String containing the name of the closing situation. If empty 
        /// the <see cref="IllustrationPointProperties.ClosingSituation"/> property will not be visible.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="illustrationPointNode"/> is not of type <see cref="FaultTreeIllustrationPoint"/>.</exception>
        public FaultTreeIllustrationPointProperties(
            IllustrationPointNode illustrationPointNode, string windDirection, string closingSituation) : base(illustrationPointNode, windDirection, closingSituation)
        {
            var illustrationPoint = data.Data as FaultTreeIllustrationPoint;
            if (illustrationPoint == null)
            {
                throw new ArgumentException($"{nameof(illustrationPointNode)} type has to be {nameof(FaultTreeIllustrationPoint)}");
            }

            faultTreeIllustrationPoint = illustrationPoint;
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Alpha))]
        public Stochast[] AlphaValues
        {
            get
            {
                return faultTreeIllustrationPoint.Stochasts.ToArray();
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_Durations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Duration))]
        public Stochast[] Durations
        {
            get
            {
                return faultTreeIllustrationPoint.Stochasts.ToArray();
            }
        }
    }
}