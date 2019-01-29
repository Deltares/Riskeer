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
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.Revetment.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveConditionsOutput"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WaveConditionsOutputProperties : ObjectProperties<WaveConditionsOutput>
    {
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.WaterLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_WaterLevel_Description))]
        public RoundedDouble WaterLevel
        {
            get
            {
                return data.WaterLevel;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsOutput_WaveHeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_WaveHeight_Description))]
        public RoundedDouble WaveHeight
        {
            get
            {
                return data.WaveHeight;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsOutput_WavePeakPeriod_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_WavePeakPeriod_Description))]
        public RoundedDouble WavePeakPeriod
        {
            get
            {
                return data.WavePeakPeriod;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsOutput_WaveDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_WaveDirection_Description))]
        public RoundedDouble WaveDirection
        {
            get
            {
                return data.WaveDirection;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsOutput_WaveAngle_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_WaveAngle_Description))]
        public RoundedDouble WaveAngle
        {
            get
            {
                return data.WaveAngle;
            }
        }

        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsOutput_TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_TargetProbability_Description))]
        public double TargetProbability
        {
            get
            {
                return data.TargetProbability;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsOutput_TargetReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_TargetReliability_Description))]
        public RoundedDouble TargetReliability
        {
            get
            {
                return data.TargetReliability;
            }
        }

        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_CalculatedProbability_Description))]
        public double CalculatedProbability
        {
            get
            {
                return data.CalculatedProbability;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_CalculatedReliability_Description))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                return data.CalculatedReliability;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsOutput_Convergence_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsOutput_Convergence_Description))]
        public string Convergence
        {
            get
            {
                return new EnumDisplayWrapper<CalculationConvergence>(data.CalculationConvergence).DisplayName;
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}