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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// Service for synchronizing grass cover erosion inwards data.
    /// </summary>
    public static class GrassCoverErosionInwardsDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations in the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutput(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations
                                   .Cast<GrassCoverErosionInwardsCalculation>()
                                   .SelectMany(ClearCalculationOutput)
                                   .ToArray();
        }

        /// <summary>
        /// Clears the output of the given <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/>
        /// to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        /// <returns>All objects that have been changed.</returns>
        public static IEnumerable<IObservable> ClearCalculationOutput(GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (calculation.HasOutput)
            {
                calculation.ClearOutput();
                return new[]
                {
                    calculation
                };
            }
            return Enumerable.Empty<IObservable>();
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the calculations
        /// in the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removal of data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutputAndHydraulicBoundaryLocations(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedItems = new List<IObservable>();
            foreach (GrassCoverErosionInwardsCalculation calculation in failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>())
            {
                affectedItems.AddRange(ClearCalculationOutput(calculation)
                                           .Concat(ClearHydraulicBoundaryLocation(calculation.InputParameters)));
            }

            return affectedItems;
        }

        /// <summary>
        /// Clears all data dependent, either directly or indirectly, on the parent reference line.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be cleared.</param>
        /// <returns>The results of the clear action.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static ClearResults ClearReferenceLineDependentData(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var observables = new List<IObservable>();
            object[] removedObjects = failureMechanism.Sections
                                                      .OfType<object>()
                                                      .Concat(failureMechanism.SectionResults)
                                                      .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                      .Concat(failureMechanism.DikeProfiles)
                                                      .ToArray();

            failureMechanism.ClearAllSections();
            observables.Add(failureMechanism);

            failureMechanism.CalculationsGroup.Children.Clear();
            observables.Add(failureMechanism.CalculationsGroup);

            failureMechanism.DikeProfiles.Clear();
            observables.Add(failureMechanism.DikeProfiles);

            return new ClearResults(observables, removedObjects);
        }

        /// <summary>
        /// Removes the <see cref="DikeProfile"/> from the <see cref="GrassCoverErosionInwardsFailureMechanism"/>
        /// and clears all the data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <param name="dikeProfile">The dike profile to be removed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all affected objects by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveDikeProfile(GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                 DikeProfile dikeProfile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            if (dikeProfile == null)
            {
                throw new ArgumentNullException(nameof(dikeProfile));
            }

            IEnumerable<GrassCoverErosionInwardsCalculation> affectedCalculations = failureMechanism.Calculations
                                                                                                    .Cast<GrassCoverErosionInwardsCalculation>()
                                                                                                    .Where(calc => ReferenceEquals(dikeProfile, calc.InputParameters.DikeProfile));

            var affectedObjects = new List<IObservable>
            {
                failureMechanism.DikeProfiles
            };
            foreach (GrassCoverErosionInwardsCalculation calculation in affectedCalculations)
            {
                affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                affectedObjects.AddRange(ClearDikeProfile(calculation.InputParameters));
            }

            failureMechanism.DikeProfiles.Remove(dikeProfile);
            return affectedObjects;
        }

        /// <summary>
        /// Removes all the <see cref="DikeProfile"/> from the <see cref="GrassCoverErosionInwardsFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism for which all
        /// the dike profiles need to be removed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all affected objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllDikeProfiles(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<GrassCoverErosionInwardsCalculation> affectedCalculations =
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>()
                                .Where(calc => calc.InputParameters.DikeProfile != null)
                                .ToArray();

            var affectedObjects = new List<IObservable>();
            foreach (GrassCoverErosionInwardsCalculation calculation in affectedCalculations)
            {
                affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                affectedObjects.AddRange(ClearDikeProfile(calculation.InputParameters));
            }

            failureMechanism.DikeProfiles.Clear();
            affectedObjects.Add(failureMechanism.DikeProfiles);
            return affectedObjects;
        }

        private static IEnumerable<IObservable> ClearDikeProfile(GrassCoverErosionInwardsInput inputParameters)
        {
            if (inputParameters.DikeProfile != null)
            {
                inputParameters.DikeProfile = null;
                return new[]
                {
                    inputParameters
                };
            }
            return Enumerable.Empty<IObservable>();
        }

        private static IEnumerable<IObservable> ClearHydraulicBoundaryLocation(GrassCoverErosionInwardsInput input)
        {
            if (input.HydraulicBoundaryLocation != null)
            {
                input.HydraulicBoundaryLocation = null;
                return new[]
                {
                    input
                };
            }
            return Enumerable.Empty<IObservable>();
        }
    }
}