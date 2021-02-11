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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses.Probabilistic
{
    /// <summary>
    /// ViewModel of profile specific <see cref="PartialProbabilisticFaultTreePipingOutput"/> for properties panel.
    /// </summary>
    public class ProbabilisticPipingProfileSpecificOutputProperties : ObjectProperties<IPartialProbabilisticPipingOutput>
    {
        private const int requiredProbabilityIndex = 0;
        private const int requiredReliabilityIndex = 1;
        private const int probabilityIndex = 2;
        private const int reliabilityIndex = 3;
        private const int safetyFactorIndex = 4;

        private readonly ProbabilityAssessmentOutput derivedOutput;

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingProfileSpecificOutputProperties"/>.
        /// </summary>
        /// <param name="output">The output to show the properties for.</param>
        /// <param name="calculation">The calculation the output belongs to.</param>
        /// <param name="failureMechanism">The failure mechanism the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section the output belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected ProbabilisticPipingProfileSpecificOutputProperties(IPartialProbabilisticPipingOutput output,
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

            derivedOutput = PipingProbabilityAssessmentOutputFactory.Create(output,
                                                                            calculation,
                                                                            failureMechanism,
                                                                            assessmentSection);
        }

        #region Result

        [PropertyOrder(requiredProbabilityIndex)]
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

        [PropertyOrder(requiredReliabilityIndex)]
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

        [PropertyOrder(probabilityIndex)]
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

        [PropertyOrder(reliabilityIndex)]
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

        [PropertyOrder(safetyFactorIndex)]
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
    }
}