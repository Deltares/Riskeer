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
using System.Collections.Generic;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.WaveImpactAsphaltCover.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerRevetmentFormsResources = Riskeer.Revetment.Forms.Properties.Resources;

namespace Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses
{
    public class WaveImpactAsphaltCoverFailureMechanismProperties : ObjectProperties<WaveImpactAsphaltCoverFailureMechanism>
    {
        private readonly Dictionary<string, int> propertyIndexLookup;

        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverCalculationsProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="constructionProperties">The property values required to create an instance of <see cref="WaveImpactAsphaltCoverFailureMechanismProperties"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/>
        /// is <c>null</c>.</exception>
        public WaveImpactAsphaltCoverFailureMechanismProperties(WaveImpactAsphaltCoverFailureMechanism data,
                                                                ConstructionProperties constructionProperties)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
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
        /// Class holding the various construction parameters for <see cref="WaveImpactAsphaltCoverFailureMechanismProperties"/>.
        /// </summary>
        public class ConstructionProperties
        {
            #region General

            /// <summary>
            /// Gets or sets the property index for <see cref="WaveImpactAsphaltCoverFailureMechanismProperties.Name"/>.
            /// </summary>
            public int NamePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="WaveImpactAsphaltCoverFailureMechanismProperties.Code"/>.
            /// </summary>
            public int CodePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="WaveImpactAsphaltCoverFailureMechanismProperties.Group"/>.
            /// </summary>
            public int GroupPropertyIndex { get; set; }

            #endregion
        }

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

        #endregion
    }
}