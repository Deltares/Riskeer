﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsOutput"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsOutputProperties : ObjectProperties<GrassCoverErosionInwardsOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsOutputProperties"/>.
        /// </summary>
        /// <param name="grassCoverErosionInwardsOutput">The grass cover erosion inwards output to create the object properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="grassCoverErosionInwardsOutput"/> is <c>null</c>.</exception>
        public GrassCoverErosionInwardsOutputProperties(GrassCoverErosionInwardsOutput grassCoverErosionInwardsOutput)
        {
            if (grassCoverErosionInwardsOutput == null)
            {
                throw new ArgumentNullException(nameof(grassCoverErosionInwardsOutput));
            }

            Data = grassCoverErosionInwardsOutput;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return propertyName.Contains(nameof(DikeHeight)) && data.DikeHeightOutput != null
                   || propertyName.Contains(nameof(OvertoppingRate)) && data.OvertoppingRateOutput != null
                   || propertyName.Equals(nameof(WaveHeight)) && !double.IsNaN(data.OvertoppingOutput.WaveHeight);
        }

        #region GrassCoverErosionInwards result

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_OvertoppingOutput), 1, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Probability_Displayname))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Probability_Description))]
        public string Probability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(StatisticsConverter.ReliabilityToProbability(data.OvertoppingOutput.Reliability));
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_OvertoppingOutput), 1, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Reliability_Displayname))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ProbabilityAssessmentOutput_Reliability_Description))]
        public RoundedDouble Reliability
        {
            get
            {
                return new RoundedDouble(5, data.OvertoppingOutput.Reliability);
            }
        }

        [PropertyOrder(3)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_OvertoppingOutput), 1, 3)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_WaveHeight_Displayname))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_WaveHeight_Description))]
        public RoundedDouble WaveHeight
        {
            get
            {
                return data.OvertoppingOutput.WaveHeight;
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_OvertoppingOutput), 1, 3)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_IsOvertoppingDominant_Displayname))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_IsOvertoppingDominant_Description))]
        public bool IsOvertoppingDominant
        {
            get
            {
                return data.OvertoppingOutput.IsOvertoppingDominant;
            }
        }

        #endregion

        #region Dike height

        [PropertyOrder(5)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 2, 3)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_DikeHeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_DikeHeight_Description))]
        public RoundedDouble DikeHeight
        {
            get
            {
                return data.DikeHeightOutput?.DikeHeight
                       ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(6)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 2, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double DikeHeightTargetProbability
        {
            get
            {
                return data.DikeHeightOutput?.TargetProbability
                       ?? double.NaN;
            }
        }

        [PropertyOrder(7)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 2, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetReliability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble DikeHeightTargetReliability
        {
            get
            {
                return data.DikeHeightOutput?.TargetReliability
                       ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(8)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 2, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double DikeHeightCalculatedProbability
        {
            get
            {
                return data.DikeHeightOutput?.CalculatedProbability
                       ?? double.NaN;
            }
        }

        [PropertyOrder(9)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 2, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble DikeHeightCalculatedReliability
        {
            get
            {
                return data.DikeHeightOutput?.CalculatedReliability
                       ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(10)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.DikeHeight_DisplayName), 2, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_Convergence_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeHeightOutput_Convergence_Description))]
        public string DikeHeightConvergence
        {
            get
            {
                return EnumDisplayNameHelper.GetDisplayName(data.DikeHeightOutput?.CalculationConvergence
                                                            ?? CalculationConvergence.NotCalculated);
            }
        }

        #endregion

        #region Overtopping rate

        [PropertyOrder(11)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 3, 3)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_OvertoppingRate_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionInwardsOutput_OvertoppingRate_Description))]
        public RoundedDouble OvertoppingRate
        {
            get
            {
                return data.OvertoppingRateOutput != null
                           ? new RoundedDouble(2, data.OvertoppingRateOutput.OvertoppingRate * 1000)
                           : RoundedDouble.NaN;
            }
        }

        [PropertyOrder(12)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 3, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double OvertoppingRateTargetProbability
        {
            get
            {
                return data.OvertoppingRateOutput?.TargetProbability
                       ?? double.NaN;
            }
        }

        [PropertyOrder(13)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 3, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetReliability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_TargetReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble OvertoppingRateTargetReliability
        {
            get
            {
                return data.OvertoppingRateOutput?.TargetReliability
                       ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(14)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 3, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedProbability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedProbability_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double OvertoppingRateCalculatedProbability
        {
            get
            {
                return data.OvertoppingRateOutput?.CalculatedProbability
                       ?? double.NaN;
            }
        }

        [PropertyOrder(15)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 3, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedReliability_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_CalculatedReliability_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble OvertoppingRateCalculatedReliability
        {
            get
            {
                return data.OvertoppingRateOutput?.CalculatedReliability
                       ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(16)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.OvertoppingRate_DisplayName), 3, 3)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.CalculationOutput_Convergence_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.OvertoppingRateOutput_Convergence_Description))]
        public string OvertoppingRateConvergence
        {
            get
            {
                return EnumDisplayNameHelper.GetDisplayName(data.OvertoppingRateOutput?.CalculationConvergence
                                                            ?? CalculationConvergence.NotCalculated);
            }
        }

        #endregion
    }
}