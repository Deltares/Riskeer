﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.Input;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses.StandAlone
{
    /// <summary>
    /// Failure path related ViewModel of <see cref="MacroStabilityOutwardsFailureMechanism"/> for properties panel.
    /// </summary>
    public class MacroStabilityOutwardsFailurePathProperties : ObjectProperties<MacroStabilityOutwardsFailureMechanism>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int contributionPropertyIndex = 4;
        private const int inAssemblyPropertyIndex = 5;
        private const int aPropertyIndex = 6;
        private const int bPropertyIndex = 7;
        private const int sectionLengthPropertyIndex = 8;
        private const int nPropertyIndex = 9;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityOutwardsFailurePathProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="assessmentSection">The assessment section the data belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityOutwardsFailurePathProperties(MacroStabilityOutwardsFailureMechanism data, IAssessmentSection assessmentSection)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Data = data;
            this.assessmentSection = assessmentSection;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (!data.InAssembly && ShouldHidePropertyWhenFailureMechanismIrrelevant(propertyName))
            {
                return false;
            }

            return true;
        }

        private bool ShouldHidePropertyWhenFailureMechanismIrrelevant(string propertyName)
        {
            return nameof(Contribution).Equals(propertyName)
                   || nameof(A).Equals(propertyName)
                   || nameof(B).Equals(propertyName)
                   || nameof(SectionLength).Equals(propertyName)
                   || nameof(N).Equals(propertyName);
        }

        #region General

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Name_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(codePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Code_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Code_Description))]
        public string Code
        {
            get
            {
                return data.Code;
            }
        }

        [PropertyOrder(groupPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Group_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Group_Description))]
        public int Group
        {
            get
            {
                return data.Group;
            }
        }

        [DynamicVisible]
        [PropertyOrder(contributionPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Contribution_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Contribution_Description))]
        public double Contribution
        {
            get
            {
                return data.Contribution;
            }
        }

        [PropertyOrder(inAssemblyPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_InAssembly_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_InAssembly_Description))]
        public bool InAssembly
        {
            get
            {
                return data.InAssembly;
            }
        }

        #endregion

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
                return data.MacroStabilityOutwardsProbabilityAssessmentInput.A;
            }
            set
            {
                data.MacroStabilityOutwardsProbabilityAssessmentInput.A = value;
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
                return data.MacroStabilityOutwardsProbabilityAssessmentInput.B;
            }
        }

        [DynamicVisible]
        [PropertyOrder(sectionLengthPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ReferenceLine_Length_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ReferenceLine_Length_Rounded_Description))]
        public RoundedDouble SectionLength
        {
            get
            {
                return new RoundedDouble(2, assessmentSection.ReferenceLine.Length);
            }
        }

        [DynamicVisible]
        [PropertyOrder(nPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_Rounded_Description))]
        public RoundedDouble N
        {
            get
            {
                MacroStabilityOutwardsProbabilityAssessmentInput probabilityAssessmentInput = data.MacroStabilityOutwardsProbabilityAssessmentInput;
                return new RoundedDouble(2, probabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length));
            }
        }

        #endregion
    }
}