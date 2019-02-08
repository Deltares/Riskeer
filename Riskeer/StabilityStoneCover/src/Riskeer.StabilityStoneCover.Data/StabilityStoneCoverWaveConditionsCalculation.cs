// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Core.Common.Base;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Revetment.Data;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Data
{
    /// <summary>
    /// Class holding information about a wave conditions calculation for the <see cref="StabilityStoneCoverFailureMechanism"/>.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculation : CloneableObservable, ICalculation<StabilityStoneCoverWaveConditionsInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsCalculation"/>.
        /// </summary>
        public StabilityStoneCoverWaveConditionsCalculation()
        {
            Name = RiskeerCommonDataResources.Calculation_DefaultName;
            InputParameters = new StabilityStoneCoverWaveConditionsInput();
            Comments = new Comment();
        }

        /// <summary>
        /// Gets or sets the output which contains the results of a wave conditions calculation.
        /// </summary>
        public StabilityStoneCoverWaveConditionsOutput Output { get; set; }

        public StabilityStoneCoverWaveConditionsInput InputParameters { get; private set; }

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
            var clone = (StabilityStoneCoverWaveConditionsCalculation) base.Clone();

            clone.Comments = (Comment) Comments.Clone();
            clone.InputParameters = (StabilityStoneCoverWaveConditionsInput) InputParameters.Clone();

            if (Output != null)
            {
                clone.Output = (StabilityStoneCoverWaveConditionsOutput) Output.Clone();
            }

            return clone;
        }
    }
}