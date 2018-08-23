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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableCombinedFailureMechanismSectionAssemblyTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var combinedSectionAssembly = new SerializableCombinedFailureMechanismSectionAssembly();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(combinedSectionAssembly);
            Assert.IsNull(combinedSectionAssembly.Id);
            Assert.IsNull(combinedSectionAssembly.TotalAssemblyResultId);
            Assert.IsNull(combinedSectionAssembly.FailureMechanismSectionId);
            Assert.IsNull(combinedSectionAssembly.CombinedSectionResult);
            Assert.IsNull(combinedSectionAssembly.FailureMechanismResults);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableCombinedFailureMechanismSectionAssembly), "GecombineerdToetsoordeel");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.Id), "GecombineerdToetsoordeelID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.TotalAssemblyResultId), "VeiligheidsoordeelIDRef");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.FailureMechanismSectionId), "WaterkeringsectieIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.CombinedSectionResult), "toetsoordeelGecombineerd");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.FailureMechanismResults), "eindtoetsoordeelToetsspoor");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_IdInvalid_ThrowsArgumentException(string id)
        {
            // Call
            TestDelegate call = () => new SerializableCombinedFailureMechanismSectionAssembly(id,
                                                                                              new SerializableTotalAssemblyResult(),
                                                                                              new SerializableFailureMechanismSection(),
                                                                                              new SerializableCombinedFailureMechanismSectionAssemblyResult[0],
                                                                                              new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            const string expectedMessage = "'id' must have a value.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableCombinedFailureMechanismSectionAssembly("id",
                                                                                              null,
                                                                                              new SerializableFailureMechanismSection(),
                                                                                              new SerializableCombinedFailureMechanismSectionAssemblyResult[0],
                                                                                              new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("totalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableCombinedFailureMechanismSectionAssembly("id",
                                                                                              new SerializableTotalAssemblyResult(),
                                                                                              null,
                                                                                              new SerializableCombinedFailureMechanismSectionAssemblyResult[0],
                                                                                              new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableCombinedFailureMechanismSectionAssembly("id",
                                                                                              new SerializableTotalAssemblyResult(),
                                                                                              new SerializableFailureMechanismSection(),
                                                                                              null,
                                                                                              new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismResults", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableCombinedFailureMechanismSectionAssembly("id",
                                                                                              new SerializableTotalAssemblyResult(),
                                                                                              new SerializableFailureMechanismSection(),
                                                                                              new SerializableCombinedFailureMechanismSectionAssemblyResult[0],
                                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionResult", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "id";

            var random = new Random(39);
            var totalAssembly = new SerializableTotalAssemblyResult("total assembly ID",
                                                                    new SerializableAssessmentProcess(),
                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                    new SerializableAssessmentSectionAssemblyResult());
            var section = new SerializableFailureMechanismSection("section ID",
                                                                  new SerializableFailureMechanismSectionCollection(),
                                                                  random.NextDouble(),
                                                                  random.NextDouble(),
                                                                  new[]
                                                                  {
                                                                      new Point2D(random.NextDouble(), random.NextDouble())
                                                                  },
                                                                  SerializableFailureMechanismSectionType.Combined);
            var sectionResults = new SerializableCombinedFailureMechanismSectionAssemblyResult[0];
            var combinedSectionResult = new SerializableFailureMechanismSectionAssemblyResult();

            // Call
            var combinedSectionAssembly = new SerializableCombinedFailureMechanismSectionAssembly(id,
                                                                                                  totalAssembly,
                                                                                                  section,
                                                                                                  sectionResults,
                                                                                                  combinedSectionResult);

            // Assert
            Assert.AreEqual(id, combinedSectionAssembly.Id);
            Assert.AreEqual(totalAssembly.Id, combinedSectionAssembly.TotalAssemblyResultId);
            Assert.AreEqual(section.Id, combinedSectionAssembly.FailureMechanismSectionId);
            Assert.AreSame(sectionResults, combinedSectionAssembly.FailureMechanismResults);
            Assert.AreSame(combinedSectionResult, combinedSectionAssembly.CombinedSectionResult);
        }
    }
}