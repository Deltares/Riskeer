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
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Revetment.Forms.PropertyClasses
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WaveConditionsOutputProperties : ObjectProperties<WaveConditionsOutput>
    {
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsOutput_WaterLevel_DisplayName")]
        public RoundedDouble WaterLevel
        {
            get
            {
                return data.WaterLevel;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsOutput_WaveHeight_DisplayName")]
        public RoundedDouble WaveHeight
        {
            get
            {
                return data.WaveHeight;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsOutput_WavePeakPeriod_DisplayName")]
        public RoundedDouble WavePeakPeriod
        {
            get
            {
                return data.WavePeakPeriod;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsOutput_WaveAngle_DisplayName")]
        public RoundedDouble WaveAngle
        {
            get
            {
                return data.WaveAngle;
            }
        }
    }
}