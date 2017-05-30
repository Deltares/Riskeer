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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.Forms;
using Ringtoets.Common.IO.Structures;
using Ringtoets.Common.Service;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.Plugin.FileImporters
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
            : base(failureMechanism, new StructureIdEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateStructuresWithImportedData(StructureCollection<StabilityPointStructure> targetDataCollection,
                                                                         IEnumerable<StabilityPointStructure> readStructures,
                                                                         string sourceFilePath)
        {
            return UpdateTargetCollectionData(targetDataCollection, readStructures, sourceFilePath);
        }

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(StabilityPointStructure objectToUpdate, StabilityPointStructure objectToUpdateFrom)
        {
            objectToUpdate.CopyProperties(objectToUpdateFrom);

            var affectedObjects = new List<IObservable>
            {
                objectToUpdate
            };
            affectedObjects.AddRange(FailureMechanism.Calculations
                                                     .OfType<StructuresCalculation<StabilityPointStructuresInput>>()
                                                     .Select(calc => calc.InputParameters)
                                                     .Where(inp => ReferenceEquals(inp.Structure, objectToUpdate)));

            return affectedObjects;
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(StabilityPointStructure removedObject)
        {
            return RingtoetsCommonDataSynchronizationService.RemoveStructure(removedObject, FailureMechanism.Calculations
                                                                                                            .OfType<StructuresCalculation<StabilityPointStructuresInput>>()
                                                                                                            .Where(calc => ReferenceEquals(calc.InputParameters.Structure, removedObject)),
                                                                             FailureMechanism.StabilityPointStructures,
                                                                             FailureMechanism.SectionResults);
        }
    }
}