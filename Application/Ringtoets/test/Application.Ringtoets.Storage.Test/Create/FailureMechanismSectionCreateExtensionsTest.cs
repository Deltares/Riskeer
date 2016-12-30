﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class FailureMechanismSectionCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanismSection = new TestFailureMechanismSection();

            // Call
            TestDelegate test = () => failureMechanismSection.Create(null);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithCollectorAndGeometry_ReturnsFailureMechanismSectionWithGeometryStringSet()
        {
            // Setup
            string testName = "testName";
            var geometryPoints = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 0)
            };
            var failureMechanismSection = new FailureMechanismSection(testName, geometryPoints);
            var registry = new PersistenceRegistry();

            // Call
            var entity = failureMechanismSection.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            string expectedXml = new Point2DXmlSerializer().ToXml(geometryPoints);
            Assert.AreEqual(expectedXml, entity.FailureMechanismSectionPointXml);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            string testName = "original name";
            var geometryPoints = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 0)
            };
            var failureMechanismSection = new FailureMechanismSection(testName, geometryPoints);
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismSectionEntity entity = failureMechanismSection.Create(registry);

            // Assert
            Assert.AreNotSame(testName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(testName, entity.Name);
        }
    }
}