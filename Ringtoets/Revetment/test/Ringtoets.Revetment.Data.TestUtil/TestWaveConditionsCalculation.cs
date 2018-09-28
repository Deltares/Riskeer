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

using System;
using Core.Common.Base;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Revetment.Data.TestUtil
{
    /// <summary>
    /// Creates a simple <see cref="ICalculation{T}"/> implementation for
    /// wave conditions, which can have an object set as output.
    /// </summary>
    /// <typeparam name="T">The type of the wave conditions input contained
    /// by the calculation.</typeparam>
    public class TestWaveConditionsCalculation<T> : Observable, ICalculation<T>
        where T : WaveConditionsInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestWaveConditionsCalculation{T}"/>.
        /// </summary>
        /// <param name="input">The wave conditions input to set to the calculation.</param>
        public TestWaveConditionsCalculation(T input)
        {
            Name = RingtoetsCommonDataResources.Calculation_DefaultName;
            InputParameters = input;
        }

        public T InputParameters { get; }

        public bool ShouldCalculate
        {
            get
            {
                return !HasOutput;
            }
        }

        public bool HasOutput { get; }

        public void ClearOutput() {}

        #region Irrelevant for test

        public string Name { get; set; }

        public Comment Comments { get; }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}