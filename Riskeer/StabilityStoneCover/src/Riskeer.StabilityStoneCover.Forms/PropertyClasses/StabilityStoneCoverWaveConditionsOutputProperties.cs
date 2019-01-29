// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityStoneCoverWaveConditionsOutput"/> for properties panel.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsOutputProperties : ObjectProperties<StabilityStoneCoverWaveConditionsOutput>
    {
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsOutputProperties_Blocks_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsOutputProperties_Blocks_Description))]
        public WaveConditionsOutputProperties[] Blocks
        {
            get
            {
                return data.BlocksOutput.Select(waveConditionsOutput => new WaveConditionsOutputProperties
                {
                    Data = waveConditionsOutput
                }).ToArray();
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsOutputProperties_Columns_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsOutputProperties_Columns_Description))]
        public WaveConditionsOutputProperties[] Columns
        {
            get
            {
                return data.ColumnsOutput.Select(waveConditionsOutput => new WaveConditionsOutputProperties
                {
                    Data = waveConditionsOutput
                }).ToArray();
            }
        }
    }
}