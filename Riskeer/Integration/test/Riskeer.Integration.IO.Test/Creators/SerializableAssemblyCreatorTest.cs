﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.ModelOld;
using Riskeer.AssemblyTool.IO.ModelOld.Gml;
using Riskeer.AssemblyTool.IO.ModelOld.Helpers;
using Riskeer.AssemblyTool.IO.TestUtil;
using Riskeer.Integration.IO.Creators;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableAssemblyCreatorTest
    {
        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SerializableAssemblyCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            IEnumerable<ExportableFailureMechanism> failureMechanisms =
                new[]
                {
                    CreateFailureMechanism(),
                    CreateFailureMechanism(),
                    CreateFailureMechanism(),
                    CreateFailureMechanism()
                };

            var exportableAssessmentSection = new ExportableAssessmentSection(assessmentSectionId,
                                                                              assessmentSectionName,
                                                                              geometry,
                                                                              assessmentSectionAssembly,
                                                                              failureMechanisms, new[]
                                                                              {
                                                                                  CreateCombinedSectionAssembly(CreateCombinedFailureMechanismSection()),
                                                                                  CreateCombinedSectionAssembly(CreateCombinedFailureMechanismSection())
                                                                              });

            // Call
            SerializableAssembly serializableAssembly = SerializableAssemblyCreator.Create(exportableAssessmentSection);

            // Assert
            Assert.AreEqual("Assemblage.0", serializableAssembly.Id);
            AssertSerializableBoundary(exportableAssessmentSection.Geometry, serializableAssembly.Boundary);

            SerializableFeatureMember[] serializableAssemblyFeatureMembers = serializableAssembly.FeatureMembers;
            Assert.AreEqual(24, serializableAssemblyFeatureMembers.Length);

            var serializableAssessmentSection = (SerializableAssessmentSection) serializableAssemblyFeatureMembers[0];
            AssertSerializableAssessmentSection($"Wks.{assessmentSectionId}", assessmentSectionName, geometry, serializableAssessmentSection);
            var serializableAssessmentProcess = (SerializableAssessmentProcess) serializableAssemblyFeatureMembers[1];
            AssertSerializableAssessmentProcess("Bp.0", serializableAssessmentSection, serializableAssessmentProcess);
            var serializableTotalAssemblyResult = (SerializableTotalAssemblyResult) serializableAssemblyFeatureMembers[2];
            AssertSerializableTotalAssemblyResult("Vo.0",
                                                  assessmentSectionAssembly,
                                                  serializableAssessmentProcess,
                                                  serializableTotalAssemblyResult);

            AssertFailureMechanismConnections(3, 0, serializableTotalAssemblyResult, serializableAssemblyFeatureMembers);
            AssertFailureMechanismConnections(4, 1, serializableTotalAssemblyResult, serializableAssemblyFeatureMembers);
            AssertFailureMechanismConnections(5, 2, serializableTotalAssemblyResult, serializableAssemblyFeatureMembers);
            AssertFailureMechanismConnections(6, 3, serializableTotalAssemblyResult, serializableAssemblyFeatureMembers);

            var combinedFailureMechanismSectionCollection = (SerializableFailureMechanismSectionCollection) serializableAssemblyFeatureMembers[17];
            AssertSerializableFailureMechanismSectionCollection("Vi.4", combinedFailureMechanismSectionCollection);
            AssertCombinedFailureMechanismSectionAssemblyConnections(11, 4, 0, combinedFailureMechanismSectionCollection, serializableTotalAssemblyResult, serializableAssemblyFeatureMembers);
            AssertCombinedFailureMechanismSectionAssemblyConnections(12, 5, 1, combinedFailureMechanismSectionCollection, serializableTotalAssemblyResult, serializableAssemblyFeatureMembers);
        }

        #region Test Helpers: Factory Methods

        private static IEnumerable<Point2D> CreateGeometry()
        {
            return new[]
            {
                new Point2D(1, 1),
                new Point2D(4, 4),
                new Point2D(5, -1)
            };
        }

        private static ExportableCombinedFailureMechanismSection CreateCombinedFailureMechanismSection()
        {
            var random = new Random(10);
            return new ExportableCombinedFailureMechanismSection(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            }, random.NextDouble(), random.NextDouble(), random.NextEnumValue<ExportableAssemblyMethod>());
        }

        private static ExportableCombinedSectionAssembly CreateCombinedSectionAssembly(ExportableCombinedFailureMechanismSection section)
        {
            return new ExportableCombinedSectionAssembly(
                section,
                ExportableFailureMechanismSectionAssemblyResultTestFactory.Create(section, 21),
                new[]
                {
                    CreateCombinedSectionAssemblyResult(21),
                    CreateCombinedSectionAssemblyResult(22)
                });
        }

        private static ExportableFailureMechanismCombinedSectionAssemblyResult CreateCombinedSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            return new ExportableFailureMechanismCombinedSectionAssemblyResult(
                ExportableFailureMechanismSubSectionAssemblyResultTestFactory.Create(),
                random.NextEnumValue<ExportableFailureMechanismType>(),
                "code",
                "name");
        }

        private static ExportableFailureMechanism CreateFailureMechanism()
        {
            var random = new Random(21);
            ExportableFailureMechanismSection failureMechanismSection = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            return new ExportableFailureMechanism(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResult(),
                new[]
                {
                    ExportableFailureMechanismSectionAssemblyResultTestFactory.CreateWithProbability(failureMechanismSection, random.Next())
                },
                random.NextEnumValue<ExportableFailureMechanismType>(),
                "code",
                "name");
        }

        #endregion

        #region TestHelper: Asserts

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
                                                                  ExportableAssessmentSectionAssemblyResult expectedAssessmentSectionAssemblyResult,
                                                                  SerializableAssessmentProcess expectedAssessmentProcess,
                                                                  SerializableTotalAssemblyResult serializableTotalAssembly)
        {
            Assert.AreEqual(expectedId, serializableTotalAssembly.Id);
            Assert.AreEqual(expectedAssessmentProcess.Id, serializableTotalAssembly.AssessmentProcessId);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedAssessmentSectionAssemblyResult.ProbabilityAssemblyMethod),
                            serializableTotalAssembly.ProbabilityAssemblyMethod);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedAssessmentSectionAssemblyResult.AssemblyGroupAssemblyMethod),
                            serializableTotalAssembly.AssemblyGroupAssemblyMethod);
            Assert.AreEqual(SerializableAssessmentSectionAssemblyGroupCreator.Create(expectedAssessmentSectionAssemblyResult.AssemblyGroup),
                            serializableTotalAssembly.AssemblyGroup);
            Assert.AreEqual(expectedAssessmentSectionAssemblyResult.Probability, serializableTotalAssembly.Probability);
        }

        private static void AssertSerializableFailureMechanismSection(string expectedId,
                                                                      SerializableFailureMechanismSectionCollection expectedSerializableFailureMechanismSectionCollection,
                                                                      SerializableFailureMechanismSection serializableFailureMechanismSection)
        {
            Assert.AreEqual(expectedSerializableFailureMechanismSectionCollection.Id, serializableFailureMechanismSection.FailureMechanismSectionCollectionId);
            Assert.AreEqual(expectedId, serializableFailureMechanismSection.Id);
        }

        #region Failure mechanisms

        private static void AssertFailureMechanismConnections(int indexSerializableFailureMechanism,
                                                              int expectedId,
                                                              SerializableTotalAssemblyResult expectedSerializableTotalAssemblyResult,
                                                              SerializableFeatureMember[] serializableAssemblyFeatureMembers)
        {
            var failureMechanism = (SerializableFailureMechanism) serializableAssemblyFeatureMembers[indexSerializableFailureMechanism];
            AssertSerializableFailureMechanism($"Fm.{expectedId}",
                                               expectedSerializableTotalAssemblyResult,
                                               failureMechanism);
            var failureMechanismSectionCollection = (SerializableFailureMechanismSectionCollection) serializableAssemblyFeatureMembers[indexSerializableFailureMechanism + 10];
            AssertSerializableFailureMechanismSectionCollection($"Vi.{expectedId}",
                                                                failureMechanismSectionCollection);
            var serializableFailureMechanismSection = (SerializableFailureMechanismSection) serializableAssemblyFeatureMembers[indexSerializableFailureMechanism + 15];
            AssertSerializableFailureMechanismSection($"Bv.{expectedId}",
                                                      failureMechanismSectionCollection,
                                                      serializableFailureMechanismSection);
            AssertSerializableFailureMechanismSectionAssembly($"Fa.{expectedId}",
                                                              failureMechanism,
                                                              serializableFailureMechanismSection,
                                                              (SerializableFailureMechanismSectionAssembly) serializableAssemblyFeatureMembers[indexSerializableFailureMechanism + 4]);
        }

        private static void AssertSerializableFailureMechanism(string expectedId,
                                                               SerializableTotalAssemblyResult expectedSerializableTotalAssembly,
                                                               SerializableFailureMechanism serializableFailureMechanism)
        {
            Assert.AreEqual(expectedSerializableTotalAssembly.Id, serializableFailureMechanism.TotalAssemblyResultId);
            Assert.AreEqual(expectedId, serializableFailureMechanism.Id);
        }

        private static void AssertSerializableFailureMechanismSectionCollection(string expectedId,
                                                                                SerializableFailureMechanismSectionCollection serializableFailureMechanismSectionCollection)
        {
            Assert.AreEqual(expectedId, serializableFailureMechanismSectionCollection.Id);
        }

        private static void AssertSerializableFailureMechanismSectionAssembly(string expectedId,
                                                                              SerializableFailureMechanism expectedSerializableFailureMechanism,
                                                                              SerializableFailureMechanismSection expectedSerializableFailureMechanismSection,
                                                                              SerializableFailureMechanismSectionAssembly serializableFailureMechanismSectionAssembly)
        {
            Assert.AreEqual(expectedSerializableFailureMechanism.Id, serializableFailureMechanismSectionAssembly.FailureMechanismId);
            Assert.AreEqual(expectedSerializableFailureMechanismSection.Id, serializableFailureMechanismSectionAssembly.FailureMechanismSectionId);
            Assert.AreEqual(expectedId, serializableFailureMechanismSectionAssembly.Id);
        }

        #endregion

        #region Combined failure mechanism sections

        private static void AssertCombinedFailureMechanismSectionAssemblyConnections(int indexSerializableCombinedSectionAssembly,
                                                                                     int expectedSectionId,
                                                                                     int expectedCombinedSectionAssemblyId,
                                                                                     SerializableFailureMechanismSectionCollection expectedCombinedFailureMechanismSectionCollection,
                                                                                     SerializableTotalAssemblyResult expectedSerializableTotalAssemblyResult,
                                                                                     SerializableFeatureMember[] serializableAssemblyFeatureMembers)
        {
            var combinedFailureMechanismSection = (SerializableFailureMechanismSection) serializableAssemblyFeatureMembers[indexSerializableCombinedSectionAssembly + 11];
            AssertSerializableFailureMechanismSection($"Bv.{expectedSectionId}",
                                                      expectedCombinedFailureMechanismSectionCollection,
                                                      combinedFailureMechanismSection);
            AssertSerializableCombinedFailureMechanismSectionAssembly($"Gf.{expectedCombinedSectionAssemblyId}",
                                                                      expectedSerializableTotalAssemblyResult,
                                                                      combinedFailureMechanismSection,
                                                                      (SerializableCombinedFailureMechanismSectionAssembly) serializableAssemblyFeatureMembers[indexSerializableCombinedSectionAssembly]);
        }

        private static void AssertSerializableCombinedFailureMechanismSectionAssembly(string expectedId,
                                                                                      SerializableTotalAssemblyResult expectedSerializableTotalAssemblyResult,
                                                                                      SerializableFailureMechanismSection expectedSerializableFailureMechanismSection,
                                                                                      SerializableCombinedFailureMechanismSectionAssembly serializableCombinedFailureMechanismSectionAssembly)
        {
            Assert.AreEqual(expectedSerializableTotalAssemblyResult.Id, serializableCombinedFailureMechanismSectionAssembly.TotalAssemblyResultId);
            Assert.AreEqual(expectedSerializableFailureMechanismSection.Id, serializableCombinedFailureMechanismSectionAssembly.FailureMechanismSectionId);
            Assert.AreEqual(expectedId, serializableCombinedFailureMechanismSectionAssembly.Id);
        }

        #endregion

        #endregion
    }
}