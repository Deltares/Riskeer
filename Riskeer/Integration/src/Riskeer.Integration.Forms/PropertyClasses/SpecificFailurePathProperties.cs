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
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// Object properties class for <see cref="SpecificFailurePath"/>.
    /// </summary>
    public class SpecificFailurePathProperties : ObjectProperties<SpecificFailurePath>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int inAssemblyPropertyIndex = 3;
        private const int nPropertyIndex = 4;
        private const int applyLengthEffectInSectionPropertyIndex = 5;

        /// <summary>
        /// Creates a new instance of <see cref="SpecificFailurePathProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public SpecificFailurePathProperties(SpecificFailurePath data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.InAssembly || !ShouldHidePropertyWhenFailurePathNotPartOfAssembly(propertyName);
        }

        private static bool ShouldHidePropertyWhenFailurePathNotPartOfAssembly(string propertyName)
        {
            return nameof(N).Equals(propertyName)
                   || nameof(ApplyLengthEffectInSection).Equals(propertyName);
        }

        #region General

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailurePath_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(codePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Code_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailurePath_Code_Description))]
        public string Code
        {
            get
            {
                return data.Code;
            }
            set
            {
                data.Code = value;
                data.NotifyObservers();
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
        [PropertyOrder(nPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_Description))]
        public RoundedDouble N
        {
            get => data.GeneralInput.N;
            set
            {
                data.GeneralInput.N = value;
                data.NotifyObservers();
            }
        }

        [DynamicVisible]
        [PropertyOrder(applyLengthEffectInSectionPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_ApplyLengthEffectInSection_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_ApplyLengthEffectInSection_Description))]
        public bool ApplyLengthEffectInSection
        {
            get => data.GeneralInput.ApplyLengthEffectInSection;
            set
            {
                data.GeneralInput.ApplyLengthEffectInSection = value;
                data.NotifyObservers();
            }
        }

        #endregion
    }
}