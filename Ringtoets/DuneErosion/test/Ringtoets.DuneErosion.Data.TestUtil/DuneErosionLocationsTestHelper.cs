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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;

namespace Ringtoets.DuneErosion.Data.TestUtil
{
    /// <summary>
    /// Test helper for dealing with dune locations and calculations in
    /// <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public static class DuneErosionLocationsTestHelper
    {
        /// <summary>
        /// Gets all the <see cref="DuneLocation"/> and <see cref="DuneLocationCalculation"/>
        /// that have an output within the <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to retrieve the dune erosion location and calculations from.</param>
        /// <returns>A collection of all the dune erosion location and calculations that contain an output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> GetAllDuneErosionLocationCalculationsWithOutput(DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.DuneLocations.Where(loc => loc.Calculation.Output != null).Cast<IObservable>()
                                   .Concat(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.Where(HasDuneErosionLocationCalculationOutput))
                                   .Concat(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.Where(HasDuneErosionLocationCalculationOutput))
                                   .Concat(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.Where(HasDuneErosionLocationCalculationOutput))
                                   .Concat(failureMechanism.CalculationsForLowerLimitNorm.Where(HasDuneErosionLocationCalculationOutput))
                                   .Concat(failureMechanism.CalculationsForFactorizedLowerLimitNorm.Where(HasDuneErosionLocationCalculationOutput))
                                   .ToArray();
        }

        /// <summary>
        /// Asserts if all the dune erosion location and calculations within the <paramref name="failureMechanism"/>
        /// have no outputs.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        /// <exception cref="AssertionException">Thrown when any of the dune erosion location and calculations within the 
        /// <paramref name="failureMechanism"/> contains output.</exception>
        public static void AssertDuneLocationCalculationsHaveNoOutputs(DuneErosionFailureMechanism failureMechanism)
        {
            Assert.True(failureMechanism.DuneLocations.All(dl => dl.Calculation.Output == null));

            Assert.True(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.All(calc => calc.Output == null));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.All(calc => calc.Output == null));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.All(calc => calc.Output == null));
            Assert.True(failureMechanism.CalculationsForLowerLimitNorm.All(calc => calc.Output == null));
            Assert.True(failureMechanism.CalculationsForFactorizedLowerLimitNorm.All(calc => calc.Output == null));
        }

        private static bool HasDuneErosionLocationCalculationOutput(DuneLocationCalculation calculation)
        {
            return calculation.Output != null;
        }
    }
}