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

using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using CoreCommonResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    public class PipingSemiProbabilisticOutputProperties : ObjectProperties<PipingSemiProbabilisticOutput>
    {
        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift", 1, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftFactorOfSafety_Description")]
        [PropertyOrder(1)]
        public RoundedDouble UpliftFactorOfSafety
        {
            get
            {
                return data.UpliftFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift", 1, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftReliability_Description")]
        [PropertyOrder(2)]
        public RoundedDouble UpliftReliability
        {
            get
            {
                return data.UpliftReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift", 1, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftProbability_Description")]
        [PropertyOrder(3)]
        public string UpliftProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.UpliftProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Heave", 2, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveFactorOfSafety_Description")]
        [PropertyOrder(11)]
        public RoundedDouble HeaveFactorOfSafety
        {
            get
            {
                return data.HeaveFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Heave", 2, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveReliability_Description")]
        [PropertyOrder(12)]
        public RoundedDouble HeaveReliability
        {
            get
            {
                return data.HeaveReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Heave", 2, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveProbability_Description")]
        [PropertyOrder(13)]
        public string HeaveProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.HeaveProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer", 3, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerFactorOfSafety_Description")]
        [PropertyOrder(21)]
        public RoundedDouble SellmeijerFactorOfSafety
        {
            get
            {
                return data.SellmeijerFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer", 3, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerReliability_Description")]
        [PropertyOrder(22)]
        public RoundedDouble SellmeijerReliability
        {
            get
            {
                return data.SellmeijerReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer", 3, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerProbability_Description")]
        [PropertyOrder(23)]
        public string SellmeijerProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.SellmeijerProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredProbability_Description")]
        [PropertyOrder(31)]
        public string RequiredProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.RequiredProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredReliability_Description")]
        [PropertyOrder(32)]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return data.RequiredReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingProbability_Description")]
        [PropertyOrder(33)]
        public string PipingProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.PipingProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingReliability_Description")]
        [PropertyOrder(34)]
        public RoundedDouble PipingReliability
        {
            get
            {
                return data.PipingReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingFactorOfSafety_Description")]
        [PropertyOrder(35)]
        public RoundedDouble PipingFactorOfSafety
        {
            get
            {
                return data.PipingFactorOfSafety;
            }
        }
    }
}