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
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses.Probabilistic
{
    /// <summary>
    /// ViewModel of profile specific <see cref="PartialProbabilisticPipingOutput"/> for properties panel.
    /// </summary>
    public class ProbabilisticPipingProfileSpecificOutputProperties : ObjectProperties<PartialProbabilisticPipingOutput>
    {
        private readonly ProbabilityAssessmentOutput derivedOutput;

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingProfileSpecificOutputProperties"/>.
        /// </summary>
        /// <param name="output">The output to show the properties for.</param>
        /// <param name="calculation">The calculation the output belongs to.</param>
        /// <param name="failureMechanism">The failure mechanism the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section the output belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ProbabilisticPipingProfileSpecificOutputProperties(PartialProbabilisticPipingOutput output,
                                                                         ProbabilisticPipingCalculationScenario calculation,
                                                                         PipingFailureMechanism failureMechanism,
                                                                         IAssessmentSection assessmentSection)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Data = output;

            derivedOutput = ProbabilityAssessmentOutputFactory.Create(assessmentSection.FailureMechanismContribution.Norm,
                                                                      failureMechanism.Contribution,
                                                                      GetSectionLength(calculation, failureMechanism),
                                                                      output.Reliability);
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

        private static double GetSectionLength(ProbabilisticPipingCalculationScenario calculation, PipingFailureMechanism failureMechanism)
        {
            FailureMechanismSection failureMechanismSection = failureMechanism
                                                              .Sections
                                                              .First(section => calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Math2D.ConvertPointsToLineSegments(section.Points)));

            return failureMechanismSection?.Length ?? double.NaN;
        }

        #region Result

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_RequiredProbability_Displayname))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_RequiredProbability_Description))]
        public string RequiredProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(derivedOutput.RequiredProbability);
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_RequiredReliability_Displayname))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_RequiredReliability_Description))]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return derivedOutput.RequiredReliability;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Probability_Displayname))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Probability_Description))]
        public string Probability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(derivedOutput.Probability);
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Reliability_Displayname))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Reliability_Description))]
        public RoundedDouble Reliability
        {
            get
            {
                return derivedOutput.Reliability;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result), 1, 2)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_FactorOfSafety_Displayname))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_FactorOfSafety_Description))]
        public RoundedDouble FactorOfSafety
        {
            get
            {
                return derivedOutput.FactorOfSafety;
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