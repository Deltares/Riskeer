// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Forms.Properties;

namespace Riskeer.Piping.Forms.PropertyClasses.SemiProbabilistic
{
    /// <summary>
    /// ViewModel of <see cref="SemiProbabilisticPipingOutput"/> for properties panel.
    /// </summary>
    public class SemiProbabilisticPipingOutputProperties : ObjectProperties<SemiProbabilisticPipingOutput>
    {
        private readonly DerivedSemiProbabilisticPipingOutput derivedOutput;

        /// <summary>
        /// Creates a new instance of <see cref="SemiProbabilisticPipingOutputProperties"/>.
        /// </summary>
        /// <param name="output">The output to show the properties for.</param>
        /// <param name="norm">The norm to assess for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <c>null</c>.</exception>
        public SemiProbabilisticPipingOutputProperties(SemiProbabilisticPipingOutput output, double norm)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            Data = output;

            derivedOutput = DerivedSemiProbabilisticPipingOutputFactory.Create(output, norm);
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Uplift), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_UpliftEffectiveStress_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_UpliftEffectiveStress_Description))]
        [PropertyOrder(1)]
        public RoundedDouble UpliftEffectiveStress
        {
            get
            {
                return data.UpliftEffectiveStress;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Uplift), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_UpliftFactorOfSafety_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_UpliftFactorOfSafety_Description))]
        [PropertyOrder(2)]
        public RoundedDouble UpliftFactorOfSafety
        {
            get
            {
                return derivedOutput.UpliftFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Uplift), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_UpliftReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_UpliftReliability_Description))]
        [PropertyOrder(3)]
        public RoundedDouble UpliftReliability
        {
            get
            {
                return derivedOutput.UpliftReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Uplift), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_UpliftProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_UpliftProbability_Description))]
        [PropertyOrder(4)]
        public string UpliftProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(derivedOutput.UpliftProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_HeaveGradient_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_HeaveGradient_Description))]
        [PropertyOrder(11)]
        public RoundedDouble HeaveGradient
        {
            get
            {
                return data.HeaveGradient;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_HeaveFactorOfSafety_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_HeaveFactorOfSafety_Description))]
        [PropertyOrder(12)]
        public RoundedDouble HeaveFactorOfSafety
        {
            get
            {
                return derivedOutput.HeaveFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_HeaveReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_HeaveReliability_Description))]
        [PropertyOrder(13)]
        public RoundedDouble HeaveReliability
        {
            get
            {
                return derivedOutput.HeaveReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_HeaveProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_HeaveProbability_Description))]
        [PropertyOrder(14)]
        public string HeaveProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(derivedOutput.HeaveProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerCreepCoefficient_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerCreepCoefficient_Description))]
        [PropertyOrder(21)]
        public RoundedDouble SellmeijerCreepCoefficient
        {
            get
            {
                return data.SellmeijerCreepCoefficient;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerCriticalFall_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerCriticalFall_Description))]
        [PropertyOrder(22)]
        public RoundedDouble SellmeijerCriticalFall
        {
            get
            {
                return data.SellmeijerCriticalFall;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerReducedFall_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerReducedFall_Description))]
        [PropertyOrder(23)]
        public RoundedDouble SellmeijerReducedFall
        {
            get
            {
                return data.SellmeijerReducedFall;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerFactorOfSafety_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerFactorOfSafety_Description))]
        [PropertyOrder(24)]
        public RoundedDouble SellmeijerFactorOfSafety
        {
            get
            {
                return derivedOutput.SellmeijerFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerReliability_Description))]
        [PropertyOrder(25)]
        public RoundedDouble SellmeijerReliability
        {
            get
            {
                return derivedOutput.SellmeijerReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer), 3, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_SellmeijerProbability_Description))]
        [PropertyOrder(26)]
        public string SellmeijerProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(derivedOutput.SellmeijerProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Piping), 4, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_PipingProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_PipingProbability_Description))]
        [PropertyOrder(31)]
        public string PipingProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(derivedOutput.PipingProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Piping), 4, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingOutput_PipingReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingOutput_PipingReliability_Description))]
        [PropertyOrder(32)]
        public RoundedDouble PipingReliability
        {
            get
            {
                return derivedOutput.PipingReliability;
            }
        }
    }
}