// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
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

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableCombinedFailureMechanismSectionAssembly), "FaalanalyseGecombineerd");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.Id), "FaalanalyseGecombineerdID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.TotalAssemblyResultId), "VeiligheidsoordeelIDRef");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.FailureMechanismSectionId), "WaterkeringsectieIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.CombinedSectionResult), "analyseGecombineerdDeelvak");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableCombinedFailureMechanismSectionAssembly>(
                nameof(SerializableCombinedFailureMechanismSectionAssembly.FailureMechanismResults), "analyseDeelvak");
        }

        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Call
            void Call() => new SerializableCombinedFailureMechanismSectionAssembly(invalidId,
                                                                                   new SerializableTotalAssemblyResult(),
                                                                                   new SerializableFailureMechanismSection(),
                                                                                   Array.Empty<SerializableCombinedFailureMechanismSectionAssemblyResult>(),
                                                                                   new SerializableFailureMechanismSubSectionAssemblyResult());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SerializableCombinedFailureMechanismSectionAssembly("id",
                                                                                   null,
                                                                                   new SerializableFailureMechanismSection(),
                                                                                   Array.Empty<SerializableCombinedFailureMechanismSectionAssemblyResult>(),
                                                                                   new SerializableFailureMechanismSubSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("totalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SerializableCombinedFailureMechanismSectionAssembly("id",
                                                                                   new SerializableTotalAssemblyResult(),
                                                                                   null,
                                                                                   Array.Empty<SerializableCombinedFailureMechanismSectionAssemblyResult>(),
                                                                                   new SerializableFailureMechanismSubSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SerializableCombinedFailureMechanismSectionAssembly("id",
                                                                                   new SerializableTotalAssemblyResult(),
                                                                                   new SerializableFailureMechanismSection(),
                                                                                   null,
                                                                                   new SerializableFailureMechanismSubSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismResults", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SerializableCombinedFailureMechanismSectionAssembly("id",
                                                                                   new SerializableTotalAssemblyResult(),
                                                                                   new SerializableFailureMechanismSection(),
                                                                                   Array.Empty<SerializableCombinedFailureMechanismSectionAssemblyResult>(),
                                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("combinedSectionResult", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "id";

            var random = new Random(39);
            var totalAssembly = new SerializableTotalAssemblyResult("totalAssemblyID",
                                                                    new SerializableAssessmentProcess(),
                                                                    random.NextEnumValue<SerializableAssemblyMethod>(),
                                                                    random.NextEnumValue<SerializableAssemblyMethod>(),
                                                                    random.NextEnumValue<SerializableAssessmentSectionAssemblyGroup>(),
                                                                    random.NextDouble());
            var section = new SerializableFailureMechanismSection("sectionID",
                                                                  new SerializableFailureMechanismSectionCollection(),
                                                                  random.NextDouble(),
                                                                  random.NextDouble(),
                                                                  new[]
                                                                  {
                                                                      new Point2D(random.NextDouble(), random.NextDouble())
                                                                  },
                                                                  SerializableFailureMechanismSectionType.Combined);
            SerializableCombinedFailureMechanismSectionAssemblyResult[] sectionResults = Array.Empty<SerializableCombinedFailureMechanismSectionAssemblyResult>();
            var combinedSectionResult = new SerializableFailureMechanismSubSectionAssemblyResult();

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