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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class DikeProfileTest
    {
        [Test]
        public void Constructor_Valid()
        {
            // Setup
            var worldCoordinate = new Point2D(1.1, 2.2);
            var dikeGeometry = new[]
            {
                new RoughnessPoint(new Point2D(1.1, 2.2), 0.7),
                new RoughnessPoint(new Point2D(3.3, 4.4), 0.7),
            };

            var foreshoreGeometry = new[]
            {
                new Point2D(0.0, 1.1), 
                new Point2D(8.0, 9.1), 
            };

            // Call
            var dikeProfile = new DikeProfile(worldCoordinate, dikeGeometry, foreshoreGeometry);

            // Assert
            Assert.IsInstanceOf<RoundedDouble>(dikeProfile.Orientation);
            Assert.IsInstanceOf<RoundedDouble>(dikeProfile.DikeHeight);
            Assert.IsInstanceOf<double>(dikeProfile.X0);

            Assert.AreEqual("Dijkprofiel", dikeProfile.Name);
            Assert.AreEqual(string.Empty, dikeProfile.Memo);
            Assert.AreSame(worldCoordinate, dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0.0, dikeProfile.X0);
            Assert.AreEqual(0.0, dikeProfile.Orientation.Value);
            Assert.AreEqual(2, dikeProfile.Orientation.NumberOfDecimalPlaces);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.AreEqual(dikeGeometry, dikeProfile.DikeGeometry);
            CollectionAssert.AreEqual(foreshoreGeometry, dikeProfile.ForeshoreGeometry);
            Assert.AreEqual(0.0, dikeProfile.DikeHeight.Value);
            Assert.AreEqual(2, dikeProfile.DikeHeight.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_WorldReferencePointIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(null, new RoughnessPoint[0], new Point2D[0]);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("worldCoordinate", paramName);
        }

        [Test]
        public void Constructor_DikeGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(new Point2D(0, 0), null, new Point2D[0]);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            StringAssert.StartsWith(Resources.DikeProfile_SetGeometry_Collection_of_points_for_geometry_is_null, exception.Message);
            Assert.AreEqual("points", exception.ParamName);
        }

        [Test]
        public void Constructor_DikeGeometryContainsNullPoint_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(new Point2D(0.0, 0.0), new RoughnessPoint[]
            {
                null
            }, new Point2D[0]);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            StringAssert.StartsWith(Resources.DikeProfile_SetGeometry_A_point_in_the_collection_is_null, exception.Message);
        }

        [Test]
        public void Constructor_ForeshoreGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            StringAssert.StartsWith(Resources.DikeProfile_SetForshoreGeometry_Collection_of_points_for_foreshore_geometry_is_null, exception.Message);
            Assert.AreEqual("points", exception.ParamName);
        }

        [Test]
        public void Constructor_ForeshoreGeometryContainsNullPoint_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(new Point2D(0.0, 0.0), new RoughnessPoint[0], new Point2D[]
            {
                null
            });

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            StringAssert.StartsWith(Resources.DikeProfile_SetForshoreGeometry_A_point_in_the_collection_is_null, exception.Message);
        }

        [Test]
        public void X0_SetNewValue_GetsNewValue([Random(-9999.99, 9999.99, 1)] double newValue)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            // Call
            dikeProfile.X0 = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfile.X0);
        }

        [Test]
        public void Orientation_SetToValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            int originalNumberOfDecimalPlaces = dikeProfile.Orientation.NumberOfDecimalPlaces;

            // Call
            dikeProfile.Orientation = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, dikeProfile.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, dikeProfile.Orientation.Value);
        }

        [Test]
        public void DikeHeight_SetToValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            int originalNumberOfDecimalPlaces = dikeProfile.DikeHeight.NumberOfDecimalPlaces;

            // Call
            dikeProfile.DikeHeight = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, dikeProfile.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, dikeProfile.DikeHeight.Value);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Cool new name!")]
        public void Name_SetNewValue_GetsNewValue(string name)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            // Call
            dikeProfile.Name = name;

            // Assert
            Assert.AreEqual(name, dikeProfile.Name);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Very informative memo")]
        public void Memo_SetNewValue_GetsNewValue(string memo)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            // Call
            dikeProfile.Memo = memo;

            // Assert
            Assert.AreEqual(memo, dikeProfile.Memo);
        }

        [Test]
        public void BreakWater_SetToNull_GetsNewlySetNull()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            // Call
            dikeProfile.BreakWater = null;

            // Assert
            Assert.IsNull(dikeProfile.BreakWater);
        }

        [Test]
        public void BreakWater_SetToNewInstance_GetsNewlySetInstance()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            var newBreakWater = new BreakWater(BreakWaterType.Caisson, 1.1);

            // Call
            dikeProfile.BreakWater = newBreakWater;

            // Assert
            Assert.AreSame(newBreakWater, dikeProfile.BreakWater);
        }

        [Test]
        public void HasBreakWater_BreakWaterSetToNull_ReturnFalse()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0])
            {
                BreakWater = null
            };

            // Call
            bool hasBreakWater = dikeProfile.HasBreakWater;

            // Assert
            Assert.IsFalse(hasBreakWater);
        }

        [Test]
        public void HasBreakWater_BreakWaterSetToAnInstance_ReturnTrue()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0])
            {
                BreakWater = new BreakWater(BreakWaterType.Dam, 12.34)
            };

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
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0])
            {
                Name = testName
            };

            // Call
            var result = dikeProfile.ToString();

            // Assert
            Assert.AreEqual(testName, result);
        }
    }
}