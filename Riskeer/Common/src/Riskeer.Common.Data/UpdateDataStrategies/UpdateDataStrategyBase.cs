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
using Core.Common.Util;
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
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism in which the target collection should be updated.</typeparam>
    public abstract class UpdateDataStrategyBase<TTargetData, TFailureMechanism>
        where TTargetData : Observable
        where TFailureMechanism : IFailureMechanism
    {
        private readonly IEqualityComparer<TTargetData> equalityComparer;
        private readonly ObservableUniqueItemCollectionWithSourcePath<TTargetData> targetCollection;

        /// <summary>
        /// Creates a new instance of <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism which needs to be updated.</param>
        /// <param name="targetCollection">The collection to add the updated objects to.</param>
        /// <param name="equalityComparer">The comparer which should be used to determine when two objects are equal.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        protected UpdateDataStrategyBase(
            TFailureMechanism failureMechanism,
            ObservableUniqueItemCollectionWithSourcePath<TTargetData> targetCollection,
            IEqualityComparer<TTargetData> equalityComparer)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (equalityComparer == null)
            {
                throw new ArgumentNullException(nameof(equalityComparer));
            }

            if (targetCollection == null)
            {
                throw new ArgumentNullException(nameof(targetCollection));
            }

            this.equalityComparer = equalityComparer;
            this.targetCollection = targetCollection;
            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        protected TFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Updates the unequal object and its dependent data with data from the imported data.
        /// </summary>
        /// <param name="objectToUpdate">The object that needs to be updated.</param>
        /// <param name="objectToUpdateFrom">The object to update from.</param>
        /// <returns>An <see cref="IEnumerable{IObservable}"/> with affected objects.</returns>
        /// <exception cref="InvalidOperationException">Thrown when duplicate items are found.</exception>
        protected abstract IEnumerable<IObservable> UpdateObjectAndDependentData(TTargetData objectToUpdate,
                                                                                 TTargetData objectToUpdateFrom);

        /// <summary>
        /// Removes the object and its dependent data.
        /// </summary>
        /// <param name="removedObject">The object to remove.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with affected objects.</returns>
        protected abstract IEnumerable<IObservable> RemoveObjectAndDependentData(TTargetData removedObject);

        /// <summary>
        /// Updates the items and their associated data within the target collection with the data contained 
        /// in the imported data collection.
        /// </summary>
        /// <param name="importedDataCollection">The imported data collection that is used to update
        /// the <see cref="targetCollection"/>.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of affected objects.</returns>
        /// <exception cref="UpdateDataException">Thrown when an error occurred while updating the data.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="importedDataCollection"/> contains 
        /// <c>null</c> elements.</exception>
        protected IEnumerable<IObservable> UpdateTargetCollectionData(IEnumerable<TTargetData> importedDataCollection,
                                                                      string sourceFilePath)
        {
            if (importedDataCollection == null)
            {
                throw new ArgumentNullException(nameof(importedDataCollection));
            }

            if (sourceFilePath == null)
            {
                throw new ArgumentNullException(nameof(sourceFilePath));
            }

            return ModifyDataCollection(importedDataCollection, sourceFilePath);
        }

        /// <summary>
        /// Identifies which items were changed, removed and added to the target collection 
        /// when compared with the imported data and performs the necessary operations for 
        /// the dependent data of the affected elements.
        /// </summary>
        /// <param name="importedDataCollection">The imported data collection which is used to update 
        /// the <see cref="targetCollection"/>.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with affected objects.</returns>
        /// <exception cref="UpdateDataException">Thrown when:
        /// <list type="bullet">
        /// <item>duplicate items are being added to the <see cref="targetCollection"/>;</item>
        /// <item>duplicate items are found in the <paramref name="importedDataCollection"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="importedDataCollection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="importedDataCollection"/> contains 
        /// <c>null</c> elements.</exception>
        private IEnumerable<IObservable> ModifyDataCollection(IEnumerable<TTargetData> importedDataCollection,
                                                              string sourceFilePath)
        {
            TTargetData[] importedObjects = importedDataCollection.ToArray();

            var modification = new CollectionModification(targetCollection, importedObjects, equalityComparer);

            var affectedObjects = new List<IObservable>();
            if (modification.HasUpdates())
            {
                affectedObjects.Add(targetCollection);
            }

            affectedObjects.AddRange(UpdateData(modification.ObjectsToBeUpdated.Values, importedObjects));
            affectedObjects.AddRange(RemoveData(modification.ObjectsToBeRemoved));
            targetCollection.Clear();
            targetCollection.AddRange(modification.GetModifiedCollection(), sourceFilePath);

            return affectedObjects.Distinct(new ReferenceEqualityComparer<IObservable>());
        }

        /// <summary>
        /// Updates all the objects and their dependent data that needs to be 
        /// updated with data from the imported data collection.
        /// </summary>
        /// <param name="objectsToUpdate">The objects that need to be updated.</param>
        /// <param name="importedDataCollection">The data to update from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of affected items.</returns>
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

                if (IsUpdateObjectDataUnequal(objectToUpdate, objectToUpdateFrom))
                {
                    affectedObjects.Add(objectToUpdate);
                    affectedObjects.AddRange(UpdateObjectAndDependentData(objectToUpdate, objectToUpdateFrom));
                }
            }

            return affectedObjects;
        }

        private static bool IsUpdateObjectDataUnequal(TTargetData objectToUpdate, TTargetData objectToUpdateFrom)
        {
            return !objectToUpdate.Equals(objectToUpdateFrom);
        }

        /// <summary>
        /// Removes all the objects and their dependent data.
        /// </summary>
        /// <param name="objectsToRemove">The objects that need to be removed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of affected items.</returns>
        private IEnumerable<IObservable> RemoveData(IEnumerable<TTargetData> objectsToRemove)
        {
            var affectedObjects = new List<IObservable>();

            foreach (TTargetData objectToRemove in objectsToRemove)
            {
                affectedObjects.AddRange(RemoveObjectAndDependentData(objectToRemove));
            }

            return affectedObjects;
        }

        /// <summary>
        /// Inner class for obtaining the modifications of an update action.
        /// </summary>
        private class CollectionModification
        {
            /// <summary>
            /// Creates a new instance of <see cref="CollectionModification"/>.
            /// </summary>
            /// <param name="existingObjects">The current collection of objects.</param>
            /// <param name="updatedObjects">The collection containing objects that were imported
            /// and thus could contain updates for the existing objects.</param>
            /// <param name="equalityComparer">The comparer to test whether elements in
            /// <paramref name="existingObjects"/> have a matching element in 
            /// <paramref name="updatedObjects"/>.</param>
            /// <exception cref="ArgumentNullException">Thrown if any parameter is <c>null</c>.</exception>
            /// <exception cref="ArgumentException">Thrown when <paramref name="existingObjects"/>
            /// or <paramref name="updatedObjects"/> contains a <c>null</c> element.</exception>
            public CollectionModification(IEnumerable<TTargetData> existingObjects,
                                          IEnumerable<TTargetData> updatedObjects,
                                          IEqualityComparer<TTargetData> equalityComparer)
            {
                if (existingObjects == null)
                {
                    throw new ArgumentNullException(nameof(existingObjects));
                }

                if (updatedObjects == null)
                {
                    throw new ArgumentNullException(nameof(updatedObjects));
                }

                if (equalityComparer == null)
                {
                    throw new ArgumentNullException(nameof(equalityComparer));
                }

                if (existingObjects.Contains(null))
                {
                    throw new ArgumentException(@"Cannot determine modifications from a collection which contain null.", nameof(existingObjects));
                }

                if (updatedObjects.Contains(null))
                {
                    throw new ArgumentException(@"Cannot determine modifications with a collection which contain null.", nameof(updatedObjects));
                }

                TTargetData[] existingArray = existingObjects.ToArray();

                var index = 0;
                foreach (TTargetData importedObject in updatedObjects)
                {
                    int existingObjectIndex = FindIndex(existingArray, importedObject, equalityComparer);
                    if (existingObjectIndex > -1)
                    {
                        ObjectsToBeUpdated.Add(index, existingArray[existingObjectIndex]);
                        existingArray[existingObjectIndex] = null;
                    }
                    else
                    {
                        ObjectsToBeAdded.Add(index, importedObject);
                    }

                    index++;
                }

                ObjectsToBeRemoved = existingArray.Where(e => e != null);
            }

            /// <summary>
            /// Gets the objects that were updated.
            /// </summary>
            public Dictionary<int, TTargetData> ObjectsToBeUpdated { get; } = new Dictionary<int, TTargetData>();

            /// <summary>
            /// Gets the objects that were removed.
            /// </summary>
            public IEnumerable<TTargetData> ObjectsToBeRemoved { get; }

            /// <summary>
            /// Gets a collection of updated objects from the existing object collection and the
            /// added objects from the imported object collection in the same order that was
            /// found in the imported object collection.
            /// </summary>
            /// <returns>An ordered collection of updated and added elements.</returns>
            public IEnumerable<TTargetData> GetModifiedCollection()
            {
                KeyValuePair<int, TTargetData>[] remainingObjects = ObjectsToBeUpdated.Union(ObjectsToBeAdded).ToArray();

                var orderedObjects = new TTargetData[remainingObjects.Length];

                foreach (KeyValuePair<int, TTargetData> remainingObject in remainingObjects)
                {
                    orderedObjects[remainingObject.Key] = remainingObject.Value;
                }

                return orderedObjects;
            }

            /// <summary>
            /// Finds out whether there was a difference between the existing and the imported
            /// object collections.
            /// </summary>
            /// <returns><c>true</c> if there were any updates, <c>false</c> otherwise.</returns>
            public bool HasUpdates()
            {
                return ObjectsToBeRemoved.Any() || ObjectsToBeAdded.Any() || ObjectsToBeUpdated.Any();
            }

            private Dictionary<int, TTargetData> ObjectsToBeAdded { get; } = new Dictionary<int, TTargetData>();

            private static int FindIndex(TTargetData[] collectionToLookIn, TTargetData objectToFind, IEqualityComparer<TTargetData> equalityComparer)
            {
                for (var i = 0; i < collectionToLookIn.Length; i++)
                {
                    TTargetData targetData = collectionToLookIn[i];
                    if (targetData != null && equalityComparer.Equals(targetData, objectToFind))
                    {
                        return i;
                    }
                }

                return -1;
            }
        }
    }
}