﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Diagnostics;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class FingerprintHelperTest
    {
        [Test]
        public void Get_EntityIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => FingerprintHelper.Get(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void GivenProjectEntity_WhenGeneratingFingerprintMultipleTime_ThenHashRemainsEqual()
        {
            // Given
            RingtoetsProject project = RingtoetsProjectTestHelper.GetFullTestProject();
            ProjectEntity entity = project.Create(new PersistenceRegistry());

            // When
            byte[] hash1 = FingerprintHelper.Get(entity);
            byte[] hash2 = FingerprintHelper.Get(entity);

            // Then
            Assert.IsNotNull(hash1);
            CollectionAssert.IsNotEmpty(hash1);
            CollectionAssert.AreEqual(hash1, hash2);
        }

        [Test]
        public void GivenProjectEntity_WhenComparingFingerprintsBeforeAndAfterChange_ThenHashDifferent()
        {
            // Given
            RingtoetsProject project = RingtoetsProjectTestHelper.GetFullTestProject();
            ProjectEntity entity = project.Create(new PersistenceRegistry());

            // When
            byte[] hash1 = FingerprintHelper.Get(entity);
            entity.AssessmentSectionEntities.First().Name = "Something completely different";
            byte[] hash2 = FingerprintHelper.Get(entity);

            // Then
            Assert.IsNotNull(hash1);
            CollectionAssert.IsNotEmpty(hash1);
            CollectionAssert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void GivenProjectEntity_WhenGeneratingFingerprintEqualData_ThenHashRemainsEqual()
        {
            // Setup
            RingtoetsProject project1 = RingtoetsProjectTestHelper.GetFullTestProject();
            RingtoetsProject project2 = RingtoetsProjectTestHelper.GetFullTestProject();
            ProjectEntity entity1 = project1.Create(new PersistenceRegistry());
            ProjectEntity entity2 = project2.Create(new PersistenceRegistry());

            // Call
            byte[] hash1 = FingerprintHelper.Get(entity1);
            byte[] hash2 = FingerprintHelper.Get(entity2);

            // Assert
            Assert.IsNotNull(hash1);
            CollectionAssert.IsNotEmpty(hash1);
            CollectionAssert.AreEqual(hash1, hash2);
        }

        [Test]
        public void AreEqual_ArraysAreNotEqual_ReturnFalse()
        {
            // Setup
            var random = new Random(42);
            const int arraySize = 10;
            var array1 = new byte[arraySize];
            var array2 = new byte[arraySize];
            random.NextBytes(array1);
            random.NextBytes(array2);

            // Precondition
            CollectionAssert.AreNotEqual(array1, array2);

            // Call
            bool areCollectionEqual = FingerprintHelper.AreEqual(array1, array2);

            // Assert
            Assert.IsFalse(areCollectionEqual);
        }

        [Test]
        public void AreEqual_ArraysAreEqual_ReturnTrue()
        {
            // Setup
            var random = new Random(42);
            const int arraySize = 10;
            var array1 = new byte[arraySize];
            random.NextBytes(array1);

            // Precondition
            CollectionAssert.AreEqual(array1, array1);

            // Call
            bool areCollectionEqual = FingerprintHelper.AreEqual(array1, array1);

            // Assert
            Assert.IsTrue(areCollectionEqual);
        }

        [Test]
        public void GivenNotEqualData_WhenComparingPerformance_ThenPerformanceShouldBeBetterThanLinq()
        {
            // Given
            var random = new Random(42);
            const int arraySize = 100000000;
            var array1 = new byte[arraySize];
            var array2 = new byte[arraySize];
            random.NextBytes(array1);
            random.NextBytes(array2);

            // Precondition
            CollectionAssert.AreNotEqual(array1, array2);

            // When
            var stopwatch = new Stopwatch();
            long timeToBeat = 0, actualTime = 0;
            for (var i = 0; i < 100000; i++)
            {
                stopwatch.Start();
                Assert.IsFalse(SlowBaselineLinqEquality(array1, array2));
                stopwatch.Stop();
                timeToBeat += stopwatch.ElapsedTicks;

                stopwatch.Reset();
                stopwatch.Start();
                Assert.IsFalse(FingerprintHelper.AreEqual(array1, array2));
                stopwatch.Stop();
                actualTime += stopwatch.ElapsedTicks;
            }

            // Then
            Assert.Less(actualTime, timeToBeat, string.Format("actualTime '{0}' is not less than timeToBeat '{1}'", actualTime, timeToBeat));
        }

        private static bool SlowBaselineLinqEquality(byte[] array1, byte[] array2)
        {
            return array1.SequenceEqual(array2);
        }
    }
}