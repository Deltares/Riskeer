﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Util.Attributes;
using Core.Common.Util.Enums;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using DuneErosionDataResources = Riskeer.DuneErosion.Data.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DuneLocationCalculation"/> for the properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DuneLocationCalculationProperties : ObjectProperties<DuneLocationCalculation>
    {
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationProperties"/>.
        /// </summary>
        /// <param name="calculation">The dune location calculation at stake.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public DuneLocationCalculationProperties(DuneLocationCalculation calculation, IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;

            Data = calculation;
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_Description))]
        public long Id
        {
            get
            {
                return data.DuneLocation.Id;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_Description))]
        public string Name
        {
            get
            {
                return data.DuneLocation.Name;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DuneLocation_CoastalAreaId_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DuneLocation_CoastalAreaId_Description))]
        public int CoastalAreaId
        {
            get
            {
                return data.DuneLocation.CoastalAreaId;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DuneLocation_Offset_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DuneLocation_Offset_Description))]
        public string Offset
        {
            get
            {
                return data.DuneLocation.Offset
                           .ToString(DuneErosionDataResources.DuneLocation_Offset_format,
                                     CultureInfo.InvariantCulture);
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Location_Coordinates_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Location_Coordinates_Description))]
        public Point2D Location
        {
            get
            {
                return data.DuneLocation.Location;
            }
        }
        
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Description))]
        public string HRDFileName
        {
            get
            {
                return assessmentSection.GetHydraulicBoundaryLocationLookup()[data.DuneLocation.HydraulicBoundaryLocation];
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_WaterLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_WaterLevel_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaterLevel
        {
            get
            {
                return data.Output?.WaterLevel ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_WaveHeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_WaveHeight_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaveHeight
        {
            get
            {
                return data.Output?.WaveHeight ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_WavePeriod_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_WavePeriod_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WavePeriod
        {
            get
            {
                return data.Output?.WavePeriod ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_MeanTidalAmplitude_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_MeanTidalAmplitude_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble MeanTidalAmplitude
        {
            get
            {
                return data.Output?.MeanTidalAmplitude ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_WaveDirectionalSpread_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_WaveDirectionalSpread_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaveDirectionalSpread
        {
            get
            {
                return data.Output?.WaveDirectionalSpread ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_TideSurgePhaseDifference_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DuneLocationCalculationOutput_TideSurgePhaseDifference_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble TideSurgePhaseDifference
        {
            get
            {
                return data.Output?.TideSurgePhaseDifference ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double TargetProbability
        {
            get
            {
                return data.Output?.TargetProbability ?? double.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetReliability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble TargetReliability
        {
            get
            {
                return data.Output?.TargetReliability ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                return data.Output?.CalculatedProbability ?? double.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                return data.Output?.CalculatedReliability ?? RoundedDouble.NaN;
            }
        }

        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_Convergence_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DuneLocation_Convergence_Description))]
        public string Convergence
        {
            get
            {
                CalculationConvergence convergence = data.Output?.CalculationConvergence ?? CalculationConvergence.NotCalculated;
                return EnumDisplayNameHelper.GetDisplayName(convergence);
            }
        }

        public override string ToString()
        {
            return $"{Name} {Location}";
        }
    }
}