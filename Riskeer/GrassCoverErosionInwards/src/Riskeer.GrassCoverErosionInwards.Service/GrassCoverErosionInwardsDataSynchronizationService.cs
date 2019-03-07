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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Util;

namespace Riskeer.GrassCoverErosionInwards.Service
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
        /// Clears the illustration points of the provided grass cover erosion inwards calculations.
        /// </summary>
        /// <param name="calculations">The calculations for which the illustration points need to be cleared.</param>
        /// <returns>All objects changed during the clear.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearIllustrationPoints(IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            var affectedObjects = new List<IObservable>();
            foreach (GrassCoverErosionInwardsCalculation calculation in calculations)
            {
                if (GrassCoverErosionInwardsIllustrationPointsHelper.HasIllustrationPoints(calculation))
                {
                    affectedObjects.Add(calculation);
                    calculation.ClearIllustrationPoints();
                }
            }

            return affectedObjects;
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

            var changedObjects = new List<IObservable>();
            object[] removedObjects = failureMechanism.Sections
                                                      .OfType<object>()
                                                      .Concat(failureMechanism.SectionResults)
                                                      .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                      .Concat(failureMechanism.DikeProfiles)
                                                      .ToArray();

            failureMechanism.ClearAllSections();
            changedObjects.Add(failureMechanism);
            changedObjects.Add(failureMechanism.SectionResults);

            failureMechanism.CalculationsGroup.Children.Clear();
            changedObjects.Add(failureMechanism.CalculationsGroup);

            failureMechanism.DikeProfiles.Clear();
            changedObjects.Add(failureMechanism.DikeProfiles);

            return new ClearResults(changedObjects, removedObjects);
        }

        /// <summary>
        /// Removes the <see cref="DikeProfile"/>, unassigns them from the <paramref name="calculations"/>
        /// and clears all the data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="dikeProfileToRemove">The dike profile to remove.</param>
        /// <param name="calculations">The calculations that may have 
        /// <paramref name="dikeProfileToRemove"/> assigned.</param>
        /// <param name="dikeProfiles">The collection of <see cref="DikeProfile"/> in
        /// which <paramref name="dikeProfileToRemove"/> is contained.</param>
        /// <param name="sectionResults">The section results that may have an assignment to a calculation 
        /// based on the <paramref name="dikeProfileToRemove"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all affected objects by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveDikeProfile(DikeProfile dikeProfileToRemove,
                                                                 IEnumerable<GrassCoverErosionInwardsCalculation> calculations,
                                                                 DikeProfileCollection dikeProfiles,
                                                                 IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults)
        {
            if (dikeProfileToRemove == null)
            {
                throw new ArgumentNullException(nameof(dikeProfileToRemove));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (dikeProfiles == null)
            {
                throw new ArgumentNullException(nameof(dikeProfiles));
            }

            if (sectionResults == null)
            {
                throw new ArgumentNullException(nameof(sectionResults));
            }

            IEnumerable<GrassCoverErosionInwardsCalculation> affectedCalculations =
                calculations.Where(calc => ReferenceEquals(dikeProfileToRemove, calc.InputParameters.DikeProfile));

            var affectedObjects = new List<IObservable>
            {
                dikeProfiles
            };
            affectedObjects.AddRange(ClearDikeProfileDependentData(sectionResults, affectedCalculations, calculations));

            dikeProfiles.Remove(dikeProfileToRemove);
            return affectedObjects;
        }

        /// <summary>
        /// Clears <paramref name="dikeProfiles"/>, unassigns the elements from the <paramref name="calculations"/>
        /// and clears all the data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="calculations">The calculations that may have 
        /// an assigned element of <see cref="dikeProfiles"/>.</param>
        /// <param name="dikeProfiles">The collection to be cleared.</param>
        /// <param name="sectionResults">The section results that may have an assignment to a calculation 
        /// based on the elements of <paramref name="dikeProfiles"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all affected objects by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllDikeProfiles(IEnumerable<GrassCoverErosionInwardsCalculation> calculations,
                                                                     DikeProfileCollection dikeProfiles,
                                                                     IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (dikeProfiles == null)
            {
                throw new ArgumentNullException(nameof(dikeProfiles));
            }

            if (sectionResults == null)
            {
                throw new ArgumentNullException(nameof(sectionResults));
            }

            IEnumerable<GrassCoverErosionInwardsCalculation> affectedCalculations =
                calculations.Where(calc => calc.InputParameters.DikeProfile != null)
                            .ToArray();

            var affectedObjects = new List<IObservable>
            {
                dikeProfiles
            };
            affectedObjects.AddRange(ClearDikeProfileDependentData(sectionResults,
                                                                   affectedCalculations,
                                                                   calculations));

            dikeProfiles.Clear();
            return affectedObjects;
        }

        private static IEnumerable<IObservable> ClearDikeProfileDependentData(
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults,
            IEnumerable<GrassCoverErosionInwardsCalculation> calculationsWithRemovedDikeProfile,
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            var affectedObjects = new List<IObservable>();
            foreach (GrassCoverErosionInwardsCalculation calculation in calculationsWithRemovedDikeProfile)
            {
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                affectedObjects.AddRange(ClearDikeProfile(calculation.InputParameters));
            }

            affectedObjects.AddRange(GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(
                                         sectionResults,
                                         calculations));
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