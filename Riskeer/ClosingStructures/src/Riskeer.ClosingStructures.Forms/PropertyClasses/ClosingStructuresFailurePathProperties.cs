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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.ClosingStructures.Forms.PropertyClasses
{
    /// <summary>
    /// Failure path related ViewModel of <see cref="ClosingStructuresFailureMechanism"/> for properties panel.
    /// </summary>
    public class ClosingStructuresFailurePathProperties : ClosingStructuresFailureMechanismProperties
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int contributionPropertyIndex = 4;
        private const int inAssemblyPropertyIndex = 5;
        
        private const int cPropertyIndex = 6;
        private const int n2APropertyIndex = 7;
        private const int nPropertyIndex = 8;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresFailurePathProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public ClosingStructuresFailurePathProperties(ClosingStructuresFailureMechanism data) : base(data, new ConstructionProperties
        {
            NamePropertyIndex = namePropertyIndex,
            CodePropertyIndex = codePropertyIndex,
            GroupPropertyIndex = groupPropertyIndex
        }) {}

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
                   || nameof(C).Equals(propertyName)
                   || nameof(N2A).Equals(propertyName)
                   || nameof(N).Equals(propertyName);
        }

        
        #region General

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

        #endregion

        #region Length effect parameters

        [DynamicVisible]
        [PropertyOrder(cPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ClosingStructuresFailurePathProperties_C_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ClosingStructuresFailurePathProperties_C_Description))]
        public RoundedDouble C
        {
            get
            {
                return data.GeneralInput.C;
            }
        }

        [DynamicVisible]
        [PropertyOrder(n2APropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ClosingStructuresFailurePathProperties_N2A_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ClosingStructuresFailurePathProperties_N2A_Description))]
        public int N2A
        {
            get
            {
                return data.GeneralInput.N2A;
            }
            set
            {
                data.GeneralInput.N2A = value;
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
                return new RoundedDouble(2, data.GeneralInput.N);
            }
        }

        #endregion
    }
}