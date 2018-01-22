// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingOutput"/> for properties panel.
    /// </summary>
    public class PipingOutputContextProperties : ObjectProperties<PipingOutput>
    {
        private readonly PipingSemiProbabilisticOutput semiProbabilisticOutput;

        /// <summary>
        /// Creates a new instance of <see cref="PipingOutputContextProperties"/>.
        /// </summary>
        /// <param name="output">The output to show the properties for.</param>
        /// <param name="semiProbabilisticOutput">The semi probabilistic output to
        /// show properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public PipingOutputContextProperties(PipingOutput output, PipingSemiProbabilisticOutput semiProbabilisticOutput)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (semiProbabilisticOutput == null)
            {
                throw new ArgumentNullException(nameof(semiProbabilisticOutput));
            }

            Data = output;
            this.semiProbabilisticOutput = semiProbabilisticOutput;
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.PipingOutputContext_Categories_Uplift), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_UpliftEffectiveStress_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_UpliftEffectiveStress_Description))]
        [PropertyOrder(1)]
        public RoundedDouble UpliftEffectiveStress
        {
            get
            {
                return data.UpliftEffectiveStress;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.PipingOutputContext_Categories_Uplift), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_UpliftFactorOfSafety_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_UpliftFactorOfSafety_Description))]
        [PropertyOrder(2)]
        public RoundedDouble UpliftFactorOfSafety
        {
            get
            {
                return semiProbabilisticOutput.UpliftFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.PipingOutputContext_Categories_Uplift), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_UpliftReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_UpliftReliability_Description))]
        [PropertyOrder(3)]
        public RoundedDouble UpliftReliability
        {
            get
            {
                return semiProbabilisticOutput.UpliftReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.PipingOutputContext_Categories_Uplift), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_UpliftProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_UpliftProbability_Description))]
        [PropertyOrder(4)]
        public string UpliftProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(semiProbabilisticOutput.UpliftProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_HeaveGradient_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_HeaveGradient_Description))]
        [PropertyOrder(11)]
        public RoundedDouble HeaveGradient
        {
            get
            {
                return data.HeaveGradient;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_HeaveFactorOfSafety_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_HeaveFactorOfSafety_Description))]
        [PropertyOrder(12)]
        public RoundedDouble HeaveFactorOfSafety
        {
            get
            {
                return semiProbabilisticOutput.HeaveFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_HeaveReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_HeaveReliability_Description))]
        [PropertyOrder(13)]
        public RoundedDouble HeaveReliability
        {
            get
            {
                return semiProbabilisticOutput.HeaveReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_HeaveProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_HeaveProbability_Description))]
        [PropertyOrder(14)]
        public string HeaveProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(semiProbabilisticOutput.HeaveProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerCreepCoefficient_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerCreepCoefficient_Description))]
        [PropertyOrder(21)]
        public RoundedDouble SellmeijerCreepCoefficient
        {
            get
            {
                return data.SellmeijerCreepCoefficient;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerCriticalFall_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerCriticalFall_Description))]
        [PropertyOrder(22)]
        public RoundedDouble SellmeijerCriticalFall
        {
            get
            {
                return data.SellmeijerCriticalFall;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerReducedFall_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerReducedFall_Description))]
        [PropertyOrder(23)]
        public RoundedDouble SellmeijerReducedFall
        {
            get
            {
                return data.SellmeijerReducedFall;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerFactorOfSafety_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerFactorOfSafety_Description))]
        [PropertyOrder(24)]
        public RoundedDouble SellmeijerFactorOfSafety
        {
            get
            {
                return semiProbabilisticOutput.SellmeijerFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerReliability_Description))]
        [PropertyOrder(25)]
        public RoundedDouble SellmeijerReliability
        {
            get
            {
                return semiProbabilisticOutput.SellmeijerReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_SellmeijerProbability_Description))]
        [PropertyOrder(26)]
        public string SellmeijerProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(semiProbabilisticOutput.SellmeijerProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.PipingOutputContext_Categories_Piping), 4, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_RequiredProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_RequiredProbability_Description))]
        [PropertyOrder(31)]
        public string RequiredProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(semiProbabilisticOutput.RequiredProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.PipingOutputContext_Categories_Piping), 4, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_RequiredReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_RequiredReliability_Description))]
        [PropertyOrder(32)]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return semiProbabilisticOutput.RequiredReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.PipingOutputContext_Categories_Piping), 4, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_PipingProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_PipingProbability_Description))]
        [PropertyOrder(33)]
        public string PipingProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(semiProbabilisticOutput.PipingProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.PipingOutputContext_Categories_Piping), 4, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_PipingReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_PipingReliability_Description))]
        [PropertyOrder(34)]
        public RoundedDouble PipingReliability
        {
            get
            {
                return semiProbabilisticOutput.PipingReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.PipingOutputContext_Categories_Piping), 4, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutputContext_PipingFactorOfSafety_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutputContext_PipingFactorOfSafety_Description))]
        [PropertyOrder(35)]
        public RoundedDouble PipingFactorOfSafety
        {
            get
            {
                return semiProbabilisticOutput.PipingFactorOfSafety;
            }
        }
    }
}