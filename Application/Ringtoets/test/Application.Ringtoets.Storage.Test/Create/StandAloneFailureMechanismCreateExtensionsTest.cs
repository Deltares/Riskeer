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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class StandAloneFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StandAloneFailureMechanism("name", "code");

            // Call
            TestDelegate test = () => failureMechanism.Create(FailureMechanismType.DuneErosion, null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollector_ReturnsFailureMechanismEntityWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var failureMechanism = new StandAloneFailureMechanism("name", "code")
            {
                IsRelevant = isRelevant
            };
            var collector = new CreateConversionCollector();
            var failureMechanismType = FailureMechanismType.DuneErosion;

            // Call
            var entity = failureMechanism.Create(failureMechanismType, collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short)failureMechanismType, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.IsEmpty(entity.StochasticSoilModelEntities);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithSections_ReturnsFailureMechanismEntityWithFailureMechanismSectionEntities(bool isRelevant)
        {
            // Setup
            var failureMechanism = new StandAloneFailureMechanism("name", "code");
            failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[] { new Point2D(0, 0) }));
            failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[] { new Point2D(0, 0) }));
            var collector = new CreateConversionCollector();
            var failureMechanismType = FailureMechanismType.DuneErosion;

            // Call
            var entity = failureMechanism.Create(failureMechanismType, collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(2, entity.FailureMechanismSectionEntities.Count);
        }
    }
}