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
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.Forms;
using Riskeer.Common.IO.Structures;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Service;
using Riskeer.StabilityPointStructures.Util;

namespace Riskeer.StabilityPointStructures.Plugin.FileImporters
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> 
    /// implementation for updating stability point structures based on imported
    /// data.
    /// </summary>
    public class StabilityPointStructureUpdateDataStrategy : UpdateDataStrategyBase<StabilityPointStructure, StabilityPointStructuresFailureMechanism>,
                                                             IStructureUpdateStrategy<StabilityPointStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructureUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the 
        /// structures are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public StabilityPointStructureUpdateDataStrategy(StabilityPointStructuresFailureMechanism failureMechanism)
            : base(failureMechanism, failureMechanism?.StabilityPointStructures, new StructureIdEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateStructuresWithImportedData(IEnumerable<StabilityPointStructure> readStructures, string sourceFilePath)
        {
            return UpdateTargetCollectionData(readStructures, sourceFilePath);
        }

        #region Removing Data Functions

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(StabilityPointStructure removedObject)
        {
            return StabilityPointStructuresDataSynchronizationService.RemoveStructure(removedObject, FailureMechanism);
        }

        #endregion

        #region Updating Data Functions

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(StabilityPointStructure objectToUpdate, StabilityPointStructure objectToUpdateFrom)
        {
            objectToUpdate.CopyProperties(objectToUpdateFrom);

            return UpdateStabilityPointStructureDependentData(objectToUpdate);
        }

        private IEnumerable<IObservable> UpdateStabilityPointStructureDependentData(StabilityPointStructure structure)
        {
            var affectedObjects = new List<IObservable>();

            affectedObjects.AddRange(GetAffectedCalculationsWithStabilityPointStructure(structure)
                                         .Select(affectedCalculation => affectedCalculation.InputParameters));

            affectedObjects.AddRange(StabilityPointStructuresHelper.UpdateCalculationToSectionResultAssignments(FailureMechanism));

            return affectedObjects;
        }

        private IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> GetAffectedCalculationsWithStabilityPointStructure(StabilityPointStructure structure)
        {
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> affectedCalculations =
                FailureMechanism.Calculations
                                .Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                .Where(calc => ReferenceEquals(calc.InputParameters.Structure, structure));
            return affectedCalculations;
        }

        #endregion
    }
}