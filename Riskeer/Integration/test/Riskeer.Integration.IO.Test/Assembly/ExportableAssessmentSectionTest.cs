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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAssessmentSectionTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAssessmentSection(null,
                                                                      string.Empty,
                                                                      Enumerable.Empty<Point2D>(),
                                                                      ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_InvalidId_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      null,
                                                                      Enumerable.Empty<Point2D>(),
                                                                      ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("id", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      string.Empty,
                                                                      null,
                                                                      ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      string.Empty,
                                                                      Enumerable.Empty<Point2D>(),
                                                                      null,
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismAssemblyWithProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      string.Empty,
                                                                      Enumerable.Empty<Point2D>(),
                                                                      ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                                      null,
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismAssemblyWithProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismAssemblyWithoutProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      string.Empty,
                                                                      Enumerable.Empty<Point2D>(),
                                                                      ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                      null,
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismAssemblyWithoutProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsWithProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      string.Empty,
                                                                      Enumerable.Empty<Point2D>(),
                                                                      ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                      null,
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismsWithProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsWithoutProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      string.Empty,
                                                                      Enumerable.Empty<Point2D>(),
                                                                      ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                                      null,
                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismsWithoutProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      string.Empty,
                                                                      Enumerable.Empty<Point2D>(),
                                                                      ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                      ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                                      Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyResultWithProbability =
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability();
            ExportableFailureMechanismAssemblyResult failureMechanismAssemblyResultWithoutProbability =
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            var assessmentSection = new ExportableAssessmentSection(name,
                                                                    id,
                                                                    geometry,
                                                                    assessmentSectionAssembly,
                                                                    failureMechanismAssemblyResultWithProbability,
                                                                    failureMechanismAssemblyResultWithoutProbability,
                                                                    failureMechanismsWithProbability,
                                                                    failureMechanismsWithoutProbability,
                                                                    combinedSectionAssemblyResults);

            // Assert
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreEqual(id, assessmentSection.Id);
            Assert.AreSame(geometry, assessmentSection.Geometry);
            Assert.AreSame(assessmentSectionAssembly, assessmentSection.AssessmentSectionAssembly);
            Assert.AreSame(failureMechanismAssemblyResultWithProbability, assessmentSection.FailureMechanismAssemblyWithProbability);
            Assert.AreSame(failureMechanismAssemblyResultWithoutProbability, assessmentSection.FailureMechanismAssemblyWithoutProbability);
            Assert.AreSame(failureMechanismsWithProbability, assessmentSection.FailureMechanismsWithProbability);
            Assert.AreSame(failureMechanismsWithoutProbability, assessmentSection.FailureMechanismsWithoutProbability);
            Assert.AreSame(combinedSectionAssemblyResults, assessmentSection.CombinedSectionAssemblies);
        }
    }
}