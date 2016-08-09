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

using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Serializers;

using Core.Common.Base.Geometry;

using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class FailureMechanismSectionEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FailureMechanismSectionEntity().Read(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        } 

        [Test]
        public void Read_Always_NewPoint()
        {
            // Setup
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            string name = "testName";
            var points = new[]
            {
                new Point2D(0, 0)
            };
            string pointXml = new Point2DXmlSerializer().ToXml(points);
            var entity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = entityId,
                Name = name,
                FailureMechanismSectionPointXml = pointXml
            };

            // Call
            var readConversionCollector = new ReadConversionCollector();
            var section = entity.Read(readConversionCollector);

            // Assert
            Assert.IsNotNull(section);
            Assert.AreEqual(entityId, section.StorageId);
            Assert.AreEqual(name, section.Name);
            Assert.AreEqual(section, readConversionCollector.Get(entity));
        } 
    }
}