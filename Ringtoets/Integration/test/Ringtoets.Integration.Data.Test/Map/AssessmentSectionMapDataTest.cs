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
using Core.Components.Gis.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data.Map;

namespace Ringtoets.Integration.Data.Test.Map
{
    [TestFixture]
    public class AssessmentSectionMapDataTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            var mapData = new AssessmentSectionMapData(assessmentSection);

            // Assert
            Assert.IsInstanceOf<MapDataCollection>(mapData);
            Assert.IsInstanceOf<IEquatable<AssessmentSectionMapData>>(mapData);
            CollectionAssert.IsEmpty(mapData.List);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMapData(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Equals_EqualsWithItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var mapData = new AssessmentSectionMapData(assessmentSection);

            // Call
            var isEqual = mapData.Equals(mapData);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_EqualsWithNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var mapData = new AssessmentSectionMapData(assessmentSection);

            // Call
            var isEqual = mapData.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_EqualsWithOtherTypeOfObject_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var mapData = new AssessmentSectionMapData(assessmentSection);

            var objectOfDifferentType = new object();

            // Call
            var isEqual = mapData.Equals(objectOfDifferentType);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_EqualsWithOtherEqualMapData_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            object mapData1 = new AssessmentSectionMapData(assessmentSection);
            AssessmentSectionMapData mapData2 = new AssessmentSectionMapData(assessmentSection);

            // Call
            var isEqual1 = mapData1.Equals(mapData2);
            var isEqual2 = mapData2.Equals(mapData1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void Equals_TwoUnequalAssessmentSectionMapDataInstances_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection1 = mocks.Stub<AssessmentSectionBase>();
            var assessmentSection2 = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            object mapData1 = new AssessmentSectionMapData(assessmentSection1);
            AssessmentSectionMapData mapData2 = new AssessmentSectionMapData(assessmentSection2);

            // Call
            var isEqual1 = mapData1.Equals(mapData2);
            var isEqual2 = mapData2.Equals(mapData1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }


        [Test]
        public void GetHashCode_TwoEqualAssessmentSectionMapDataInstances_ReturnSameHash()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            object mapData1 = new AssessmentSectionMapData(assessmentSection);
            AssessmentSectionMapData mapData2 = new AssessmentSectionMapData(assessmentSection);

            // Call
            int hash1 = mapData1.GetHashCode();
            int hash2 = mapData2.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }
    }
}