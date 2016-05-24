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
using Application.Ringtoets.Storage.Create;
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
            var failureMechanismSection = new FailureMechanismSection("", new [] { new Point2D(0,0), new Point2D(0,0) });

            // Call
            TestDelegate test = () => failureMechanismSection.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollectorAndLayers_ReturnsSoilProfileEntityWithPropertiesAndSoilLayerEntitiesSet()
        {
            // Setup
            string testName = "testName";
            var geometryPoints = new[] { new Point2D(0, 0), new Point2D(0, 0) };
            var failureMechanismSection = new FailureMechanismSection(testName, geometryPoints);
            var collector = new PersistenceRegistry();

            // Call
            var entity = failureMechanismSection.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(2, entity.FailureMechanismSectionPointEntities.Count);
        }   
    }
}