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
using Riskeer.Revetment.Forms.PropertyClasses;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Forms.PropertyClasses
{
    /// <summary>
    /// Hydraulic loads related ViewModel of <see cref="StabilityStoneCoverFailureMechanism"/> for properties panel.
    /// </summary>
    public class StabilityStoneCoverHydraulicLoadsProperties : StabilityStoneCoverFailureMechanismProperties
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int blocksPropertyIndex = 4;
        private const int columnsPropertyIndex = 5;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverHydraulicLoadsProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public StabilityStoneCoverHydraulicLoadsProperties(StabilityStoneCoverFailureMechanism data) 
            : base(data, new ConstructionProperties
        {
            NamePropertyIndex = namePropertyIndex,
            CodePropertyIndex = codePropertyIndex,
            GroupPropertyIndex = groupPropertyIndex
        }) {}

        #region Model settings

        [PropertyOrder(blocksPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditions_Blocks_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsFailureMechanism_Blocks_Description))]
        public GeneralWaveConditionsInputProperties Blocks
        {
            get
            {
                return new GeneralWaveConditionsInputProperties
                {
                    Data = data.GeneralInput.GeneralBlocksWaveConditionsInput
                };
            }
        }

        [PropertyOrder(columnsPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditions_Columns_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsFailureMechanism_Columns_Description))]
        public GeneralWaveConditionsInputProperties Columns
        {
            get
            {
                return new GeneralWaveConditionsInputProperties
                {
                    Data = data.GeneralInput.GeneralColumnsWaveConditionsInput
                };
            }
        }

        #endregion
    }
}