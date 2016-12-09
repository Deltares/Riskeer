// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryLocation"/> with <see cref="WaveHeight"/> for properties panel.
    /// </summary>
    public class WaveHeightLocationContextProperties : HydraulicBoundaryLocationProperties
    {
        [PropertyOrder(1)]
        public override long Id
        {
            get
            {
                return base.Id;
            }
        }

        [PropertyOrder(2)]
        public override string Name
        {
            get
            {
                return base.Name;
            }
        }

        [PropertyOrder(3)]
        public override Point2D Location
        {
            get
            {
                return base.Location;
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryDatabase_Location_WaveHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_Location_WaveHeight_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaveHeight
        {
            get
            {
                return data.HydraulicBoundaryLocation.WaveHeight;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocationOutput_TargetProbability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocationOutput_TargetProbability_Description")]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double TargetProbability
        {
            get
            {
                HydraulicBoundaryLocationOutput output = data.HydraulicBoundaryLocation.WaveHeightOutput;
                return output == null ? double.NaN : output.TargetProbability;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocationOutput_TargetReliability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocationOutput_TargetReliability_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble TargetReliability
        {
            get
            {
                HydraulicBoundaryLocationOutput output = data.HydraulicBoundaryLocation.WaveHeightOutput;
                return output != null ? output.TargetReliability : RoundedDouble.NaN;
            }
        }

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocationOutput_CalculatedProbability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocationOutput_CalculatedProbability_Description")]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                HydraulicBoundaryLocationOutput output = data.HydraulicBoundaryLocation.WaveHeightOutput;
                return output == null ? double.NaN : output.CalculatedProbability;
            }
        }

        [PropertyOrder(8)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocationOutput_CalculatedReliability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocationOutput_CalculatedReliability_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                HydraulicBoundaryLocationOutput output = data.HydraulicBoundaryLocation.WaveHeightOutput;
                return output != null ? output.CalculatedReliability : RoundedDouble.NaN;
            }
        }

        [PropertyOrder(9)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryDatabase_Convergence_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_Convergence_WaveHeight_Description")]
        public string Convergence
        {
            get
            {
                HydraulicBoundaryLocationOutput output = data.HydraulicBoundaryLocation.WaveHeightOutput;
                return new EnumDisplayWrapper<CalculationConvergence>(output != null
                                                                          ? output.CalculationConvergence
                                                                          : CalculationConvergence.NotCalculated).DisplayName;
            }
        }
    }
}