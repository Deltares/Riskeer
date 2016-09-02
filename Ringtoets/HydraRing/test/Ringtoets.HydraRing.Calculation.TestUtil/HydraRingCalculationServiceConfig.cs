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

using System;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.TestUtil
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="TestHydraRingCalculationService"/>
    /// for <see cref="HydraRingCalculationService.Instance"/> while testing.
    /// Disposing an instance of this class will revert the
    /// <see cref="HydraRingCalculationService.Instance"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new HydraRingCalculationServiceConfig())
    /// {
    ///     var testService = (TestHydraRingCalculationService) HydraRingCalculationService.Instance;
    /// 
    ///     // Perform test with service
    /// }
    /// </code>
    /// </example>
    public class HydraRingCalculationServiceConfig : IDisposable
    {
        private readonly IHydraRingCalculationService previousService;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingCalculationServiceConfig"/>.
        /// Sets a <see cref="TestHydraRingCalculationService"/> to
        /// <see cref="HydraRingCalculationService.Instance"/>.
        /// </summary>
        public HydraRingCalculationServiceConfig()
        {
            previousService = HydraRingCalculationService.Instance;
            HydraRingCalculationService.Instance = new TestHydraRingCalculationService();
        }

        /// <summary>
        /// Reverts the <see cref="HydraRingCalculationService.Instance"/> to the value
        /// it had at time of construction of the <see cref="HydraRingCalculationServiceConfig"/>.
        /// </summary>
        public void Dispose()
        {
            HydraRingCalculationService.Instance = previousService;
        }
    }
}