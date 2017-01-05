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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.Test.DikeProfiles
{
    [TestFixture]
    public class ForeshoreProfileTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var worldCoordinate = new Point2D(1.1, 2.2);

            var foreshoreGeometry = new[]
            {
                new Point2D(0.0, 1.1),
                new Point2D(8.0, 9.1)
            };

            // Call
            var foreshoreProfile = new ForeshoreProfile(worldCoordinate, foreshoreGeometry,
                                                        null, new ForeshoreProfile.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<RoundedDouble>(foreshoreProfile.Orientation);
            Assert.IsInstanceOf<double>(foreshoreProfile.X0);

            Assert.IsNull(foreshoreProfile.Name);
            Assert.AreSame(worldCoordinate, foreshoreProfile.WorldReferencePoint);
            Assert.AreEqual(0.0, foreshoreProfile.X0);
            Assert.AreEqual(0.0, foreshoreProfile.Orientation.Value);
            Assert.AreEqual(2, foreshoreProfile.Orientation.NumberOfDecimalPlaces);
            Assert.IsNull(foreshoreProfile.BreakWater);
            CollectionAssert.AreEqual(foreshoreGeometry, foreshoreProfile.Geometry);
            Assert.AreEqual(2, foreshoreProfile.Geometry.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_WorldCoordinateNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfile(null, new Point2D[0], null, new ForeshoreProfile.ConstructionProperties());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("worldCoordinate", paramName);
        }

        [Test]
        public void Constructor_ForeshoreGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfile(new Point2D(0, 0), null, null, new ForeshoreProfile.ConstructionProperties());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("geometry", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesIsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfile(new Point2D(0, 0), new Point2D[0], null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("properties", paramName);
        }

        [Test]
        public void Constructor_ForeshoreGeometryContainsNullPoint_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfile(new Point2D(0.0, 0.0),
                                                           new Point2D[]
                                                           {
                                                               null
                                                           },
                                                           null, new ForeshoreProfile.ConstructionProperties());

            // Assert
            var expectedMessage = "Een punt in de geometrie voor het voorlandprofiel heeft geen waarde.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Orientation_SetToValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Call
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Orientation = 1.23456
                                                        });

            // Assert
            Assert.AreEqual(2, foreshoreProfile.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, foreshoreProfile.Orientation.Value);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Cool new name!")]
        public void Name_SetNewValue_GetsNewValue(string name)
        {
            // Call
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Name = name
                                                        });

            // Assert
            Assert.AreEqual(name, foreshoreProfile.Name);
        }

        [Test]
        public void BreakWater_SetToNull_GetsNewlySetNull()
        {
            // Call
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties());

            // Assert
            Assert.IsNull(foreshoreProfile.BreakWater);
        }

        [Test]
        public void BreakWater_SetToNewInstance_GetsNewlySetInstance()
        {
            // Setup
            var newBreakWater = new BreakWater(BreakWaterType.Caisson, 1.1);

            // Call
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        newBreakWater, new ForeshoreProfile.ConstructionProperties());

            // Assert
            Assert.AreSame(newBreakWater, foreshoreProfile.BreakWater);
        }

        [Test]
        public void HasBreakWater_BreakWaterSetToNull_ReturnFalse()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties());

            // Call
            bool hasBreakWater = foreshoreProfile.HasBreakWater;

            // Assert
            Assert.IsFalse(hasBreakWater);
        }

        [Test]
        public void HasBreakWater_BreakWaterSetToAnInstance_ReturnTrue()
        {
            // Setup
            var breakWater = new BreakWater(BreakWaterType.Dam, 12.34);
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        breakWater, new ForeshoreProfile.ConstructionProperties());

            // Call
            bool hasBreakWater = foreshoreProfile.HasBreakWater;

            // Assert
            Assert.IsTrue(hasBreakWater);
        }

        [Test]
        public void ToString_Always_ReturnsName()
        {
            // Setup
            var testName = "testName";
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Name = testName
                                                        });

            // Call
            var result = foreshoreProfile.ToString();

            // Assert
            Assert.AreEqual(testName, result);
        }
    }
}