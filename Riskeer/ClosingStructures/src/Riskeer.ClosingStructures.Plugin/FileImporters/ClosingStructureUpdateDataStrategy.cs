﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Service;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.Forms;
using Riskeer.Common.IO.Structures;

namespace Riskeer.ClosingStructures.Plugin.FileImporters
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> implementation for 
    /// updating closing structures based on imported data.
    /// </summary>
    public class ClosingStructureUpdateDataStrategy : UpdateDataStrategyBase<ClosingStructure, ClosingStructuresFailureMechanism>,
                                                      IStructureUpdateStrategy<ClosingStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructureUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the structures are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public ClosingStructureUpdateDataStrategy(ClosingStructuresFailureMechanism failureMechanism)
            : base(failureMechanism, failureMechanism?.ClosingStructures, new StructureIdEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateStructuresWithImportedData(IEnumerable<ClosingStructure> readStructures, string sourceFilePath)
        {
            return UpdateTargetCollectionData(readStructures, sourceFilePath);
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(ClosingStructure removedObject)
        {
            return ClosingStructuresDataSynchronizationService.RemoveStructure(removedObject, FailureMechanism);
        }

        #region Updating Data Functions

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(ClosingStructure objectToUpdate,
                                                                                 ClosingStructure objectToUpdateFrom)
        {
            objectToUpdate.CopyProperties(objectToUpdateFrom);

            return UpdateClosingStructureDependentData(objectToUpdate);
        }

        private IEnumerable<IObservable> UpdateClosingStructureDependentData(ClosingStructure structure)
        {
            var affectedObjects = new List<IObservable>();

            affectedObjects.AddRange(GetAffectedCalculationsWithClosingStructure(structure)
                                         .Select(affectedCalculation => affectedCalculation.InputParameters));

            return affectedObjects;
        }

        private IEnumerable<StructuresCalculation<ClosingStructuresInput>> GetAffectedCalculationsWithClosingStructure(ClosingStructure structure)
        {
            IEnumerable<StructuresCalculation<ClosingStructuresInput>> affectedCalculations =
                FailureMechanism.Calculations
                                .Cast<StructuresCalculation<ClosingStructuresInput>>()
                                .Where(calc => ReferenceEquals(calc.InputParameters.Structure, structure));
            return affectedCalculations;
        }

        #endregion
    }
}