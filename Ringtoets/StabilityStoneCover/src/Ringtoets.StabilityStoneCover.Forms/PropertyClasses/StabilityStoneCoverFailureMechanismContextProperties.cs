﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityStoneCoverFailureMechanismContext"/> for properties panel.
    /// </summary>
    public class StabilityStoneCoverFailureMechanismContextProperties : ObjectProperties<StabilityStoneCoverFailureMechanismContext>
    {
        #region General

        [PropertyOrder(1)]
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

        [PropertyOrder(2)]
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

        [PropertyOrder(3)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditions_Blocks_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsFailureMechanism_Blocks_Description")]
        public GeneralWaveConditionsInputProperties Blocks
        {
            get
            {
                return new GeneralWaveConditionsInputProperties
                {
                    Data = data.WrappedData.GeneralInput.GeneralBlocksWaveConditionsInput
                };
            }
        }

        [PropertyOrder(4)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditions_Columns_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsFailureMechanism_Columns_Description")]
        public GeneralWaveConditionsInputProperties Columns
        {
            get
            {
                return new GeneralWaveConditionsInputProperties
                {
                    Data = data.WrappedData.GeneralInput.GeneralColumnsWaveConditionsInput
                };
            }
        }

        #endregion
    }
}