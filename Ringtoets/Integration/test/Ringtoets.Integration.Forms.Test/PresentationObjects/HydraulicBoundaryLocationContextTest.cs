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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HydraulicBoundaryLocationContextTest
    {
        [Test]
        public void Constructor_NullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_NullHydraulicBoundariesDatabase_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationContext(null, hydraulicBoundaryLocation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("wrappedData", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            // Call
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<HydraulicBoundaryDatabase>>(context);
            Assert.AreSame(hydraulicBoundaryDatabase, context.WrappedData);
            Assert.AreSame(hydraulicBoundaryLocation, context.HydraulicBoundaryLocation);
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Call
            var isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Call
            var isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_ToOtherWithDifferentDatabaseAndSameLocation_ReturnFalse()
        {
            // Setup
            var hydraulicBoundaryDatabase1 = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryDatabase2 = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);
            var context1 = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase1, hydraulicBoundaryLocation);
            var context2 = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase2, hydraulicBoundaryLocation);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToOtherWithSameDatabaseAndDifferentLocation_ReturnFalse()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(1, "First name", 2.0, 3.0);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(2, "Second name", 4.0, 5.0);
            var context1 = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation1);
            var context2 = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation2);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToOtherWithSameDatabaseAndSameLocation_ReturnTrue()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);
            var context1 = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);
            var context2 = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnSameHashCode()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);
            var context1 = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);
            var context2 = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Precondition
            Assert.AreEqual(context1, context2);

            // Call
            var hashCode1 = context1.GetHashCode();
            var hashCode2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }

        private class TestHydraulicBoundaryLocationContext : HydraulicBoundaryLocationContext
        {
            public TestHydraulicBoundaryLocationContext(HydraulicBoundaryDatabase wrappedData, HydraulicBoundaryLocation hydraulicBoundaryLocation)
                : base(wrappedData, hydraulicBoundaryLocation) {}
        }
    }
}