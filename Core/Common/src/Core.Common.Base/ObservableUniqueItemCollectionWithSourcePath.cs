﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;

namespace Core.Common.Base
{
    /// <summary>
    /// A collection to store unique elements based on their feature and the source path
    /// they were imported from.
    /// </summary>
    /// <typeparam name="TObject">The type of elements in the collection.</typeparam>
    public abstract class ObservableUniqueItemCollectionWithSourcePath<TObject> : Observable, IEnumerable<TObject>
        where TObject : class
    {
        private readonly List<TObject> collection = new List<TObject>();
        private readonly Func<TObject, object> getUniqueFeature;
        private readonly string typeDescriptor;
        private readonly string featureDescription;

        /// <summary>
        /// Instantiates a <see cref="ObservableUniqueItemCollectionWithSourcePath{TObject}"/>.
        /// </summary>
        /// <param name="getUniqueFeature">A function to retrieve the unique feature of the items it stores.</param>
        /// <param name="typeDescriptor">The description of the item that is validated.</param>
        /// <param name="featureDescription">The description of the feature of the item to be validated on.</param>
        public ObservableUniqueItemCollectionWithSourcePath(Func<TObject, object> getUniqueFeature,
                                                            string typeDescriptor,
                                                            string featureDescription)
        {
            if (getUniqueFeature == null)
            {
                throw new ArgumentNullException(nameof(getUniqueFeature));
            }
            if (typeDescriptor == null)
            {
                throw new ArgumentNullException(nameof(typeDescriptor));
            }
            if (featureDescription == null)
            {
                throw new ArgumentNullException(nameof(featureDescription));
            }

            this.getUniqueFeature = getUniqueFeature;
            this.typeDescriptor = typeDescriptor;
            this.featureDescription = featureDescription;
        }

        /// <summary>
        /// Gets the element at index <paramref name="i"/> in the collection.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The element at index <paramref name="i"/> in the collection.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="i"/> is not 
        /// between [0, <see cref="Count"/>)</exception>
        public TObject this[int i]
        {
            get
            {
                return collection[i];
            }
        }

        /// <summary>
        /// Gets the amount of items stored in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return collection.Count;
            }
        }

        /// <summary>
        /// Gets the last known file path from which the elements were imported.
        /// </summary>
        /// <returns>The path where the elements originate
        /// from, or <c>null</c> if the collection is cleared.</returns>
        public string SourcePath { get; private set; }

        /// <summary>
        /// Removes the first occurrence of <paramref name="item"/> in the collection.
        /// </summary>
        /// <param name="item">The item of type <see cref="TObject"/> to be removed.</param>
        /// <returns><c>True</c> if the <paramref name="item"/> was successfully removed from the collection;
        /// <c>False</c> if otherwise or if the <paramref name="item"/> was not found in the collection. </returns>
        public bool Remove(TObject item)
        {
            bool remove = collection.Remove(item);
            if (remove && Count == 0)
            {
                SourcePath = null;
            }
            return remove;
        }

        /// <summary>
        /// Clears the imported items in the collection and the <see cref="SourcePath"/>.
        /// </summary>
        public void Clear()
        {
            SourcePath = null;
            collection.Clear();
        }

        /// <summary>
        /// Adds all elements originating from a source file.
        /// </summary>
        /// <param name="items">The elements to add</param>
        /// <param name="filePath">The path to the source file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="items"/> contains <c>null</c>.</item>
        /// <item><paramref name="filePath"/> is not a valid file path.</item>
        /// <item>an element in <paramref name="items"/> is invalid.</item>
        /// </list>
        /// </exception>
        public void AddRange(IEnumerable<TObject> items, string filePath)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (!IOUtils.IsValidFilePath(filePath))
            {
                throw new ArgumentException($"'{filePath}' is not a valid filepath.", nameof(filePath));
            }
            InternalValidateItems(items);

            SourcePath = filePath;
            collection.AddRange(items);
        }

        public IEnumerator<TObject> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void InternalValidateItems(IEnumerable<TObject> items)
        {
            if (items.Contains(null))
            {
                throw new ArgumentException("Collection cannot contain null.", nameof(items));
            }
            ValidateItems(items);
        }

        /// <summary>
        /// Perform additional validations over <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to validate.</param>
        /// <exception cref="ArgumentException">Throw an exception when validation fails.</exception>
        private void ValidateItems(IEnumerable<TObject> items)
        {
            ValidateListOnDuplicateFeature(items);
        }

        /// <summary>
        /// Validates the items of an <see cref="IEnumerable{TObject}"/> based on their feature.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when any parameters are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a duplicate item was found.</exception>
        private void ValidateListOnDuplicateFeature(IEnumerable<TObject> items)
        {
            IEnumerable<IGrouping<object, TObject>> duplicateItems =
                items.GroupBy(getUniqueFeature)
                     .Where(group => group.Count() > 1);

            if (duplicateItems.Any())
            {
                var duplicateFeatures = string.Join(", ", duplicateItems.Select(group => group.First()));
                string exceptionMessage = $"{typeDescriptor} moeten een unieke {featureDescription} hebben. Gevonden dubbele elementen: {duplicateFeatures}.";
                throw new ArgumentException(exceptionMessage);
            }
        }
    }
}