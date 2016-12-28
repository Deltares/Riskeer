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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsFailureMechanismContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismContextProperties : ObjectProperties<GrassCoverErosionInwardsFailureMechanismContext>
    {
        private readonly IFailureMechanismPropertyChangeHandler propertyChangeHandler;
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int lengthEffectPropertyIndex = 3;
        private const int frunupModelFactorPropertyIndex = 4;
        private const int fbFactorPropertyIndex = 5;
        private const int fnFactorPropertyIndex = 6;
        private const int fshallowModelFactorPropertyIndex = 7;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismContextProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsFailureMechanismContextProperties(
            GrassCoverErosionInwardsFailureMechanismContext data, 
            IFailureMechanismPropertyChangeHandler handler)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            Data = data;
            propertyChangeHandler = handler;
        }

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
                if (propertyChangeHandler.ConfirmPropertyChange())
                {
                    data.WrappedData.GeneralInput.N = value;

                    var changedObjects = propertyChangeHandler.PropertyChanged(data.WrappedData);
                    foreach (IObservable changedObject in changedObjects)
                    {
                        changedObject.NotifyObservers();
                    }
                    data.WrappedData.NotifyObservers();
                }
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

        #endregion

        #region Model settings

        [PropertyOrder(frunupModelFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_FrunupModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_FrunupModelFactor_Description")]
        public TruncatedNormalDistributionProperties FrunupModelFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.FrunupModelFactor
                };
            }
        }

        [PropertyOrder(fbFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_FbFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_FbFactor_Description")]
        public TruncatedNormalDistributionProperties FbFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.FbFactor
                };
            }
        }

        [PropertyOrder(fnFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_FnFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_FnFactor_Description")]
        public TruncatedNormalDistributionProperties FnFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.FnFactor
                };
            }
        }

        [PropertyOrder(fshallowModelFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_FshallowModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_FshallowModelFactor_Description")]
        public TruncatedNormalDistributionProperties FshallowModelFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.FshallowModelFactor
                };
            }
        }

        #endregion
    }
}