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
using Core.Common.Base;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Service;

namespace Ringtoets.HeightStructures.Plugin.FileImporters
{
    /// <summary>
    /// A <see cref="ReplaceDataStrategyBase{TTargetData,TFailureMechanism}"/> to replace height 
    /// structures with the imported height structures.
    /// </summary>
    public class HeightStructureReplaceDataStrategy : ReplaceDataStrategyBase<HeightStructure, HeightStructuresFailureMechanism>,
                                                      IStructureUpdateStrategy<HeightStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructureReplaceDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the height structures are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public HeightStructureReplaceDataStrategy(HeightStructuresFailureMechanism failureMechanism)
            : base(failureMechanism) {}

        public IEnumerable<IObservable> UpdateStructuresWithImportedData(StructureCollection<HeightStructure> targetDataCollection,
                                                                         IEnumerable<HeightStructure> readStructures,
                                                                         string sourceFilePath)
        {
            try
            {
                return ReplaceTargetCollectionWithImportedData(targetDataCollection, readStructures, sourceFilePath);
            }
            catch (UpdateDataException e)
            {
                string message = string.Format(Resources.IStructureUpdateStrategy_UpdateStructuresWithImportedData_Importing_Structures_failed_Reason_0_,
                                               e.Message);
                throw new StructureUpdateException(message, e);
            }
        }

        protected override IEnumerable<IObservable> ClearData()
        {
            return HeightStructuresDataSynchronizationService.RemoveAllStructures(FailureMechanism);
        }
    }
}