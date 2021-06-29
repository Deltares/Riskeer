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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.ClosingStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ClosingStructuresFailureMechanism"/> for properties panel.
    /// </summary>
    public class ClosingStructuresFailureMechanismProperties : ObjectProperties<ClosingStructuresFailureMechanism>
    {
        private readonly Dictionary<string, int> propertyIndexLookup;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="constructionProperties">The property values required to create an instance of <see cref="ClosingStructuresFailureMechanismProperties"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClosingStructuresFailureMechanismProperties(ClosingStructuresFailureMechanism data,
                                                           ConstructionProperties constructionProperties)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            Data = data;

            propertyIndexLookup = new Dictionary<string, int>
            {
                {
                    nameof(Name), constructionProperties.NamePropertyIndex
                },
                {
                    nameof(Code), constructionProperties.CodePropertyIndex
                },
                {
                    nameof(Group), constructionProperties.GroupPropertyIndex
                },
                {
                    nameof(Contribution), constructionProperties.ContributionPropertyIndex
                },
                {
                    nameof(C), constructionProperties.CPropertyIndex
                },
                {
                    nameof(N2A), constructionProperties.N2APropertyIndex
                },
                {
                    nameof(N), constructionProperties.NPropertyIndex
                }
            };
        }

        [DynamicPropertyOrderEvaluationMethod]
        public int DynamicPropertyOrderEvaluationMethod(string propertyName)
        {
            propertyIndexLookup.TryGetValue(propertyName, out int propertyIndex);

            return propertyIndex;
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="ClosingStructuresFailureMechanismProperties"/>.
        /// </summary>
        public class ConstructionProperties
        {
            #region Length effect parameters

            /// <summary>
            /// Gets or sets the property index for <see cref="ClosingStructuresFailureMechanismProperties.C"/>.
            /// </summary>
            public int CPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="ClosingStructuresFailureMechanismProperties.N2A"/>.
            /// </summary>
            public int N2APropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="ClosingStructuresFailureMechanismProperties.N"/>.
            /// </summary>
            public int NPropertyIndex { get; set; }

            #endregion

            #region General

            /// <summary>
            /// Gets or sets the property index for <see cref="ClosingStructuresFailureMechanismProperties.Name"/>.
            /// </summary>
            public int NamePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="ClosingStructuresFailureMechanismProperties.Code"/>.
            /// </summary>
            public int CodePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="ClosingStructuresFailureMechanismProperties.Group"/>.
            /// </summary>
            public int GroupPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="ClosingStructuresFailureMechanismProperties.Contribution"/>.
            /// </summary>
            public int ContributionPropertyIndex { get; set; }

            #endregion
        }

        #region Length effect parameters

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ClosingStructuresFailureMechanismProperties_C_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ClosingStructuresFailureMechanismProperties_C_Description))]
        public RoundedDouble C
        {
            get
            {
                return data.GeneralInput.C;
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ClosingStructuresFailureMechanismProperties_N2A_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ClosingStructuresFailureMechanismProperties_N2A_Description))]
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

        [DynamicPropertyOrder]
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

        #region General

        [DynamicPropertyOrder]
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

        [DynamicPropertyOrder]
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

        [DynamicPropertyOrder]
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

        [DynamicPropertyOrder]
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
    }
}