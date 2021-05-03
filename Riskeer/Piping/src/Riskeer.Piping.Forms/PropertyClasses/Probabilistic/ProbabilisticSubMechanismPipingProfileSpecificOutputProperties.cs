// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses.Probabilistic
{
    /// <summary>
    /// ViewModel of profile specific <see cref="PartialProbabilisticSubMechanismPipingOutput"/> for properties panel.
    /// </summary>
    public class ProbabilisticSubMechanismPipingProfileSpecificOutputProperties : ProbabilisticPipingProfileSpecificOutputProperties
    {
        private const int windDirectionIndex = 5;
        private const int alphaValuesIndex = 6;
        private const int durationsIndex = 7;
        private const int illustrationPointsIndex = 8;
        private readonly PartialProbabilisticSubMechanismPipingOutput subMechanismOutput;

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticSubMechanismPipingProfileSpecificOutputProperties"/>.
        /// </summary>
        /// <param name="output">The output to show the properties for.</param>
        /// <param name="calculation">The calculation the output belongs to.</param>
        /// <param name="failureMechanism">The failure mechanism the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section the output belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ProbabilisticSubMechanismPipingProfileSpecificOutputProperties(
            PartialProbabilisticSubMechanismPipingOutput output,
            ProbabilisticPipingCalculationScenario calculation,
            PipingFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
            : base(output, calculation, failureMechanism, assessmentSection)
        {
            subMechanismOutput = output;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.HasGeneralResult
                   && (propertyName.Equals(nameof(WindDirection))
                       || propertyName.Equals(nameof(AlphaValues))
                       || propertyName.Equals(nameof(Durations))
                       || propertyName.Equals(nameof(IllustrationPoints)));
        }

        private TopLevelSubMechanismIllustrationPointProperties[] GetTopLevelIllustrationPointProperties()
        {
            TopLevelSubMechanismIllustrationPoint[] subMechanismIllustrationPoints = subMechanismOutput.GeneralResult.TopLevelIllustrationPoints.ToArray();
            IEnumerable<string> closingSituations = subMechanismIllustrationPoints.Select(p => p.ClosingSituation);

            return subMechanismIllustrationPoints.Select(
                point => new TopLevelSubMechanismIllustrationPointProperties(
                    point, closingSituations)).ToArray();
        }

        private Stochast[] GetStochasts()
        {
            return subMechanismOutput.GeneralResult?.Stochasts.ToArray();
        }

        #region Illustration points

        [DynamicVisible]
        [PropertyOrder(windDirectionIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_GoverningWindDirection_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_GoverningWindDirection_Description))]
        public string WindDirection
        {
            get
            {
                return subMechanismOutput.GeneralResult?.GoverningWindDirection.Name;
            }
        }

        [DynamicVisible]
        [PropertyOrder(alphaValuesIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Alpha))]
        public Stochast[] AlphaValues
        {
            get
            {
                return GetStochasts();
            }
        }

        [DynamicVisible]
        [PropertyOrder(durationsIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_Durations_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPoint_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(Stochast.Name), nameof(Stochast.Duration))]
        public Stochast[] Durations
        {
            get
            {
                return GetStochasts();
            }
        }

        [DynamicVisible]
        [PropertyOrder(illustrationPointsIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_IllustrationPoints), 2, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPointProperty_IllustrationPoints_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.IllustrationPointProperty_IllustrationPoints_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public TopLevelSubMechanismIllustrationPointProperties[] IllustrationPoints
        {
            get
            {
                return !data.HasGeneralResult
                           ? new TopLevelSubMechanismIllustrationPointProperties[0]
                           : GetTopLevelIllustrationPointProperties();
            }
        }

        #endregion
    }
}