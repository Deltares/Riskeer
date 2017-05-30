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
using Ringtoets.MacroStabilityInwards.KernelWrapper.SubCalculator;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="TestMacroStabilityInwardsSubCalculatorFactory"/> 
    /// for <see cref="MacroStabilityInwardsSubCalculatorFactory.Instance"/> while testing. 
    /// Disposing an instance of this class will revert the 
    /// <see cref="MacroStabilityInwardsSubCalculatorFactory.Instance"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new MacroStabilityInwardsCalculationServiceConfig()) {
    ///     var testFactory = (TestMacroStabilityInwardsSubCalculatorFactory) MacroStabilityInwardsCalculationService.SubCalculatorFactory;
    /// 
    ///     // Perform tests with testFactory
    /// }
    /// </code>
    /// </example>
    public class MacroStabilityInwardsSubCalculatorFactoryConfig : IDisposable
    {
        private readonly IMacroStabilityInwardsSubCalculatorFactory previousFactory;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSubCalculatorFactoryConfig"/>.
        /// Sets a <see cref="TestMacroStabilityInwardsSubCalculatorFactory"/> to 
        /// <see cref="MacroStabilityInwardsSubCalculatorFactory.Instance"/>
        /// </summary>
        public MacroStabilityInwardsSubCalculatorFactoryConfig()
        {
            previousFactory = MacroStabilityInwardsSubCalculatorFactory.Instance;
            MacroStabilityInwardsSubCalculatorFactory.Instance = new TestMacroStabilityInwardsSubCalculatorFactory();
        }

        /// <summary>
        /// Reverts the <see cref="MacroStabilityInwardsSubCalculatorFactory.Instance"/> to the value
        /// it had at time of construction of the <see cref="MacroStabilityInwardsSubCalculatorFactoryConfig"/>.
        /// </summary>
        public void Dispose()
        {
            MacroStabilityInwardsSubCalculatorFactory.Instance = previousFactory;
        }
    }
}