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
using Core.Common.Base;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Service;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.Structures;

namespace Riskeer.ClosingStructures.Plugin.FileImporters
{
    /// <summary>
    /// A <see cref="ReplaceDataStrategyBase{TTargetData,TFailureMechanism}"/> to replace closing 
    /// structures with the imported closing structures.
    /// </summary>
    public class ClosingStructureReplaceDataStrategy : ReplaceDataStrategyBase<ClosingStructure, ClosingStructuresFailureMechanism>,
                                                       IStructureUpdateStrategy<ClosingStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructureReplaceDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the closing structures are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public ClosingStructureReplaceDataStrategy(ClosingStructuresFailureMechanism failureMechanism)
            : base(failureMechanism, failureMechanism?.ClosingStructures) {}

        public IEnumerable<IObservable> UpdateStructuresWithImportedData(IEnumerable<ClosingStructure> readStructures, string sourceFilePath)
        {
            return ReplaceTargetCollectionWithImportedData(readStructures, sourceFilePath);
        }

        protected override IEnumerable<IObservable> ClearData()
        {
            return ClosingStructuresDataSynchronizationService.RemoveAllStructures(FailureMechanism);
        }
    }
}