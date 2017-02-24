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
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.UpdateDataStrategies
{
    /// <summary>
    /// Strategy for updating the current collection and dependent data with imported data:
    /// <list type="bullet">
    /// <item>Adds imported items that are not part of the current collection.</item>
    /// <item>Removes items that are part of the current collection, but are not part of the imported item collection.</item>
    /// <item>Updates the items that are part of the current collection and are part of the imported item collection.</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TTargetData">The target data type.</typeparam>
    /// <typeparam name="TFailureMechanism">The failure mechanism in which the target collection should be updated.</typeparam>
    public abstract class UpdateDataStrategyBase<TTargetData, TFailureMechanism>
        where TTargetData : class
        where TFailureMechanism : IFailureMechanism
    {
        protected readonly TFailureMechanism failureMechanism;
        private readonly IEqualityComparer<TTargetData> equalityComparer;

        /// <summary>
        /// Instantiates a <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> object.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism which needs to be updated.</param>
        /// <param name="equalityComparer">The comparer which should be used to determine when two objects are equal.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        protected UpdateDataStrategyBase(TFailureMechanism failureMechanism, IEqualityComparer<TTargetData> equalityComparer)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            if (equalityComparer == null)
            {
                throw new ArgumentNullException(nameof(equalityComparer));
            }

            this.equalityComparer = equalityComparer;
            this.failureMechanism = failureMechanism;
        }

        /// <summary>
        /// Updates the (dependent) objects with new data from the imported data.
        /// </summary>
        /// <param name="objectsToUpdate">Objects that need to be updated.</param>
        /// <param name="importedDataCollection">The data that was imported.</param>
        /// <returns>An <see cref="IEnumerable{IObservable}"/> with affected objects.</returns>
        /// <exception cref="InvalidOperationException">Thrown when duplicate items are found.</exception>
        protected abstract IEnumerable<IObservable> UpdateData(IEnumerable<TTargetData> objectsToUpdate,
                                                               IEnumerable<TTargetData> importedDataCollection);

        /// <summary>
        /// Removes the objects and their dependent data.
        /// </summary>
        /// <param name="removedObjects">The objects that are removed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with affected objects.</returns>
        protected abstract IEnumerable<IObservable> RemoveData(IEnumerable<TTargetData> removedObjects);

        /// <summary>
        /// Updates the items and their associated data within the target collection with the data contained 
        /// in the imported data collection.
        /// </summary>
        /// <param name="targetDataCollection">The target collection that needs to be updated.</param>
        /// <param name="importedDataCollection">The imported data collection that is used to update
        /// the <paramref name="targetDataCollection"/>.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of affected objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when duplicate items are being added to the 
        /// <paramref name="targetDataCollection"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when duplicate items are found during the 
        /// update of the items in the <paramref name="targetDataCollection"/>.</exception>
        protected IEnumerable<IObservable> UpdateTargetCollectionData(ObservableUniqueItemCollectionWithSourcePath<TTargetData> targetDataCollection,
                                                                      IEnumerable<TTargetData> importedDataCollection,
                                                                      string sourceFilePath)
        {
            if (targetDataCollection == null)
            {
                throw new ArgumentNullException(nameof(targetDataCollection));
            }
            if (importedDataCollection == null)
            {
                throw new ArgumentNullException(nameof(importedDataCollection));
            }
            if (sourceFilePath == null)
            {
                throw new ArgumentNullException(nameof(sourceFilePath));
            }

            return ModifyDataCollection(targetDataCollection, importedDataCollection, sourceFilePath);
        }

        /// <summary>
        /// Identifies which items were changed, removed and added to the target collection 
        /// when compared with the imported data and performs the necessary operations for 
        /// the dependent data of the affected elements. 
        /// </summary>
        /// <param name="targetDataCollection">The target data collection which needs to be updated.</param>
        /// <param name="importedDataCollection">The imported data collection which is used to update 
        /// the <paramref name="targetDataCollection"/> </param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> with affected objects.</returns>
        /// <exception cref="ArgumentException">Thrown when duplicate items are being added to the 
        /// <paramref name="targetDataCollection"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when duplicate items are found during the 
        /// update of the items to be updatd in the <paramref name="targetDataCollection"/>.</exception>
        private IEnumerable<IObservable> ModifyDataCollection(ObservableUniqueItemCollectionWithSourcePath<TTargetData> targetDataCollection,
                                                              IEnumerable<TTargetData> importedDataCollection,
                                                              string sourceFilePath)
        {
            TTargetData[] importedObjects = importedDataCollection.ToArray();
            TTargetData[] objectsToBeAdded = GetObjectsToBeAdded(targetDataCollection, importedObjects).ToArray();
            TTargetData[] objectsToBeRemoved = GetObjectsToBeRemoved(targetDataCollection, importedObjects).ToArray();
            TTargetData[] objectsToBeUpdated = GetObjectsToBeUpdated(targetDataCollection, importedObjects).ToArray();

            var affectedObjects = new List<IObservable>();
            if (objectsToBeAdded.Any())
            {
                affectedObjects.Add(targetDataCollection);
            }
            affectedObjects.AddRange(UpdateData(objectsToBeUpdated, importedObjects));
            affectedObjects.AddRange(RemoveData(objectsToBeRemoved));
            targetDataCollection.Clear();
            targetDataCollection.AddRange(objectsToBeUpdated.Union(objectsToBeAdded), sourceFilePath);

            return affectedObjects.Distinct(new ReferenceEqualityComparer<IObservable>());
        }

        private IEnumerable<TTargetData> GetObjectsToBeRemoved(IEnumerable<TTargetData> existingCollection, IEnumerable<TTargetData> importedDataOjects)
        {
            return existingCollection.Except(importedDataOjects, equalityComparer);
        }

        private IEnumerable<TTargetData> GetObjectsToBeUpdated(IEnumerable<TTargetData> existingCollection, IEnumerable<TTargetData> importedDataObjects)
        {
            return existingCollection.Intersect(importedDataObjects, equalityComparer);
        }

        private IEnumerable<TTargetData> GetObjectsToBeAdded(IEnumerable<TTargetData> existingCollection, IEnumerable<TTargetData> importedDataObjects)
        {
            return importedDataObjects.Where(source => !existingCollection.Contains(source, equalityComparer));
        }
    }
}