// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.Properties;
using Riskeer.Revetment.Forms.PropertyClasses;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Forms.PropertyClasses
{
    /// <summary>
    /// Hydraulic loads related ViewModel of <see cref="GrassCoverErosionOutwardsFailureMechanism"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionOutwardsHydraulicLoadsProperties : GrassCoverErosionOutwardsFailureMechanismProperties
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int contributionPropertyIndex = 4;
        private const int waveRunUpPropertyIndex = 5;
        private const int waveImpactPropertyIndex = 6;
        private const int tailorMadeWaveImpactPropertyIndex = 7;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsHydraulicLoadsProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsHydraulicLoadsProperties(
            GrassCoverErosionOutwardsFailureMechanism data) : base(data, new ConstructionProperties
        {
            NamePropertyIndex = namePropertyIndex,
            CodePropertyIndex = codePropertyIndex,
            GroupPropertyIndex = groupPropertyIndex,
            ContributionPropertyIndex = contributionPropertyIndex
        }) {}

        #region Model settings

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

        [PropertyOrder(tailorMadeWaveImpactPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsFailureMechanismProperties_TailorMadeWaveImpact_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsFailureMechanismProperties_TailorMadeWaveImpact_Description))]
        public GeneralWaveConditionsInputProperties TailorMadeWaveImpact
        {
            get
            {
                return new GeneralWaveConditionsInputProperties
                {
                    Data = data.GeneralInput.GeneralTailorMadeWaveImpactWaveConditionsInput
                };
            }
        }

        #endregion
    }
}