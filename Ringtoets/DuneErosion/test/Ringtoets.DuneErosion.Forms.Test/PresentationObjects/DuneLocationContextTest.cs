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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;

namespace Ringtoets.DuneErosion.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DuneLocationContextTest
    {
        [Test]
        public void Constructor_DuneLocationNull_ThrowArgumentNullException()
        {
            // Setup
            var locations = new ObservableList<DuneLocation>();

            // Call
            TestDelegate test = () => new DuneLocationContext(locations, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneLocation", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var locations = new ObservableList<DuneLocation>();
            var location = new TestDuneLocation();

            // Call
            var context = new DuneLocationContext(locations, location);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableList<DuneLocation>>>(context);
            Assert.AreSame(locations, context.WrappedData);
            Assert.AreSame(location, context.DuneLocation);
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var duneLocations = new ObservableList<DuneLocation>();
            var duneLocation = new TestDuneLocation();

            var context = new DuneLocationContext(duneLocations, duneLocation);

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var duneLocations = new ObservableList<DuneLocation>();
            var duneLocation = new TestDuneLocation();

            var context = new DuneLocationContext(duneLocations, duneLocation);

            // Call
            bool isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_ToOtherWithDifferentLocationListAndSameLocation_ReturnFalse()
        {
            // Setup
            var duneLocations1 = new ObservableList<DuneLocation>();
            var duneLocations2 = new ObservableList<DuneLocation>();
            var duneLocation = new TestDuneLocation();
            var context1 = new DuneLocationContext(duneLocations1, duneLocation);
            var context2 = new DuneLocationContext(duneLocations2, duneLocation);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToOtherWithSameLocationListAndDifferentLocation_ReturnFalse()
        {
            // Setup
            var duneLocations = new ObservableList<DuneLocation>();
            var duneLocation1 = new TestDuneLocation();
            var duneLocation2 = new TestDuneLocation();
            var context1 = new DuneLocationContext(duneLocations, duneLocation1);
            var context2 = new DuneLocationContext(duneLocations, duneLocation2);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToOtherWithSameLocationListAndSameLocation_ReturnTrue()
        {
            // Setup
            var duneLocations = new ObservableList<DuneLocation>();
            var duneLocation = new TestDuneLocation();
            var context1 = new DuneLocationContext(duneLocations, duneLocation);
            var context2 = new DuneLocationContext(duneLocations, duneLocation);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnSameHashCode()
        {
            // Setup
            var duneLocations = new ObservableList<DuneLocation>();
            var duneLocation = new TestDuneLocation();
            var context1 = new DuneLocationContext(duneLocations, duneLocation);
            var context2 = new DuneLocationContext(duneLocations, duneLocation);

            // Precondition
            Assert.AreEqual(context1, context2);

            // Call
            int hashCode1 = context1.GetHashCode();
            int hashCode2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }
    }
}