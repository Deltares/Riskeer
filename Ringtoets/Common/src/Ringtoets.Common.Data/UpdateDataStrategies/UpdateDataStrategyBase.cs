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
using Core.Common.Utils;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Properties;

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
        where TTargetData : Observable
        where TFailureMechanism : IFailureMechanism
    {
        protected readonly TFailureMechanism FailureMechanism;
        private readonly IEqualityComparer<TTargetData> equalityComparer;

        /// <summary>
        /// Creates a new instance of <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> object.
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
            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Updates the object and its dependent data with data from the imported data.
        /// </summary>
        /// <param name="objectToUpdate">Object that needs to be updated.</param>
        /// <param name="objectToUpdateFrom">The object to update from.</param>
        /// <returns>An <see cref="IEnumerable{IObservable}"/> with affected objects.</returns>
        /// <exception cref="InvalidOperationException">Thrown when duplicate items are found.</exception>
        protected abstract IEnumerable<IObservable> UpdateObjectAndDependentData(TTargetData objectToUpdate,
                                                                                 TTargetData objectToUpdateFrom);

        /// <summary>
        /// Removes the objects and their dependent data.
        /// </summary>
        /// <param name="removedObject">The object that is removed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with affected objects.</returns>
        protected abstract IEnumerable<IObservable> RemoveObjectAndDependentData(TTargetData removedObject);

        /// <summary>
        /// Updates the items and their associated data within the target collection with the data contained 
        /// in the imported data collection.
        /// </summary>
        /// <param name="targetDataCollection">The target collection that needs to be updated.</param>
        /// <param name="importedDataCollection">The imported data collection that is used to update
        /// the <paramref name="targetDataCollection"/>.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of affected objects.</returns>
        /// <exception cref="UpdateDataException">Thrown when an error occurred while updating the data.</exception>
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

            try
            {
                return ModifyDataCollection(targetDataCollection, importedDataCollection, sourceFilePath);
            }
            catch (ArgumentException e)
            {
                throw new UpdateDataException(e.Message, e);
            }
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
        /// <exception cref="UpdateDataException">Thrown when duplicate items are found in the 
        /// <paramref name="importedDataCollection"/>.</exception>
        private IEnumerable<IObservable> ModifyDataCollection(ObservableUniqueItemCollectionWithSourcePath<TTargetData> targetDataCollection,
                                                              IEnumerable<TTargetData> importedDataCollection,
                                                              string sourceFilePath)
        {
            TTargetData[] importedObjects = importedDataCollection.ToArray();
            TTargetData[] objectsToBeAdded = GetObjectsToBeAdded(targetDataCollection, importedObjects).ToArray();
            TTargetData[] objectsToBeRemoved = GetObjectsToBeRemoved(targetDataCollection, importedObjects).ToArray();
            TTargetData[] objectsToBeUpdated = GetObjectsToBeUpdated(targetDataCollection, importedObjects).ToArray();

            var affectedObjects = new List<IObservable>();
            if (objectsToBeAdded.Any() || objectsToBeRemoved.Any() || objectsToBeUpdated.Any())
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

        /// <summary>
        /// Updates all the objects and their dependent data that needs to be 
        /// updated with data from the imported data collection.
        /// </summary>
        /// <param name="objectsToUpdate">The objects that need to be updated.</param>
        /// <param name="importedDataCollection">The data to update from.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of affected items.</returns>
        /// <exception cref="UpdateDataException">Thrown when the imported 
        /// <paramref name="importedDataCollection"/> contains duplicate items.</exception>
        private IEnumerable<IObservable> UpdateData(IEnumerable<TTargetData> objectsToUpdate,
                                                    IEnumerable<TTargetData> importedDataCollection)
        {
            var affectedObjects = new List<IObservable>();
            if (importedDataCollection.Count() != importedDataCollection.Distinct(equalityComparer).Count())
            {
                throw new UpdateDataException(Resources.UpdateDataStrategyBase_UpdateTargetCollectionData_Imported_data_must_contain_unique_items);
            }

            foreach (TTargetData objectToUpdate in objectsToUpdate)
            {
                TTargetData objectToUpdateFrom = importedDataCollection.Single(importedObject =>
                                                                                   equalityComparer.Equals(importedObject, objectToUpdate));
                if (!objectToUpdate.Equals(objectToUpdateFrom))
                {
                    affectedObjects.Add(objectToUpdate);
                }
                affectedObjects.AddRange(UpdateObjectAndDependentData(objectToUpdate, objectToUpdateFrom));
            }

            return affectedObjects;
        }

        /// <summary>
        /// Removes all the objects and their dependent data.
        /// </summary>
        /// <param name="objectsToRemove">The objects that need to be removed.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of affected items.</returns>
        private IEnumerable<IObservable> RemoveData(IEnumerable<TTargetData> objectsToRemove)
        {
            var affectedObjects = new List<IObservable>();

            foreach (TTargetData objectToRemove in objectsToRemove)
            {
                affectedObjects.AddRange(RemoveObjectAndDependentData(objectToRemove));
            }
            return affectedObjects;
        }
    }
}