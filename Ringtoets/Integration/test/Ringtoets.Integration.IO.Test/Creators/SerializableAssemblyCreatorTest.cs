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
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Gml;
using Ringtoets.AssemblyTool.IO.Model.Helpers;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableAssemblyCreatorTest
    {
        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableAssemblyCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateSerializableAssembly_WithValidArguments_ReturnsSerializableAssembly()
        {
            // Setup
            const string assessmentSectionName = "assessmentSectionName";
            const string assessmentSectionId = "assessmentSectionId";

            IEnumerable<Point2D> geometry = CreateGeometry();
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly =
                ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult();
            ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyResultWithProbability =
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability();
            ExportableFailureMechanismAssemblyResult failureMechanismAssemblyResultWithoutProbability =
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();
            ExportableCombinedSectionAssemblyCollection combinedSectionAssemblyResults = CreateCombinedSectionAssemblyCollection();

            var exportableAssessmentSection = new ExportableAssessmentSection(assessmentSectionName,
                                                                              assessmentSectionId,
                                                                              geometry,
                                                                              assessmentSectionAssembly,
                                                                              failureMechanismAssemblyResultWithProbability,
                                                                              failureMechanismAssemblyResultWithoutProbability,
                                                                              failureMechanismsWithProbability,
                                                                              failureMechanismsWithoutProbability,
                                                                              combinedSectionAssemblyResults);

            // Call
            SerializableAssembly serializableAssembly = SerializableAssemblyCreator.Create(exportableAssessmentSection);

            // Assert
            Assert.AreEqual("0", serializableAssembly.Id);
            AssertSerializableBoundary(exportableAssessmentSection.Geometry, serializableAssembly.Boundary);
            Assert.AreEqual(3, serializableAssembly.FeatureMembers.Length);

            var serializableAssessmentSection = (SerializableAssessmentSection) serializableAssembly.FeatureMembers[0];
            AssertSerializableAssessmentSection($"Wks.{assessmentSectionId}", assessmentSectionName, geometry, serializableAssessmentSection);
            var serializableAssessmentProcess = (SerializableAssessmentProcess) serializableAssembly.FeatureMembers[1];
            AssertSerializableAssessmentProcess("2", serializableAssessmentSection, serializableAssessmentProcess);
            AssertSerializableTotalAssemblyResult("3",
                                                  failureMechanismAssemblyResultWithoutProbability,
                                                  failureMechanismAssemblyResultWithProbability,
                                                  assessmentSectionAssembly,
                                                  serializableAssessmentProcess,
                                                  (SerializableTotalAssemblyResult) serializableAssembly.FeatureMembers[2]);
        }

        private static IEnumerable<Point2D> CreateGeometry()
        {
            return new[]
            {
                new Point2D(1, 1),
                new Point2D(4, 4),
                new Point2D(5, -1)
            };
        }

        private static ExportableCombinedSectionAssemblyCollection CreateCombinedSectionAssemblyCollection()
        {
            return new ExportableCombinedSectionAssemblyCollection(Enumerable.Empty<ExportableCombinedFailureMechanismSection>(),
                                                                   Enumerable.Empty<ExportableCombinedSectionAssembly>());
        }

        private static void AssertSerializableBoundary(IEnumerable<Point2D> geometry,
                                                       SerializableBoundary actualBoundary)
        {
            var expectedLowerCorner = new Point2D(geometry.Select(p => p.X).Min(),
                                                  geometry.Select(p => p.Y).Min());

            var expectedUpperCorner = new Point2D(geometry.Select(p => p.X).Max(),
                                                  geometry.Select(p => p.Y).Max());

            string expectedLowerCornerFormat = GeometrySerializationFormatter.Format(expectedLowerCorner);
            string expectedUpperCornerFormat = GeometrySerializationFormatter.Format(expectedUpperCorner);

            SerializableEnvelope envelope = actualBoundary.Envelope;
            Assert.AreEqual(expectedLowerCornerFormat, envelope.LowerCorner);
            Assert.AreEqual(expectedUpperCornerFormat, envelope.UpperCorner);
        }

        private static void AssertSerializableAssessmentSection(string expectedId,
                                                                string expectedAssessmentSectionName,
                                                                IEnumerable<Point2D> expectedGeometry,
                                                                SerializableAssessmentSection actualAssessmentSection)
        {
            Assert.AreEqual(expectedId, actualAssessmentSection.Id);
            Assert.AreEqual(expectedAssessmentSectionName, actualAssessmentSection.Name);
            Assert.AreEqual(GeometrySerializationFormatter.Format(expectedGeometry), actualAssessmentSection.ReferenceLineGeometry.LineString.Geometry);
        }

        private static void AssertSerializableAssessmentProcess(string expectedId,
                                                                SerializableAssessmentSection expectedAssessmentSection,
                                                                SerializableAssessmentProcess serializableAssessmentProcess)
        {
            Assert.AreEqual(expectedId, serializableAssessmentProcess.Id);
            Assert.AreEqual(expectedAssessmentSection.Id, serializableAssessmentProcess.AssessmentSectionId);
        }

        private static void AssertSerializableTotalAssemblyResult(string expectedId,
                                                                  ExportableFailureMechanismAssemblyResult expectedFailureMechanismAssemblyResultWithoutProbability,
                                                                  ExportableFailureMechanismAssemblyResultWithProbability expectedFailureMechanismAssemblyResultWithProbability,
                                                                  ExportableAssessmentSectionAssemblyResult expectedAssessmentSectionAssemblyResult,
                                                                  SerializableAssessmentProcess expectedAssessmentProcess,
                                                                  SerializableTotalAssemblyResult serializableTotalAssembly)
        {
            Assert.AreEqual(expectedId, serializableTotalAssembly.Id);
            Assert.AreEqual(expectedAssessmentProcess.Id, serializableTotalAssembly.AssessmentProcessId);

            SerializableFailureMechanismAssemblyResult serializableAssemblyResultWithoutProbability = serializableTotalAssembly.AssemblyResultWithoutProbability;
            Assert.AreEqual(SerializableFailureMechanismCategoryGroupCreator.Create(expectedFailureMechanismAssemblyResultWithoutProbability.AssemblyCategory),
                            serializableAssemblyResultWithoutProbability.CategoryGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedFailureMechanismAssemblyResultWithoutProbability.AssemblyMethod),
                            serializableAssemblyResultWithoutProbability.AssemblyMethod);
            Assert.IsNull(serializableAssemblyResultWithoutProbability.Probability);

            SerializableFailureMechanismAssemblyResult serializableAssemblyResultWithProbability = serializableTotalAssembly.AssemblyResultWithProbability;
            Assert.AreEqual(SerializableFailureMechanismCategoryGroupCreator.Create(expectedFailureMechanismAssemblyResultWithProbability.AssemblyCategory),
                            serializableAssemblyResultWithProbability.CategoryGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedFailureMechanismAssemblyResultWithProbability.AssemblyMethod),
                            serializableAssemblyResultWithProbability.AssemblyMethod);
            Assert.AreEqual(expectedFailureMechanismAssemblyResultWithProbability.Probability,
                            serializableAssemblyResultWithProbability.Probability);

            SerializableAssessmentSectionAssemblyResult serializableAssessmentSectionAssemblyResult = serializableTotalAssembly.AssessmentSectionAssemblyResult;
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedAssessmentSectionAssemblyResult.AssemblyMethod),
                            serializableAssessmentSectionAssemblyResult.AssemblyMethod);
            Assert.AreEqual(SerializableAssessmentSectionCategoryGroupCreator.Create(expectedAssessmentSectionAssemblyResult.AssemblyCategory),
                            serializableAssessmentSectionAssemblyResult.CategoryGroup);
        }
    }
}