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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;

namespace Ringtoets.DuneErosion.Data.TestUtil
{
    /// <summary>
    /// Test helper for dealing with dune location calculations in <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public static class DuneLocationsTestHelper
    {
        /// <summary>
        /// Gets all the dune location calculations within the <paramref name="failureMechanism"/> that have an output.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to retrieve the dune location calculations from.</param>
        /// <returns>A collection of all dune location calculations that contain an output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> GetAllDuneLocationCalculationsWithOutput(DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.Where(HasDuneLocationCalculationOutput)
                                   .Concat(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.Where(HasDuneLocationCalculationOutput))
                                   .Concat(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.Where(HasDuneLocationCalculationOutput))
                                   .Concat(failureMechanism.CalculationsForLowerLimitNorm.Where(HasDuneLocationCalculationOutput))
                                   .Concat(failureMechanism.CalculationsForFactorizedLowerLimitNorm.Where(HasDuneLocationCalculationOutput))
                                   .ToArray();
        }

        /// <summary>
        /// Asserts if all the dune location calculations within the <paramref name="failureMechanism"/> have no output.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        /// <exception cref="AssertionException">Thrown when any of the dune location calculations within the 
        /// <paramref name="failureMechanism"/> contains output.</exception>
        public static void AssertDuneLocationCalculationsHaveNoOutputs(DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            Assert.True(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));
            Assert.True(failureMechanism.CalculationsForLowerLimitNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));
            Assert.True(failureMechanism.CalculationsForFactorizedLowerLimitNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));
        }

        /// <summary>
        /// Sets dummy output for all dune location calculations within <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to set the dune location calculation outputs for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static void SetDuneLocationCalculationOutput(DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var random = new Random(39);

            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, random);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, random);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, random);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForLowerLimitNorm, random);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForFactorizedLowerLimitNorm, random);
        }

        private static bool HasDuneLocationCalculationOutput(DuneLocationCalculation calculation)
        {
            return calculation.Output != null;
        }

        private static void SetDuneLocationCalculationOutput(IEnumerable<DuneLocationCalculation> calculations, Random random)
        {
            foreach (DuneLocationCalculation duneLocationCalculation in calculations)
            {
                duneLocationCalculation.Output = new TestDuneLocationCalculationOutput(random.NextDouble(),
                                                                                       random.NextDouble(),
                                                                                       random.NextDouble());
            }
        }
    }
}