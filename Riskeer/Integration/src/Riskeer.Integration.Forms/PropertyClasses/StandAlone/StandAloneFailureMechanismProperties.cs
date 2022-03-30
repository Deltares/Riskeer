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
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.FailureMechanism;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses.StandAlone
{
    /// <summary>
    /// Registration state related ViewModel of <see cref="IHasGeneralInput"/> for properties panel.
    /// </summary>
    public class StandAloneFailureMechanismProperties : ObjectProperties<IHasGeneralInput>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int inAssemblyPropertyIndex = 3;
        private const int nPropertyIndex = 4;
        private const int applyLengthEffectInSectionPropertyIndex = 5;

        /// <summary>
        /// Creates a new instance of <see cref="StandAloneFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StandAloneFailureMechanismProperties(IHasGeneralInput failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            Data = failureMechanism;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.InAssembly || !ShouldHidePropertyWhenFailureMechanismNotPartOfAssembly(propertyName);
        }

        private static bool ShouldHidePropertyWhenFailureMechanismNotPartOfAssembly(string propertyName)
        {
            return nameof(N).Equals(propertyName)
                   || nameof(ApplyLengthEffectInSection).Equals(propertyName);
        }

        #region Length effect parameters

        [DynamicVisible]
        [PropertyOrder(nPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_Description))]
        public RoundedDouble N
        {
            get
            {
                return data.GeneralInput.N;
            }
            set
            {
                data.GeneralInput.N = value;
                data.NotifyObservers();
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
                return data.GeneralInput.ApplyLengthEffectInSection;
            }
            set
            {
                data.GeneralInput.ApplyLengthEffectInSection = value;
                data.NotifyObservers();
            }
        }

        #endregion

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
    }
}