// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Strategy to replace the surface lines with the imported surface lines.
    /// </summary>
    public class RingtoetsPipingSurfaceLineReplaceDataStrategy : ISurfaceLineUpdateSurfaceLineStrategy
    {
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsPipingSurfaceLineReplaceDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the surface lines are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="failureMechanism"/>is <c>null</c>.</exception>
        public RingtoetsPipingSurfaceLineReplaceDataStrategy(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(this.failureMechanism));
            }

            this.failureMechanism = failureMechanism;
        }

        public IEnumerable<IObservable> UpdateSurfaceLinesWithImportedData(ObservableCollectionWithSourcePath<RingtoetsPipingSurfaceLine> targetCollection,
                                                                    IEnumerable<RingtoetsPipingSurfaceLine> readRingtoetsPipingSurfaceLines,
                                                                    string sourceFilePath)
        {
            if (targetCollection == null)
            {
                throw new ArgumentNullException(nameof(targetCollection));
            }
            if (readRingtoetsPipingSurfaceLines == null)
            {
                throw new ArgumentNullException(nameof(readRingtoetsPipingSurfaceLines));
            }
            if (sourceFilePath == null)
            {
                throw new ArgumentNullException(nameof(sourceFilePath));
            }

            var affectedObjects = new List<IObservable>
            {
                targetCollection
            };

            foreach (RingtoetsPipingSurfaceLine surfaceLine in targetCollection.ToArray())
            {
                affectedObjects.AddRange(PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, surfaceLine));
            }
            targetCollection.AddRange(readRingtoetsPipingSurfaceLines, sourceFilePath);

            return affectedObjects.Distinct();
        }
    }
}