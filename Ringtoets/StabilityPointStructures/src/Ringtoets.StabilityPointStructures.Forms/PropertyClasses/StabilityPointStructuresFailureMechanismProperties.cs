// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityPointStructuresFailureMechanism"/> for properties panel.
    /// </summary>
    public class StabilityPointStructuresFailureMechanismProperties : ObjectProperties<StabilityPointStructuresFailureMechanism>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int gravitationalAccelerationPropertyIndex = 3;
        private const int lengthEffectPropertyIndex = 4;
        private const int modelFactorStorageVolumePropertyIndex = 5;
        private const int modelFactorSubCriticalFlowPropertyIndex = 6;
        private const int modelFactorCollisionLoadPropertyIndex = 7;
        private const int modelFactorLoadEffectPropertyIndex = 8;
        private const int waveRatioMaxHNPropertyIndex = 9;
        private const int waveRatioMaxHStandardDeviationPropertyIndex = 10;

        #region Length effect parameters

        [PropertyOrder(lengthEffectPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_LengthEffect")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "FailureMechanism_N_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "FailureMechanism_N_Description")]
        public int LengthEffect
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

        #endregion

        #region General

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "FailureMechanism_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "FailureMechanism_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(codePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "FailureMechanism_Code_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "FailureMechanism_Code_Description")]
        public string Code
        {
            get
            {
                return data.Code;
            }
        }

        [PropertyOrder(gravitationalAccelerationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "GravitationalAcceleration_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "GravitationalAcceleration_Description")]
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "StructuresInputFailureMechanismContext_ModelFactorStorageVolume_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "StructuresInputFailureMechanismContext_ModelFactorStorageVolume_Description")]
        public LogNormalDistributionProperties ModelFactorStorageVolume
        {
            get
            {
                return new LogNormalDistributionProperties
                {
                    Data = data.GeneralInput.ModelFactorStorageVolume
                };
            }
        }

        [PropertyOrder(modelFactorSubCriticalFlowPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "StructuresInputFailureMechanismContext_ModelFactorSubCriticalFlow_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "StructuresInputFailureMechanismContext_ModelFactorSubCriticalFlow_Description")]
        public VariationCoefficientNormalDistributionProperties ModelFactorSubCriticalFlow
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties
                {
                    Data = data.GeneralInput.ModelFactorSubCriticalFlow
                };
            }
        }

        [PropertyOrder(modelFactorCollisionLoadPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_ModelFactorCollisionLoad_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_ModelFactorCollisionLoad_Description")]
        public VariationCoefficientNormalDistributionProperties ModelFactorCollisionLoad
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties
                {
                    Data = data.GeneralInput.ModelFactorCollisionLoad
                };
            }
        }

        [PropertyOrder(modelFactorLoadEffectPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_ModelFactorLoadEffect_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_ModelFactorLoadEffect_Description")]
        public NormalDistributionProperties ModelFactorLoadEffect
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.GeneralInput.ModelFactorLoadEffect
                };
            }
        }

        #endregion
    }
}