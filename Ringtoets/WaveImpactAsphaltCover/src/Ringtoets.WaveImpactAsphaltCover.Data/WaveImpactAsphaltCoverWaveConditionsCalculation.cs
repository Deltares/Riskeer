﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Revetment.Data;
using Ringtoets.WaveImpactAsphaltCover.Data.Properties;

namespace Ringtoets.WaveImpactAsphaltCover.Data
{
    /// <summary>
    /// Class holding information about a wave conditions calculation for the <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
    /// </summary>
    public class WaveImpactAsphaltCoverWaveConditionsCalculation: Observable, ICalculation
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/>.
        /// </summary>
        public WaveImpactAsphaltCoverWaveConditionsCalculation()
        {
            Name = Resources.WaveImpactAsphaltCoverWaveConditionsCalculation_DefaultName;
            InputParameters = new WaveConditionsInput(WaveConditionsRevetment.Asphalt);
        }

        /// <summary>
        /// Gets the input parameters to perform a wave conditions calculation with.
        /// </summary>
        public WaveConditionsInput InputParameters { get; private set; }

        /// <summary>
        /// Gets or sets the output which contains the results of a wave conditions calculation.
        /// </summary>
        public WaveImpactAsphaltCoverWaveConditionsOutput Output { get; set; }

        public string Name { get; set; }

        public string Comments { get; set; }

        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public void ClearOutput()
        {
            Output = null;
        }

        public ICalculationInput GetObservableInput()
        {
            return InputParameters;
        }

        public ICalculationOutput GetObservableOutput()
        {
            return Output;
        }
    }
}