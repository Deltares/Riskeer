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

using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.StabilityStoneCover.Data.Properties;

namespace Ringtoets.StabilityStoneCover.Data
{
    /// <summary>
    /// Class holding information about a calculation for the <see cref="StabilityStoneCoverFailureMechanism"/>.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculation : Observable, ICalculation
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsCalculation"/>.
        /// </summary>
        public StabilityStoneCoverWaveConditionsCalculation()
        {
            Name = Resources.StabilityStoneCoverWaveConditionsCalculation_DefaultName;
        }

        public string Name { get; set; }
        public string Comments { get; set; }
        public bool HasOutput { get; private set; }
        public void ClearOutput()
        {
            throw new System.NotImplementedException();
        }

        public ICalculationInput GetObservableInput()
        {
            throw new System.NotImplementedException();
        }

        public ICalculationOutput GetObservableOutput()
        {
            throw new System.NotImplementedException();
        }
    }
}
