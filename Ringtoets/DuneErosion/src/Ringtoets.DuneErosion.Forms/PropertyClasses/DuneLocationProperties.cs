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
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DuneLocation"/> for the properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DuneLocationProperties : ObjectProperties<DuneLocation>
    {
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Id_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Id_Description")]
        public long Id
        {
            get
            {
                return data.Id;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocation_CoastalAreaId_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_CoastalAreaId_Description")]
        public int CoastalAreaId
        {
            get
            {
                return data.CoastalAreaId;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocation_Offset_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_Offset_Description")]
        public RoundedDouble Offset
        {
            get
            {
                return data.Offset;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Coordinates_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Coordinates_Description")]
        public Point2D Location
        {
            get
            {
                return data.Location;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocation_WaterLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_WaterLevel_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaterLevel
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.WaterLevel;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocation_WaveHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_WaveHeight_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaveHeight
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.WaveHeight;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocation_WavePeriod_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_WavePeriod_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WavePeriod
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.WavePeriod;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocation_D50_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_D50_Description")]
        public RoundedDouble D50
        {
            get
            {
                return data.D50;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_TargetProbability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "CalculationOutput_TargetProbability_Description")]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double TargetProbability
        {
            get
            {
                return data.Output == null
                           ? double.NaN
                           : data.Output.TargetProbability;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_TargetReliability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "CalculationOutput_TargetReliability_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble TargetReliability
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.TargetReliability;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_CalculatedProbability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "CalculationOutput_CalculatedProbability_Description")]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                return data.Output == null
                           ? double.NaN
                           : data.Output.CalculatedProbability;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_CalculatedReliability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "CalculationOutput_CalculatedReliability_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.CalculatedReliability;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_Convergence_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_Convergence_Description")]
        public string Convergence
        {
            get
            {
                return new EnumDisplayWrapper<CalculationConvergence>(data.Output == null
                                                                          ? CalculationConvergence.NotCalculated
                                                                          : data.Output.CalculationConvergence).DisplayName;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Location);
        }
    }
}