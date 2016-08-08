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
using System.Linq;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;

using NUnit.Framework;

using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class FingerprintGeneratorTest
    {
        [Test]
        public void Get_EntityIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => FingerprintGenerator.Get(null);

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
            byte[] hash1 = FingerprintGenerator.Get(entity);
            byte[] hash2 = FingerprintGenerator.Get(entity);

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
            byte[] hash1 = FingerprintGenerator.Get(entity);
            entity.AssessmentSectionEntities.First().Name = "Something completely different";
            byte[] hash2 = FingerprintGenerator.Get(entity);

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
            byte[] hash1 = FingerprintGenerator.Get(entity1);
            byte[] hash2 = FingerprintGenerator.Get(entity2);

            // Assert
            Assert.IsNotNull(hash1);
            CollectionAssert.IsNotEmpty(hash1);
            CollectionAssert.AreEqual(hash1, hash2);
        }
    }
}