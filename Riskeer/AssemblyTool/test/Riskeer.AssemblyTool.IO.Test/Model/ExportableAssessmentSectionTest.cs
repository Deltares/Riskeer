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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class ExportableAssessmentSectionTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection(null, string.Empty, Enumerable.Empty<Point2D>(),
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           Enumerable.Empty<ExportableFailureMechanism>(),
                                                           Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_IdNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection(string.Empty, null, Enumerable.Empty<Point2D>(),
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           Enumerable.Empty<ExportableFailureMechanism>(),
                                                           Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("id", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection(string.Empty, string.Empty, null,
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           Enumerable.Empty<ExportableFailureMechanism>(),
                                                           Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection(string.Empty, string.Empty, Enumerable.Empty<Point2D>(), null,
                                                           Enumerable.Empty<ExportableFailureMechanism>(),
                                                           Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSectionAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection(string.Empty, string.Empty, Enumerable.Empty<Point2D>(),
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           null,
                                                           Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableAssessmentSection(string.Empty, string.Empty, Enumerable.Empty<Point2D>(),
                                                           ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                           Enumerable.Empty<ExportableFailureMechanism>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("combinedSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        [TestCase("", "")]
        [TestCase("   ", "   ")]
        [TestCase("Valid name", "Valid Id")]
        public void Constructor_WithValidArguments_ExpectedValues(string name, string id)
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult();
            IEnumerable<ExportableFailureMechanism> failureMechanisms = Enumerable.Empty<ExportableFailureMechanism>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            var assessmentSection = new ExportableAssessmentSection(
                name, id, geometry, assessmentSectionAssembly, failureMechanisms, combinedSectionAssemblyResults);

            // Assert
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreEqual(id, assessmentSection.Id);
            Assert.AreSame(geometry, assessmentSection.Geometry);
            Assert.AreSame(assessmentSectionAssembly, assessmentSection.AssessmentSectionAssembly);
            Assert.AreSame(failureMechanisms, assessmentSection.FailureMechanisms);
            Assert.AreSame(combinedSectionAssemblyResults, assessmentSection.CombinedSectionAssemblies);
        }
    }
}