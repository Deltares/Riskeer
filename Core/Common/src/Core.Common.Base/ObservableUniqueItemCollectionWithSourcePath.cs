// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Properties;
using Core.Common.Util;

namespace Core.Common.Base
{
    /// <summary>
    /// A collection to store unique elements based on their feature and the source path
    /// they were imported from.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in the collection.</typeparam>
    public abstract class ObservableUniqueItemCollectionWithSourcePath<TElement> : Observable, IEnumerable<TElement>
        where TElement : class
    {
        private readonly List<TElement> collection = new List<TElement>();
        private readonly Func<TElement, object> getUniqueFeature;
        private readonly string typeDescriptor;
        private readonly string featureDescription;

        /// <summary>
        /// Creates a new instance of <see cref="ObservableUniqueItemCollectionWithSourcePath{TObject}"/>.
        /// </summary>
        /// <param name="getUniqueFeature">A function to retrieve the unique feature of the items it stores.</param>
        /// <param name="typeDescriptor">The description of the item that is validated.</param>
        /// <param name="featureDescription">The description of the feature of the item to be validated on.</param>
        protected ObservableUniqueItemCollectionWithSourcePath(Func<TElement, object> getUniqueFeature,
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
        public TElement this[int i]
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
        /// <param name="item">The item of type <see cref="TElement"/> to be removed.</param>
        /// <returns><c>true</c> if the <paramref name="item"/> was successfully removed from the collection;
        /// <c>false</c> if otherwise or if the <paramref name="item"/> was not found in the collection. </returns>
        public bool Remove(TElement item)
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
        public void AddRange(IEnumerable<TElement> items, string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!IOUtils.IsValidFilePath(filePath) && filePath.Length > 0)
            {
                throw new ArgumentException($@"'{filePath}' is not a valid file path.", nameof(filePath));
            }

            InternalValidateItems(items);

            SourcePath = filePath;
            collection.AddRange(items);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Validates the items of <paramref name="items"/> based on their feature.
        /// </summary>
        /// <param name="items">The items to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is 
        /// <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when 
        /// <list type="bullet">
        /// <item>one of the items is <c>null</c></item>
        /// <item>when a duplicate item was found.</item>
        /// </list>
        /// </exception>
        private void InternalValidateItems(IEnumerable<TElement> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (items.Contains(null))
            {
                throw new ArgumentException(@"Collection cannot contain null.", nameof(items));
            }

            IEnumerable<IGrouping<object, TElement>> duplicateItems =
                items.Concat(collection).GroupBy(getUniqueFeature)
                     .Where(group => group.Count() > 1);

            if (duplicateItems.Any())
            {
                string duplicateFeatures = string.Join(", ", duplicateItems.Select(group => getUniqueFeature(group.First())));
                string exceptionMessage = string.Format(
                    Resources.ObservableUniqueItemCollectionWithSourcePath_ValidateItems_TypeDescriptor_0_must_have_unique_FeatureDescription_1_Found_duplicate_items_DuplicateFeatures_2,
                    typeDescriptor,
                    featureDescription,
                    duplicateFeatures);
                throw new ArgumentException(exceptionMessage);
            }
        }
    }
}