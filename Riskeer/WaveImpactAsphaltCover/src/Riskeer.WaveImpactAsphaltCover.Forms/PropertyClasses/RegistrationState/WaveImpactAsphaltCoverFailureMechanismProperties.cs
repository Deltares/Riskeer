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
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerRevetmentFormsResources = Riskeer.Revetment.Forms.Properties.Resources;

namespace Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses.RegistrationState
{
    /// <summary>
    /// Registration state related ViewModel of <see cref="WaveImpactAsphaltCoverFailureMechanism"/> for properties panel.
    /// </summary>
    public class WaveImpactAsphaltCoverFailureMechanismProperties : WaveImpactAsphaltCoverFailureMechanismPropertiesBase
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int inAssemblyPropertyIndex = 3;
        private const int sectionLengthPropertyIndex = 4;
        private const int deltaLPropertyIndex = 5;
        private const int nPropertyIndex = 6;
        private const int applyLengthEffectInSectionPropertyIndex = 7;

        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public WaveImpactAsphaltCoverFailureMechanismProperties(WaveImpactAsphaltCoverFailureMechanism data, IAssessmentSection assessmentSection)
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
            return nameof(DeltaL).Equals(propertyName)
                   || nameof(SectionLength).Equals(propertyName)
                   || nameof(N).Equals(propertyName)
                   || nameof(ApplyLengthEffectInSection).Equals(propertyName);
        }

        #region Length effect parameters

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
        [PropertyOrder(deltaLPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveImpactAsphaltCoverFailureMechanismProperties_DeltaL_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveImpactAsphaltCoverFailureMechanismProperties_DeltaL_Description))]
        public RoundedDouble DeltaL
        {
            get
            {
                return data.GeneralWaveImpactAsphaltCoverInput.DeltaL;
            }
            set
            {
                data.GeneralWaveImpactAsphaltCoverInput.DeltaL = value;
                data.NotifyObservers();
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
                return new RoundedDouble(2, data.GeneralWaveImpactAsphaltCoverInput.GetN(assessmentSection.ReferenceLine.Length));
            }
        }

        [DynamicVisible]
        [PropertyOrder(applyLengthEffectInSectionPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ApplyLengthEffectInSection_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ApplyLengthEffectInSection_Description))]
        public bool ApplyLengthEffectInSection
        {
            get
            {
                return data.GeneralWaveImpactAsphaltCoverInput.ApplyLengthEffectInSection;
            }
            set
            {
                data.GeneralWaveImpactAsphaltCoverInput.ApplyLengthEffectInSection = value;
                data.NotifyObservers();
            }
        }

        #endregion
    }
}