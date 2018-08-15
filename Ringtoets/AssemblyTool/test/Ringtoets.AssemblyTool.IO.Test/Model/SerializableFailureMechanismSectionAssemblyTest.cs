﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableFailureMechanismSectionAssemblyTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var sectionAssembly = new SerializableFailureMechanismSectionAssembly();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(sectionAssembly);
            Assert.IsNull(sectionAssembly.Id);
            Assert.IsNull(sectionAssembly.FailureMechanismId);
            Assert.IsNull(sectionAssembly.CombinedSectionResult);
            Assert.IsNull(sectionAssembly.SectionResults);

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.Id), "ToetsID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.FailureMechanismId), "ToetsspoorIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.CombinedSectionResult), "eindtoetsoordeel");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.SectionResults), "toetsoordeelVak");
        }

        [Test]
        public void Constructor_IdNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly(null,
                                                                                      new SerializableFailureMechanism(),
                                                                                      new SerializableFailureMechanismSectionAssemblyResult[0],
                                                                                      new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("id", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly("id",
                                                                                      null,
                                                                                      new SerializableFailureMechanismSectionAssemblyResult[0],
                                                                                      new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly("id",
                                                                                      new SerializableFailureMechanism(),
                                                                                      null,
                                                                                      new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly("id",
                                                                                      new SerializableFailureMechanism(),
                                                                                      new SerializableFailureMechanismSectionAssemblyResult[0],
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

            var failureMechanism = new SerializableFailureMechanism();
            var sectionResults = new SerializableFailureMechanismSectionAssemblyResult[0];
            var combinedSectionResult = new SerializableFailureMechanismSectionAssemblyResult();

            // Call
            var sectionAssembly = new SerializableFailureMechanismSectionAssembly(id,
                                                                                  failureMechanism,
                                                                                  sectionResults,
                                                                                  combinedSectionResult);

            // Assert
            Assert.AreEqual(id, sectionAssembly.Id);
            Assert.AreEqual(failureMechanism.Id, sectionAssembly.FailureMechanismId);
            Assert.AreSame(sectionResults, sectionAssembly.SectionResults);
            Assert.AreSame(combinedSectionResult, sectionAssembly.CombinedSectionResult);
        }
    }
}