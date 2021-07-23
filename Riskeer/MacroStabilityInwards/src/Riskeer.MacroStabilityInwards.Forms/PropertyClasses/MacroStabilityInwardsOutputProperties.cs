// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsOutput"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsOutputProperties : ObjectProperties<MacroStabilityInwardsOutput>
    {
        private const int macroStabilityInwardsFactorOfStabilityIndex = 1;
        private const int macroStabilityInwardsProbabilityIndex = 2;
        private const int macroStabilityInwardsReliabilityIndex = 3;

        private readonly DerivedMacroStabilityInwardsOutput derivedOutput;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsOutputProperties"/>.
        /// </summary>
        /// <param name="output">The output to show the properties for.</param>
        /// <param name="modelFactor">The model factor used to calculate a reliability from a stability factor.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsOutputProperties(MacroStabilityInwardsOutput output,
                                                     double modelFactor)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            Data = output;

            derivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(output, modelFactor);
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_FactorOfStability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_FactorOfStability_Description))]
        [PropertyOrder(macroStabilityInwardsFactorOfStabilityIndex)]
        public RoundedDouble MacroStabilityInwardsFactorOfStability
        {
            get
            {
                return derivedOutput.FactorOfStability;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsProbability_Description))]
        [PropertyOrder(macroStabilityInwardsProbabilityIndex)]
        public string MacroStabilityInwardsProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(derivedOutput.MacroStabilityInwardsProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MacroStabilityInwards))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsReliability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsOutput_MacroStabilityInwardsReliability_Description))]
        [PropertyOrder(macroStabilityInwardsReliabilityIndex)]
        public RoundedDouble MacroStabilityInwardsReliability
        {
            get
            {
                return derivedOutput.MacroStabilityInwardsReliability;
            }
        }
    }
}