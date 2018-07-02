// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;
using Ringtoets.Storage.Core.Read.GrassCoverErosionInwards;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Test.Read.GrassCoverErosionInwards
{
    [TestFixture]
    public class DikeProfileEntityReadExtensionsTest
    {
        private const string validRoughnessPointXml = "<ArrayOfSerializableRoughnessPoint " +
                                                      "xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                                                      "</ArrayOfSerializableRoughnessPoint>";

        [Test]
        public void Read_ReadConversionCollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new DikeProfileEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_EntityNotReadBefore_EntityRegistered()
        {
            // Setup
            var collector = new ReadConversionCollector();

            var entity = new DikeProfileEntity
            {
                Id = "id",
                ForeshoreXml = new Point2DXmlSerializer().ToXml(new Point2D[0]),
                DikeGeometryXml = new RoughnessPointXmlSerializer().ToXml(new RoughnessPoint[0])
            };

            // Precondition
            Assert.IsFalse(collector.Contains(entity));

            // Call
            entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Read_DikeGeometryXmlNullOrEmpty_ThrowsArgumentException(string xml)
        {
            // Setup
            var profile = new DikeProfileEntity
            {
                DikeGeometryXml = xml,
                ForeshoreXml = validRoughnessPointXml
            };

            // Call
            TestDelegate test = () => profile.Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Read_ForeshoreXmlNullOrEmpty_ThrowsArgumentException(string xml)
        {
            // Setup
            var profile = new DikeProfileEntity
            {
                DikeGeometryXml = validRoughnessPointXml,
                ForeshoreXml = xml
            };

            // Call
            TestDelegate test = () => profile.Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void Read_DikeProfileEntityWithBreakWaterPropertiesNull_ReturnDikeProfileWithoutBreakWater()
        {
            // Setup
            var foreshorePoints = new[]
            {
                new Point2D(-9.9, 8.8),
                new Point2D(-7.7, 5.5)
            };
            var roughnessPoints = new[]
            {
                new RoughnessPoint(new Point2D(-7.7, 5.5), 1.0),
                new RoughnessPoint(new Point2D(5.5, 6.6), 0.5)
            };
            var entity = new DikeProfileEntity
            {
                Id = "saved",
                Name = "Just saved",
                Orientation = 45.67,
                BreakWaterHeight = null,
                BreakWaterType = null,
                ForeshoreXml = new Point2DXmlSerializer().ToXml(foreshorePoints),
                DikeGeometryXml = new RoughnessPointXmlSerializer().ToXml(roughnessPoints),
                DikeHeight = 1.2,
                X = 3.4,
                Y = 5.6,
                X0 = -7.8
            };

            var collector = new ReadConversionCollector();

            // Call
            DikeProfile dikeProfile = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Id, dikeProfile.Id);
            Assert.AreEqual(entity.Name, dikeProfile.Name);
            Assert.AreEqual(entity.Orientation, dikeProfile.Orientation.Value);
            CollectionAssert.AreEqual(foreshorePoints, dikeProfile.ForeshoreGeometry);
            CollectionAssert.AreEqual(roughnessPoints, dikeProfile.DikeGeometry, new RoughnessPointComparer());
            Assert.AreEqual(entity.X, dikeProfile.WorldReferencePoint.X);
            Assert.AreEqual(entity.Y, dikeProfile.WorldReferencePoint.Y);
            Assert.AreEqual(entity.X0, dikeProfile.X0);

            Assert.IsFalse(dikeProfile.HasBreakWater);
        }

        [Test]
        [TestCase(BreakWaterType.Caisson, 1.1)]
        [TestCase(BreakWaterType.Dam, -22.2)]
        [TestCase(BreakWaterType.Wall, 45.67)]
        public void Read_DikeProfileEntityWithBreakWater_ReturnDikeProfileWithBreakWater(BreakWaterType type, double height)
        {
            // Setup
            var foreshorePoints = new Point2D[0];
            var roughnessPoints = new[]
            {
                new RoughnessPoint(new Point2D(1.1, 2.2), 1.0),
                new RoughnessPoint(new Point2D(3.3, 4.4), 0.6),
                new RoughnessPoint(new Point2D(5.5, 6.6), 1.0),
                new RoughnessPoint(new Point2D(7.7, 8.8), 0.5)
            };
            var entity = new DikeProfileEntity
            {
                Id = "with_breakwater",
                Name = "I have a Breakwater!",
                Orientation = 360.0,
                BreakWaterHeight = height,
                BreakWaterType = Convert.ToByte(type),
                ForeshoreXml = new Point2DXmlSerializer().ToXml(foreshorePoints),
                DikeGeometryXml = new RoughnessPointXmlSerializer().ToXml(roughnessPoints),
                DikeHeight = 4.5,
                X = 93.0,
                Y = 945.6,
                X0 = 9.34
            };

            var collector = new ReadConversionCollector();

            // Call
            DikeProfile dikeProfile = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Id, dikeProfile.Id);
            Assert.AreEqual(entity.Name, dikeProfile.Name);
            Assert.AreEqual(entity.Orientation, dikeProfile.Orientation.Value);
            CollectionAssert.AreEqual(foreshorePoints, dikeProfile.ForeshoreGeometry);
            CollectionAssert.AreEqual(roughnessPoints, dikeProfile.DikeGeometry, new RoughnessPointComparer());
            Assert.AreEqual(entity.X, dikeProfile.WorldReferencePoint.X);
            Assert.AreEqual(entity.Y, dikeProfile.WorldReferencePoint.Y);
            Assert.AreEqual(entity.X0, dikeProfile.X0);

            Assert.AreEqual(height, dikeProfile.BreakWater.Height.Value);
            Assert.AreEqual(type, dikeProfile.BreakWater.Type);

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_AlreadyReadDikeProfileEntity_ReturnDikeProfile()
        {
            // Setup
            var registeredEntity = new DikeProfileEntity();
            DikeProfile registeredProfile = DikeProfileTestFactory.CreateDikeProfile();
            var collector = new ReadConversionCollector();
            collector.Read(registeredEntity, registeredProfile);

            // Call
            DikeProfile returnedProfile = registeredEntity.Read(collector);

            // Assert
            Assert.AreSame(registeredProfile, returnedProfile);
        }

        private class RoughnessPointComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var p1 = (RoughnessPoint) x;
                var p2 = (RoughnessPoint) y;
                if (p1.Point.Equals(p2.Point) && p1.Roughness.Equals(p2.Roughness))
                {
                    return 0;
                }

                return 1;
            }
        }
    }
}