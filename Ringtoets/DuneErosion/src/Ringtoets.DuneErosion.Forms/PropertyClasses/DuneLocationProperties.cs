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
using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using DuneErosionDataResources = Ringtoets.DuneErosion.Data.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DuneLocationContext"/> for the properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DuneLocationProperties : ObjectProperties<DuneLocationContext>
    {
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Id_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Id_Description")]
        public long Id
        {
            get
            {
                return data.DuneLocation.Id;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Name_Description")]
        public string Name
        {
            get
            {
                return data.DuneLocation.Name;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocation_CoastalAreaId_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_CoastalAreaId_Description")]
        public int CoastalAreaId
        {
            get
            {
                return data.DuneLocation.CoastalAreaId;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocation_Offset_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_Offset_Description")]
        public string Offset
        {
            get
            {
                return data.DuneLocation.Offset.ToString(DuneErosionDataResources.DuneLocation_Offset_format, CultureInfo.InvariantCulture);
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Coordinates_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Coordinates_Description")]
        public Point2D Location
        {
            get
            {
                return data.DuneLocation.Location;
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
                return data.DuneLocation.Output?.WaterLevel ?? RoundedDouble.NaN;
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
                return data.DuneLocation.Output?.WaveHeight ?? RoundedDouble.NaN;
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
                return data.DuneLocation.Output?.WavePeriod ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocation_D50_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_D50_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble D50
        {
            get
            {
                return data.DuneLocation.D50;
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
                return data.DuneLocation.Output?.TargetProbability ?? double.NaN;
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
                return data.DuneLocation.Output?.TargetReliability ?? RoundedDouble.NaN;
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
                return data.DuneLocation.Output?.CalculatedProbability ?? double.NaN;
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
                return data.DuneLocation.Output?.CalculatedReliability ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_Convergence_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocation_Convergence_Description")]
        public string Convergence
        {
            get
            {
                return new EnumDisplayWrapper<CalculationConvergence>(data.DuneLocation.Output?.CalculationConvergence ?? CalculationConvergence.NotCalculated).DisplayName;
            }
        }

        public override string ToString()
        {
            return $"{Name} {Location}";
        }
    }
}