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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Test.Read
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Read_FailureMechanismSectionPointXmlNullOrEmpty_ThrowsArgumentException(string xml)
        {
            // Setup
            var entity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionPointXml = xml
            };

            // Call
            TestDelegate test = () => entity.Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void Read_WithCollector_NewPointAndEntityRegistered()
        {
            // Setup
            const string name = "testName";
            var points = new[]
            {
                new Point2D(0, 0)
            };
            string pointXml = new Point2DCollectionXmlSerializer().ToXml(points);
            var entity = new FailureMechanismSectionEntity
            {
                Name = name,
                FailureMechanismSectionPointXml = pointXml
            };

            var collector = new ReadConversionCollector();

            // Call
            FailureMechanismSection section = entity.Read(collector);

            // Assert
            Assert.IsNotNull(section);
            Assert.AreEqual(name, section.Name);
            Assert.AreEqual(section, collector.Get(entity));

            Assert.IsTrue(collector.Contains(entity));
        }
    }
}