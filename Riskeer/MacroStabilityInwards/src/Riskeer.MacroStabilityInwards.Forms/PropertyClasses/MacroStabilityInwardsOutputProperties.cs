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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsOutput"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsOutputProperties : ObjectProperties<MacroStabilityInwardsOutput>
    {
        private const int macroStabilityInwardsFactorOfStabilityIndex = 1;
        private const int requiredProbabilityIndex = 2;
        private const int requiredReliabilityIndex = 3;
        private const int macroStabilityInwardsProbabilityIndex = 4;
        private const int macroStabilityInwardsReliabilityIndex = 5;
        private const int macroStabilityInwardsFactorOfSafetyIndex = 6;
        private DerivedMacroStabilityInwardsOutput derivedOutput;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsOutputProperties"/>.
        /// </summary>
        /// <param name="output">The output to show the properties for.</param>
        /// <param name="failureMechanism">The failure mechanism the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section the output belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsOutputProperties(MacroStabilityInwardsOutput output,
                                                     MacroStabilityInwardsFailureMechanism failureMechanism,
                                                     IAssessmentSection assessmentSection)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
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
            CreateDerivedOutput(output, failureMechanism, assessmentSection);
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_FactorOfStability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_FactorOfStability_Description))]
        [PropertyOrder(macroStabilityInwardsFactorOfStabilityIndex)]
        public RoundedDouble MacroStabilityInwardsFactorOfStability
        {
            get
            {
                return derivedOutput.FactorOfStability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_RequiredProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_RequiredProbability_Description))]
        [PropertyOrder(requiredProbabilityIndex)]
        public string RequiredProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(derivedOutput.RequiredProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_RequiredReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_RequiredReliability_Description))]
        [PropertyOrder(requiredReliabilityIndex)]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return derivedOutput.RequiredReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsProbability_Description))]
        [PropertyOrder(macroStabilityInwardsProbabilityIndex)]
        public string MacroStabilityInwardsProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(derivedOutput.MacroStabilityInwardsProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsReliability_Description))]
        [PropertyOrder(macroStabilityInwardsReliabilityIndex)]
        public RoundedDouble MacroStabilityInwardsReliability
        {
            get
            {
                return derivedOutput.MacroStabilityInwardsReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsFactorOfSafety_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsFactorOfSafety_Description))]
        [PropertyOrder(macroStabilityInwardsFactorOfSafetyIndex)]
        public RoundedDouble MacroStabilityInwardsFactorOfSafety
        {
            get
            {
                return derivedOutput.MacroStabilityInwardsFactorOfSafety;
            }
        }

        private void CreateDerivedOutput(MacroStabilityInwardsOutput output,
                                         MacroStabilityInwardsFailureMechanism failureMechanism,
                                         IAssessmentSection assessmentSection)
        {
            derivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(output, failureMechanism, assessmentSection);
        }
    }
}