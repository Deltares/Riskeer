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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.UITypeEditors;

namespace Ringtoets.Common.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class SelectableHydraulicBoundaryLocationTest
    {
        [Test]
        [TestCaseSource("ReferencePointLocations")]
        public void Constructor_ArgumentsNotNull_ReturnsRightData(Point2D referencePoint, double expectedDistance)
        {
            // Setup 
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Location", 0, 0);

            // Call
            var inputItem = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, referencePoint);

            // Assert 
            Assert.AreSame(hydraulicBoundaryLocation, inputItem.HydraulicBoundaryLocation);
            Assert.AreEqual(0, inputItem.Distance.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedDistance, inputItem.Distance.Value);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationNull_ThrowsArgumentException()
        {
            // Setup 
            Point2D referencePoint = new Point2D(0, 0);

            // Call
            TestDelegate call = () => new SelectableHydraulicBoundaryLocation(null, referencePoint);

            // Assert 
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 0, 1);
            var inputItem = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);

            // Call
            bool areEqualObjects = inputItem.Equals(inputItem);

            // Assert
            Assert.IsTrue(areEqualObjects);
        }

        [Test]
        [TestCaseSource("EqualityReferencePoints")]
        public void Equals_ToOtherWithSameHydraulicBoundaryLocationsAndVaryingReferencePoints_ReturnsTrue(Point2D referencePoint1,
                                                                                                          Point2D referencePoint2)
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 0, 1);
            var inputItem1 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, referencePoint1);
            var inputItem2 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, referencePoint2);

            // Call
            bool areEqualObjects12 = inputItem1.Equals(inputItem2);
            bool areEqualObjects21 = inputItem2.Equals(inputItem1);

            // Assert
            Assert.IsTrue(areEqualObjects12);
            Assert.IsTrue(areEqualObjects21);
        }

        [Test]
        [TestCaseSource("EqualityReferencePoints")]
        public void Equals_ToOtherWithDifferentHydraulicBoundaryLocationsAndVaryingReferencePoints_ReturnsTrue(Point2D referencePoint1,
                                                                                                               Point2D referencePoint2)
        {
            // Setup
            var inputItem1 = new SelectableHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "Name", 0, 1),
                                                                     referencePoint1);
            var inputItem2 = new SelectableHydraulicBoundaryLocation(new HydraulicBoundaryLocation(2, "Name", 0, 1),
                                                                     referencePoint2);

            // Call
            bool areEqualObjects12 = inputItem1.Equals(inputItem2);
            bool areEqualObjects21 = inputItem2.Equals(inputItem1);

            // Assert
            Assert.IsFalse(areEqualObjects12);
            Assert.IsFalse(areEqualObjects21);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup 
            var inputWithRefPoint = new SelectableHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "Name", 0, 1),
                                                                            new Point2D(0, 0));
            var inputWithoutRefPoint = new SelectableHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "Name", 0, 1),
                                                                               null);

            // Call
            bool areEqualObjectWithRefPoint = inputWithRefPoint.Equals(null);
            bool areEqualObjectsWithoutRefPoint = inputWithoutRefPoint.Equals(null);

            // Assert
            Assert.IsFalse(areEqualObjectWithRefPoint);
            Assert.IsFalse(areEqualObjectsWithoutRefPoint);
        }

        [Test]
        public void Equals_OtherObject_ReturnsFalse()
        {
            // Setup
            var calculationInput = new SelectableHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "Name", 0, 1), null);
            var otherObject = new object();

            // Call
            bool areEqualObjects12 = calculationInput.Equals(otherObject);
            bool areEqualObjects21 = otherObject.Equals(calculationInput);

            // Assert
            Assert.IsFalse(areEqualObjects12);
            Assert.IsFalse(areEqualObjects21);
        }

        [Test]
        public void Equals_TransitivePropertyWithoutReferencePoint_ReturnsTrue()
        {
            // Setup 
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 0, 1);
            var calculationInput1 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);
            var calculationInput2 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);
            var calculationInput3 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);

            // Call
            bool areEqualObjects12 = calculationInput1.Equals(calculationInput2);
            bool areEqualObjects23 = calculationInput2.Equals(calculationInput3);
            bool areEqualObjects13 = calculationInput1.Equals(calculationInput3);

            // Assert
            Assert.IsTrue(areEqualObjects12);
            Assert.IsTrue(areEqualObjects23);
            Assert.IsTrue(areEqualObjects13);
        }

        [Test]
        public void Equals_TransitivePropertyWithDifferentReferencePoints_ReturnsTrue()
        {
            // Setup 
            var referencePoint = new Point2D(0, 0);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 0, 1);
            var calculationInput1 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, referencePoint);
            var calculationInput2 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);
            var calculationInput3 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, referencePoint);

            // Call
            bool areEqualObjects12 = calculationInput1.Equals(calculationInput2);
            bool areEqualObjects23 = calculationInput2.Equals(calculationInput3);
            bool areEqualObjects13 = calculationInput1.Equals(calculationInput3);

            // Assert
            Assert.IsTrue(areEqualObjects12);
            Assert.IsTrue(areEqualObjects23);
            Assert.IsTrue(areEqualObjects13);
        }

        [Test]
        [TestCaseSource("EqualityReferencePoints")]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode(Point2D referencePoint1,
                                                                 Point2D referencePoint2)
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 0, 1);
            var inputItem1 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, referencePoint1);
            var inputItem2 = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, referencePoint2);

            // Pre-condition
            Assert.AreEqual(inputItem1, inputItem2);

            // Call
            int hashCodeItem1 = inputItem1.GetHashCode();
            int hashCodeItem2 = inputItem2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeItem1, hashCodeItem2);
        }

        [Test]
        [TestCaseSource("EqualityReferencePoints")]
        public void GetHashCode_NotEqualObjects_ReturnsDifferenHashCode(Point2D referencePoint1,
                                                                        Point2D referencePoint2)
        {
            // Setup
            var inputItem1 = new SelectableHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "Name", 0, 1),
                                                                     referencePoint1);
            var inputItem2 = new SelectableHydraulicBoundaryLocation(new HydraulicBoundaryLocation(2, "Name", 0, 1),
                                                                     referencePoint2);

            // Pre-condition
            Assert.AreNotEqual(inputItem1, inputItem2);

            // Call
            int hashCodeItem1 = inputItem1.GetHashCode();
            int hashCodeItem2 = inputItem2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hashCodeItem1, hashCodeItem2);
        }

        [Test]
        [TestCaseSource("StringRepresentations")]
        public void ToString_DifferentReferencePoints_ReturnsExpectedString(HydraulicBoundaryLocation location,
                                                                            Point2D referencePoint, string expectedString)
        {
            // Setup
            var inputItem = new SelectableHydraulicBoundaryLocation(location, referencePoint);

            // Call
            string stringRepresentation = inputItem.ToString();

            // Assert
            Assert.AreEqual(expectedString, stringRepresentation);
        }

        private static IEnumerable<TestCaseData> ReferencePointLocations()
        {
            yield return new TestCaseData(null, double.NaN);
            yield return new TestCaseData(new Point2D(0, 10), 10);
            yield return new TestCaseData(new Point2D(10, 0), 10);
            yield return new TestCaseData(new Point2D(10, 10), 14);
            yield return new TestCaseData(new Point2D(1000, 1000), 1414);
            yield return new TestCaseData(new Point2D(10000, 10000), 14142);
        }

        private static IEnumerable<TestCaseData> StringRepresentations()
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(5, "Name", 0, 0);

            Point2D meters = new Point2D(0, 10);
            Point2D kilometers = new Point2D(10000, 10000);

            yield return new TestCaseData(hydraulicBoundaryLocation, null, hydraulicBoundaryLocation.Name);
            yield return new TestCaseData(hydraulicBoundaryLocation, meters, GetStringRepresentation(hydraulicBoundaryLocation, meters));
            yield return new TestCaseData(hydraulicBoundaryLocation, kilometers, GetStringRepresentation(hydraulicBoundaryLocation, kilometers));
        }

        private static IEnumerable<TestCaseData> EqualityReferencePoints()
        {
            Point2D referencePoint1 = new Point2D(0, 0);
            Point2D referencePoint2 = new Point2D(1, 1);

            yield return new TestCaseData(null, null);
            yield return new TestCaseData(null, referencePoint1);
            yield return new TestCaseData(referencePoint1, referencePoint1);
            yield return new TestCaseData(referencePoint1, referencePoint2);
        }

        private static string GetStringRepresentation(HydraulicBoundaryLocation location, Point2D referencePoint)
        {
            if (referencePoint == null)
                return location.Name;

            var distance = location.Location.GetEuclideanDistanceTo(referencePoint);

            return distance/1000 < 1
                       ? string.Format("{0} ({1:f0} m)", location.Name, distance)
                       : string.Format("{0} ({1:f1} km)", location.Name, distance/1000);
        }
    }
}