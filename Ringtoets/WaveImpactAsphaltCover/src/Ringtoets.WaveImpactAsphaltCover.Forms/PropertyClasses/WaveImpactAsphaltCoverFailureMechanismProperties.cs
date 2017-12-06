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

using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.WaveImpactAsphaltCover.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsRevetmentFormsResources = Ringtoets.Revetment.Forms.Properties.Resources;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveImpactAsphaltCoverFailureMechanism"/> for properties panel.
    /// </summary>
    public class WaveImpactAsphaltCoverFailureMechanismProperties : ObjectProperties<WaveImpactAsphaltCoverFailureMechanism>
    {
        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (!data.IsRelevant && ShouldHidePropertyWhenFailureMechanismIrrelevant(propertyName))
            {
                return false;
            }
            return true;
        }

        private bool ShouldHidePropertyWhenFailureMechanismIrrelevant(string propertyName)
        {
            return nameof(A).Equals(propertyName)
                   || nameof(B).Equals(propertyName)
                   || nameof(C).Equals(propertyName);
        }

        #region General

        [PropertyOrder(1)]
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

        [PropertyOrder(2)]
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

        [PropertyOrder(3)]
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
        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsRevetmentFormsResources), nameof(RingtoetsRevetmentFormsResources.GeneralWaveConditionsInput_A_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsRevetmentFormsResources), nameof(RingtoetsRevetmentFormsResources.GeneralWaveConditionsInput_A_Description))]
        public RoundedDouble A
        {
            get
            {
                return data.GeneralInput.A;
            }
        }

        [DynamicVisible]
        [PropertyOrder(5)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsRevetmentFormsResources), nameof(RingtoetsRevetmentFormsResources.GeneralWaveConditionsInput_B_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsRevetmentFormsResources), nameof(RingtoetsRevetmentFormsResources.GeneralWaveConditionsInput_B_Description))]
        public RoundedDouble B
        {
            get
            {
                return data.GeneralInput.B;
            }
        }

        [DynamicVisible]
        [PropertyOrder(6)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RingtoetsRevetmentFormsResources), nameof(RingtoetsRevetmentFormsResources.GeneralWaveConditionsInput_C_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsRevetmentFormsResources), nameof(RingtoetsRevetmentFormsResources.GeneralWaveConditionsInput_C_Description))]
        public RoundedDouble C
        {
            get
            {
                return data.GeneralInput.C;
            }
        }

        #endregion
    }
}