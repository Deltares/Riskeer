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

using System;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="TestMacroStabilityInwardsCalculatorFactory"/> 
    /// for <see cref="MacroStabilityInwardsCalculatorFactory.Instance"/> while testing. 
    /// Disposing an instance of this class will revert the <see cref="MacroStabilityInwardsCalculatorFactory.Instance"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new MacroStabilityInwardsCalculatorFactoryConfig())
    /// {
    ///     var testFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
    /// 
    ///     // Perform tests with testFactory
    /// }
    /// </code>
    /// </example>
    public class MacroStabilityInwardsCalculatorFactoryConfig : IDisposable
    {
        private readonly IMacroStabilityInwardsCalculatorFactory previousFactory;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculatorFactoryConfig"/>.
        /// Sets a <see cref="TestMacroStabilityInwardsCalculatorFactory"/> to 
        /// <see cref="MacroStabilityInwardsCalculatorFactory.Instance"/>
        /// </summary>
        public MacroStabilityInwardsCalculatorFactoryConfig()
        {
            previousFactory = MacroStabilityInwardsCalculatorFactory.Instance;
            MacroStabilityInwardsCalculatorFactory.Instance = new TestMacroStabilityInwardsCalculatorFactory();
        }

        /// <summary>
        /// Reverts the <see cref="MacroStabilityInwardsCalculatorFactory.Instance"/> to the value
        /// it had at time of construction of the <see cref="MacroStabilityInwardsCalculatorFactoryConfig"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                MacroStabilityInwardsCalculatorFactory.Instance = previousFactory;
            }
        }
    }
}