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
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="TopLevelSubMechanismIllustrationPoint"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TopLevelSubMechanismIllustrationPointProperties : ObjectProperties<TopLevelSubMechanismIllustrationPoint>
    {
        private readonly IEnumerable<string> closingSituations;

        /// <summary>
        /// Creates a new instance of <see cref="TopLevelSubMechanismIllustrationPointProperties"/>.
        /// </summary>
        /// <param name="data">The <see cref="TopLevelSubMechanismIllustrationPoint"/>
        /// to create the properties for.</param>
        /// <param name="closingSituations">The calculated closing situations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is 
        /// <c>null</c>.</exception>
        public TopLevelSubMechanismIllustrationPointProperties(TopLevelSubMechanismIllustrationPoint data,
                                                               IEnumerable<string> closingSituations)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (closingSituations == null)
            {
                throw new ArgumentNullException(nameof(closingSituations));
            }

            Data = data;
            this.closingSituations = closingSituations;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_IllustrationPointName_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_IllustrationPointName_Description))]
        public string Name
        {
            get
            {
                return data.SubMechanismIllustrationPoint.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                return StatisticsConverter.ReliabilityToProbability(data.SubMechanismIllustrationPoint.Beta);
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                return data.SubMechanismIllustrationPoint.Beta;
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_WindDirection_Description))]
        public string WindDirection
        {
            get
            {
                return data.WindDirection.Name;
            }
        }

        [PropertyOrder(5)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_ClosingSituation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_ClosingSituation_Description))]
        public string ClosingSituation
        {
            get
            {
                return data.ClosingSituation;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(SubMechanismIllustrationPointStochast.Name), nameof(SubMechanismIllustrationPointStochast.Alpha))]
        public SubMechanismIllustrationPointStochast[] AlphaValues
        {
            get
            {
                return data.SubMechanismIllustrationPoint.Stochasts.ToArray();
            }
        }

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_Durations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(SubMechanismIllustrationPointStochast.Name), nameof(SubMechanismIllustrationPointStochast.Duration))]
        public SubMechanismIllustrationPointStochast[] Durations
        {
            get
            {
                return data.SubMechanismIllustrationPoint.Stochasts.ToArray();
            }
        }

        [PropertyOrder(8)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_IllustrationPointValues_DisplayName))]
        public SubMechanismIllustrationPointValuesProperties SubMechanismIllustrationPointValues
        {
            get
            {
                return new SubMechanismIllustrationPointValuesProperties(data.SubMechanismIllustrationPoint);
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName == nameof(ClosingSituation))
            {
                return !AreClosingSituationsSame();
            }

            return false;
        }

        public override string ToString()
        {
            return AreClosingSituationsSame()
                       ? data.WindDirection.Name
                       : string.Format(Resources.TopLevelSubMechanismIllustrationPointProperties_ToString_WindDirectionName_0_ClosingSituation_1,
                                       data.WindDirection.Name,
                                       data.ClosingSituation);
        }

        private bool AreClosingSituationsSame()
        {
            return !closingSituations.HasMultipleUniqueValues(c => c);
        }
    }
}