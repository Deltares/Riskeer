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

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="IMacroStabilityInwardsCalculatorFactory"/>
    /// for <see cref="MacroStabilityInwardsCalculatorFactory.Instance"/> while testing.
    /// Disposing an instance of this class will revert the
    /// <see cref="MacroStabilityInwardsCalculatorFactory.Instance"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// var mockRepository = new MockRepository();
    /// var calculatorFactory = mockRepository.Stub&lt;IMacroStabilityInwardsCalculatorFactory&gt;();
    /// mockRepository.ReplayAll();
    /// 
    /// using(new MacroStabilityInwardsCalculatorFactoryConfig(calculatorFactory))
    /// {
    ///     // Perform test with mocked factory
    /// }
    /// </code>
    /// 
    /// mockRepository.VerifyAll();
    /// </example>
    public class MacroStabilityInwardsCalculatorFactoryConfig : IDisposable
    {
        private readonly IMacroStabilityInwardsCalculatorFactory previousFactory;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculatorFactoryConfig"/>.
        /// Sets the <paramref name="newFactory"/> to <see cref="MacroStabilityInwardsCalculatorFactory.Instance"/>.
        /// </summary>
        /// <param name="newFactory">The factory that will be used while testing.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newFactory"/> 
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsCalculatorFactoryConfig(IMacroStabilityInwardsCalculatorFactory newFactory)
        {
            if (newFactory == null)
            {
                throw new ArgumentNullException(nameof(newFactory));
            }
            previousFactory = MacroStabilityInwardsCalculatorFactory.Instance;
            MacroStabilityInwardsCalculatorFactory.Instance = newFactory;
        }

        public void Dispose()
        {
            MacroStabilityInwardsCalculatorFactory.Instance = previousFactory;
        }
    }
}