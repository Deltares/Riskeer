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
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.WaveImpactAsphaltCover.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerRevetmentFormsResources = Riskeer.Revetment.Forms.Properties.Resources;

namespace Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveImpactAsphaltCoverFailureMechanism"/> for properties panel.
    /// </summary>
    public class WaveImpactAsphaltCoverCalculationsProperties : ObjectProperties<WaveImpactAsphaltCoverFailureMechanism>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int aPropertyIndex = 4;
        private const int bPropertyIndex = 5;
        private const int cPropertyIndex = 6;

        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverCalculationsProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public WaveImpactAsphaltCoverCalculationsProperties(WaveImpactAsphaltCoverFailureMechanism data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
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

        #endregion

        #region Model settings

        [PropertyOrder(aPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RiskeerRevetmentFormsResources), nameof(RiskeerRevetmentFormsResources.GeneralWaveConditionsInput_A_DisplayName))]
        [ResourcesDescription(typeof(RiskeerRevetmentFormsResources), nameof(RiskeerRevetmentFormsResources.GeneralWaveConditionsInput_A_Description))]
        public RoundedDouble A
        {
            get
            {
                return data.GeneralInput.A;
            }
        }

        [PropertyOrder(bPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RiskeerRevetmentFormsResources), nameof(RiskeerRevetmentFormsResources.GeneralWaveConditionsInput_B_DisplayName))]
        [ResourcesDescription(typeof(RiskeerRevetmentFormsResources), nameof(RiskeerRevetmentFormsResources.GeneralWaveConditionsInput_B_Description))]
        public RoundedDouble B
        {
            get
            {
                return data.GeneralInput.B;
            }
        }

        [PropertyOrder(cPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(RiskeerRevetmentFormsResources), nameof(RiskeerRevetmentFormsResources.GeneralWaveConditionsInput_C_DisplayName))]
        [ResourcesDescription(typeof(RiskeerRevetmentFormsResources), nameof(RiskeerRevetmentFormsResources.GeneralWaveConditionsInput_C_Description))]
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