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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.Service;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Service
{
    /// <summary>
    /// Service for synchronizing piping data.
    /// </summary>
    public static class PipingDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations in the <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutput(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations
                                   .Cast<PipingCalculation>()
                                   .SelectMany(ClearCalculationOutput)
                                   .ToArray();
        }

        /// <summary>
        /// Clears the output of the given <see cref="PipingCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <returns>All objects that have been changed.</returns>
        public static IEnumerable<IObservable> ClearCalculationOutput(PipingCalculation calculation)
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
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the calculations in the <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removing data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutputAndHydraulicBoundaryLocations(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedItems = new List<IObservable>();
            foreach (PipingCalculation calculation in failureMechanism.Calculations.Cast<PipingCalculation>())
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
        public static ClearResults ClearReferenceLineDependentData(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var changedObjects = new Collection<IObservable>();
            object[] removedObjects = failureMechanism.Sections.OfType<object>()
                                                      .Concat(failureMechanism.SectionResults)
                                                      .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                      .Concat(failureMechanism.StochasticSoilModels)
                                                      .Concat(failureMechanism.SurfaceLines)
                                                      .ToArray();

            failureMechanism.ClearAllSections();
            changedObjects.Add(failureMechanism);
            changedObjects.Add(failureMechanism.SectionResults);

            failureMechanism.CalculationsGroup.Children.Clear();
            changedObjects.Add(failureMechanism.CalculationsGroup);

            failureMechanism.StochasticSoilModels.Clear();
            changedObjects.Add(failureMechanism.StochasticSoilModels);

            failureMechanism.SurfaceLines.Clear();
            changedObjects.Add(failureMechanism.SurfaceLines);

            return new ClearResults(changedObjects, removedObjects);
        }

        /// <summary>
        /// Removes a given <see cref="PipingSurfaceLine"/> from the <see cref="PipingFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="surfaceLine"/>.</param>
        /// <param name="surfaceLine">The surface line residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveSurfaceLine(PipingFailureMechanism failureMechanism, PipingSurfaceLine surfaceLine)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            IEnumerable<PipingCalculation> pipingCalculationScenarios =
                failureMechanism.Calculations
                                .Cast<PipingCalculation>()
                                .Where(pcs => ReferenceEquals(pcs.InputParameters.SurfaceLine, surfaceLine));

            List<IObservable> changedObservables = RemoveSurfaceLineDependentData(pipingCalculationScenarios).ToList();

            failureMechanism.SurfaceLines.Remove(surfaceLine);
            changedObservables.Add(failureMechanism.SurfaceLines);

            return changedObservables;
        }

        /// <summary>
        /// Removes all <see cref="PipingSurfaceLine"/> from the <see cref="PipingFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <returns>All objects that are affected by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllSurfaceLines(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<PipingCalculation> affectedCalculationScenarios =
                failureMechanism.Calculations
                                .Cast<PipingCalculation>()
                                .Where(calc => calc.InputParameters.SurfaceLine != null).ToArray();

            List<IObservable> affectedObjects = RemoveSurfaceLineDependentData(affectedCalculationScenarios).ToList();

            failureMechanism.SurfaceLines.Clear();
            affectedObjects.Add(failureMechanism.SurfaceLines);
            return affectedObjects;
        }

        /// <summary>
        /// Removes a given <see cref="PipingStochasticSoilModel"/> from the <see cref="PipingFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="soilModel"/>.</param>
        /// <param name="soilModel">The soil model residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="soilModel"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveStochasticSoilModel(PipingFailureMechanism failureMechanism,
                                                                         PipingStochasticSoilModel soilModel)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (soilModel == null)
            {
                throw new ArgumentNullException(nameof(soilModel));
            }

            var changedObservables = new List<IObservable>();

            IEnumerable<PipingCalculation> pipingCalculationScenarios =
                failureMechanism.Calculations
                                .Cast<PipingCalculation>()
                                .Where(pcs => ReferenceEquals(pcs.InputParameters.StochasticSoilModel, soilModel));

            foreach (PipingCalculation pipingCalculationScenario in pipingCalculationScenarios)
            {
                changedObservables.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(pipingCalculationScenario));
                changedObservables.AddRange(ClearStochasticSoilModel(pipingCalculationScenario.InputParameters));
            }

            failureMechanism.StochasticSoilModels.Remove(soilModel);
            changedObservables.Add(failureMechanism.StochasticSoilModels);

            return changedObservables;
        }

        /// <summary>
        /// Removes all <see cref="StochasticSoilModel"/> from the <see cref="PipingFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <returns>All objects that are affected by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllStochasticSoilModels(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<PipingCalculation> affectedCalculationScenarios =
                failureMechanism.Calculations
                                .Cast<PipingCalculation>()
                                .Where(calc => calc.InputParameters.StochasticSoilModel != null).ToArray();

            var affectedObjects = new List<IObservable>();
            foreach (PipingCalculation calculation in affectedCalculationScenarios)
            {
                affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                affectedObjects.AddRange(ClearStochasticSoilModel(calculation.InputParameters));
            }

            failureMechanism.StochasticSoilModels.Clear();
            affectedObjects.Add(failureMechanism.StochasticSoilModels);
            return affectedObjects;
        }

        /// <summary>
        /// Removes a given <see cref="PipingStochasticSoilProfile"/> from calculations in the <see cref="PipingFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="soilProfile"/>.</param>
        /// <param name="soilProfile">The soil profile residing in <paramref name="failureMechanism"/>
        /// that has beeen removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveStochasticSoilProfileFromInput(PipingFailureMechanism failureMechanism,
                                                                                    PipingStochasticSoilProfile soilProfile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            var changedObservables = new List<IObservable>();

            foreach (PipingCalculation pipingCalculationScenario in GetCalculationsWithSoilProfileAssigned(failureMechanism, soilProfile))
            {
                changedObservables.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(pipingCalculationScenario));
                changedObservables.AddRange(ClearStochasticSoilProfile(pipingCalculationScenario.InputParameters));
            }

            return changedObservables;
        }

        /// <summary>
        /// Clears data dependent on a given <see cref="PipingStochasticSoilProfile"/>, either directly or indirectly,
        /// from calculations in the <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="soilProfile"/>.</param>
        /// <param name="soilProfile">The soil profile residing in <paramref name="failureMechanism"/>
        /// that has been updated.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearStochasticSoilProfileDependentData(PipingFailureMechanism failureMechanism,
                                                                                       PipingStochasticSoilProfile soilProfile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            var changedObservables = new List<IObservable>();

            foreach (PipingCalculation calculation in GetCalculationsWithSoilProfileAssigned(failureMechanism, soilProfile))
            {
                changedObservables.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                changedObservables.Add(calculation.InputParameters);
            }

            return changedObservables;
        }

        private static IEnumerable<IObservable> RemoveSurfaceLineDependentData(IEnumerable<PipingCalculation> pipingCalculationScenarios)
        {
            var changedObservables = new List<IObservable>();
            foreach (PipingCalculation pipingCalculationScenario in pipingCalculationScenarios)
            {
                changedObservables.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(pipingCalculationScenario));
                changedObservables.AddRange(ClearSurfaceLine(pipingCalculationScenario.InputParameters));
            }

            return changedObservables;
        }

        private static IEnumerable<PipingCalculation> GetCalculationsWithSoilProfileAssigned(PipingFailureMechanism failureMechanism,
                                                                                             PipingStochasticSoilProfile soilProfile)
        {
            IEnumerable<PipingCalculation> pipingCalculationScenarios =
                failureMechanism.Calculations
                                .Cast<PipingCalculation>()
                                .Where(pcs => ReferenceEquals(pcs.InputParameters.StochasticSoilProfile, soilProfile));
            return pipingCalculationScenarios;
        }

        private static IEnumerable<IObservable> ClearSurfaceLine(PipingInput input)
        {
            input.SurfaceLine = null;
            return new[]
            {
                input
            };
        }

        private static IEnumerable<IObservable> ClearStochasticSoilModel(PipingInput input)
        {
            input.StochasticSoilModel = null;
            input.StochasticSoilProfile = null;

            return new[]
            {
                input
            };
        }

        private static IEnumerable<IObservable> ClearStochasticSoilProfile(PipingInput input)
        {
            input.StochasticSoilProfile = null;

            return new[]
            {
                input
            };
        }

        private static IEnumerable<IObservable> ClearHydraulicBoundaryLocation(PipingInput input)
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