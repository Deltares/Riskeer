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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.GrassCoverErosionInwards.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsOutput"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsOutputProperties : ObjectProperties<GrassCoverErosionInwardsOutput>
    {
        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.DikeHeightAssessmentOutput != null;
        }

        #region GrassCoverErosionInwards result

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result", 1, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_RequiredProbability_Displayname")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_RequiredProbability_Description")]
        public string RequiredProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.ProbabilityAssessmentOutput.RequiredProbability);
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result", 1, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_RequiredReliability_Displayname")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_RequiredReliability_Description")]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return data.ProbabilityAssessmentOutput.RequiredReliability;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result", 1, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_Probability_Displayname")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_Probability_Description")]
        public string Probability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.ProbabilityAssessmentOutput.Probability);
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result", 1, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_Reliability_Displayname")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_Reliability_Description")]
        public RoundedDouble Reliability
        {
            get
            {
                return data.ProbabilityAssessmentOutput.Reliability;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Result", 1, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_FactorOfSafety_Displayname")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "ProbabilityAssessmentOutput_FactorOfSafety_Description")]
        public RoundedDouble FactorOfSafety
        {
            get
            {
                return data.ProbabilityAssessmentOutput.FactorOfSafety;
            }
        }

        #endregion

        #region Indicative wave height

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_Indicative_WaveHeight", 2, 3)]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_WaveHeight_Displayname")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_WaveHeight_Description")]
        public RoundedDouble WaveHeight
        {
            get
            {
                return data.WaveHeight;
            }
        }

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_Indicative_WaveHeight", 2, 3)]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_IsOvertoppingDominant_Displayname")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_IsOvertoppingDominant_Description")]
        public bool IsOvertoppingDominant
        {
            get
            {
                return data.IsOvertoppingDominant;
            }
        }

        #endregion

        #region Dike height

        [PropertyOrder(8)]
        [DynamicVisible]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_DikeHeight_Result", 3, 3)]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_DikeHeight_DisplayName")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_DikeHeight_Description")]
        public RoundedDouble DikeHeight
        {
            get
            {
                return data.DikeHeight;
            }
        }

        [PropertyOrder(9)]
        [DynamicVisible]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_DikeHeight_Result", 3, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_TargetProbability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "CalculationOutput_TargetProbability_Description")]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double TargetProbability
        {
            get
            {
                DikeHeightAssessmentOutput output = data.DikeHeightAssessmentOutput;
                return output == null ? double.NaN : output.TargetProbability;
            }
        }

        [PropertyOrder(10)]
        [DynamicVisible]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_DikeHeight_Result", 3, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_TargetReliability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "CalculationOutput_TargetReliability_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble TargetReliability
        {
            get
            {
                DikeHeightAssessmentOutput output = data.DikeHeightAssessmentOutput;
                return output != null ? output.TargetReliability : RoundedDouble.NaN;
            }
        }

        [PropertyOrder(11)]
        [DynamicVisible]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_DikeHeight_Result", 3, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_CalculatedProbability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "CalculationOutput_CalculatedProbability_Description")]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                DikeHeightAssessmentOutput output = data.DikeHeightAssessmentOutput;
                return output == null ? double.NaN : output.CalculatedProbability;
            }
        }

        [PropertyOrder(12)]
        [DynamicVisible]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_DikeHeight_Result", 3, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_CalculatedReliability_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "CalculationOutput_CalculatedReliability_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                DikeHeightAssessmentOutput output = data.DikeHeightAssessmentOutput;
                return output != null ? output.CalculatedReliability : RoundedDouble.NaN;
            }
        }

        [PropertyOrder(13)]
        [DynamicVisible]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_DikeHeight_Result", 3, 3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "CalculationOutput_Convergence_DisplayName")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "DikeHeightAssessmentOutput_Convergence_Description")]
        public string Convergence
        {
            get
            {
                DikeHeightAssessmentOutput output = data.DikeHeightAssessmentOutput;
                return new EnumDisplayWrapper<CalculationConvergence>(output != null
                                                                          ? output.CalculationConvergence
                                                                          : CalculationConvergence.NotCalculated).DisplayName;
            }
        }

        #endregion
    }
}