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

using Core.Common.Base;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            var pressentationObject = new HydraulicBoundaryDatabaseContext(assessmentSectionBaseMock);

            // Assert
            Assert.IsInstanceOf<IObservable>(pressentationObject);
            Assert.AreSame(assessmentSectionBaseMock, pressentationObject.Parent);
        }

        [Test]
        public void Constructor_AssesmentSectionBaseIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseContext(null);

            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("Assessment section cannot be null.", customMessage);
        }

        [Test]
        public void Equals_EqualsWithItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var context  = new HydraulicBoundaryDatabaseContext(assessmentSection);

            // Call
            var isEqual = context.Equals(context);

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

            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);

            // Call
            var isEqual = context.Equals(null);

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

            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);

            var objectOfDifferentType = new object();

            // Call
            var isEqual = context.Equals(objectOfDifferentType);

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

            object context1 = new HydraulicBoundaryDatabaseContext(assessmentSection);
            HydraulicBoundaryDatabaseContext context2 = new HydraulicBoundaryDatabaseContext(assessmentSection);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context2.Equals(context1);

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

            object context1 = new HydraulicBoundaryDatabaseContext(assessmentSection1);
            HydraulicBoundaryDatabaseContext context2 = new HydraulicBoundaryDatabaseContext(assessmentSection2);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context2.Equals(context1);

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

            object context1 = new HydraulicBoundaryDatabaseContext(assessmentSection);
            HydraulicBoundaryDatabaseContext context2 = new HydraulicBoundaryDatabaseContext(assessmentSection);

            // Call
            int hash1 = context1.GetHashCode();
            int hash2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }
    }
}