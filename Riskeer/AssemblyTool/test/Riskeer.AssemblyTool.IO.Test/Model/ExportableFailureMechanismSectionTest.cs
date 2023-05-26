// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class ExportableFailureMechanismSectionTest
    {
        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new ExportableFailureMechanismSection(invalidId, null, random.NextDouble(), random.NextDouble());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new ExportableFailureMechanismSection("id", null, random.NextDouble(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_WithGeometry_ExpectedValues()
        {
            // Setup
            const string id = "id";

            var random = new Random(21);
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            double startDistance = random.NextDouble();
            double endDistance = random.NextDouble();

            // Call
            var section = new ExportableFailureMechanismSection(id, geometry, startDistance, endDistance);

            // Assert
            Assert.AreEqual(id, section.Id);
            Assert.AreSame(geometry, section.Geometry);
            Assert.AreEqual(startDistance, section.StartDistance);
            Assert.AreEqual(endDistance, section.EndDistance);
        }
    }
}