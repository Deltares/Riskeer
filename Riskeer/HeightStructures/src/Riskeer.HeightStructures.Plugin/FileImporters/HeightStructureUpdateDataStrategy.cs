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
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.Forms;
using Ringtoets.Common.IO.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Service;
using Ringtoets.HeightStructures.Util;

namespace Riskeer.HeightStructures.Plugin.FileImporters
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> implementation for 
    /// updating height structures based on imported data.
    /// </summary>
    public class HeightStructureUpdateDataStrategy : UpdateDataStrategyBase<HeightStructure, HeightStructuresFailureMechanism>,
                                                     IStructureUpdateStrategy<HeightStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructureUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the structures are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public HeightStructureUpdateDataStrategy(HeightStructuresFailureMechanism failureMechanism)
            : base(failureMechanism, failureMechanism?.HeightStructures, new StructureIdEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateStructuresWithImportedData(IEnumerable<HeightStructure> readStructures, string sourceFilePath)
        {
            return UpdateTargetCollectionData(readStructures, sourceFilePath);
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(HeightStructure removedObject)
        {
            return HeightStructuresDataSynchronizationService.RemoveStructure(removedObject, FailureMechanism);
        }

        #region Updating Data Functions

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(HeightStructure objectToUpdate,
                                                                                 HeightStructure objectToUpdateFrom)
        {
            objectToUpdate.CopyProperties(objectToUpdateFrom);

            return UpdateHeightStructureDependentData(objectToUpdate);
        }

        private IEnumerable<IObservable> UpdateHeightStructureDependentData(HeightStructure structure)
        {
            var affectedObjects = new List<IObservable>();

            affectedObjects.AddRange(GetAffectedCalculationsWithHeightStructure(structure)
                                         .Select(c => c.InputParameters));

            affectedObjects.AddRange(HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(FailureMechanism));

            return affectedObjects;
        }

        private IEnumerable<StructuresCalculation<HeightStructuresInput>> GetAffectedCalculationsWithHeightStructure(HeightStructure structure)
        {
            IEnumerable<StructuresCalculation<HeightStructuresInput>> affectedCalculations =
                FailureMechanism.Calculations
                                .Cast<StructuresCalculation<HeightStructuresInput>>()
                                .Where(calc => ReferenceEquals(calc.InputParameters.Structure, structure));
            return affectedCalculations;
        }

        #endregion
    }
}