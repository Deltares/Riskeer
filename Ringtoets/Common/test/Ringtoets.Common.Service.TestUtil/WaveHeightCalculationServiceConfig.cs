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

namespace Ringtoets.Common.Service.TestUtil
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="IHydraulicBoundaryLocationCalculationService"/>
    /// for <see cref="WaveHeightCalculationService.Instance"/> while testing.
    /// Disposing an instance of this class will revert the
    /// <see cref="DesignWaterLevelCalculationService.Instance"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new WaveHeightCalculationServiceConfig())
    /// {
    ///     var testService = (TestHydraulicBoundaryLocationCalculationService) WaveHeightCalculationService.Instance;
    /// 
    ///     // Perform test with service
    /// }
    /// </code>
    /// </example>
    public class WaveHeightCalculationServiceConfig : IDisposable
    {
        private readonly IHydraulicBoundaryLocationCalculationService previousInstance;

        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightCalculationServiceConfig"/>.
        /// Sets a <see cref="TestHydraulicBoundaryLocationCalculationService"/> to
        /// <see cref="WaveHeightCalculationService.Instance"/>.
        /// </summary>
        public WaveHeightCalculationServiceConfig()
        {
            previousInstance = WaveHeightCalculationService.Instance;
            WaveHeightCalculationService.Instance = new TestHydraulicBoundaryLocationCalculationService();
        }

        /// <summary>
        /// Reverts the <see cref="WaveHeightCalculationService.Instance"/> to the value
        /// it had at time of construction of the <see cref="WaveHeightCalculationServiceConfig"/>.
        /// </summary>
        public void Dispose()
        {
            WaveHeightCalculationService.Instance = previousInstance;
        }
    }
}