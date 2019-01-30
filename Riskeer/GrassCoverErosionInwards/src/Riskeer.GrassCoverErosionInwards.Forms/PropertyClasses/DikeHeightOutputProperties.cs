// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DikeHeightOutput"/> for properties panel.
    /// </summary>
    public class DikeHeightOutputProperties : ObjectProperties<DikeHeightOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DikeHeightOutputProperties"/>.
        /// </summary>
        /// <param name="dikeHeightOutput">The dike height output to create the object properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dikeHeightOutput"/> is <c>null</c>.</exception>
        public DikeHeightOutputProperties(DikeHeightOutput dikeHeightOutput)
        {
            if (dikeHeightOutput == null)
            {
                throw new ArgumentNullException(nameof(dikeHeightOutput));
            }

            Data = dikeHeightOutput;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_DikeHeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_DikeHeight_Description))]
        public RoundedDouble DikeHeight
        {
            get
            {
                return data.DikeHeight;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double DikeHeightTargetProbability
        {
            get
            {
                return data.TargetProbability;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetReliability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble DikeHeightTargetReliability
        {
            get
            {
                return data.TargetReliability;
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double DikeHeightCalculatedProbability
        {
            get
            {
                return data.CalculatedProbability;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble DikeHeightCalculatedReliability
        {
            get
            {
                return data.CalculatedReliability;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_Convergence_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeHeightOutput_Convergence_Description))]
        public string DikeHeightConvergence
        {
            get
            {
                return new EnumDisplayWrapper<CalculationConvergence>(data.CalculationConvergence).DisplayName;
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_GoverningWindDirection_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_GoverningWindDirection_Description))]
        public string WindDirection
        {
            get
            {
                return data.GeneralResult?.GoverningWindDirection.Name;
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Alpha))]
        public Stochast[] AlphaValues
        {
            get
            {
                return data.GeneralResult?.Stochasts.ToArray();
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_Durations_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Duration))]
        public Stochast[] Durations
        {
            get
            {
                return data.GeneralResult?.Stochasts.ToArray();
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public TopLevelFaultTreeIllustrationPointProperties[] IllustrationPoints
        {
            get
            {
                if (!data.HasGeneralResult)
                {
                    return new TopLevelFaultTreeIllustrationPointProperties[0];
                }

                bool areClosingSituationsSame = !data.GeneralResult
                                                     .TopLevelIllustrationPoints
                                                     .HasMultipleUniqueValues(p => p.ClosingSituation);

                return data.GeneralResult
                           .TopLevelIllustrationPoints
                           .Select(point =>
                                       new TopLevelFaultTreeIllustrationPointProperties(
                                           point, areClosingSituationsSame)).ToArray();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName.Equals(nameof(WindDirection)) ||
                propertyName.Equals(nameof(AlphaValues)) ||
                propertyName.Equals(nameof(Durations)) ||
                propertyName.Equals(nameof(IllustrationPoints)))
            {
                return data.HasGeneralResult;
            }

            return false;
        }
    }
}