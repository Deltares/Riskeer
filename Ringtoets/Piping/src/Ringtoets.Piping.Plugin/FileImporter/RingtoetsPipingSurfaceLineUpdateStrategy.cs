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
using Core.Common.Utils;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Strategy for updating the current surface lines with the imported surface lines:
    /// - Adds imported surface lines that are not part of the current collection.
    /// - Removes surface lines that are part of the current collection, but are not part of the imported surface line collection
    /// - Updates the surface lines that are part of the current collection and are poart of the imported sufrace line collection 
    /// </summary>
    public class RingtoetsPipingSurfaceLineUpdateDataStrategy : ISurfaceLineUpdateSurfaceLineStrategy
    {
        private PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsPipingSurfaceLineUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the surface lines are updated.</param>
        public RingtoetsPipingSurfaceLineUpdateDataStrategy(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
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

            return ModifySurfaceLineCollection(targetCollection, readRingtoetsPipingSurfaceLines, sourceFilePath);
        }

        private IEnumerable<IObservable> ModifySurfaceLineCollection(ObservableCollectionWithSourcePath<RingtoetsPipingSurfaceLine> existingCollection,
                                                                     IEnumerable<RingtoetsPipingSurfaceLine> readSurfaceLines,
                                                                     string sourceFilePath)
        {
            List<RingtoetsPipingSurfaceLine> readSurfaceLineList = readSurfaceLines.ToList();
            List<RingtoetsPipingSurfaceLine> addedSurfaceLines = GetAddedReadSurfaceLines(existingCollection, readSurfaceLineList).ToList();
            List<RingtoetsPipingSurfaceLine> updatedSurfaceLines = GetUpdatedSurfaceLines(existingCollection, readSurfaceLineList).ToList();
            List<RingtoetsPipingSurfaceLine> removedSurfaceLines = GetRemovedSurfaceLines(existingCollection, readSurfaceLineList).ToList();

            var affectedObjects = new List<IObservable>();
            if (addedSurfaceLines.Any())
            {
                affectedObjects.Add(existingCollection);
            }
            affectedObjects.AddRange(RemoveSurfaceLines(removedSurfaceLines));

            existingCollection.Clear();
            existingCollection.AddRange(addedSurfaceLines.Union(updatedSurfaceLines), sourceFilePath);

            return affectedObjects.Distinct(new ReferenceEqualityComparer<IObservable>());
        }

        private static IEnumerable<RingtoetsPipingSurfaceLine> GetRemovedSurfaceLines(IEnumerable<RingtoetsPipingSurfaceLine> existingCollection,
                                                                                      IEnumerable<RingtoetsPipingSurfaceLine> readSurfaceLine)
        {
            return existingCollection.Except(readSurfaceLine, new RingtoetsPipingSurfaceLineNameEqualityComparer());
        }

        private static IEnumerable<RingtoetsPipingSurfaceLine> GetUpdatedSurfaceLines(IEnumerable<RingtoetsPipingSurfaceLine> existingCollection,
                                                                                      IEnumerable<RingtoetsPipingSurfaceLine> readSurfaceLines)
        {
            return existingCollection.Intersect(readSurfaceLines, new RingtoetsPipingSurfaceLineNameEqualityComparer());
        }

        private static IEnumerable<RingtoetsPipingSurfaceLine> GetAddedReadSurfaceLines(IEnumerable<RingtoetsPipingSurfaceLine> existingCollection,
                                                                                        IEnumerable<RingtoetsPipingSurfaceLine> readSurfaceLines)
        {
            return readSurfaceLines.Except(existingCollection, new RingtoetsPipingSurfaceLineNameEqualityComparer());
        }

        #region Removing surface line helpers

        private IEnumerable<IObservable> RemoveSurfaceLines(IEnumerable<RingtoetsPipingSurfaceLine> removedSurfaceLines)
        {
            var affectedObjects = new List<IObservable>();

            foreach (RingtoetsPipingSurfaceLine surfaceLine in removedSurfaceLines)
            {
                affectedObjects.AddRange(ClearSurfaceLineDependentData(surfaceLine));
            }
            return affectedObjects;
        }

        private IEnumerable<IObservable> ClearSurfaceLineDependentData(RingtoetsPipingSurfaceLine surfaceLine)
        {
            return PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, surfaceLine);
        }

        #endregion

        /// <summary>
        /// Class fpr comparing <see cref="RingtoetsPipingSurfaceLine"/> by only the name.
        /// </summary>
        private class RingtoetsPipingSurfaceLineNameEqualityComparer : IEqualityComparer<RingtoetsPipingSurfaceLine>
        {
            public bool Equals(RingtoetsPipingSurfaceLine x, RingtoetsPipingSurfaceLine y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(RingtoetsPipingSurfaceLine obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}