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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.Structures;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Service;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;

namespace Ringtoets.ClosingStructures.Plugin.FileImporters
{
    /// <summary>
    /// A <see cref="ReplaceDataStrategyBase{TTargetData,TFailureMechanism}"/> to replace Closing 
    /// structures with the imported Closing structures.
    /// </summary>
    public class ClosingStructureReplaceDataStrategy : ReplaceDataStrategyBase<ClosingStructure, ClosingStructuresFailureMechanism>,
                                                      IStructureUpdateStrategy<ClosingStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructureReplaceDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the Closing structures are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public ClosingStructureReplaceDataStrategy(ClosingStructuresFailureMechanism failureMechanism)
            : base(failureMechanism) {}

        public IEnumerable<IObservable> UpdateStructuresWithImportedData(StructureCollection<ClosingStructure> targetDataCollection,
                                                                         IEnumerable<ClosingStructure> readStructures,
                                                                         string sourceFilePath)
        {
            return ReplaceTargetCollectionWithImportedData(targetDataCollection, readStructures, sourceFilePath);
        }

        protected override IEnumerable<IObservable> ClearData()
        {
            return RingtoetsCommonDataSynchronizationService.RemoveAllStructures(
                FailureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>(), 
                FailureMechanism.ClosingStructures, 
                FailureMechanism.SectionResults);
        }
    }
}