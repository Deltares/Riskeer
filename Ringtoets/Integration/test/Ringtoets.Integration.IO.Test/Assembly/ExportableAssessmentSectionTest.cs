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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAssessmentSectionTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(null,
                                                                      geometry,
                                                                      CreateAssessmentSectionAssembly(),
                                                                      CreateFailureMechanismAssemblyResultWithProbability(),
                                                                      CreateFailureMechanismAssemblyResultWithoutProbability(),
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability,
                                                                      CreateCombinedSectionAssemblyCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      null,
                                                                      CreateAssessmentSectionAssembly(),
                                                                      CreateFailureMechanismAssemblyResultWithProbability(),
                                                                      CreateFailureMechanismAssemblyResultWithoutProbability(),
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability,
                                                                      CreateCombinedSectionAssemblyCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      null,
                                                                      CreateFailureMechanismAssemblyResultWithProbability(),
                                                                      CreateFailureMechanismAssemblyResultWithoutProbability(),
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability, CreateCombinedSectionAssemblyCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismAssemblyWithProbabilityNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      CreateAssessmentSectionAssembly(),
                                                                      null,
                                                                      CreateFailureMechanismAssemblyResultWithoutProbability(),
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability,
                                                                      CreateCombinedSectionAssemblyCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismAssemblyWithProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismAssemblyWithoutProbabilityNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      CreateAssessmentSectionAssembly(),
                                                                      CreateFailureMechanismAssemblyResultWithProbability(),
                                                                      null,
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability,
                                                                      CreateCombinedSectionAssemblyCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismAssemblyWithoutProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsWithProbabilityNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      CreateAssessmentSectionAssembly(),
                                                                      CreateFailureMechanismAssemblyResultWithProbability(),
                                                                      CreateFailureMechanismAssemblyResultWithoutProbability(),
                                                                      null,
                                                                      failureMechanismsWithoutProbability, CreateCombinedSectionAssemblyCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismsWithProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsWithoutProbabilityNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      CreateAssessmentSectionAssembly(),
                                                                      CreateFailureMechanismAssemblyResultWithProbability(),
                                                                      CreateFailureMechanismAssemblyResultWithoutProbability(),
                                                                      failureMechanismsWithProbability,
                                                                      null,
                                                                      CreateCombinedSectionAssemblyCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismsWithoutProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      CreateAssessmentSectionAssembly(),
                                                                      CreateFailureMechanismAssemblyResultWithProbability(),
                                                                      CreateFailureMechanismAssemblyResultWithoutProbability(),
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability,
                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("Valid name")]
        public void Constructor_WithValidArguments_ExpectedValues(string name)
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = CreateAssessmentSectionAssembly();
            ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyResultWithProbability = CreateFailureMechanismAssemblyResultWithProbability();
            ExportableFailureMechanismAssemblyResult failureMechanismAssemblyResultWithoutProbability = CreateFailureMechanismAssemblyResultWithoutProbability();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();
            ExportableCombinedSectionAssemblyCollection combinedSectionAssemblyResults = CreateCombinedSectionAssemblyCollection();

            // Call
            var assessmentSection = new ExportableAssessmentSection(name,
                                                                    geometry,
                                                                    assessmentSectionAssembly,
                                                                    failureMechanismAssemblyResultWithProbability,
                                                                    failureMechanismAssemblyResultWithoutProbability,
                                                                    failureMechanismsWithProbability,
                                                                    failureMechanismsWithoutProbability, combinedSectionAssemblyResults);

            // Assert
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreSame(geometry, assessmentSection.Geometry);
            Assert.AreSame(assessmentSectionAssembly, assessmentSection.AssessmentSectionAssembly);
            Assert.AreSame(failureMechanismAssemblyResultWithProbability, assessmentSection.FailureMechanismAssemblyWithProbability);
            Assert.AreSame(failureMechanismAssemblyResultWithoutProbability, assessmentSection.FailureMechanismAssemblyWithoutProbability);
            Assert.AreSame(failureMechanismsWithProbability, assessmentSection.FailureMechanismsWithProbability);
            Assert.AreSame(failureMechanismsWithoutProbability, assessmentSection.FailureMechanismsWithoutProbability);
            Assert.AreSame(combinedSectionAssemblyResults, assessmentSection.CombinedSectionAssemblyResults);
        }

        private static ExportableFailureMechanismAssemblyResult CreateFailureMechanismAssemblyResultWithoutProbability()
        {
            var random = new Random(21);
            return new ExportableFailureMechanismAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
        }

        private static ExportableFailureMechanismAssemblyResultWithProbability CreateFailureMechanismAssemblyResultWithProbability()
        {
            var random = new Random(21);
            return new ExportableFailureMechanismAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                               random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>(),
                                                                               random.NextDouble());
        }

        private static ExportableCombinedSectionAssemblyCollection CreateCombinedSectionAssemblyCollection()
        {
            return new ExportableCombinedSectionAssemblyCollection(Enumerable.Empty<ExportableCombinedFailureMechanismSection>(),
                                                                   Enumerable.Empty<ExportableCombinedSectionAssembly>());
        }

        private static ExportableAssessmentSectionAssemblyResult CreateAssessmentSectionAssembly()
        {
            var random = new Random(21);
            return new ExportableAssessmentSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                 random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
        }
    }
}