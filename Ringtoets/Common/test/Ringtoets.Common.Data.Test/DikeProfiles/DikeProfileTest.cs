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
    public class DikeProfileTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            string validId = "id";
            var worldCoordinate = new Point2D(1.1, 2.2);
            var dikeGeometry = new[]
            {
                new RoughnessPoint(new Point2D(1.1, 2.2), 0.7),
                new RoughnessPoint(new Point2D(3.3, 4.4), 0.7),
            };

            var foreshoreGeometry = new[]
            {
                new Point2D(0.0, 1.1),
                new Point2D(8.0, 9.1)
            };

            // Call
            var dikeProfile = new DikeProfile(worldCoordinate, dikeGeometry, foreshoreGeometry,
                                              null, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = validId
                                              });

            // Assert
            Assert.IsInstanceOf<RoundedDouble>(dikeProfile.Orientation);
            Assert.IsInstanceOf<RoundedDouble>(dikeProfile.DikeHeight);
            Assert.IsInstanceOf<double>(dikeProfile.X0);

            Assert.AreEqual(validId, dikeProfile.Id);
            Assert.AreEqual(validId, dikeProfile.Name);
            Assert.AreSame(worldCoordinate, dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0.0, dikeProfile.X0);
            Assert.AreEqual(0.0, dikeProfile.Orientation.Value);
            Assert.AreEqual(2, dikeProfile.Orientation.NumberOfDecimalPlaces);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.AreEqual(dikeGeometry, dikeProfile.DikeGeometry);
            CollectionAssert.AreEqual(foreshoreGeometry, dikeProfile.ForeshoreGeometry);
            Assert.AreEqual(0.0, dikeProfile.DikeHeight.Value);
            Assert.AreEqual(2, dikeProfile.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(2, dikeProfile.ForeshoreGeometry.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_WorldReferencePointIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(null, new RoughnessPoint[0], new Point2D[0],
                                                      null, new DikeProfile.ConstructionProperties
                                                      {
                                                          Id = "id"
                                                      });

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("worldCoordinate", paramName);
        }

        [Test]
        public void Constructor_DikeGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(new Point2D(0, 0), null, new Point2D[0],
                                                      null, new DikeProfile.ConstructionProperties
                                                      {
                                                          Id = "id"
                                                      });

            // Assert
            var expectedMessage = "De geometrie die opgegeven werd voor het dijkprofiel heeft geen waarde.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
            Assert.AreEqual("points", exception.ParamName);
        }

        [Test]
        public void Constructor_DikeGeometryContainsNullPoint_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(new Point2D(0.0, 0.0),
                                                      new RoughnessPoint[]
                                                      {
                                                          null
                                                      },
                                                      new Point2D[0],
                                                      null, new DikeProfile.ConstructionProperties
                                                      {
                                                          Id = "id"
                                                      });

            // Assert
            var expectedMessage = "Een punt in de geometrie voor het dijkprofiel heeft geen waarde.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ForeshoreGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], null,
                                                      null, new DikeProfile.ConstructionProperties
                                                      {
                                                          Id = "id"
                                                      });

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("geometry", paramName);
        }

        [Test]
        public void Constructor_ForeshoreGeometryContainsNullPoint_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(new Point2D(0.0, 0.0), new RoughnessPoint[0],
                                                      new Point2D[]
                                                      {
                                                          null
                                                      }, null, new DikeProfile.ConstructionProperties
                                                      {
                                                          Id = "id"
                                                      });

            // Assert
            var expectedMessage = "Een punt in de geometrie voor het voorlandprofiel heeft geen waarde.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ConstructionPropertiesIsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0], null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("properties", paramName);
        }

        [Test]
        public void Orientation_SetToValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Call
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = "id",
                                                  Orientation = 1.23456
                                              });

            // Assert
            Assert.AreEqual(2, dikeProfile.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, dikeProfile.Orientation.Value);
        }

        [Test]
        public void DikeHeight_SetToValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Call
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = "id",
                                                  DikeHeight = 1.23456
                                              });

            // Assert
            Assert.AreEqual(2, dikeProfile.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, dikeProfile.DikeHeight.Value);
        }

        [Test]
        [TestCase("It has a name")]
        [TestCase("Cool new name!")]
        public void Name_SetNameDifferentFromId_GetsGivenNameValue(string name)
        {
            // Call
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = "id",
                                                  Name = name
                                              });

            // Assert
            Assert.AreEqual(name, dikeProfile.Name);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_NullOrWhitespaceName_NameSetToId(string name)
        {
            // Setup
            var id = "id";

            // Call
            var foreshoreProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                   null, new DikeProfile.ConstructionProperties
                                                   {
                                                       Id = id,
                                                       Name = name
                                                   });

            // Assert
            Assert.AreEqual(id, foreshoreProfile.Name);
        }

        [Test]
        public void BreakWater_SetToNull_GetsNewlySetNull()
        {
            // Call
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = "id"
                                              });

            // Assert
            Assert.IsNull(dikeProfile.BreakWater);
        }

        [Test]
        public void BreakWater_SetToNewInstance_GetsNewlySetInstance()
        {
            // Setup
            var newBreakWater = new BreakWater(BreakWaterType.Caisson, 1.1);

            // Call
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              newBreakWater, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = "id"
                                              });

            // Assert
            Assert.AreSame(newBreakWater, dikeProfile.BreakWater);
        }

        [Test]
        public void HasBreakWater_BreakWaterSetToNull_ReturnFalse()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = "id"
                                              });

            // Call
            bool hasBreakWater = dikeProfile.HasBreakWater;

            // Assert
            Assert.IsFalse(hasBreakWater);
        }

        [Test]
        public void HasBreakWater_BreakWaterSetToAnInstance_ReturnTrue()
        {
            // Setup
            var breakWater = new BreakWater(BreakWaterType.Dam, 12.34);
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              breakWater, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = "id"
                                              });

            // Call
            bool hasBreakWater = dikeProfile.HasBreakWater;

            // Assert
            Assert.IsTrue(hasBreakWater);
        }

        [Test]
        public void ToString_Always_ReturnsName()
        {
            // Setup
            var testName = "testName";
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = "id",
                                                  Name = testName
                                              });

            // Call
            var result = dikeProfile.ToString();

            // Assert
            Assert.AreEqual(testName, result);
        }
    }
}