﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Util.Attributes;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsFailureMechanism"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismProperties : ObjectProperties<GrassCoverErosionInwardsFailureMechanism>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int isRelevantPropertyIndex = 4;
        private const int nPropertyIndex = 5;
        private const int frunupModelFactorPropertyIndex = 6;
        private const int fbFactorPropertyIndex = 7;
        private const int fnFactorPropertyIndex = 8;
        private const int fshallowModelFactorPropertyIndex = 9;
        private readonly IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsFailureMechanismProperties(
            GrassCoverErosionInwardsFailureMechanism data,
            IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism> handler)
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

        #region Length effect parameters

        [DynamicVisible]
        [PropertyOrder(nPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_N_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_N_Description))]
        public RoundedDouble N
        {
            get
            {
                return data.GeneralInput.N;
            }
            set
            {
                IEnumerable<IObservable> affectedObjects = propertyChangeHandler.SetPropertyValueAfterConfirmation(
                    data,
                    value,
                    (f, v) => f.GeneralInput.N = v);

                NotifyAffectedObjects(affectedObjects);
            }
        }

        #endregion

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
            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        private bool ShouldHidePropertyWhenFailureMechanismIrrelevant(string propertyName)
        {
            return nameof(Contribution).Equals(propertyName)
                   || nameof(N).Equals(propertyName)
                   || nameof(FrunupModelFactor).Equals(propertyName)
                   || nameof(FbFactor).Equals(propertyName)
                   || nameof(FnFactor).Equals(propertyName)
                   || nameof(FshallowModelFactor).Equals(propertyName);
        }

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

        [DynamicVisible]
        [PropertyOrder(contributionPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Contribution_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Contribution_Description))]
        public double Contribution
        {
            get
            {
                return data.Contribution;
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

        #endregion

        #region Model settings

        [DynamicVisible]
        [PropertyOrder(frunupModelFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FrunupModelFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FrunupModelFactor_Description))]
        public TruncatedNormalDistributionProperties FrunupModelFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties(data.GeneralInput.FrunupModelFactor);
            }
        }

        [DynamicVisible]
        [PropertyOrder(fbFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FbFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FbFactor_Description))]
        public TruncatedNormalDistributionProperties FbFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties(data.GeneralInput.FbFactor);
            }
        }

        [DynamicVisible]
        [PropertyOrder(fnFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FnFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsInput_FnFactor_Description))]
        public TruncatedNormalDistributionProperties FnFactor
        {
            get
            {
                return new TruncatedNormalDistributionProperties(data.GeneralInput.FnFactor);
            }
        }

        [DynamicVisible]
        [PropertyOrder(fshallowModelFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
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