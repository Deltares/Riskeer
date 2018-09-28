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
using Core.Common.Base;
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
            const string validId = "id";
            var worldCoordinate = new Point2D(1.1, 2.2);
            var dikeGeometry = new[]
            {
                new RoughnessPoint(new Point2D(1.1, 2.2), 0.7),
                new RoughnessPoint(new Point2D(3.3, 4.4), 0.7)
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
            Assert.IsInstanceOf<Observable>(dikeProfile);
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
            const string expectedMessage = "De geometrie die opgegeven werd voor het dijkprofiel heeft geen waarde.";
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
            const string expectedMessage = "Een punt in de geometrie voor het dijkprofiel heeft geen waarde.";
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
            const string expectedMessage = "Een punt in de geometrie voor het voorlandprofiel heeft geen waarde.";
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
            const string id = "id";

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
            const string testName = "testName";
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties
                                              {
                                                  Id = "id",
                                                  Name = testName
                                              });

            // Call
            string result = dikeProfile.ToString();

            // Assert
            Assert.AreEqual(testName, result);
        }

        [Test]
        public void CopyProperties_FromDikeProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            DikeProfile dikeProfile = CreateFullyDefinedDikeProfile();

            // Call
            TestDelegate call = () => dikeProfile.CopyProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("fromDikeProfile", exception.ParamName);
        }

        [Test]
        public void CopyProperties_FromDikeProfileAllPropertiesChanged_PropertiesUpdated()
        {
            // Setup
            DikeProfile dikeProfileToUpdate = CreateFullyDefinedDikeProfile();

            const string expectedId = "new_id";
            const string expectedName = "new_name";

            var random = new Random(21);
            double expectedX0 = dikeProfileToUpdate.X0 + random.NextDouble();
            var expectedOrientation = new RoundedDouble(2, (dikeProfileToUpdate.Orientation + random.NextDouble()) % 360);
            var expectedDikeHeight = new RoundedDouble(2, dikeProfileToUpdate.DikeHeight + random.NextDouble());

            double expectedBreakWaterHeight = dikeProfileToUpdate.BreakWater.Height + random.NextDouble();
            var expectedBreakWater = new BreakWater(random.NextEnumValue<BreakWaterType>(), expectedBreakWaterHeight);

            var expectedForeshoreGeometry = new[]
            {
                new Point2D(10, 10),
                new Point2D(15, 10)
            };

            var expectedDikeGeometry = new[]
            {
                new RoughnessPoint(new Point2D(10, 10), 1),
                new RoughnessPoint(new Point2D(15, 10), 2)
            };

            var expectedWorldReferencePoint = new Point2D(13, 37);

            var dikeProfileToUpdateFrom = new DikeProfile(expectedWorldReferencePoint,
                                                          expectedDikeGeometry,
                                                          expectedForeshoreGeometry,
                                                          expectedBreakWater,
                                                          new DikeProfile.ConstructionProperties
                                                          {
                                                              Id = expectedId,
                                                              Name = expectedName,
                                                              X0 = expectedX0,
                                                              Orientation = expectedOrientation,
                                                              DikeHeight = expectedDikeHeight
                                                          });

            // Call
            dikeProfileToUpdate.CopyProperties(dikeProfileToUpdateFrom);

            // Assert
            TestHelper.AssertAreEqualButNotSame(expectedWorldReferencePoint, dikeProfileToUpdate.WorldReferencePoint);
            CollectionAssert.AreEqual(expectedForeshoreGeometry, dikeProfileToUpdate.ForeshoreGeometry);
            TestHelper.AssertCollectionAreNotSame(expectedForeshoreGeometry, dikeProfileToUpdate.ForeshoreGeometry);
            CollectionAssert.AreEqual(expectedDikeGeometry, dikeProfileToUpdate.DikeGeometry);
            TestHelper.AssertCollectionAreNotSame(expectedDikeGeometry, dikeProfileToUpdate.DikeGeometry);
            TestHelper.AssertAreEqualButNotSame(expectedBreakWater, dikeProfileToUpdate.BreakWater);
            Assert.AreEqual(expectedId, dikeProfileToUpdate.Id);
            Assert.AreEqual(expectedName, dikeProfileToUpdate.Name);
            Assert.AreEqual(expectedX0, dikeProfileToUpdate.X0);
            Assert.AreEqual(expectedOrientation, dikeProfileToUpdate.Orientation);
            Assert.AreEqual(expectedDikeHeight, dikeProfileToUpdate.DikeHeight);
        }

        [TestFixture]
        private class DikeProfileEqualsTest : EqualsTestFixture<DikeProfile, TestDikeProfile>
        {
            protected override DikeProfile CreateObject()
            {
                return CreateFullyDefinedDikeProfile();
            }

            protected override TestDikeProfile CreateDerivedObject()
            {
                DikeProfile baseProfile = CreateFullyDefinedDikeProfile();
                return new TestDikeProfile(baseProfile);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                DikeProfile baseProfile = CreateFullyDefinedDikeProfile();

                var random = new Random(21);
                double offset = random.NextDouble();

                yield return new TestCaseData(new DikeProfile(new Point2D(500, 1000),
                                                              baseProfile.DikeGeometry,
                                                              baseProfile.ForeshoreGeometry,
                                                              baseProfile.BreakWater,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  Name = baseProfile.Name,
                                                                  Id = baseProfile.Id,
                                                                  Orientation = baseProfile.Orientation,
                                                                  DikeHeight = baseProfile.DikeHeight,
                                                                  X0 = baseProfile.X0
                                                              }))
                    .SetName("WorldReferencePoint");
                yield return new TestCaseData(new DikeProfile(baseProfile.WorldReferencePoint,
                                                              new[]
                                                              {
                                                                  new RoughnessPoint(new Point2D(1, 0), 1),
                                                                  new RoughnessPoint(new Point2D(2, 1), 3)
                                                              },
                                                              baseProfile.ForeshoreGeometry,
                                                              baseProfile.BreakWater,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  Name = baseProfile.Name,
                                                                  Id = baseProfile.Id,
                                                                  Orientation = baseProfile.Orientation,
                                                                  DikeHeight = baseProfile.DikeHeight,
                                                                  X0 = baseProfile.X0
                                                              }))
                    .SetName("DikeGeometry");
                yield return new TestCaseData(new DikeProfile(baseProfile.WorldReferencePoint,
                                                              baseProfile.DikeGeometry,
                                                              new[]
                                                              {
                                                                  new Point2D(50, 100),
                                                                  new Point2D(100, 50)
                                                              },
                                                              baseProfile.BreakWater,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  Name = baseProfile.Name,
                                                                  Id = baseProfile.Id,
                                                                  Orientation = baseProfile.Orientation,
                                                                  DikeHeight = baseProfile.DikeHeight,
                                                                  X0 = baseProfile.X0
                                                              }))
                    .SetName("ForeshoreGeometry");
                yield return new TestCaseData(new DikeProfile(baseProfile.WorldReferencePoint,
                                                              baseProfile.DikeGeometry,
                                                              baseProfile.ForeshoreGeometry,
                                                              null,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  Name = baseProfile.Name,
                                                                  Id = baseProfile.Id,
                                                                  Orientation = baseProfile.Orientation,
                                                                  DikeHeight = baseProfile.DikeHeight,
                                                                  X0 = baseProfile.X0
                                                              }))
                    .SetName("Breakwater");
                yield return new TestCaseData(new DikeProfile(baseProfile.WorldReferencePoint,
                                                              baseProfile.DikeGeometry,
                                                              baseProfile.ForeshoreGeometry,
                                                              baseProfile.BreakWater,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  Name = baseProfile.Name,
                                                                  Id = "Different Id",
                                                                  Orientation = baseProfile.Orientation,
                                                                  DikeHeight = baseProfile.DikeHeight,
                                                                  X0 = baseProfile.X0
                                                              }))
                    .SetName("Id");

                yield return new TestCaseData(new DikeProfile(baseProfile.WorldReferencePoint,
                                                              baseProfile.DikeGeometry,
                                                              baseProfile.ForeshoreGeometry,
                                                              baseProfile.BreakWater,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  Name = "Different Name",
                                                                  Id = baseProfile.Id,
                                                                  Orientation = baseProfile.Orientation,
                                                                  DikeHeight = baseProfile.DikeHeight,
                                                                  X0 = baseProfile.X0
                                                              }))
                    .SetName("Name");
                yield return new TestCaseData(new DikeProfile(baseProfile.WorldReferencePoint,
                                                              baseProfile.DikeGeometry,
                                                              baseProfile.ForeshoreGeometry,
                                                              baseProfile.BreakWater,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  Name = baseProfile.Name,
                                                                  Id = baseProfile.Id,
                                                                  Orientation = baseProfile.Orientation + offset,
                                                                  DikeHeight = baseProfile.DikeHeight,
                                                                  X0 = baseProfile.X0
                                                              }))
                    .SetName("Orientation");
                yield return new TestCaseData(new DikeProfile(baseProfile.WorldReferencePoint,
                                                              baseProfile.DikeGeometry,
                                                              baseProfile.ForeshoreGeometry,
                                                              baseProfile.BreakWater,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  Name = baseProfile.Name,
                                                                  Id = baseProfile.Id,
                                                                  Orientation = baseProfile.Orientation,
                                                                  DikeHeight = baseProfile.DikeHeight + offset,
                                                                  X0 = baseProfile.X0
                                                              }))
                    .SetName("DikeHeight");
                yield return new TestCaseData(new DikeProfile(baseProfile.WorldReferencePoint,
                                                              baseProfile.DikeGeometry,
                                                              baseProfile.ForeshoreGeometry,
                                                              baseProfile.BreakWater,
                                                              new DikeProfile.ConstructionProperties
                                                              {
                                                                  Name = baseProfile.Name,
                                                                  Id = baseProfile.Id,
                                                                  Orientation = baseProfile.Orientation,
                                                                  DikeHeight = baseProfile.DikeHeight,
                                                                  X0 = baseProfile.X0 + offset
                                                              }))
                    .SetName("X0");
            }
        }

        /// <summary>
        /// Creates a default <see cref="DikeProfile"/> with all properties set.
        /// </summary>
        /// <returns>A fully defined <see cref="DikeProfile"/>.</returns>
        private static DikeProfile CreateFullyDefinedDikeProfile()
        {
            const string id = "id";
            const string name = "What's in a name?";

            const double x0 = 13.37;
            const double orientation = 179;
            const double dikeHeight = 10;

            return CreateDikeProfile(new DikeProfile.ConstructionProperties
            {
                Id = id,
                Name = name,
                X0 = x0,
                Orientation = orientation,
                DikeHeight = dikeHeight
            });
        }

        /// <summary>
        /// Creates a <see cref="DikeProfile"/> with all properties set, except for the 
        /// parameters related to <see cref="DikeProfile.ConstructionProperties"/> which
        /// are user specified.
        /// </summary>
        /// <param name="properties">The construction properties.</param>
        /// <returns>A <see cref="DikeProfile"/> with default parameters and 
        /// specified values of the <see cref="DikeProfile.ConstructionProperties"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="properties.Id"/>
        /// is <c>null</c>, empty or a whitespace.</exception>
        private static DikeProfile CreateDikeProfile(DikeProfile.ConstructionProperties properties)
        {
            var worldCoordinate = new Point2D(0, 0);
            var foreshoreGeometry = new[]
            {
                new Point2D(0, 1),
                new Point2D(2, 1)
            };
            var dikeGeometry = new[]
            {
                new RoughnessPoint(new Point2D(0, 1), 1),
                new RoughnessPoint(new Point2D(1, 2), 3)
            };
            var breakWater = new BreakWater(BreakWaterType.Caisson, 1.3);

            return new DikeProfile(worldCoordinate, dikeGeometry, foreshoreGeometry, breakWater, properties);
        }

        private class TestDikeProfile : DikeProfile
        {
            public TestDikeProfile(DikeProfile profile)
                : base(profile.WorldReferencePoint, profile.DikeGeometry, profile.ForeshoreGeometry, profile.BreakWater,
                       new ConstructionProperties
                       {
                           Name = profile.Name,
                           DikeHeight = profile.DikeHeight,
                           Id = profile.Id,
                           Orientation = profile.Orientation,
                           X0 = profile.X0
                       }) {}
        }
    }
}