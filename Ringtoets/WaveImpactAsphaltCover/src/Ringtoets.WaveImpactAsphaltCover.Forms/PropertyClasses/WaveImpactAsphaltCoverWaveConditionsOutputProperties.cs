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

using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveImpactAsphaltCoverWaveConditionsOutput"/> for properties panel
    /// </summary>
    public class WaveImpactAsphaltCoverWaveConditionsOutputProperties : ObjectProperties<WaveImpactAsphaltCoverWaveConditionsOutput>
    {
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveImpactAsphaltCoverWaveConditionsOutputProperties_HydraulicBoundaryLocationResult_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveImpactAsphaltCoverWaveConditionsOutputProperties_HydraulicBoundaryLocationResult_Description))]
        public WaveConditionsOutputProperties[] Items
        {
            get
            {
                return data.Items.Select(waveConditionOutput => new WaveConditionsOutputProperties
                {
                    Data = waveConditionOutput
                }).ToArray();
            }
        }
    }
}