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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Base properties for the child Illustration Points in a tree
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class IllustrationPointChildProperties : ObjectProperties<IllustrationPointNode>
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointChildProperties"/>.
        /// </summary>
        /// <param name="illustrationPointNode">The data to use for the properties. </param>
        /// <param name="windDirection">String containing the wind direction for this illustration point</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public IllustrationPointChildProperties(
            IllustrationPointNode illustrationPointNode, string windDirection)
        {
            if (illustrationPointNode == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointNode));
            }
            if (windDirection == null)
            {
                throw new ArgumentNullException(nameof(windDirection));
            }
            data = illustrationPointNode;
            WindDirection = windDirection;
        }

        [ReadOnly(true)]
        [PropertyOrder(0)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_Description))]
        public double CalculatedProbability
        {
            get
            {
                return StatisticsConverter.ReliabilityToProbability(data.Data.Beta);
            }
        }

        [ReadOnly(true)]
        [PropertyOrder(1)]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_Description))]
        public RoundedDouble Reliability
        {
            get
            {
                return data.Data.Beta;
            }
        }

        [ReadOnly(true)]
        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_Description))]
        public string WindDirection { get; }

        [ReadOnly(true)]
        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_ClosingSituation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_ClosingSituation_Description))]
        public string Name
        {
            get
            {
                return data.Data.Name;
            }
        }

        [ReadOnly(true)]
        [DynamicVisible]
        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [KeyValueElement(nameof(WindDirection), "")]
        public IllustrationPointChildProperties[] IllustrationPoints
        {
            get
            {
                var points = new List<IllustrationPointChildProperties>();
                foreach (IllustrationPointNode illustrationPointNode in data.Children)
                {
                    var faultTree = illustrationPointNode.Data as FaultTreeIllustrationPoint;
                    if (faultTree != null)
                    {
                        points.Add(new FaultTreeIllustrationPointChildProperties(illustrationPointNode, WindDirection));
                        continue;
                    }

                    var subMechanism = illustrationPointNode.Data as SubMechanismIllustrationPoint;
                    if (subMechanism != null)
                    {
                        points.Add(new SubMechanismIllustrationPointChildProperties(illustrationPointNode, WindDirection));
                    }
                }
                return points.ToArray();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool IsDynamicVisible(string propertyName)
        {
            return propertyName == "IllustrationPoints" && data.Children.Any();
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}