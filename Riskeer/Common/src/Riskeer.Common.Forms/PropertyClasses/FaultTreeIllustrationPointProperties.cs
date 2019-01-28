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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Util.Attributes;
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
        private readonly IEnumerable<IllustrationPointNode> childNodes;

        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPointProperties"/>.
        /// </summary>
        /// <param name="illustrationPoint">The fault tree illustration point to use for the properties.</param>
        /// <param name="childNodes">The child nodes that belongs to the <paramref name="illustrationPoint"/>.</param>
        /// <param name="windDirection">The wind direction of the illustration point.</param>
        /// <param name="closingSituation">The closing situation of the illustration point.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public FaultTreeIllustrationPointProperties(FaultTreeIllustrationPoint illustrationPoint, IEnumerable<IllustrationPointNode> childNodes,
                                                    string windDirection, string closingSituation)
            : base(illustrationPoint, windDirection, closingSituation)
        {
            if (childNodes == null)
            {
                throw new ArgumentNullException(nameof(childNodes));
            }

            faultTreeIllustrationPoint = illustrationPoint;
            this.childNodes = childNodes;
        }

        [PropertyOrder(5)]
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

        [PropertyOrder(6)]
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

        [DynamicVisible]
        [PropertyOrder(7)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [KeyValueElement(nameof(WindDirection), "")]
        public IllustrationPointProperties[] IllustrationPoints
        {
            get
            {
                var points = new List<IllustrationPointProperties>();
                foreach (IllustrationPointNode illustrationPointNode in childNodes)
                {
                    var faultTreeIllustrationPointChild = illustrationPointNode.Data as FaultTreeIllustrationPoint;
                    if (faultTreeIllustrationPointChild != null)
                    {
                        points.Add(new FaultTreeIllustrationPointProperties(faultTreeIllustrationPointChild,
                                                                            illustrationPointNode.Children,
                                                                            WindDirection, ClosingSituation));
                        continue;
                    }

                    var subMechanismIllustrationPoint = illustrationPointNode.Data as SubMechanismIllustrationPoint;
                    if (subMechanismIllustrationPoint != null)
                    {
                        points.Add(new SubMechanismIllustrationPointProperties(subMechanismIllustrationPoint,
                                                                               WindDirection, ClosingSituation));
                        continue;
                    }

                    throw new NotSupportedException($"IllustrationPointNode of type {illustrationPointNode.Data.GetType().Name} is not supported. " +
                                                    $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}");
                }

                return points.ToArray();
            }
        }

        [DynamicVisibleValidationMethod]
        public override bool IsDynamicVisible(string propertyName)
        {
            return propertyName.Equals(nameof(IllustrationPoints)) ? childNodes.Any() : base.IsDynamicVisible(propertyName);
        }
    }
}