﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Service
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
                throw new ArgumentNullException("failureMechanism");
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
                throw new ArgumentNullException("calculation");
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
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedItems = new List<IObservable>();
            foreach (var calculation in failureMechanism.Calculations.Cast<PipingCalculation>())
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
                throw new ArgumentNullException("failureMechanism");
            }

            var changedObjects = new Collection<IObservable>();
            var removedObjects = failureMechanism.Sections.OfType<object>()
                                                 .Concat(failureMechanism.SectionResults)
                                                 .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                 .Concat(failureMechanism.StochasticSoilModels)
                                                 .Concat(failureMechanism.SurfaceLines)
                                                 .ToArray();

            failureMechanism.ClearAllSections();
            changedObjects.Add(failureMechanism);

            failureMechanism.CalculationsGroup.Children.Clear();
            changedObjects.Add(failureMechanism.CalculationsGroup);

            failureMechanism.StochasticSoilModels.Clear();
            changedObjects.Add(failureMechanism.StochasticSoilModels);

            failureMechanism.SurfaceLines.Clear();
            changedObjects.Add(failureMechanism.SurfaceLines);

            return new ClearResults(changedObjects, removedObjects);
        }

        /// <summary>
        /// Removes a given <see cref="RingtoetsPipingSurfaceLine"/> from the <see cref="PipingFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="surfaceLine"/>.</param>
        /// <param name="surfaceLine">The surfaceline residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveSurfaceLine(PipingFailureMechanism failureMechanism, RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            var changedObservables = new List<IObservable>();
            foreach (PipingCalculation pipingCalculationScenario in failureMechanism.Calculations.Cast<PipingCalculation>())
            {
                if (ReferenceEquals(pipingCalculationScenario.InputParameters.SurfaceLine, surfaceLine))
                {
                    changedObservables.AddRange(ClearSurfaceLine(pipingCalculationScenario.InputParameters));
                }
            }

            failureMechanism.SurfaceLines.Remove(surfaceLine);
            changedObservables.Add(failureMechanism.SurfaceLines);

            return changedObservables;
        }

        /// <summary>
        /// Removes a given <see cref="StochasticSoilModel"/> from the <see cref="PipingFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="soilModel"/>.</param>
        /// <param name="soilModel">The soil model residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="soilModel"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveStochasticSoilModel(PipingFailureMechanism failureMechanism, StochasticSoilModel soilModel)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            if (soilModel == null)
            {
                throw new ArgumentNullException("soilModel");
            }

            var changedObservables = new List<IObservable>();
            foreach (PipingCalculation pipingCalculationScenario in failureMechanism.Calculations.Cast<PipingCalculation>())
            {
                if (ReferenceEquals(pipingCalculationScenario.InputParameters.StochasticSoilModel, soilModel))
                {
                    changedObservables.AddRange(ClearStochasticSoilModel(pipingCalculationScenario.InputParameters));
                }
            }

            failureMechanism.StochasticSoilModels.Remove(soilModel);
            changedObservables.Add(failureMechanism.StochasticSoilModels);

            return changedObservables;
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