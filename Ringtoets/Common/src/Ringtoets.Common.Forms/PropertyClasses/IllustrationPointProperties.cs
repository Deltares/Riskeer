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
    /// Properties for the illustration points.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class IllustrationPointProperties : ObjectProperties<IllustrationPointBase>
    {
        private readonly IEnumerable<IllustrationPointNode> childNodes;

        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointProperties"/>.
        /// </summary>
        /// <param name="illustrationPoint">The data to use for the properties.</param>
        /// <param name="childNodes">The child nodes that belongs to the <paramref name="illustrationPoint"/>.</param>
        /// <param name="windDirection">String containing the wind direction for this illustration point.</param>
        /// <param name="closingSituation">String containing the name of the closing situation. If empty 
        /// the <see cref="ClosingSituation"/> property will not be visible.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public IllustrationPointProperties(IllustrationPointBase illustrationPoint, IEnumerable<IllustrationPointNode> childNodes,
                                           string windDirection, string closingSituation)
        {
            if (illustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(illustrationPoint));
            }
            if (childNodes == null)
            {
                throw new ArgumentNullException(nameof(childNodes));
            }
            if (windDirection == null)
            {
                throw new ArgumentNullException(nameof(windDirection));
            }
            if (closingSituation == null)
            {
                throw new ArgumentNullException(nameof(closingSituation));
            }
            data = illustrationPoint;
            this.childNodes = childNodes;
            WindDirection = windDirection;
            ClosingSituation = closingSituation;
        }

        [PropertyOrder(0)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_Description))]
        public double CalculatedProbability
        {
            get
            {
                return StatisticsConverter.ReliabilityToProbability(data.Beta);
            }
        }

        [PropertyOrder(1)]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_Description))]
        public RoundedDouble Reliability
        {
            get
            {
                return data.Beta;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_Description))]
        public string WindDirection { get; }

        [DynamicVisible]
        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_ClosingSituation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_ClosingSituation_Description))]
        public string ClosingSituation { get; }

        [DynamicVisible]
        [PropertyOrder(6)]
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
                    if (illustrationPointNode.Data is FaultTreeIllustrationPoint)
                    {
                        points.Add(new FaultTreeIllustrationPointProperties(illustrationPointNode.Data,
                                                                            illustrationPointNode.Children,
                                                                            WindDirection, ClosingSituation));
                        continue;
                    }

                    if (illustrationPointNode.Data is SubMechanismIllustrationPoint)
                    {
                        points.Add(new SubMechanismIllustrationPointProperties(illustrationPointNode.Data,
                                                                               illustrationPointNode.Children,
                                                                               WindDirection, ClosingSituation));
                        continue;
                    }

                    // If type is not supported, throw exception (currently not possible, safeguard for future)
                    throw new NotSupportedException($"IllustrationPointNode of type {illustrationPointNode.Data.GetType().Name} is not supported. " +
                                                    $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}");
                }
                return points.ToArray();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool IsDynamicVisible(string propertyName)
        {
            if (propertyName.Equals(nameof(IllustrationPoints)))
            {
                return childNodes.Any();
            }

            if (propertyName.Equals(nameof(ClosingSituation)))
            {
                return ClosingSituation != string.Empty;
            }

            return false;
        }

        public override string ToString()
        {
            return $"{data.Name}";
        }
    }
}