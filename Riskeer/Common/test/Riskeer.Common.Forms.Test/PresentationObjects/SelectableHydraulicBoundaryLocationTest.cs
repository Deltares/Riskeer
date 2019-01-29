// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class SelectableHydraulicBoundaryLocationTest
    {
        [Test]
        [TestCaseSource(nameof(ReferencePointLocations))]
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
            var referencePoint = new Point2D(0, 0);

            // Call
            TestDelegate call = () => new SelectableHydraulicBoundaryLocation(null, referencePoint);

            // Assert 
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        [TestCaseSource(nameof(StringRepresentations))]
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

        [TestFixture]
        private class SelectableHydraulicBoundaryLocationEqualsTest : EqualsTestFixture<SelectableHydraulicBoundaryLocation, DerivedSelectableHydraulicBOundaryLocation>
        {
            private readonly HydraulicBoundaryLocation location = new HydraulicBoundaryLocation(1, "Name", 0, 1);

            [Test]
            [TestCaseSource(nameof(EqualityReferencePoints))]
            public void Equals_ToOtherWithSameHydraulicBoundaryLocationsAndDifferentReferencePoints_ReturnsTrue(Point2D referencePoint1,
                                                                                                                Point2D referencePoint2)
            {
                // Setup
                var inputItem1 = new SelectableHydraulicBoundaryLocation(location, referencePoint1);
                var inputItem2 = new SelectableHydraulicBoundaryLocation(location, referencePoint2);

                // Call
                bool areEqualObjects12 = inputItem1.Equals(inputItem2);
                bool areEqualObjects21 = inputItem2.Equals(inputItem1);

                // Assert
                Assert.IsTrue(areEqualObjects12);
                Assert.IsTrue(areEqualObjects21);
            }

            protected override SelectableHydraulicBoundaryLocation CreateObject()
            {
                return new SelectableHydraulicBoundaryLocation(location, CreateReferencePoint());
            }

            protected override DerivedSelectableHydraulicBOundaryLocation CreateDerivedObject()
            {
                return new DerivedSelectableHydraulicBOundaryLocation(location,
                                                                      CreateReferencePoint());
            }

            private static Point2D CreateReferencePoint()
            {
                var random = new Random(21);
                return new Point2D(random.NextDouble(), random.NextDouble());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new SelectableHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "Name", 0, 1),
                                                                                      CreateReferencePoint()))
                    .SetName("Hydraulic Boundary Location");
            }

            private static IEnumerable<TestCaseData> EqualityReferencePoints()
            {
                var referencePoint1 = new Point2D(0, 0);
                var referencePoint2 = new Point2D(1, 1);

                yield return new TestCaseData(null, null);
                yield return new TestCaseData(null, referencePoint1);
                yield return new TestCaseData(referencePoint1, referencePoint1);
                yield return new TestCaseData(referencePoint1, referencePoint2);
            }
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

            var meters = new Point2D(0, 10);
            var kilometers = new Point2D(10000, 10000);

            yield return new TestCaseData(hydraulicBoundaryLocation, null, hydraulicBoundaryLocation.Name);
            yield return new TestCaseData(hydraulicBoundaryLocation, meters, GetStringRepresentation(hydraulicBoundaryLocation, meters));
            yield return new TestCaseData(hydraulicBoundaryLocation, kilometers, GetStringRepresentation(hydraulicBoundaryLocation, kilometers));
        }

        private static string GetStringRepresentation(HydraulicBoundaryLocation location, Point2D referencePoint)
        {
            if (referencePoint == null)
            {
                return location.Name;
            }

            double distance = location.Location.GetEuclideanDistanceTo(referencePoint);

            return distance / 1000 < 1
                       ? $"{location.Name} ({distance:f0} m)"
                       : $"{location.Name} ({distance / 1000:f1} km)";
        }

        private class DerivedSelectableHydraulicBOundaryLocation : SelectableHydraulicBoundaryLocation
        {
            public DerivedSelectableHydraulicBOundaryLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation, Point2D referencePoint)
                : base(hydraulicBoundaryLocation, referencePoint) {}
        }
    }
}