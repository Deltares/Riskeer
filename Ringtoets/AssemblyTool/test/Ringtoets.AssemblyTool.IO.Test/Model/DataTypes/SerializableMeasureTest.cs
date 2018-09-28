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
using System.Linq;
using System.Xml.Serialization;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model.DataTypes
{
    [TestFixture]
    public class SerializableMeasureTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var measure = new SerializableMeasure();

            // Assert
            Assert.AreEqual("m", measure.UnitOfMeasure);
            Assert.IsNaN(measure.Value);

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableMeasure>(
                nameof(SerializableMeasure.UnitOfMeasure), "uom");

            IEnumerable<XmlTextAttribute> xmlTextAttributes = TypeUtils.GetPropertyAttributes<SerializableMeasure, XmlTextAttribute>(
                nameof(SerializableMeasure.Value));
            Assert.AreEqual(1, xmlTextAttributes.Count());
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            double value = new Random(39).NextDouble();

            // Call
            var measure = new SerializableMeasure(value);

            // Assert
            Assert.AreEqual("m", measure.UnitOfMeasure);
            Assert.AreEqual(value, measure.Value);
        }
    }
}