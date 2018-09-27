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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MapCalculationDataTest
    {
        [Test]
        public void Constructor_WithoutName_ThrowArgumentNullException()
        {
            // Setup
            var calculationLocation = new Point2D(0.0, 2.3);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0.1, 2.3);

            // Call
            TestDelegate test = () => new MapCalculationData(
                null,
                calculationLocation,
                hydraulicBoundaryLocation);

            // Assert
            const string expectedMessage = "A calculation name is required.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage)
                                         .ParamName;

            Assert.AreEqual("calculationName", paramName);
        }

        [Test]
        public void Constructor_WithoutCalculationLocation_ThrowArgumentNullException()
        {
            // Setup
            const string calculationName = "name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0.1, 2.3);

            // Call
            TestDelegate test = () => new MapCalculationData(
                calculationName,
                null,
                hydraulicBoundaryLocation);

            // Assert
            const string expectedMessage = "A location for the calculation is required.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage)
                                         .ParamName;

            Assert.AreEqual("calculationLocation", paramName);
        }

        [Test]
        public void Constructor_WithoutHydraulicBoundaryLocation_ThrowArgumentNullException()
        {
            // Setup
            const string calculationName = "name";
            var calculationLocation = new Point2D(0.0, 2.3);

            // Call
            TestDelegate test = () => new MapCalculationData(
                calculationName,
                calculationLocation,
                null);

            // Assert
            const string expectedMessage = "A hydraulic boundary location is required.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage)
                                         .ParamName;

            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string calculationName = "name";
            var calculationLocation = new Point2D(0.0, 2.3);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0.1, 2.3);

            // Call
            var mapCalculationData = new MapCalculationData(calculationName,
                                                            calculationLocation,
                                                            hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(calculationName, mapCalculationData.Name);
            Assert.AreSame(calculationLocation, mapCalculationData.CalculationLocation);
            Assert.AreSame(hydraulicBoundaryLocation, mapCalculationData.HydraulicBoundaryLocation);
        }
    }
}