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
    public class ForeshoreProfileTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            const string validId = "id";
            var worldCoordinate = new Point2D(1.1, 2.2);

            var foreshoreGeometry = new[]
            {
                new Point2D(0.0, 1.1),
                new Point2D(8.0, 9.1)
            };

            // Call
            var foreshoreProfile = new ForeshoreProfile(worldCoordinate, foreshoreGeometry,
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = validId
                                                        });

            // Assert
            Assert.IsInstanceOf<Observable>(foreshoreProfile);

            Assert.IsInstanceOf<RoundedDouble>(foreshoreProfile.Orientation);
            Assert.IsInstanceOf<double>(foreshoreProfile.X0);

            Assert.AreEqual(validId, foreshoreProfile.Id);
            Assert.AreEqual(validId, foreshoreProfile.Name);
            Assert.AreSame(worldCoordinate, foreshoreProfile.WorldReferencePoint);
            Assert.AreEqual(0.0, foreshoreProfile.X0);
            Assert.AreEqual(0.0, foreshoreProfile.Orientation.Value);
            Assert.AreEqual(2, foreshoreProfile.Orientation.NumberOfDecimalPlaces);
            Assert.IsNull(foreshoreProfile.BreakWater);
            CollectionAssert.AreEqual(foreshoreGeometry, foreshoreProfile.Geometry);
            Assert.AreEqual(2, foreshoreProfile.Geometry.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_InvalidId_ThrowNullException(string id)
        {
            // Call
            TestDelegate call = () => new ForeshoreProfile(new Point2D(0, 0), new Point2D[0], null, new ForeshoreProfile.ConstructionProperties
            {
                Id = id
            });

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("properties", paramName);
        }

        [Test]
        public void Constructor_WorldCoordinateNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfile(null, new Point2D[0], null, new ForeshoreProfile.ConstructionProperties
            {
                Id = "id"
            });

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("worldCoordinate", paramName);
        }

        [Test]
        public void Constructor_ForeshoreGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfile(new Point2D(0, 0), null, null, new ForeshoreProfile.ConstructionProperties
            {
                Id = "id"
            });

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
                                                           null, new ForeshoreProfile.ConstructionProperties
                                                           {
                                                               Id = "id"
                                                           });

            // Assert
            const string expectedMessage = "Een punt in de geometrie voor het voorlandprofiel heeft geen waarde.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Orientation_SetToValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Call
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = "id",
                                                            Orientation = 1.23456
                                                        });

            // Assert
            Assert.AreEqual(2, foreshoreProfile.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, foreshoreProfile.Orientation.Value);
        }

        [Test]
        [TestCase("It has a name")]
        [TestCase("Cool new name!")]
        public void Constructor_NonEmptyOrWhitespaceNameDifferentFromId_NameValueSet(string name)
        {
            // Call
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = "id",
                                                            Name = name
                                                        });

            // Assert
            Assert.AreEqual(name, foreshoreProfile.Name);
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
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0], null, new ForeshoreProfile.ConstructionProperties
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
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = "id"
                                                        });

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
                                                        newBreakWater, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = "id"
                                                        });

            // Assert
            Assert.AreSame(newBreakWater, foreshoreProfile.BreakWater);
        }

        [Test]
        public void HasBreakWater_BreakWaterSetToNull_ReturnFalse()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = "id"
                                                        });

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
                                                        breakWater, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = "id"
                                                        });

            // Call
            bool hasBreakWater = foreshoreProfile.HasBreakWater;

            // Assert
            Assert.IsTrue(hasBreakWater);
        }

        [Test]
        public void ToString_Always_ReturnsName()
        {
            // Setup
            const string testName = "testName";
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = "id",
                                                            Name = testName
                                                        });

            // Call
            string result = foreshoreProfile.ToString();

            // Assert
            Assert.AreEqual(testName, result);
        }

        [Test]
        public void CopyProperties_FromForeshoreProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            ForeshoreProfile foreshoreProfile = CreateFullyDefinedForeshoreProfile();

            // Call
            TestDelegate call = () => foreshoreProfile.CopyProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("fromForeshoreProfile", exception.ParamName);
        }

        [Test]
        public void CopyProperties_FromForeshoreProfileAllPropertiesChanged_PropertiesChanged()
        {
            // Setup
            ForeshoreProfile foreshoreProfileToUpdate = CreateFullyDefinedForeshoreProfile();

            const string expectedId = "new_id";
            const string expectedName = "new_name";

            var random = new Random(21);
            double expectedX0 = foreshoreProfileToUpdate.X0 + random.NextDouble();
            var expectedOrientation = new RoundedDouble(2, (foreshoreProfileToUpdate.Orientation + random.NextDouble()) % 360);

            double expectedBreakWaterHeight = foreshoreProfileToUpdate.BreakWater.Height + random.NextDouble();
            var expectedBreakWater = new BreakWater(random.NextEnumValue<BreakWaterType>(), expectedBreakWaterHeight);

            Point2D originalPoint = foreshoreProfileToUpdate.WorldReferencePoint;
            var expectedWorldReferencePoint = new Point2D(originalPoint.X + random.NextDouble(),
                                                          originalPoint.Y + random.NextDouble());

            var expectedForeshoreGeometry = new[]
            {
                new Point2D(10, 10),
                new Point2D(15, 10)
            };

            var foreshoreProfileToUpdateFrom = new ForeshoreProfile(expectedWorldReferencePoint,
                                                                    expectedForeshoreGeometry,
                                                                    expectedBreakWater,
                                                                    new ForeshoreProfile.ConstructionProperties
                                                                    {
                                                                        Id = expectedId,
                                                                        Name = expectedName,
                                                                        Orientation = expectedOrientation,
                                                                        X0 = expectedX0
                                                                    });

            // Call
            foreshoreProfileToUpdate.CopyProperties(foreshoreProfileToUpdateFrom);

            // Assert
            Assert.AreEqual(expectedWorldReferencePoint, foreshoreProfileToUpdate.WorldReferencePoint);
            Assert.AreNotSame(expectedWorldReferencePoint, foreshoreProfileToUpdate.WorldReferencePoint);
            CollectionAssert.AreEqual(expectedForeshoreGeometry, foreshoreProfileToUpdate.Geometry);
            TestHelper.AssertCollectionAreNotSame(expectedForeshoreGeometry, foreshoreProfileToUpdate.Geometry);
            Assert.AreEqual(expectedBreakWater, foreshoreProfileToUpdate.BreakWater);
            Assert.AreNotSame(expectedBreakWater, foreshoreProfileToUpdate.BreakWater);
            Assert.AreEqual(expectedId, foreshoreProfileToUpdate.Id);
            Assert.AreEqual(expectedName, foreshoreProfileToUpdate.Name);
            Assert.AreEqual(expectedX0, foreshoreProfileToUpdate.X0);
            Assert.AreEqual(expectedOrientation, foreshoreProfileToUpdate.Orientation);
        }

        [TestFixture]
        private class ForeshoreProfileEqualsTest : EqualsTestFixture<ForeshoreProfile, DerivedForeshoreProfile>
        {
            protected override ForeshoreProfile CreateObject()
            {
                return CreateFullyDefinedForeshoreProfile();
            }

            protected override DerivedForeshoreProfile CreateDerivedObject()
            {
                ForeshoreProfile baseProfile = CreateFullyDefinedForeshoreProfile();
                return new DerivedForeshoreProfile(baseProfile);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                ForeshoreProfile baseProfile = CreateFullyDefinedForeshoreProfile();

                var random = new Random(21);
                double offset = random.NextDouble();

                yield return new TestCaseData(new ForeshoreProfile(new Point2D(500, 1000),
                                                                   baseProfile.Geometry,
                                                                   baseProfile.BreakWater,
                                                                   new ForeshoreProfile.ConstructionProperties
                                                                   {
                                                                       Name = baseProfile.Name,
                                                                       Id = baseProfile.Id,
                                                                       Orientation = baseProfile.Orientation,
                                                                       X0 = baseProfile.X0
                                                                   }))
                    .SetName("WorldReferencePoint");
                yield return new TestCaseData(new ForeshoreProfile(baseProfile.WorldReferencePoint,
                                                                   new[]
                                                                   {
                                                                       new Point2D(50, 100),
                                                                       new Point2D(100, 50)
                                                                   },
                                                                   baseProfile.BreakWater,
                                                                   new ForeshoreProfile.ConstructionProperties
                                                                   {
                                                                       Name = baseProfile.Name,
                                                                       Id = baseProfile.Id,
                                                                       Orientation = baseProfile.Orientation,
                                                                       X0 = baseProfile.X0
                                                                   }))
                    .SetName("ForeshoreGeometry");
                yield return new TestCaseData(new ForeshoreProfile(baseProfile.WorldReferencePoint,
                                                                   baseProfile.Geometry,
                                                                   null,
                                                                   new ForeshoreProfile.ConstructionProperties
                                                                   {
                                                                       Name = baseProfile.Name,
                                                                       Id = baseProfile.Id,
                                                                       Orientation = baseProfile.Orientation,
                                                                       X0 = baseProfile.X0
                                                                   }))
                    .SetName("Breakwater");
                yield return new TestCaseData(new ForeshoreProfile(baseProfile.WorldReferencePoint,
                                                                   baseProfile.Geometry,
                                                                   baseProfile.BreakWater,
                                                                   new ForeshoreProfile.ConstructionProperties
                                                                   {
                                                                       Name = baseProfile.Name,
                                                                       Id = "Different Id",
                                                                       Orientation = baseProfile.Orientation,
                                                                       X0 = baseProfile.X0
                                                                   }))
                    .SetName("Id");

                yield return new TestCaseData(new ForeshoreProfile(baseProfile.WorldReferencePoint,
                                                                   baseProfile.Geometry,
                                                                   baseProfile.BreakWater,
                                                                   new ForeshoreProfile.ConstructionProperties
                                                                   {
                                                                       Name = "Different Name",
                                                                       Id = baseProfile.Id,
                                                                       Orientation = baseProfile.Orientation,
                                                                       X0 = baseProfile.X0
                                                                   }))
                    .SetName("Name");
                yield return new TestCaseData(new ForeshoreProfile(baseProfile.WorldReferencePoint,
                                                                   baseProfile.Geometry,
                                                                   baseProfile.BreakWater,
                                                                   new ForeshoreProfile.ConstructionProperties
                                                                   {
                                                                       Name = baseProfile.Name,
                                                                       Id = baseProfile.Id,
                                                                       Orientation = baseProfile.Orientation + offset,
                                                                       X0 = baseProfile.X0
                                                                   }))
                    .SetName("Orientation");
                yield return new TestCaseData(new ForeshoreProfile(baseProfile.WorldReferencePoint,
                                                                   baseProfile.Geometry,
                                                                   baseProfile.BreakWater,
                                                                   new ForeshoreProfile.ConstructionProperties
                                                                   {
                                                                       Name = baseProfile.Name,
                                                                       Id = baseProfile.Id,
                                                                       Orientation = baseProfile.Orientation,
                                                                       X0 = baseProfile.X0 + offset
                                                                   }))
                    .SetName("X0");
            }
        }

        /// <summary>
        /// Creates a default <see cref="ForeshoreProfile"/> with all properties set.
        /// </summary>
        /// <returns>A fully defined <see cref="ForeshoreProfile"/>.</returns>
        private static ForeshoreProfile CreateFullyDefinedForeshoreProfile()
        {
            const string id = "id";
            const string name = "What's in a name?";

            const double x0 = 13.37;
            const double orientation = 179;

            return CreateForeshoreProfile(new ForeshoreProfile.ConstructionProperties
            {
                Id = id,
                Name = name,
                X0 = x0,
                Orientation = orientation
            });
        }

        /// <summary>
        /// Creates a <see cref="ForeshoreProfile"/> with all properties set, except for the 
        /// parameters related to <see cref="ForeshoreProfile.ConstructionProperties"/> which
        /// are user specified.
        /// </summary>
        /// <param name="properties">The construction properties.</param>
        /// <returns>A <see cref="ForeshoreProfile"/> with default parameters and 
        /// specified values of the <see cref="ForeshoreProfile.ConstructionProperties"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="properties.Id"/>
        /// is <c>null</c>, empty or a whitespace.</exception>
        private static ForeshoreProfile CreateForeshoreProfile(ForeshoreProfile.ConstructionProperties properties)
        {
            var worldReferencePoint = new Point2D(0, 0);
            var geometry = new[]
            {
                new Point2D(0, 1),
                new Point2D(2, 1)
            };
            var breakWater = new BreakWater(BreakWaterType.Caisson, 1.3);

            return new ForeshoreProfile(worldReferencePoint, geometry, breakWater, properties);
        }

        private class DerivedForeshoreProfile : ForeshoreProfile
        {
            public DerivedForeshoreProfile(ForeshoreProfile profile)
                : base(profile.WorldReferencePoint, profile.Geometry, profile.BreakWater, new ConstructionProperties
                {
                    Id = profile.Id,
                    Name = profile.Name,
                    X0 = profile.X0,
                    Orientation = profile.X0
                }) {}
        }
    }
}