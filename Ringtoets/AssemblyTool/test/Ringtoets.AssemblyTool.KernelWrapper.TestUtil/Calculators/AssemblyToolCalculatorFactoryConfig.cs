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
using Riskeer.AssemblyTool.KernelWrapper.Calculators;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="TestAssemblyToolCalculatorFactory"/> 
    /// for <see cref="AssemblyToolCalculatorFactory.Instance"/> while testing. 
    /// Disposing an instance of this class will revert the <see cref="AssemblyToolCalculatorFactory.Instance"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new AssemblyToolCalculatorFactoryConfig())
    /// {
    ///     var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
    /// 
    ///     // Perform tests with testFactory
    /// }
    /// </code>
    /// </example>
    public class AssemblyToolCalculatorFactoryConfig : IDisposable
    {
        private readonly IAssemblyToolCalculatorFactory previousFactory;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyToolCalculatorFactoryConfig"/>.
        /// Sets a <see cref="TestAssemblyToolCalculatorFactory"/> to 
        /// <see cref="AssemblyToolCalculatorFactory.Instance"/>
        /// </summary>
        public AssemblyToolCalculatorFactoryConfig()
        {
            previousFactory = AssemblyToolCalculatorFactory.Instance;
            AssemblyToolCalculatorFactory.Instance = new TestAssemblyToolCalculatorFactory();
        }

        /// <summary>
        /// Reverts the <see cref="AssemblyToolCalculatorFactory.Instance"/> to the value
        /// it had at time of construction of the <see cref="AssemblyToolCalculatorFactory"/>.
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
                AssemblyToolCalculatorFactory.Instance = previousFactory;
            }
        }
    }
}