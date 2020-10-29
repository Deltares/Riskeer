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
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data.Probabilistic;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses.Probabilistic
{
    /// <summary>
    /// ViewModel of profile specific <see cref="PartialProbabilisticPipingOutput"/> for properties panel.
    /// </summary>
    public class ProbabilisticPipingSectionSpecificOutputProperties : ObjectProperties<PartialProbabilisticPipingOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingSectionSpecificOutputProperties"/>.
        /// </summary>
        /// <param name="output">The instance to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown <paramref name="output"/> is <c>null</c>.</exception>
        public ProbabilisticPipingSectionSpecificOutputProperties(PartialProbabilisticPipingOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            Data = output;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.HasGeneralResult &&
                   (
                       propertyName.Equals(nameof(WindDirection)) ||
                       propertyName.Equals(nameof(AlphaValues)) ||
                       propertyName.Equals(nameof(Durations)) ||
                       propertyName.Equals(nameof(IllustrationPoints))
                   );
        }

        private TopLevelFaultTreeIllustrationPointProperties[] GetTopLevelFaultTreeIllustrationPointProperties(bool areClosingSituationsSame)
        {
            return data.GeneralResult
                       .TopLevelIllustrationPoints
                       .Select(point =>
                                   new TopLevelFaultTreeIllustrationPointProperties(
                                       point, areClosingSituationsSame)).ToArray();
        }

        private Stochast[] GetStochasts()
        {
            return data.GeneralResult?.Stochasts.ToArray();
        }

        #region Result

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Probability_Displayname))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Probability_Description))]
        public string Probability
        {
            get
            {
                double reliability = ((PartialProbabilisticPipingOutput) Data).Reliability;
                double probability = StatisticsConverter.ReliabilityToProbability(reliability);
                return ProbabilityFormattingHelper.Format(probability);
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Reliability_Displayname))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Reliability_Description))]
        public RoundedDouble Reliability
        {
            get
            {
                double reliability = ((PartialProbabilisticPipingOutput) Data).Reliability;
                return new RoundedDouble(5, reliability);
            }
        }

        #endregion

        #region Illustration points

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
                return GetStochasts();
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
                return GetStochasts();
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

                return GetTopLevelFaultTreeIllustrationPointProperties(areClosingSituationsSame);
            }
        }

        #endregion
    }
}