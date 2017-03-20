// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Core.Common.Base
{
    /// <summary>
    /// Dictionary that only allows items to be added for given keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public class FilteredKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly TKey[] allowedKeys;
        private readonly IDictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Creates a new instance of <see cref="FilteredKeyDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="allowedKeys">The key values that are allowed to add items with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="allowedKeys"/> is <c>null</c>.</exception>
        public FilteredKeyDictionary(TKey[] allowedKeys)
        {
            if (allowedKeys == null)
            {
                throw new ArgumentNullException(nameof(allowedKeys));
            }
            this.allowedKeys = allowedKeys;
        }

        /// <summary>
        /// Gets or sets the value for the given key.
        /// </summary>
        /// <param name="key">The key to get or set the value for.</param>
        /// <returns>The element with the specified key.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the property is retrieved and <paramref name="key" /> is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="key"/> is not allowed
        /// to be added to the <see cref="IDictionary"/>.</exception>
        public TValue this[TKey key]
        {
            get
            {
                return dictionary[key];
            }
            set
            {
                ValidateKey(key);

                dictionary[key] = value;
            }
        }

        /// <summary>
        /// Validates if the key is valid.
        /// </summary>
        /// <param name="key">The key to validate.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="key"/> is not allowed
        /// to be added to the <see cref="IDictionary"/>.</exception>
        private void ValidateKey(TKey key)
        {
            if (!allowedKeys.Contains(key))
            {
                throw new InvalidOperationException($"Key '{key}' is not allowed to add to the dictionary.");
            }
        }

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return dictionary.IsReadOnly;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return dictionary.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return dictionary.Values;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="IDictionary" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="IDictionary" />.</param>
        /// <exception cref="ArgumentException">Thrown when an element with the same key already exists in
        /// the <see cref="IDictionary" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the key of <paramref name="item"/> is 
        /// not allowed to be added to the <see cref="IDictionary"/>.</exception>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary" />.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when an element with the same key already exists in
        /// the <see cref="IDictionary" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="key"/> is not allowed
        /// to be added to the <see cref="IDictionary"/>.</exception>
        public void Add(TKey key, TValue value)
        {
            ValidateKey(key);

            dictionary.Add(key, value);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }
    }
}