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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.PropertyClasses {
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class IllustrationPointChildProperty : ObjectProperties<IllustrationPointNode> {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointProperty"/>.
        /// </summary>
        /// <param name="faultTreeData">The data to use for the properties. </param>
        /// <param name="windDirection">String containing the wind direction for this illustration point</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public IllustrationPointChildProperty(
            IllustrationPointNode faultTreeData, string windDirection)
        {
            if (faultTreeData == null)
            {
                throw new ArgumentNullException(nameof(faultTreeData));
            }
            if (windDirection == null)
            {
                throw new ArgumentNullException(nameof(windDirection));
            }
            data = faultTreeData;
            WindDirection = windDirection;
        }

        [ReadOnly(true)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
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
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
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
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_Description))]
        public string WindDirection { get; }

        [ReadOnly(true)]
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
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [KeyValueElement(nameof(WindDirection), "")]
        public IllustrationPointChildProperty[] IllustrationPoints
        {
            get
            {
                return data.Children.Select(point => new IllustrationPointChildProperty(point, WindDirection)).ToArray();
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