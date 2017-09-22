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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryLocation"/> with <see cref="DesignWaterLevel"/> for properties panel.
    /// </summary>
    public class DesignWaterLevelLocationContextProperties : HydraulicBoundaryLocationProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelLocationContextProperties"/>.
        /// </summary>
        public DesignWaterLevelLocationContextProperties()
            : base(new ConstructionProperties
            {
                IdIndex = 1,
                NameIndex = 2,
                LocationIndex = 3,
                GoverningWindDirectionIndex = 11,
                StochastsIndex = 12,
                DurationsIndex = 13,
                IllustrationPointsIndex = 14
            }) {}

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Location_DesignWaterLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Location_DesignWaterLevel_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble DesignWaterLevel
        {
            get
            {
                return data.WrappedData.DesignWaterLevel;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_TargetProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double TargetProbability
        {
            get
            {
                HydraulicBoundaryLocationOutput output = data.WrappedData.DesignWaterLevelCalculation.Output;
                return output?.TargetProbability ?? double.NaN;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_TargetReliability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_TargetReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble TargetReliability
        {
            get
            {
                HydraulicBoundaryLocationOutput output = data.WrappedData.DesignWaterLevelCalculation.Output;
                return output?.TargetReliability ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                HydraulicBoundaryLocationOutput output = data.WrappedData.DesignWaterLevelCalculation.Output;
                return output?.CalculatedProbability ?? double.NaN;
            }
        }

        [PropertyOrder(8)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                HydraulicBoundaryLocationOutput output = data.WrappedData.DesignWaterLevelCalculation.Output;
                return output?.CalculatedReliability ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(9)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_Convergence_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Convergence_DesignWaterLevel_Description))]
        public string Convergence
        {
            get
            {
                return new EnumDisplayWrapper<CalculationConvergence>(data.WrappedData.DesignWaterLevelCalculationConvergence).DisplayName;
            }
        }

        [PropertyOrder(10)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ShouldIllustrationPointsBeCalculated_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ShouldIllustrationPointsBeCalculated_Description))]
        public bool ShouldIllustrationPointsBeCalculated
        {
            get
            {
                return data.WrappedData.DesignWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated;
            }
            set
            {
                data.WrappedData.DesignWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated = value;
                data.HydraulicBoundaryDatabase.NotifyObservers();
            }
        }

        protected override GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResult()
        {
            if (data.WrappedData.DesignWaterLevelCalculation.HasOutput
                && data.WrappedData.DesignWaterLevelCalculation.Output.HasGeneralResult)
            {
                return data.WrappedData.DesignWaterLevelCalculation.Output.GeneralResult;
            }
            return null;
        }
    }
}