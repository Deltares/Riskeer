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

using System;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Calculator
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="TestHydraRingCalculatorFactory"/>
    /// for <see cref="HydraRingCalculatorFactory.Instance"/> while testing.
    /// Disposing an instance of this class will revert the
    /// <see cref="HydraRingCalculatorFactory.Instance"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new HydraRingCalculatorFactoryConfig())
    /// {
    ///     var testCalculator = (TestHydraRingCalculatorFactory) HydraRingCalculatorFactoryConfig.Instance;
    /// 
    ///     // Perform test with factory
    /// }
    /// </code>
    /// </example>
    public class HydraRingCalculatorFactoryConfig : IDisposable
    {
        private readonly IHydraRingCalculatorFactory previousFactory;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingCalculatorFactoryConfig"/>.
        /// Sets a <see cref="HydraRingCalculatorFactory"/> to
        /// <see cref="HydraRingCalculatorFactory.Instance"/>.
        /// </summary>
        public HydraRingCalculatorFactoryConfig()
        {
            previousFactory = HydraRingCalculatorFactory.Instance;
            HydraRingCalculatorFactory.Instance = new TestHydraRingCalculatorFactory();
        }

        /// <summary>
        /// Reverts the <see cref="HydraRingCalculatorFactory.Instance"/> to the value
        /// it had at time of construction of the <see cref="HydraRingCalculatorFactoryConfig"/>.
        /// </summary>
        public void Dispose()
        {
            HydraRingCalculatorFactory.Instance = previousFactory;
        }
    }
}