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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionInwards.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// Failure path related ViewModel of <see cref="GrassCoverErosionInwardsFailureMechanism"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsFailurePathProperties : GrassCoverErosionInwardsFailureMechanismProperties
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int contributionPropertyIndex = 4;
        private const int inAssemblyPropertyIndex = 5;
        private const int nPropertyIndex = 6;
        private const int applyLengthEffectInSectionPropertyIndex = 7;
        private readonly IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailurePathProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsFailurePathProperties(
            GrassCoverErosionInwardsFailureMechanism data,
            IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism> handler) :
            base(data, new ConstructionProperties
            {
                NamePropertyIndex = namePropertyIndex,
                CodePropertyIndex = codePropertyIndex
            })
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            propertyChangeHandler = handler;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.InAssembly || !ShouldHidePropertyWhenFailureMechanismNotPartOfAssembly(propertyName);
        }

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        private static bool ShouldHidePropertyWhenFailureMechanismNotPartOfAssembly(string propertyName)
        {
            return nameof(Contribution).Equals(propertyName)
                   || nameof(N).Equals(propertyName)
                   || nameof(ApplyLengthEffectInSection).Equals(propertyName);
        }

        #region Length effect parameters

        [DynamicVisible]
        [PropertyOrder(nPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_Description))]
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

        [DynamicVisible]
        [PropertyOrder(applyLengthEffectInSectionPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_Apply_LengthEffect_In_Section_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_Apply_LengthEffect_In_Section_Description))]
        public bool ApplyLengthEffectInSection
        {
            get
            {
                return data.GeneralInput.ApplyLengthEffectInSection;
            }
            set
            {
                data.GeneralInput.ApplyLengthEffectInSection = value;
                data.NotifyObservers();
            }
        }

        #endregion

        #region General

        [PropertyOrder(groupPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_Group_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_Group_Description))]
        public int Group
        {
            get
            {
                return data.Group;
            }
        }

        [DynamicVisible]
        [PropertyOrder(contributionPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_Contribution_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_Contribution_Description))]
        public double Contribution
        {
            get
            {
                return data.Contribution;
            }
        }

        [PropertyOrder(inAssemblyPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_InAssembly_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailurePath_InAssembly_Description))]
        public bool InAssembly
        {
            get
            {
                return data.InAssembly;
            }
        }

        #endregion
    }
}