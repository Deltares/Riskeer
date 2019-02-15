// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.Properties;
using Riskeer.Revetment.Forms.PropertyClasses;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionOutwardsFailureMechanism"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanismProperties : ObjectProperties<GrassCoverErosionOutwardsFailureMechanism>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int contributionPropertyIndex = 4;
        private const int isRelevantPropertyIndex = 5;
        private const int nPropertyIndex = 6;
        private const int waveRunUpPropertyIndex = 7;
        private const int waveImpactPropertyIndex = 8;

        private readonly IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the properties for.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsFailureMechanismProperties(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism> handler)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Data = failureMechanism;
            propertyChangeHandler = handler;
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
                   || nameof(WaveRunUp).Equals(propertyName)
                   || nameof(WaveImpact).Equals(propertyName)
                   || nameof(N).Equals(propertyName);
        }

        #region General

        [PropertyOrder(namePropertyIndex)]
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

        [PropertyOrder(codePropertyIndex)]
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

        [PropertyOrder(groupPropertyIndex)]
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

        [DynamicVisible]
        [PropertyOrder(contributionPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Contribution_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Contribution_Description))]
        public double Contribution
        {
            get
            {
                return data.Contribution;
            }
        }

        [PropertyOrder(isRelevantPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_IsRelevant_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_IsRelevant_Description))]
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
        [PropertyOrder(waveRunUpPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsFailureMechanismProperties_WaveRunUp_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsFailureMechanismProperties_WaveRunUp_Description))]
        public GeneralWaveConditionsInputProperties WaveRunUp
        {
            get
            {
                return new GeneralWaveConditionsInputProperties
                {
                    Data = data.GeneralInput.GeneralWaveRunUpWaveConditionsInput
                };
            }
        }

        [DynamicVisible]
        [PropertyOrder(waveImpactPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsFailureMechanismProperties_WaveImpact_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsFailureMechanismProperties_WaveImpact_Description))]
        public GeneralWaveConditionsInputProperties WaveImpact
        {
            get
            {
                return new GeneralWaveConditionsInputProperties
                {
                    Data = data.GeneralInput.GeneralWaveImpactWaveConditionsInput
                };
            }
        }

        #endregion
    }
}