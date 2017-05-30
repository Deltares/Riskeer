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
using System.ComponentModel;
using Core.Common.Base.Data;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.MacroStabilityInwards.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="MacroStabilityInwardsCalculationScenario"/> in the <see cref="MacroStabilityInwardsScenariosView"/>.
    /// </summary>
    internal class MacroStabilityInwardsScenarioRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationRow"/>.
        /// </summary>
        /// <param name="macroStabilityInwardsCalculation">The <see cref="MacroStabilityInwardsCalculationScenario"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="macroStabilityInwardsCalculation"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsScenarioRow(MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculation)
        {
            if (macroStabilityInwardsCalculation == null)
            {
                throw new ArgumentNullException(nameof(macroStabilityInwardsCalculation));
            }

            MacroStabilityInwardsCalculation = macroStabilityInwardsCalculation;
        }

        /// <summary>
        /// Gets the <see cref="MacroStabilityInwardsCalculationScenario"/> this row contains.
        /// </summary>
        public MacroStabilityInwardsCalculationScenario MacroStabilityInwardsCalculation { get; }

        /// <summary>
        /// Gets or sets the <see cref="MacroStabilityInwardsCalculationScenario"/> is relevant.
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return MacroStabilityInwardsCalculation.IsRelevant;
            }
            set
            {
                MacroStabilityInwardsCalculation.IsRelevant = value;
                MacroStabilityInwardsCalculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the contribution of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble Contribution
        {
            get
            {
                return new RoundedDouble(0, MacroStabilityInwardsCalculation.Contribution * 100);
            }
            set
            {
                MacroStabilityInwardsCalculation.Contribution = (RoundedDouble) (value / 100);
                MacroStabilityInwardsCalculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the name of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return MacroStabilityInwardsCalculation.Name;
            }
        }

        /// <summary>
        /// Gets the failure probability of macro stability inwards of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public string FailureProbabilityMacroStabilityInwards
        {
            get
            {
                if (MacroStabilityInwardsCalculation.SemiProbabilisticOutput == null)
                {
                    return RingtoetsCommonFormsResources.RoundedRouble_No_result_dash;
                }
                return ProbabilityFormattingHelper.Format(MacroStabilityInwardsCalculation.SemiProbabilisticOutput.MacroStabilityInwardsProbability);
            }
        }
    }
}