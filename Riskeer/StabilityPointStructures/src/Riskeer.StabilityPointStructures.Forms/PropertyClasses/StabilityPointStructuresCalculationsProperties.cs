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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Forms.PropertyClasses
{
    /// <summary>
    /// Calculation related ViewModel of <see cref="StabilityPointStructuresFailureMechanism"/> for properties panel.
    /// </summary>
    public class StabilityPointStructuresCalculationsProperties : StabilityPointStructuresFailureMechanismProperties
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int gravitationalAccelerationPropertyIndex = 4;
        private const int modelFactorStorageVolumePropertyIndex = 5;
        private const int modelFactorCollisionLoadPropertyIndex = 6;
        private const int modelFactorLoadEffectPropertyIndex = 7;
        private const int modelFactorLongThresholdPropertyIndex = 8;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationsProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationsProperties(StabilityPointStructuresFailureMechanism data) : base(data, new ConstructionProperties
        {
            NamePropertyIndex = namePropertyIndex,
            CodePropertyIndex = codePropertyIndex,
            GroupPropertyIndex = groupPropertyIndex
        }) {}

        #region General

        [PropertyOrder(gravitationalAccelerationPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.GravitationalAcceleration_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.GravitationalAcceleration_Description))]
        public RoundedDouble GravitationalAcceleration
        {
            get
            {
                return data.GeneralInput.GravitationalAcceleration;
            }
        }

        #endregion

        #region Model settings

        [PropertyOrder(modelFactorStorageVolumePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorStorageVolume_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorStorageVolume_Description))]
        public LogNormalDistributionProperties ModelFactorStorageVolume
        {
            get
            {
                return new LogNormalDistributionProperties(data.GeneralInput.ModelFactorStorageVolume);
            }
        }

        [PropertyOrder(modelFactorCollisionLoadPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityPointStructuresInputFailureMechanismContext_ModelFactorCollisionLoad_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StabilityPointStructuresInputFailureMechanismContext_ModelFactorCollisionLoad_Description))]
        public VariationCoefficientNormalDistributionProperties ModelFactorCollisionLoad
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(data.GeneralInput.ModelFactorCollisionLoad);
            }
        }

        [PropertyOrder(modelFactorLoadEffectPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityPointStructuresInputFailureMechanismContext_ModelFactorLoadEffect_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StabilityPointStructuresInputFailureMechanismContext_ModelFactorLoadEffect_Description))]
        public NormalDistributionProperties ModelFactorLoadEffect
        {
            get
            {
                return new NormalDistributionProperties(data.GeneralInput.ModelFactorLoadEffect);
            }
        }

        [PropertyOrder(modelFactorLongThresholdPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorLongThreshold_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorLongThreshold_Description))]
        public NormalDistributionProperties ModelFactorLongThreshold
        {
            get
            {
                return new NormalDistributionProperties(data.GeneralInput.ModelFactorLongThreshold);
            }
        }

        #endregion
    }
}