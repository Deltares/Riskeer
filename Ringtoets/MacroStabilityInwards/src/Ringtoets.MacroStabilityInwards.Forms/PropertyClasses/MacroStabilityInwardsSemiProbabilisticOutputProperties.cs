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
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsSemiProbabilisticOutputProperties : ObjectProperties<MacroStabilityInwardsSemiProbabilisticOutput>
    {
        private const int macroStabilityInwardsFactorOfStabilityIndex = 1;
        private const int requiredProbabilityIndex = 2;
        private const int requiredReliabilityIndex = 3;
        private const int macroStabilityInwardsProbabilityIndex = 4;
        private const int macroStabilityInwardsReliabilityIndex = 5;
        private const int macroStabilityInwardsFactorOfSafetyIndex = 6;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSemiProbabilisticOutputProperties"/>.
        /// </summary>
        /// <param name="semiProbabilisticOutput">The semi probabilistic output to create the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="semiProbabilisticOutput"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsSemiProbabilisticOutputProperties(MacroStabilityInwardsSemiProbabilisticOutput semiProbabilisticOutput)
        {
            if (semiProbabilisticOutput == null)
            {
                throw new ArgumentNullException(nameof(semiProbabilisticOutput));
            }
            Data = semiProbabilisticOutput;
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_FactorOfStability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_FactorOfStability_Description))]
        [PropertyOrder(macroStabilityInwardsFactorOfStabilityIndex)]
        public RoundedDouble MacroStabilityInwardsFactorOfStability
        {
            get
            {
                return data.FactorOfStability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_RequiredProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_RequiredProbability_Description))]
        [PropertyOrder(requiredProbabilityIndex)]
        public string RequiredProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.RequiredProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_RequiredReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_RequiredReliability_Description))]
        [PropertyOrder(requiredReliabilityIndex)]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return data.RequiredReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_MacroStabilityInwardsProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_MacroStabilityInwardsProbability_Description))]
        [PropertyOrder(macroStabilityInwardsProbabilityIndex)]
        public string MacroStabilityInwardsProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.MacroStabilityInwardsProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_MacroStabilityInwardsReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_MacroStabilityInwardsReliability_Description))]
        [PropertyOrder(macroStabilityInwardsReliabilityIndex)]
        public RoundedDouble MacroStabilityInwardsReliability
        {
            get
            {
                return data.MacroStabilityInwardsReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_MacroStabilityInwardsFactorOfSafety_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsSemiProbabilisticOutput_MacroStabilityInwardsFactorOfSafety_Description))]
        [PropertyOrder(macroStabilityInwardsFactorOfSafetyIndex)]
        public RoundedDouble MacroStabilityInwardsFactorOfSafety
        {
            get
            {
                return data.MacroStabilityInwardsFactorOfSafety;
            }
        }
    }
}