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
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.Service;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms;
using Ringtoets.MacroStabilityInwards.IO.Importers;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Service;

namespace Ringtoets.MacroStabilityInwards.Plugin.FileImporter
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> for updating surface lines based on imported data.
    /// </summary>
    public class RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy : UpdateDataStrategyBase<RingtoetsMacroStabilityInwardsSurfaceLine, MacroStabilityInwardsFailureMechanism>,
                                                                               ISurfaceLineUpdateDataStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the surface lines are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(MacroStabilityInwardsFailureMechanism failureMechanism)
            : base(failureMechanism, new RingtoetsMacroStabilityInwardsSurfaceLineNameEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateSurfaceLinesWithImportedData(RingtoetsMacroStabilityInwardsSurfaceLineCollection targetDataCollection,
                                                                           IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> readSurfaceLines,
                                                                           string sourceFilePath)
        {
            return UpdateTargetCollectionData(targetDataCollection, readSurfaceLines, sourceFilePath);
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(RingtoetsMacroStabilityInwardsSurfaceLine removedSurfaceLine)
        {
            return MacroStabilityInwardsDataSynchronizationService.RemoveSurfaceLine(FailureMechanism, removedSurfaceLine);
        }

        /// <summary>
        /// Class for comparing <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> by only the name.
        /// </summary>
        private class RingtoetsMacroStabilityInwardsSurfaceLineNameEqualityComparer : IEqualityComparer<RingtoetsMacroStabilityInwardsSurfaceLine>
        {
            public bool Equals(RingtoetsMacroStabilityInwardsSurfaceLine x, RingtoetsMacroStabilityInwardsSurfaceLine y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(RingtoetsMacroStabilityInwardsSurfaceLine obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        #region Updating Data Functions

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineToUpdate,
                                                                                 RingtoetsMacroStabilityInwardsSurfaceLine matchingSurfaceLine)
        {
            surfaceLineToUpdate.CopyProperties(matchingSurfaceLine);

            var affectedObjects = new List<IObservable>();

            affectedObjects.AddRange(UpdateSurfaceLineDependentData(surfaceLineToUpdate));
            affectedObjects.AddRange(UpdateStochasticSoilModel(surfaceLineToUpdate));

            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateSurfaceLineDependentData(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            IEnumerable<MacroStabilityInwardsCalculation> affectedCalculations = GetAffectedCalculationWithSurfaceLine(surfaceLine);

            var affectedObjects = new List<IObservable>();
            foreach (MacroStabilityInwardsCalculation affectedCalculation in affectedCalculations)
            {
                affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(affectedCalculation));
            }
            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateStochasticSoilModel(RingtoetsMacroStabilityInwardsSurfaceLine updatedSurfaceLine)
        {
            IEnumerable<MacroStabilityInwardsCalculation> calculationsToUpdate = GetAffectedCalculationWithSurfaceLine(updatedSurfaceLine);

            var affectedObjects = new List<IObservable>();
            foreach (MacroStabilityInwardsCalculation calculation in calculationsToUpdate)
            {
                IEnumerable<StochasticSoilModel> matchingSoilModels = GetAvailableStochasticSoilModels(updatedSurfaceLine);

                MacroStabilityInwardsInput calculationInput = calculation.InputParameters;
                MacroStabilityInwardsInputService.SetMatchingStochasticSoilModel(calculationInput, matchingSoilModels);
                affectedObjects.Add(calculationInput);
            }

            return affectedObjects;
        }

        private IEnumerable<MacroStabilityInwardsCalculation> GetAffectedCalculationWithSurfaceLine(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            IEnumerable<MacroStabilityInwardsCalculation> affectedCalculations =
                FailureMechanism.Calculations
                                .Cast<MacroStabilityInwardsCalculation>()
                                .Where(calc => ReferenceEquals(calc.InputParameters.SurfaceLine, surfaceLine));
            return affectedCalculations;
        }

        private IEnumerable<StochasticSoilModel> GetAvailableStochasticSoilModels(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(surfaceLine,
                                                                                                             FailureMechanism.StochasticSoilModels);
        }

        #endregion
    }
}