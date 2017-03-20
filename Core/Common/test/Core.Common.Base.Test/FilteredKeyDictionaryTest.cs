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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Core.Common.Base.Test
{
    [TestFixture]
    public class FilteredKeyDictionaryTest
    {
        [Test]
        public void Constructor_AllowedKeysNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FilteredKeyDictionary<string, object>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("allowedKeys", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var keys = new[]
            {
                "key 1",
                "key 2"
            };

            // Call
            var dictionary = new FilteredKeyDictionary<string, object>(keys);

            // Assert
            Assert.IsInstanceOf<IDictionary<string, object>>(dictionary);
            CollectionAssert.IsEmpty(dictionary);
        }

        [Test]
        public void Indexer_GetItemForNonExistingKey_ThrowKeyNotFoundException()
        {
            // Setup
            var dictionary = new FilteredKeyDictionary<string, object>(new string[0]);

            // Call
            TestDelegate call = () =>
            {
                object item = dictionary["some key"];
            };

            // Assert
            Assert.Throws<KeyNotFoundException>(call);
        }

        [Test]
        public void Indexer_GetElementForExistingKey_ReturnExpectedItem()
        {
            // Setup
            const string key = "some key";
            var value = new object();

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            dictionary.Add(key, value);

            // Call
            object retrievedItem = dictionary[key];

            // Assert
            Assert.AreSame(value, retrievedItem);
        }

        [Test]
        public void Indexer_AddElementForInvalidKey_ThrowInvalidOperationException()
        {
            // Setup
            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            var value = new object();

            // Call
            TestDelegate test = () => dictionary["invalid"] = value;

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(test);
            Assert.AreEqual("Key 'invalid' is not allowed to add to the dictionary.", exception.Message);
            CollectionAssert.DoesNotContain(dictionary, new KeyValuePair<string, object>("invalid", value));
        }

        [Test]
        public void Indexer_AddElementForValidKey_KeyAdded()
        {
            // Setup
            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            var value = new object();
            var keyValuePair = new KeyValuePair<string, object>("some key", value);

            // Precondition            
            CollectionAssert.DoesNotContain(dictionary, keyValuePair);

            // Call
            dictionary["some key"] = value;

            // Assert
            CollectionAssert.Contains(dictionary, keyValuePair);
        }

        [Test]
        public void Count_Always_ReturnExpectedNumberOfElements()
        {
            // Setup
            const string key = "some key";

            var dictionary = new FilteredKeyDictionary<string, object>(new[]
            {
                key
            })
            {
                {
                    key, new object()
                }
            };

            // Call
            int nrOfElements = dictionary.Count;

            // Assert
            Assert.AreEqual(1, nrOfElements);
        }

        [Test]
        public void IsReadOnly_Always_ReturnFalse()
        {
            // Setup
            var dictionary = new FilteredKeyDictionary<string, object>(new string[0]);

            // Call
            bool isReadOnly = dictionary.IsReadOnly;

            // Assert
            Assert.IsFalse(isReadOnly);
        }

        [Test]
        public void Keys_Always_ReturnAddedKeys()
        {
            // Setup
            const string key1 = "first key";
            const string key2 = "some key";
            const string key3 = "other key";

            var dictionary = new FilteredKeyDictionary<string, object>(new[]
            {
                key1,
                key2,
                key3
            })
            {
                {
                    key1, new object()
                },
                {
                    key2, new object()
                },
                {
                    key3, new object()
                }
            };

            // Call
            ICollection<string> keys = dictionary.Keys;

            // Assert
            CollectionAssert.AreEquivalent(new[]
            {
                key1,
                key2,
                key3
            }, keys);
        }

        [Test]
        public void Values_Always_ReturnAddedValues()
        {
            // Setup
            const string key = "some key";

            object value1 = "value 1";
            object value2 = new object();
            object value3 = "value 3";

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            dictionary.Add(key, value2);

            // Call
            ICollection<object> values = dictionary.Values;

            // Assert
            CollectionAssert.AreEquivalent(new[]
            {
                value1,
                value2,
                value3
            }, values);
        }

        [Test]
        public void Add_KeyValid_ItemAdded()
        {
            // Setup
            const string key = "some key";
            var value = new object();

            var dictionary = new FilteredKeyDictionary<string, object>(new[]
            {
                key
            });

            // Call
            dictionary.Add(key, value);

            // Assert
            Assert.AreEqual(1, dictionary.Count);
            KeyValuePair<string, object> item = dictionary.First();
            Assert.AreEqual(key, item.Key);
            Assert.AreSame(value, item.Value);
        }

        [Test]
        public void AddItem_KeyValid_ItemAdded()
        {
            // Setup
            const string key = "some key";
            var value = new object();

            var dictionary = new FilteredKeyDictionary<string, object>(new[]
            {
                key
            });

            // Call
            dictionary.Add(new KeyValuePair<string, object>(key, value));

            // Assert
            Assert.AreEqual(1, dictionary.Count);
            KeyValuePair<string, object> item = dictionary.First();
            Assert.AreEqual(key, item.Key);
            Assert.AreSame(value, item.Value);
        }

        [Test]
        public void Add_KeyInvalid_ThrowInvalidOperationException()
        {
            // Setup
            const string key = "some key";

            var dictionary = new FilteredKeyDictionary<string, object>(new[]
            {
                key
            });

            // Call
            TestDelegate test = () => dictionary.Add("invalid key", new object());

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(test);
            Assert.AreEqual("Key 'invalid key' is not allowed to add to the dictionary.", exception.Message);
            CollectionAssert.IsEmpty(dictionary);
        }

        [Test]
        public void AddItem_KeyInvalid_ThrowInvalidOperationException()
        {
            // Setup
            const string key = "some key";

            var dictionary = new FilteredKeyDictionary<string, object>(new[]
            {
                key
            });

            // Call
            TestDelegate test = () => dictionary.Add(new KeyValuePair<string, object>("invalid key", new object()));

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(test);
            Assert.AreEqual("Key 'invalid key' is not allowed to add to the dictionary.", exception.Message);
            CollectionAssert.IsEmpty(dictionary);
        }

        [Test]
        public void Clear_Always_ClearDictionary()
        {
            // Setup
            const string key = "some key";

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            dictionary.Add(key, new object());

            // Precondition
            Assert.AreEqual(3, dictionary.Count);

            // Call
            dictionary.Clear();

            // Assert
            CollectionAssert.IsEmpty(dictionary);
        }

        [Test]
        public void Contains_ItemInDictionary_ReturnTrue()
        {
            // Setup
            const string key = "some key";
            var value = new object();

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            dictionary.Add(key, value);

            // Call
            bool contains = dictionary.Contains(new KeyValuePair<string, object>(key, value));

            // Assert
            Assert.IsTrue(contains);
        }

        [Test]
        public void Contains_ItemNotInDictionary_ReturnFalse()
        {
            // Setup
            const string key = "some key";

            var value = new object();

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();

            // Call
            bool contains = dictionary.Contains(new KeyValuePair<string, object>(key, value));

            // Assert
            Assert.IsFalse(contains);
        }

        [Test]
        public void RemoveItem_ItemNotInDictionary_ReturnFalse()
        {
            // Setup
            const string key = "some key";
            var value = new object();

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();

            // Call
            bool removed = dictionary.Remove(new KeyValuePair<string, object>(key, value));

            // Assert
            Assert.IsFalse(removed);
        }

        [Test]
        public void RemoveItem_ItemInDictionary_ItemRemovedAndReturnTrue()
        {
            // Setup
            const string key = "some key";
            var value = new object();

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            var item = new KeyValuePair<string, object>(key, value);
            dictionary.Add(item);

            // Precondition
            CollectionAssert.Contains(dictionary, item);

            // Call
            bool removed = dictionary.Remove(item);

            // Assert
            Assert.IsTrue(removed);
            CollectionAssert.DoesNotContain(dictionary, item);
        }

        [Test]
        public void Remove_KeyNotInDictionary_ReturnFalse()
        {
            // Setup
            const string key = "some key";

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();

            // Call
            bool removed = dictionary.Remove(key);

            // Assert
            Assert.IsFalse(removed);
        }

        [Test]
        public void Remove_KeyInDictionary_ItemRemovedAndReturnTrue()
        {
            // Setup
            const string key = "some key";

            var value = new object();

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            var item = new KeyValuePair<string, object>(key, value);
            dictionary.Add(item);

            // Precondition
            CollectionAssert.Contains(dictionary, item);

            // Call
            bool removed = dictionary.Remove(key);

            // Assert
            Assert.IsTrue(removed);
            CollectionAssert.DoesNotContain(dictionary, item);
        }

        [Test]
        public void ContainsKey_KeyNotInDictionary_ReturnFalse()
        {
            // Setup
            const string key = "some key";

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();

            // Call
            bool contains = dictionary.ContainsKey(key);

            // Assert
            Assert.IsFalse(contains);
        }

        [Test]
        public void ContainsKey_KeyInDictionary_ReturnTrue()
        {
            // Setup
            const string key = "some key";
            var value = new object();

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            var item = new KeyValuePair<string, object>(key, value);
            dictionary.Add(item);

            // Call
            bool contains = dictionary.ContainsKey(key);

            // Assert
            Assert.IsTrue(contains);
        }

        [Test]
        public void CopyTo_ValidData_CopiesItemToGivenArray()
        {
            // Setup
            FilteredKeyDictionary<string, object> dictionary = GetDictionary();

            var array = new[]
            {
                new KeyValuePair<string, object>("key", new object()),
                new KeyValuePair<string, object>("key2", new object())
            };

            // Precondition
            CollectionAssert.AreNotEqual(dictionary, array);

            // Call
            dictionary.CopyTo(array, 0);

            // Assert
            CollectionAssert.AreEqual(dictionary, array);
        }

        [Test]
        public void TryGetValue_KeyNotInDictionary_ReturnFalse()
        {
            // Setup
            const string key = "some key";

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();

            object value;

            // Call
            bool succeeded = dictionary.TryGetValue(key, out value);

            // Assert
            Assert.IsFalse(succeeded);
        }

        [Test]
        public void TryGetValue_KeyInDictionary_ReturnTrue()
        {
            // Setup
            const string key = "some key";
            var value = new object();

            FilteredKeyDictionary<string, object> dictionary = GetDictionary();
            var item = new KeyValuePair<string, object>(key, value);
            dictionary.Add(item);

            // Call
            bool succeeded = dictionary.TryGetValue(key, out value);

            // Assert
            Assert.IsTrue(succeeded);
        }

        private static FilteredKeyDictionary<string, object> GetDictionary()
        {
            const string key1 = "first key";
            const string key2 = "some key";
            const string key3 = "other key";

            var dictionary = new FilteredKeyDictionary<string, object>(new[]
            {
                key1,
                key2,
                key3
            })
            {
                {
                    key1, "value 1"
                },
                {
                    key3, "value 3"
                }
            };
            return dictionary;
        }
    }
}