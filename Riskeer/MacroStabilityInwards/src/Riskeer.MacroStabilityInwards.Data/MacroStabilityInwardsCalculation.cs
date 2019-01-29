// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Core.Common.Base;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// This class holds information about a calculation for the <see cref="MacroStabilityInwardsFailureMechanism"/>.
    /// </summary>
    public class MacroStabilityInwardsCalculation : CloneableObservable, ICalculation<MacroStabilityInwardsInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculation"/> with default values set for some of the parameters.
        /// </summary>
        public MacroStabilityInwardsCalculation()
        {
            Name = RiskeerCommonDataResources.Calculation_DefaultName;
            InputParameters = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());
            Comments = new Comment();
        }

        /// <summary>
        /// Gets or sets <see cref="MacroStabilityInwardsOutput"/>, which contains the results of a macro stability inwards calculation.
        /// </summary>
        public MacroStabilityInwardsOutput Output { get; set; }

        /// <summary>
        /// Gets the input parameters to perform a macro stability inwards calculation with.
        /// </summary>
        public MacroStabilityInwardsInput InputParameters { get; private set; }

        public string Name { get; set; }

        public bool ShouldCalculate
        {
            get
            {
                return !HasOutput;
            }
        }

        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public Comment Comments { get; private set; }

        public void ClearOutput()
        {
            Output = null;
        }

        public override object Clone()
        {
            var clone = (MacroStabilityInwardsCalculation) base.Clone();
            clone.Comments = (Comment) Comments.Clone();
            clone.InputParameters = (MacroStabilityInwardsInput) InputParameters.Clone();

            if (Output != null)
            {
                clone.Output = (MacroStabilityInwardsOutput) Output.Clone();
            }

            return clone;
        }
    }
}