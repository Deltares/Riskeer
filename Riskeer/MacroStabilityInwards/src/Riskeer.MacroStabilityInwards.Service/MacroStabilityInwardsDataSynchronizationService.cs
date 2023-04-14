﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Service;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Service
{
    /// <summary>
    /// Service for synchronizing macro stability inwards data.
    /// </summary>
    public static class MacroStabilityInwardsDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations in the <see cref="MacroStabilityInwardsFailureMechanism"/>,
        /// except where <see cref="MacroStabilityInwardsInput.UseAssessmentLevelManualInput"/> is <c>true</c>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="MacroStabilityInwardsFailureMechanism"/> which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutputWithoutManualAssessmentLevel(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations
                                   .Cast<MacroStabilityInwardsCalculationScenario>()
                                   .Where(c => !c.InputParameters.UseAssessmentLevelManualInput)
                                   .SelectMany(RiskeerCommonDataSynchronizationService.ClearCalculationOutput)
                                   .ToArray();
        }

        /// <summary>
        /// Clears:
        /// <list type="bullet">
        /// <item>The <see cref="HydraulicBoundaryLocation"/> for all the calculations in the <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// that uses an <see cref="HydraulicBoundaryLocation"/> from <paramref name="hydraulicBoundaryLocations"/>;</item>
        /// <item>The output for all these calculations in the <see cref="MacroStabilityInwardsFailureMechanism"/>,
        /// except where <see cref="MacroStabilityInwardsInput.UseAssessmentLevelManualInput"/> is <c>true</c>.</item>
        /// </list>
        /// </summary>
        /// <param name="failureMechanism">The <see cref="MacroStabilityInwardsFailureMechanism"/> which contains the calculations.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to clear for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removing data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutputAndHydraulicBoundaryLocations(
            MacroStabilityInwardsFailureMechanism failureMechanism,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            List<IObservable> affectedItems = failureMechanism.Calculations.Cast<MacroStabilityInwardsCalculation>()
                                                              .Where(c => !c.InputParameters.UseAssessmentLevelManualInput
                                                                          && hydraulicBoundaryLocations.Contains(c.InputParameters.HydraulicBoundaryLocation))
                                                              .SelectMany(RiskeerCommonDataSynchronizationService.ClearCalculationOutput)
                                                              .ToList();
            
            foreach (MacroStabilityInwardsCalculation calculation in failureMechanism.Calculations.Cast<MacroStabilityInwardsCalculation>()
                                                                                     .Where(c => hydraulicBoundaryLocations.Contains(
                                                                                                c.InputParameters.HydraulicBoundaryLocation)))
            {
                affectedItems.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocation(calculation.InputParameters));
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
        public static ClearResults ClearReferenceLineDependentData(MacroStabilityInwardsFailureMechanism failureMechanism)
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
        /// Removes a given <see cref="MacroStabilityInwardsSurfaceLine"/> from the <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="surfaceLine"/>.</param>
        /// <param name="surfaceLine">The surface line residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveSurfaceLine(MacroStabilityInwardsFailureMechanism failureMechanism, MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            IEnumerable<MacroStabilityInwardsCalculation> calculationScenarios =
                failureMechanism.Calculations
                                .Cast<MacroStabilityInwardsCalculation>()
                                .Where(pcs => ReferenceEquals(pcs.InputParameters.SurfaceLine, surfaceLine));

            List<IObservable> changedObservables = RemoveSurfaceLineDependentData(calculationScenarios).ToList();

            failureMechanism.SurfaceLines.Remove(surfaceLine);
            changedObservables.Add(failureMechanism.SurfaceLines);

            return changedObservables;
        }

        /// <summary>
        /// Removes all <see cref="MacroStabilityInwardsSurfaceLine"/> from the <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <returns>All objects that are affected by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllSurfaceLines(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<MacroStabilityInwardsCalculation> affectedCalculationScenarios =
                failureMechanism.Calculations
                                .Cast<MacroStabilityInwardsCalculation>()
                                .Where(calc => calc.InputParameters.SurfaceLine != null).ToArray();

            List<IObservable> affectedObjects = RemoveSurfaceLineDependentData(affectedCalculationScenarios).ToList();

            failureMechanism.SurfaceLines.Clear();
            affectedObjects.Add(failureMechanism.SurfaceLines);
            return affectedObjects;
        }

        /// <summary>
        /// Removes a given <see cref="MacroStabilityInwardsStochasticSoilModel"/> from the <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="soilModel"/>.</param>
        /// <param name="soilModel">The soil model residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="soilModel"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveStochasticSoilModel(MacroStabilityInwardsFailureMechanism failureMechanism, MacroStabilityInwardsStochasticSoilModel soilModel)
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

            IEnumerable<MacroStabilityInwardsCalculation> calculationScenarios =
                failureMechanism.Calculations
                                .Cast<MacroStabilityInwardsCalculation>()
                                .Where(pcs => ReferenceEquals(pcs.InputParameters.StochasticSoilModel, soilModel));

            foreach (MacroStabilityInwardsCalculation calculationScenario in calculationScenarios)
            {
                changedObservables.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculationScenario));
                changedObservables.AddRange(ClearStochasticSoilModel(calculationScenario.InputParameters));
            }

            failureMechanism.StochasticSoilModels.Remove(soilModel);
            changedObservables.Add(failureMechanism.StochasticSoilModels);

            return changedObservables;
        }

        /// <summary>
        /// Removes all <see cref="MacroStabilityInwardsStochasticSoilModel"/> from the <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <returns>All objects that are affected by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllStochasticSoilModels(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<MacroStabilityInwardsCalculation> affectedCalculationScenarios =
                failureMechanism.Calculations
                                .Cast<MacroStabilityInwardsCalculation>()
                                .Where(calc => calc.InputParameters.StochasticSoilModel != null).ToArray();

            var affectedObjects = new List<IObservable>();
            foreach (MacroStabilityInwardsCalculation calculation in affectedCalculationScenarios)
            {
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                affectedObjects.AddRange(ClearStochasticSoilModel(calculation.InputParameters));
            }

            failureMechanism.StochasticSoilModels.Clear();
            affectedObjects.Add(failureMechanism.StochasticSoilModels);
            return affectedObjects;
        }

        /// <summary>
        /// Removes a given <see cref="MacroStabilityInwardsStochasticSoilProfile"/> from calculations in the <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="soilProfile"/>.</param>
        /// <param name="soilProfile">The soil profile residing in <paramref name="failureMechanism"/>
        /// that has been removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveStochasticSoilProfileFromInput(MacroStabilityInwardsFailureMechanism failureMechanism, MacroStabilityInwardsStochasticSoilProfile soilProfile)
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

            foreach (MacroStabilityInwardsCalculation calculationScenario in GetCalculationsWithSoilProfileAssigned(failureMechanism, soilProfile))
            {
                changedObservables.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculationScenario));
                changedObservables.AddRange(ClearStochasticSoilProfile(calculationScenario.InputParameters));
            }

            return changedObservables;
        }

        /// <summary>
        /// Clears data dependent on a given <see cref="MacroStabilityInwardsStochasticSoilProfile"/>, either directly or indirectly,
        /// from calculations in the <see cref="MacroStabilityInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="soilProfile"/>.</param>
        /// <param name="soilProfile">The soil profile residing in <paramref name="failureMechanism"/>
        /// that has been updated.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearStochasticSoilProfileDependentData(MacroStabilityInwardsFailureMechanism failureMechanism, MacroStabilityInwardsStochasticSoilProfile soilProfile)
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

            foreach (MacroStabilityInwardsCalculation calculation in GetCalculationsWithSoilProfileAssigned(failureMechanism, soilProfile))
            {
                changedObservables.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                changedObservables.Add(calculation.InputParameters);
            }

            return changedObservables;
        }

        private static IEnumerable<IObservable> RemoveSurfaceLineDependentData(IEnumerable<MacroStabilityInwardsCalculation> calculationScenarios)
        {
            var changedObservables = new List<IObservable>();
            foreach (MacroStabilityInwardsCalculation calculationScenario in calculationScenarios)
            {
                changedObservables.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculationScenario));
                changedObservables.AddRange(ClearSurfaceLine(calculationScenario.InputParameters));
            }

            return changedObservables;
        }

        private static IEnumerable<MacroStabilityInwardsCalculation> GetCalculationsWithSoilProfileAssigned(MacroStabilityInwardsFailureMechanism failureMechanism, MacroStabilityInwardsStochasticSoilProfile soilProfile)
        {
            IEnumerable<MacroStabilityInwardsCalculation> calculationScenarios =
                failureMechanism.Calculations
                                .Cast<MacroStabilityInwardsCalculation>()
                                .Where(pcs => ReferenceEquals(pcs.InputParameters.StochasticSoilProfile, soilProfile));
            return calculationScenarios;
        }

        private static IEnumerable<IObservable> ClearSurfaceLine(MacroStabilityInwardsInput input)
        {
            input.SurfaceLine = null;
            return new[]
            {
                input
            };
        }

        private static IEnumerable<IObservable> ClearStochasticSoilModel(MacroStabilityInwardsInput input)
        {
            input.StochasticSoilModel = null;
            input.StochasticSoilProfile = null;

            return new[]
            {
                input
            };
        }

        private static IEnumerable<IObservable> ClearStochasticSoilProfile(MacroStabilityInwardsInput input)
        {
            input.StochasticSoilProfile = null;

            return new[]
            {
                input
            };
        }
    }
}