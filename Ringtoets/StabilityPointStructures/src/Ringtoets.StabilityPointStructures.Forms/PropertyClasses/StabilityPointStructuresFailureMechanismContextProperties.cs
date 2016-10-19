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
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityPointStructuresFailureMechanismContext"/> for properties panel.
    /// </summary>
    public class StabilityPointStructuresFailureMechanismContextProperties : ObjectProperties<StabilityPointStructuresFailureMechanismContext>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int gravitationalAccelerationPropertyIndex = 3;
        private const int lengthEffectPropertyIndex = 4;
        private const int modelFactorStorageVolumePropertyIndex = 5;
        private const int modelFactorSubCriticalFlowPropertyIndex = 6;
        private const int modelFactorCollisionLoadPropertyIndex = 7;
        private const int modelFactorLoadEffectPropertyIndex = 8;
        private const int modelFactorInflowVolumePropertyIndex = 9;
        private const int modificationFactorWavesSlowlyVaryingPressureComponentPropertyIndex = 10;
        private const int modificationFactorDynamicOrImpulsivePressureComponentPropertyIndex = 11;
        private const int waveRatioMaxHNPropertyIndex = 12;
        private const int waveRatioMaxHStandardDeviationPropertyIndex = 13;

        #region Length effect parameters

        [PropertyOrder(lengthEffectPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_LengthEffect")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "FailureMechanism_N_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "FailureMechanism_N_Description")]
        public int LengthEffect
        {
            get
            {
                return data.WrappedData.GeneralInput.N;
            }
            set
            {
                data.WrappedData.GeneralInput.N = value;
                data.WrappedData.NotifyObservers();
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
                return data.WrappedData.Name;
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
                return data.WrappedData.Code;
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
                return data.WrappedData.GeneralInput.GravitationalAcceleration;
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
                    Data = data.WrappedData.GeneralInput.ModelFactorStorageVolume
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
                    Data = data.WrappedData.GeneralInput.ModelFactorSubCriticalFlow
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
                    Data = data.WrappedData.GeneralInput.ModelFactorCollisionLoad
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
                    Data = data.WrappedData.GeneralInput.ModelFactorLoadEffect
                };
            }
        }

        [PropertyOrder(modelFactorInflowVolumePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "StructuresInputFailureMechanismContext_ModelFactorInflowVolume_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "StructuresInputFailureMechanismContext_ModelFactorInflowVolume_Description")]
        public RoundedDouble ModelFactorInflowVolume
        {
            get
            {
                return data.WrappedData.GeneralInput.ModelFactorInflowVolume;
            }
        }

        [PropertyOrder(modificationFactorWavesSlowlyVaryingPressureComponentPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_ModificationFactorWavesSlowlyVaryingPressureComponent_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_ModificationFactorWavesSlowlyVaryingPressureComponent_Description")]
        public RoundedDouble ModificationFactorWavesSlowlyVaryingPressureComponent
        {
            get
            {
                return data.WrappedData.GeneralInput.ModificationFactorWavesSlowlyVaryingPressureComponent;
            }
        }

        [PropertyOrder(modificationFactorDynamicOrImpulsivePressureComponentPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_ModificationFactorDynamicOrImpulsivePressureComponent_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_ModificationFactorDynamicOrImpulsivePressureComponent_Description")]
        public RoundedDouble ModificationFactorDynamicOrImpulsivePressureComponent
        {
            get
            {
                return data.WrappedData.GeneralInput.ModificationFactorDynamicOrImpulsivePressureComponent;
            }
        }

        [PropertyOrder(waveRatioMaxHNPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_WaveRatioMaxHN_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_WaveRatioMaxHN_Description")]
        public RoundedDouble WaveRatioMaxHN
        {
            get
            {
                return data.WrappedData.GeneralInput.WaveRatioMaxHN;
            }
        }

        [PropertyOrder(waveRatioMaxHStandardDeviationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_WaveRatioMaxHStandardDeviation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityPointStructuresInputFailureMechanismContext_WaveRatioMaxHStandardDeviation_Description")]
        public RoundedDouble WaveRatioMaxHStandardDeviation
        {
            get
            {
                return data.WrappedData.GeneralInput.WaveRatioMaxHStandardDeviation;
            }
        }

        #endregion
    }
}