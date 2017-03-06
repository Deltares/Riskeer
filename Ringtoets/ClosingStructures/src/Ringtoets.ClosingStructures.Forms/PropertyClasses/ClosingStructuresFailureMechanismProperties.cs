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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ClosingStructuresFailureMechanism"/> for properties panel.
    /// </summary>
    public class ClosingStructuresFailureMechanismProperties : ObjectProperties<ClosingStructuresFailureMechanism>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int isRelevantPropertyIndex = 3;
        private const int gravitationalAccelerationPropertyIndex = 4;

        private const int cPropertyIndex = 5;
        private const int n2APropertyIndex = 6;
        private const int lengthEffectPropertyIndex = 7;

        private const int modelFactorOvertoppingFlowPropertyIndex = 8;
        private const int modelFactorStorageVolumePropertyIndex = 9;
        private const int modelFactorSubCriticalFlowPropertyIndex = 10;
        private const int modelFactorInflowVolumePropertyIndex = 11;

        private readonly IFailureMechanismPropertyChangeHandler<ClosingStructuresFailureMechanism> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public ClosingStructuresFailureMechanismProperties(
            ClosingStructuresFailureMechanism data,
            IFailureMechanismPropertyChangeHandler<ClosingStructuresFailureMechanism> handler)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            Data = data;
            propertyChangeHandler = handler;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (!data.IsRelevant && ShouldHidePropertyWhenFailureMechanismIrrelevant(propertyName))
            {
                return false;
            }
            return true;
        }

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (var affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        private bool ShouldHidePropertyWhenFailureMechanismIrrelevant(string propertyName)
        {
            return nameof(GravitationalAcceleration).Equals(propertyName)
                   || nameof(ModelFactorOvertoppingFlow).Equals(propertyName)
                   || nameof(ModelFactorStorageVolume).Equals(propertyName)
                   || nameof(ModelFactorSubCriticalFlow).Equals(propertyName)
                   || nameof(ModelFactorInflowVolume).Equals(propertyName)
                   || nameof(C).Equals(propertyName)
                   || nameof(N2A).Equals(propertyName)
                   || nameof(LengthEffect).Equals(propertyName);
        }

        #region Length effect parameters

        [DynamicVisible]
        [PropertyOrder(cPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ClosingStructuresFailureMechanismContextProperties_C_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ClosingStructuresFailureMechanismContextProperties_C_Description))]
        public RoundedDouble C
        {
            get
            {
                return data.GeneralInput.C;
            }
        }

        [DynamicVisible]
        [PropertyOrder(n2APropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ClosingStructuresFailureMechanismContextProperties_N2A_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ClosingStructuresFailureMechanismContextProperties_N2A_Description))]
        public int N2A
        {
            get
            {
                return data.GeneralInput.N2A;
            }
            set
            {
                IEnumerable<IObservable> affectedObjects = propertyChangeHandler.SetPropertyValueAfterConfirmation(
                    data,
                    value,
                    (f, v) => f.GeneralInput.N2A = v);

                NotifyAffectedObjects(affectedObjects);
            }
        }

        [DynamicVisible]
        [PropertyOrder(lengthEffectPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_N_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_N_Description))]
        public RoundedDouble LengthEffect
        {
            get
            {
                return data.GeneralInput.N;
            }
        }

        #endregion

        #region General

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Name_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(codePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Code_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Code_Description))]
        public string Code
        {
            get
            {
                return data.Code;
            }
        }

        [PropertyOrder(isRelevantPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_IsRelevant_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_IsRelevant_Description))]
        public bool IsRelevant
        {
            get
            {
                return data.IsRelevant;
            }
        }

        [DynamicVisible]
        [PropertyOrder(gravitationalAccelerationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.GravitationalAcceleration_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.GravitationalAcceleration_Description))]
        public RoundedDouble GravitationalAcceleration
        {
            get
            {
                return data.GeneralInput.GravitationalAcceleration;
            }
        }

        #endregion

        #region Model settings

        [DynamicVisible]
        [PropertyOrder(modelFactorOvertoppingFlowPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorOvertoppingFlow_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorOvertoppingFlow_Description))]
        public LogNormalDistributionProperties ModelFactorOvertoppingFlow
        {
            get
            {
                return new LogNormalDistributionProperties(data.GeneralInput.ModelFactorOvertoppingFlow);
            }
        }

        [DynamicVisible]
        [PropertyOrder(modelFactorStorageVolumePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorStorageVolume_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorStorageVolume_Description))]
        public LogNormalDistributionProperties ModelFactorStorageVolume
        {
            get
            {
                return new LogNormalDistributionProperties(data.GeneralInput.ModelFactorStorageVolume);
            }
        }

        [DynamicVisible]
        [PropertyOrder(modelFactorSubCriticalFlowPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorSubCriticalFlow_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorSubCriticalFlow_Description))]
        public VariationCoefficientNormalDistributionProperties ModelFactorSubCriticalFlow
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(data.GeneralInput.ModelFactorSubCriticalFlow);
            }
        }

        [DynamicVisible]
        [PropertyOrder(modelFactorInflowVolumePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorInflowVolume_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.StructuresInputFailureMechanismContext_ModelFactorInflowVolume_Description))]
        public RoundedDouble ModelFactorInflowVolume
        {
            get
            {
                return data.GeneralInput.ModelFactorInflowVolume;
            }
        }

        #endregion
    }
}