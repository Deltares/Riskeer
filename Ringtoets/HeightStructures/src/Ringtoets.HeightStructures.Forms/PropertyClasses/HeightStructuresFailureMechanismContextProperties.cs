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
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HeightStructuresFailureMechanismContext"/> for properties panel.
    /// </summary>
    public class HeightStructuresFailureMechanismContextProperties : ObjectProperties<HeightStructuresFailureMechanismContext>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int gravitationalAccelerationPropertyIndex = 3;
        private const int lengthEffectPropertyIndex = 4;
        private const int modelfactorOvertoppingFlowPropertyIndex = 5;
        private const int modelFactorForStorageVolumePropertyIndex = 6;

        #region Length effect parameters

        [PropertyOrder(lengthEffectPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonDataResources), "Categories_LengthEffect")]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), "FailureMechanism_N_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonDataResources), "FailureMechanism_N_Description")]
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
        [ResourcesCategory(typeof(RingtoetsCommonDataResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), "FailureMechanism_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonDataResources), "FailureMechanism_Name_Description")]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
        }

        [PropertyOrder(codePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonDataResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonDataResources), "FailureMechanism_Code_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonDataResources), "FailureMechanism_Code_Description")]
        public string Code
        {
            get
            {
                return data.WrappedData.Code;
            }
        }

        [PropertyOrder(gravitationalAccelerationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonDataResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "HeightStructuresInputFailureMechanismContext_GravitationalAcceleration_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HeightStructuresInputFailureMechanismContext_GravitationalAcceleration_Description")]
        public RoundedDouble GravitationalAcceleration
        {
            get
            {
                return data.WrappedData.GeneralInput.GravitationalAcceleration;
            }
        }

        #endregion

        #region Model settings

        [PropertyOrder(modelfactorOvertoppingFlowPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "HeightStructuresInputFailureMechanismContext_ModelFactorOvertoppingFlow_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HeightStructuresInputFailureMechanismContext_ModelFactorOvertoppingFlow_Description")]
        public LogNormalDistributionProperties ModelFactorOvertoppingFlow
        {
            get
            {
                return new LogNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.ModelFactorOvertoppingFlow
                };
            }
        }

        [PropertyOrder(modelFactorForStorageVolumePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "HeightStructuresInputFailureMechanismContext_ModelFactorForStorageVolume_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HeightStructuresInputFailureMechanismContext_ModelFactorForStorageVolume_Description")]
        public LogNormalDistributionProperties ModelFactorForStorageVolume
        {
            get
            {
                return new LogNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.ModelFactorForStorageVolume
                };
            }
        }

        #endregion
    }
}