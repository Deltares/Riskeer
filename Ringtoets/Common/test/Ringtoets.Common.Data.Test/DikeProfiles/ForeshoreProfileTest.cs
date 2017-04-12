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
            var id = "id";

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
            var testName = "testName";
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
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            ForeshoreProfile foreshoreProfile = CreateFullyDefinedForeshoreProfile();

            // Call
            bool isEqualToItself = foreshoreProfile.Equals(foreshoreProfile);

            // Assert
            Assert.IsTrue(isEqualToItself);
        }

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            ForeshoreProfile foreshoreProfile = CreateFullyDefinedForeshoreProfile();

            // Call
            bool isForeshoreProfileEqualToNull = foreshoreProfile.Equals(null);

            // Assert
            Assert.IsFalse(isForeshoreProfileEqualToNull);
        }

        [Test]
        public void Equal_ToDifferentType_ReturnsFalse()
        {
            // Setup
            ForeshoreProfile foreshoreProfile = CreateFullyDefinedForeshoreProfile();
            var differentType = new object();

            // Call
            bool isForeshoreProfileEqualToDifferentObject = foreshoreProfile.Equals(differentType);

            // Assert
            Assert.IsFalse(isForeshoreProfileEqualToDifferentObject);
        }

        [Test]
        public void Equals_DifferentWorldReferencePoints_ReturnsFalse()
        {
            // Setup
            ForeshoreProfile foreshoreProfileOne = CreateFullyDefinedForeshoreProfile();
            var foreshoreProfileTwo = new ForeshoreProfile(new Point2D(13, 37),
                                                           foreshoreProfileOne.Geometry,
                                                           foreshoreProfileOne.BreakWater,
                                                           new ForeshoreProfile.ConstructionProperties
                                                           {
                                                               Id = foreshoreProfileOne.Id,
                                                               Name = foreshoreProfileOne.Name,
                                                               X0 = foreshoreProfileOne.X0,
                                                               Orientation = foreshoreProfileOne.Orientation
                                                           });

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToOne = foreshoreProfileTwo.Equals(foreshoreProfileOne);

            // Assert
            Assert.IsFalse(isForeshoreProfileOneEqualToTwo);
            Assert.IsFalse(isForeshoreProfileTwoEqualToOne);
        }

        [Test]
        public void Equals_DifferentForeshoreGeometry_ReturnsFalse()
        {
            // Setup
            ForeshoreProfile foreshoreProfileOne = CreateFullyDefinedForeshoreProfile();

            var foreshoreGeometry = new[]
            {
                new Point2D(10, 10),
                new Point2D(11, 11)
            };
            var foreshoreProfileTwo = new ForeshoreProfile(foreshoreProfileOne.WorldReferencePoint,
                                                           foreshoreGeometry,
                                                           foreshoreProfileOne.BreakWater,
                                                           new ForeshoreProfile.ConstructionProperties
                                                           {
                                                               Id = foreshoreProfileOne.Id,
                                                               Name = foreshoreProfileOne.Name,
                                                               X0 = foreshoreProfileOne.X0,
                                                               Orientation = foreshoreProfileOne.Orientation
                                                           });

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToOne = foreshoreProfileTwo.Equals(foreshoreProfileOne);

            // Assert
            Assert.IsFalse(isForeshoreProfileOneEqualToTwo);
            Assert.IsFalse(isForeshoreProfileTwoEqualToOne);
        }

        [Test]
        public void Equals_DifferentBreakWater_ReturnsFalse()
        {
            // Setup
            ForeshoreProfile foreshoreProfileOne = CreateFullyDefinedForeshoreProfile();
            var foreshoreProfileTwo = new ForeshoreProfile(foreshoreProfileOne.WorldReferencePoint,
                                                           foreshoreProfileOne.Geometry,
                                                           null,
                                                           new ForeshoreProfile.ConstructionProperties
                                                           {
                                                               Id = foreshoreProfileOne.Id,
                                                               Name = foreshoreProfileOne.Name,
                                                               X0 = foreshoreProfileOne.X0,
                                                               Orientation = foreshoreProfileOne.Orientation
                                                           });

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToOne = foreshoreProfileTwo.Equals(foreshoreProfileOne);

            // Assert
            Assert.IsFalse(isForeshoreProfileOneEqualToTwo);
            Assert.IsFalse(isForeshoreProfileTwoEqualToOne);
        }

        [Test]
        public void Equals_DifferentIds_ReturnsFalse()
        {
            // Setup
            const string name = "Just a name";
            const double x0 = 10.0;
            const double orientation = 179;
            ForeshoreProfile foreshoreProfileOne = CreateForeshoreProfile(new ForeshoreProfile.ConstructionProperties
            {
                Id = "id1",
                Name = name,
                X0 = x0,
                Orientation = orientation
            });

            ForeshoreProfile foreshoreProfileTwo = CreateForeshoreProfile(new ForeshoreProfile.ConstructionProperties
            {
                Id = "id2",
                Name = name,
                X0 = x0,
                Orientation = orientation
            });

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToOne = foreshoreProfileTwo.Equals(foreshoreProfileOne);

            // Assert
            Assert.IsFalse(isForeshoreProfileOneEqualToTwo);
            Assert.IsFalse(isForeshoreProfileTwoEqualToOne);
        }

        [Test]
        public void Equals_DifferentNames_ReturnsFalse()
        {
            // Setup
            const string id = "id";
            const double x0 = 10.0;
            const double orientation = 179;
            ForeshoreProfile foreshoreProfileOne = CreateForeshoreProfile(new ForeshoreProfile.ConstructionProperties
            {
                Id = id,
                Name = "Name 1",
                X0 = x0,
                Orientation = orientation
            });

            ForeshoreProfile foreshoreProfileTwo = CreateForeshoreProfile(new ForeshoreProfile.ConstructionProperties
            {
                Id = id,
                Name = "Name 2",
                X0 = x0,
                Orientation = orientation
            });

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToOne = foreshoreProfileTwo.Equals(foreshoreProfileOne);

            // Assert
            Assert.IsFalse(isForeshoreProfileOneEqualToTwo);
            Assert.IsFalse(isForeshoreProfileTwoEqualToOne);
        }

        [Test]
        public void Equals_DifferentX0_ReturnsFalse()
        {
            // Setup
            const string id = "ID";
            const string name = "Just a name";
            const double orientation = 179;
            ForeshoreProfile foreshoreProfileOne = CreateForeshoreProfile(new ForeshoreProfile.ConstructionProperties
            {
                Id = id,
                Name = name,
                X0 = 10.0,
                Orientation = orientation
            });

            ForeshoreProfile foreshoreProfileTwo = CreateForeshoreProfile(new ForeshoreProfile.ConstructionProperties
            {
                Id = id,
                Name = name,
                X0 = 11.0,
                Orientation = orientation
            });

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToOne = foreshoreProfileTwo.Equals(foreshoreProfileOne);

            // Assert
            Assert.IsFalse(isForeshoreProfileOneEqualToTwo);
            Assert.IsFalse(isForeshoreProfileTwoEqualToOne);
        }

        [Test]
        public void Equals_DifferentOrientation_ReturnsFalse()
        {
            // Setup
            const string id = "ID";
            const string name = "Just a name";
            const double x0 = 13.37;
            ForeshoreProfile foreshoreProfileOne = CreateForeshoreProfile(new ForeshoreProfile.ConstructionProperties
            {
                Id = id,
                Name = name,
                X0 = x0,
                Orientation = 179
            });

            ForeshoreProfile foreshoreProfileTwo = CreateForeshoreProfile(new ForeshoreProfile.ConstructionProperties
            {
                Id = id,
                Name = name,
                X0 = x0,
                Orientation = 180
            });

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToOne = foreshoreProfileTwo.Equals(foreshoreProfileOne);

            // Assert
            Assert.IsFalse(isForeshoreProfileOneEqualToTwo);
            Assert.IsFalse(isForeshoreProfileTwoEqualToOne);
        }

        [Test]
        public void Equals_AllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            ForeshoreProfile foreshoreProfileOne = CreateFullyDefinedForeshoreProfile();
            ForeshoreProfile foreshoreProfileTwo = CreateFullyDefinedForeshoreProfile();

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToOne = foreshoreProfileTwo.Equals(foreshoreProfileOne);

            // Assert
            Assert.IsTrue(isForeshoreProfileOneEqualToTwo);
            Assert.IsTrue(isForeshoreProfileTwoEqualToOne);
        }

        [Test]
        public void Equals_TransitivePropertyAllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            ForeshoreProfile foreshoreProfileOne = CreateFullyDefinedForeshoreProfile();
            ForeshoreProfile foreshoreProfileTwo = CreateFullyDefinedForeshoreProfile();
            ForeshoreProfile foreshoreProfileThree = CreateFullyDefinedForeshoreProfile();

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToThree = foreshoreProfileTwo.Equals(foreshoreProfileThree);
            bool isForeshoreProfileOneEqualToThree = foreshoreProfileOne.Equals(foreshoreProfileThree);

            // Assert
            Assert.IsTrue(isForeshoreProfileOneEqualToTwo);
            Assert.IsTrue(isForeshoreProfileTwoEqualToThree);
            Assert.IsTrue(isForeshoreProfileOneEqualToThree);
        }

        [Test]
        public void Equals_SameReference_ReturnsTrue()
        {
            // Setup
            ForeshoreProfile foreshoreProfileOne = CreateFullyDefinedForeshoreProfile();
            ForeshoreProfile foreshoreProfileTwo = foreshoreProfileOne;

            // Call
            bool isForeshoreProfileOneEqualToTwo = foreshoreProfileOne.Equals(foreshoreProfileTwo);
            bool isForeshoreProfileTwoEqualToOne = foreshoreProfileTwo.Equals(foreshoreProfileOne);

            // Assert
            Assert.IsTrue(isForeshoreProfileOneEqualToTwo);
            Assert.IsTrue(isForeshoreProfileTwoEqualToOne);
        }

        [Test]
        public void GetHashCode_EqualForeshoreProfiles_ReturnsSameHashCode()
        {
            // Setup
            ForeshoreProfile foreshoreProfileOne = CreateFullyDefinedForeshoreProfile();
            ForeshoreProfile foreshoreProfileTwo = CreateFullyDefinedForeshoreProfile();

            // Call
            int hashCodeOne = foreshoreProfileOne.GetHashCode();
            int hashCodeTwo = foreshoreProfileTwo.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
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
            var worldCoordinate = new Point2D(0, 0);
            var geometry = new[]
            {
                new Point2D(0, 1),
                new Point2D(2, 1)
            };
            var breakWater = new BreakWater(BreakWaterType.Caisson, 1.3);

            return new ForeshoreProfile(worldCoordinate, geometry, breakWater, properties);
        }
    }
}