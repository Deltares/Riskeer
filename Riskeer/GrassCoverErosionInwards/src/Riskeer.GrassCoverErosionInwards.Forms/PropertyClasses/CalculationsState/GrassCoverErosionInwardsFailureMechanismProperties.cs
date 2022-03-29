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
using System.ComponentModel;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses.CalculationsState
{
    /// <summary>
    /// Calculations state related ViewModel of <see cref="GrassCoverErosionInwardsFailureMechanism"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismProperties : GrassCoverErosionInwardsFailureMechanismPropertiesBase
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int frunupModelFactorPropertyIndex = 3;
        private const int fbFactorPropertyIndex = 4;
        private const int fnFactorPropertyIndex = 5;
        private const int fshallowModelFactorPropertyIndex = 6;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsFailureMechanismProperties(GrassCoverErosionInwardsFailureMechanism data)
            : base(data, new ConstructionProperties
        {
            NamePropertyIndex = namePropertyIndex,
            CodePropertyIndex = codePropertyIndex
        }) {}

        #region Model settings

        [PropertyOrder(frunupModelFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FrunupModelFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FrunupModelFactor_Description))]
        public TruncatedNormalDistributionProperties FrunupModelFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties(data.GeneralInput.FrunupModelFactor);
            }
        }

        [PropertyOrder(fbFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FbFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FbFactor_Description))]
        public TruncatedNormalDistributionProperties FbFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties(data.GeneralInput.FbFactor);
            }
        }

        [PropertyOrder(fnFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FnFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FnFactor_Description))]
        public TruncatedNormalDistributionProperties FnFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties(data.GeneralInput.FnFactor);
            }
        }

        [PropertyOrder(fshallowModelFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FshallowModelFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FshallowModelFactor_Description))]
        public TruncatedNormalDistributionProperties FshallowModelFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties(data.GeneralInput.FshallowModelFactor);
            }
        }

        #endregion
    }
}