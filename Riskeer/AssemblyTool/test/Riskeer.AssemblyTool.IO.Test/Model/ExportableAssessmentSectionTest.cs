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
    public class ExportableAssessmentSectionTest
    {
        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Call
            void Call() => new ExportableAssessmentSection(invalidId, string.Empty, Enumerable.Empty<Point2D>(),
                                                           Enumerable.Empty<ExportableFailureMechanismSectionCollection>(),
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           Enumerable.Empty<ExportableFailureMechanism>());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection("id", null, Enumerable.Empty<Point2D>(),
                                                           Enumerable.Empty<ExportableFailureMechanismSectionCollection>(),
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           Enumerable.Empty<ExportableFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection("id", string.Empty,
                                                           null, Enumerable.Empty<ExportableFailureMechanismSectionCollection>(),
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           Enumerable.Empty<ExportableFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionCollectionsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection("id", string.Empty,
                                                           Enumerable.Empty<Point2D>(), null,
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           Enumerable.Empty<ExportableFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionCollections", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection("id", string.Empty, Enumerable.Empty<Point2D>(),
                                                           Enumerable.Empty<ExportableFailureMechanismSectionCollection>(),
                                                           null, Enumerable.Empty<ExportableFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSectionAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection("id", string.Empty, Enumerable.Empty<Point2D>(),
                                                           Enumerable.Empty<ExportableFailureMechanismSectionCollection>(),
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        [TestCase("id", "")]
        [TestCase("id1", "   ")]
        [TestCase("Valid_id", "Valid name")]
        public void Constructor_ExpectedValues(string id, string name)
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            IEnumerable<ExportableFailureMechanismSectionCollection> failureMechanismSectionCollections =
                Enumerable.Empty<ExportableFailureMechanismSectionCollection>();
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult();
            IEnumerable<ExportableFailureMechanism> failureMechanisms = Enumerable.Empty<ExportableFailureMechanism>();

            // Call
            var assessmentSection = new ExportableAssessmentSection(
                id, name, geometry, failureMechanismSectionCollections, assessmentSectionAssembly, failureMechanisms);

            // Assert
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreEqual(id, assessmentSection.Id);
            Assert.AreSame(geometry, assessmentSection.Geometry);
            Assert.AreSame(failureMechanismSectionCollections, assessmentSection.FailureMechanismSectionCollections);
            Assert.AreSame(assessmentSectionAssembly, assessmentSection.AssessmentSectionAssembly);
            Assert.AreSame(failureMechanisms, assessmentSection.FailureMechanisms);
        }
    }
}