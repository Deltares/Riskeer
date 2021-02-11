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
using System.Collections.Generic;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Exceptions;

namespace Riskeer.HydraRing.Calculation.TestUtil.Calculator
{
    /// <summary>
    /// Base implementation of a Hydra-Ring calculator for testing purposes.
    /// </summary>
    public abstract class TestHydraRingCalculator<T>
    {
        public readonly HydraRingCalculationException HydraRingCalculationException = new HydraRingCalculationException();
        private readonly List<T> receivedInputs = new List<T>();
        public event EventHandler CalculationFinishedHandler;

        public bool EndInFailure { get; set; }
        public bool IsCanceled { get; private set; }

        public IEnumerable<T> ReceivedInputs
        {
            get
            {
                return receivedInputs;
            }
        }

        public GeneralResult IllustrationPointsResult { get; set; }

        public void Calculate(T input)
        {
            if (EndInFailure)
            {
                throw HydraRingCalculationException;
            }

            receivedInputs.Add(input);

            CalculationFinished(EventArgs.Empty);
        }

        public void Cancel()
        {
            IsCanceled = true;
        }

        private void CalculationFinished(EventArgs e)
        {
            CalculationFinishedHandler?.Invoke(this, e);
        }
    }
}