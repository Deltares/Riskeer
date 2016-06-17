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
using System.Collections.ObjectModel;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class IStorableExtensionsTest
    {
        [Test]
        [TestCase(0, true)]
        [TestCase(-1, true)]
        [TestCase(Int32.MinValue, true)]
        [TestCase(1, false)]
        [TestCase(5, false)]
        [TestCase(Int32.MaxValue, false)]
        public void IsNew_DifferentIds_ExpectedResult(int val, bool isNew)
        {
            // Setup
            var mocks = new MockRepository();
            var storable = mocks.StrictMock<IStorable>();
            storable.Expect(s => s.StorageId).Return(val);
            mocks.ReplayAll();

            // Call
            var result = storable.IsNew();

            // Assert
            Assert.AreEqual(isNew, result);
        }

        [Test]
        public void Find_NoItemsInContext_ThrowsException()
        {
            // Setup
            var storable = new SimpleStorable();
            var set = new TestDbSet<object>(new ObservableCollection<object>());

            // Call
            TestDelegate test = () => storable.GetCorrespondingEntity(set, o => 1);

            // Assert
            var message = Assert.Throws<EntityNotFoundException>(test).Message;
            Assert.AreEqual("Het object 'Object' met id '0' is niet gevonden.", message);
        }

        [Test]
        public void Find_NoItemsWithIdInContext_ThrowsException()
        {
            // Setup
            var storable = new SimpleStorable
            {
                StorageId = 21
            };
            var set = new TestDbSet<WithId>(new ObservableCollection<WithId>());
            set.Add(new WithId
            {
                Id = 11
            });

            // Call
            TestDelegate test = () => storable.GetCorrespondingEntity(set, o => o.Id);

            // Assert
            var message = Assert.Throws<EntityNotFoundException>(test).Message;
            Assert.AreEqual(string.Format("Het object 'WithId' met id '{0}' is niet gevonden.", storable.StorageId), message);
        }

        [Test]
        public void Find_MultipleItemsWithIdInContext_ThrowsException()
        {
            // Setup
            var storable = new SimpleStorable
            {
                StorageId = 21
            };
            var set = new TestDbSet<WithId>(new ObservableCollection<WithId>());
            set.Add(new WithId { Id = storable.StorageId });
            set.Add(new WithId { Id = storable.StorageId });

            // Call
            TestDelegate test = () => storable.GetCorrespondingEntity(set, o => o.Id);

            // Assert
            var message = Assert.Throws<EntityNotFoundException>(test).Message;
            Assert.AreEqual(string.Format("Het object 'WithId' met id '{0}' is niet gevonden.", storable.StorageId), message);
        }

        [Test]
        public void Find_ItemWithIdZeroInContext_ThrowsException()
        {
            // Setup
            var storable = new SimpleStorable
            {
                StorageId = 0
            };
            var set = new TestDbSet<WithId>(new ObservableCollection<WithId>());
            var expectedItem = new WithId
            {
                Id = storable.StorageId
            };
            set.Add(expectedItem);
            set.Add(new WithId { Id = 19 });

            // Call
            TestDelegate test = () => storable.GetCorrespondingEntity(set, o => o.Id);

            // Assert
            var message = Assert.Throws<EntityNotFoundException>(test).Message;
            Assert.AreEqual("Het object \'WithId\' met id \'0\' is niet gevonden.", message);
        }

        [Test]
        public void Find_ItemWithIdNotZeroInContext_ReturnEntityFromContext()
        {
            // Setup
            var storable = new SimpleStorable
            {
                StorageId = new Random(21).Next(1, int.MaxValue)
            };
            var set = new TestDbSet<WithId>(new ObservableCollection<WithId>());
            var expectedItem = new WithId
            {
                Id = storable.StorageId
            };
            set.Add(expectedItem);
            set.Add(new WithId { Id = 19 });

            // Call
            WithId actualItem = storable.GetCorrespondingEntity(set, o => o.Id);

            // Assert
            Assert.AreSame(expectedItem, actualItem);
        }
    }

    public class SimpleStorable : IStorable {
        public long StorageId { get; set; }
    }

    public class WithId {
        public long Id { get; set; }
    }
}