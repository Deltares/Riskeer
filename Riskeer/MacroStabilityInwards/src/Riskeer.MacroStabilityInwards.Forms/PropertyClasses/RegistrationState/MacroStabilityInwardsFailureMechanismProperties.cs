// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.MacroStabilityInwards.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses.RegistrationState
{
    /// <summary>
    /// Registration state related ViewModel of <see cref="MacroStabilityInwardsFailureMechanism"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismProperties : MacroStabilityInwardsFailureMechanismPropertiesBase
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int inAssemblyPropertyIndex = 3;
        private const int aPropertyIndex = 4;
        private const int bPropertyIndex = 5;

        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="assessmentSection">The assessment section the data belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismProperties(MacroStabilityInwardsFailureMechanism data,
                                                               IAssessmentSection assessmentSection)
            : base(data, new ConstructionProperties
            {
                NamePropertyIndex = namePropertyIndex,
                CodePropertyIndex = codePropertyIndex
            })
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;
        }

        #region General

        [PropertyOrder(inAssemblyPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_InAssembly_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_InAssembly_Description))]
        public bool InAssembly
        {
            get
            {
                return data.InAssembly;
            }
        }

        #endregion

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.InAssembly || !ShouldHidePropertyWhenFailureMechanismNotPartOfAssembly(propertyName);
        }

        private static bool ShouldHidePropertyWhenFailureMechanismNotPartOfAssembly(string propertyName)
        {
            return nameof(A).Equals(propertyName)
                   || nameof(B).Equals(propertyName);
        }

        #region Length effect parameters

        [DynamicVisible]
        [PropertyOrder(aPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_A_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_A_Description))]
        public double A
        {
            get
            {
                return data.MacroStabilityInwardsProbabilityAssessmentInput.A;
            }
            set
            {
                data.MacroStabilityInwardsProbabilityAssessmentInput.A = value;
                data.NotifyObservers();
            }
        }

        [DynamicVisible]
        [PropertyOrder(bPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_B_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_B_Description))]
        public double B
        {
            get
            {
                return data.MacroStabilityInwardsProbabilityAssessmentInput.B;
            }
        }

        #endregion
    }
}