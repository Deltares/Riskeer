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
                new[]
                {
                    CreateFailureMechanismWithProbability(),
                    CreateFailureMechanismWithProbability()
                };
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                new[]
                {
                    CreateFailureMechanismWithoutProbability(),
                    CreateFailureMechanismWithoutProbability()
                };
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
            Assert.AreEqual("Assemblage.0", serializableAssembly.Id);
            AssertSerializableBoundary(exportableAssessmentSection.Geometry, serializableAssembly.Boundary);

            SerializableFeatureMember[] serializableAssemblyFeatureMembers = serializableAssembly.FeatureMembers;
            Assert.AreEqual(22, serializableAssemblyFeatureMembers.Length);

            var serializableAssessmentSection = (SerializableAssessmentSection) serializableAssemblyFeatureMembers[0];
            AssertSerializableAssessmentSection($"Wks.{assessmentSectionId}", assessmentSectionName, geometry, serializableAssessmentSection);
            var serializableAssessmentProcess = (SerializableAssessmentProcess) serializableAssemblyFeatureMembers[1];
            AssertSerializableAssessmentProcess("Bp.1", serializableAssessmentSection, serializableAssessmentProcess);
            var serializableTotalAssemblyResult = (SerializableTotalAssemblyResult) serializableAssemblyFeatureMembers[2];
            AssertSerializableTotalAssemblyResult("Vo.2",
                                                  failureMechanismAssemblyResultWithoutProbability,
                                                  failureMechanismAssemblyResultWithProbability,
                                                  assessmentSectionAssembly,
                                                  serializableAssessmentProcess,
                                                  serializableTotalAssemblyResult);

            var failureMechanismWithProbability1 = (SerializableFailureMechanism) serializableAssemblyFeatureMembers[3];
            AssertSerializableFailureMechanism("Ts.3",
                                               serializableTotalAssemblyResult,
                                               failureMechanismWithProbability1);
            var failureMechanismSectionCollection1 = (SerializableFailureMechanismSectionCollection) serializableAssemblyFeatureMembers[11];
            AssertSerializableFailureMechanismSectionCollection("Vi.4",
                                                                failureMechanismWithProbability1,
                                                                failureMechanismSectionCollection1);
            var serializableFailureMechanismSection1 = (SerializableFailureMechanismSection) serializableAssemblyFeatureMembers[16];
            AssertSerializableFailureMechanismSection("Wks.5",
                                                      failureMechanismSectionCollection1,
                                                      serializableFailureMechanismSection1);
            AssertSerializableFailureMechanismSectionAssembly("T.6",
                                                              failureMechanismWithProbability1,
                                                              serializableFailureMechanismSection1,
                                                              (SerializableFailureMechanismSectionAssembly) serializableAssemblyFeatureMembers[7]);

            var failureMechanismWithProbability2 = (SerializableFailureMechanism) serializableAssemblyFeatureMembers[4];
            AssertSerializableFailureMechanism("Ts.7",
                                               serializableTotalAssemblyResult,
                                               failureMechanismWithProbability2);
            var failureMechanismSectionCollection2 = (SerializableFailureMechanismSectionCollection) serializableAssemblyFeatureMembers[12];
            AssertSerializableFailureMechanismSectionCollection("Vi.8",
                                                                failureMechanismWithProbability2,
                                                                failureMechanismSectionCollection2);
            var serializableFailureMechanismSection2 = (SerializableFailureMechanismSection) serializableAssemblyFeatureMembers[17];
            AssertSerializableFailureMechanismSection("Wks.9",
                                                      failureMechanismSectionCollection2,
                                                      serializableFailureMechanismSection2);
            AssertSerializableFailureMechanismSectionAssembly("T.10",
                                                              failureMechanismWithProbability2,
                                                              serializableFailureMechanismSection2,
                                                              (SerializableFailureMechanismSectionAssembly) serializableAssemblyFeatureMembers[8]);

            var failureMechanismWithoutProbability1 = (SerializableFailureMechanism) serializableAssemblyFeatureMembers[5];
            AssertSerializableFailureMechanism("Ts.11",
                                               serializableTotalAssemblyResult,
                                               failureMechanismWithoutProbability1);
            var failureMechanismSectionCollection3 = (SerializableFailureMechanismSectionCollection) serializableAssemblyFeatureMembers[13];
            AssertSerializableFailureMechanismSectionCollection("Vi.12",
                                                                failureMechanismWithoutProbability1,
                                                                failureMechanismSectionCollection3);
            var serializableFailureMechanismSection3 = (SerializableFailureMechanismSection) serializableAssemblyFeatureMembers[18];
            AssertSerializableFailureMechanismSection("Wks.13", failureMechanismSectionCollection3,
                                                      serializableFailureMechanismSection3);
            AssertSerializableFailureMechanismSectionAssembly("T.14",
                                                              failureMechanismWithoutProbability1,
                                                              serializableFailureMechanismSection3,
                                                              (SerializableFailureMechanismSectionAssembly) serializableAssemblyFeatureMembers[9]);

            var failureMechanismWithoutProbability2 = (SerializableFailureMechanism) serializableAssemblyFeatureMembers[6];
            AssertSerializableFailureMechanism("Ts.15",
                                               serializableTotalAssemblyResult,
                                               failureMechanismWithoutProbability2);
            var failureMechanismSectionCollection4 = (SerializableFailureMechanismSectionCollection) serializableAssemblyFeatureMembers[14];
            AssertSerializableFailureMechanismSectionCollection("Vi.16",
                                                                failureMechanismWithoutProbability2,
                                                                failureMechanismSectionCollection4);
            var serializableFailureMechanismSection4 = (SerializableFailureMechanismSection) serializableAssemblyFeatureMembers[19];
            AssertSerializableFailureMechanismSection("Wks.17", failureMechanismSectionCollection4,
                                                      serializableFailureMechanismSection4);
            AssertSerializableFailureMechanismSectionAssembly("T.18",
                                                              failureMechanismWithoutProbability2,
                                                              serializableFailureMechanismSection4,
                                                              (SerializableFailureMechanismSectionAssembly) serializableAssemblyFeatureMembers[10]);

            var combinedFailureMechanismSectionCollection = (SerializableFailureMechanismSectionCollection)serializableAssemblyFeatureMembers[15];
            AssertSerializableFailureMechanismSectionCollection("Vi.19",
                                                                serializableTotalAssemblyResult,
                                                                combinedFailureMechanismSectionCollection);
            var combinedFailureMechanismSection1 = (SerializableFailureMechanismSection)serializableAssemblyFeatureMembers[20];
            AssertSerializableFailureMechanismSection("Wks.20",
                                                      combinedFailureMechanismSectionCollection,
                                                      combinedFailureMechanismSection1);

            var combinedFailureMechanismSection2 = (SerializableFailureMechanismSection)serializableAssemblyFeatureMembers[21];
            AssertSerializableFailureMechanismSection("Wks.22",
                                                      combinedFailureMechanismSectionCollection,
                                                      combinedFailureMechanismSection2);
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
            ExportableCombinedFailureMechanismSection[] sections =
            {
                CreateCombinedFailureMechanismSection(),
                CreateCombinedFailureMechanismSection()
            };
            return new ExportableCombinedSectionAssemblyCollection(sections,
                                                                   new[]
                                                                   {
                                                                       CreateCombinedSectionAssembly(sections[0]),
                                                                       CreateCombinedSectionAssembly(sections[1])
                                                                   });
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

        private static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> CreateFailureMechanismWithProbability()
        {
            var random = new Random(21);
            ExportableFailureMechanismSection failureMechanismSection = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                new[]
                {
                    failureMechanismSection
                },
                new[]
                {
                    CreateSectionResult(failureMechanismSection)
                },
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());
        }

        private static ExportableCombinedSectionAssembly CreateCombinedSectionAssembly(ExportableCombinedFailureMechanismSection section)
        {
            return new ExportableCombinedSectionAssembly(section,
                                                         ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                                                         new[]
                                                         {
                                                             CreateCombinedSectionAssemblyResult(1),
                                                             CreateCombinedSectionAssemblyResult(2)
                                                         });
        }

        private static ExportableFailureMechanismCombinedSectionAssemblyResult CreateCombinedSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            return new ExportableFailureMechanismCombinedSectionAssemblyResult(CreateSectionAssemblyResult(),
                                                                               random.NextEnumValue<ExportableFailureMechanismType>());
        }

        private static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> CreateFailureMechanismWithoutProbability()
        {
            var random = new Random(21);
            ExportableFailureMechanismSection failureMechanismSection = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                new[]
                {
                    failureMechanismSection
                },
                new[]
                {
                    CreateSectionResult(failureMechanismSection)
                },
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());
        }

        private static ExportableAggregatedFailureMechanismSectionAssemblyResult CreateSectionResult(ExportableFailureMechanismSection section)
        {
            return new ExportableAggregatedFailureMechanismSectionAssemblyResult(section,
                                                                                 CreateSectionAssemblyResult(),
                                                                                 CreateSectionAssemblyResult(),
                                                                                 CreateSectionAssemblyResult(),
                                                                                 CreateSectionAssemblyResult());
        }

        private static ExportableSectionAssemblyResult CreateSectionAssemblyResult()
        {
            var random = new Random(30);
            var assemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            if (assemblyCategory == FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                assemblyCategory = FailureMechanismSectionAssemblyCategoryGroup.IIIv;
            }

            return new ExportableSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                       assemblyCategory);
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

        private static void AssertSerializableFailureMechanism(string expectedId,
                                                               SerializableTotalAssemblyResult expectedSerializableTotalAssembly,
                                                               SerializableFailureMechanism serializableFailureMechanism)
        {
            Assert.AreEqual(expectedSerializableTotalAssembly.Id, serializableFailureMechanism.TotalAssemblyResultId);
            Assert.AreEqual(expectedId, serializableFailureMechanism.Id);
        }

        private static void AssertSerializableFailureMechanismSectionCollection(string expectedId,
                                                                                SerializableFailureMechanism expectedSerializableFailureMechanism,
                                                                                SerializableFailureMechanismSectionCollection serializableFailureMechanismSectionCollection)
        {
            Assert.AreEqual(expectedSerializableFailureMechanism.Id, serializableFailureMechanismSectionCollection.FailureMechanismId);
            Assert.IsNull(serializableFailureMechanismSectionCollection.TotalAssemblyResultId);
            Assert.AreEqual(expectedId, serializableFailureMechanismSectionCollection.Id);
        }

        private static void AssertSerializableFailureMechanismSection(string expectedId,
                                                                      SerializableFailureMechanismSectionCollection expectedSerializableFailureMechanismSectionCollection,
                                                                      SerializableFailureMechanismSection serializableFailureMechanismSection)
        {
            Assert.AreEqual(expectedSerializableFailureMechanismSectionCollection.Id, serializableFailureMechanismSection.FailureMechanismSectionCollectionId);
            Assert.AreEqual(expectedId, serializableFailureMechanismSection.Id);
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


        private static void AssertSerializableFailureMechanismSectionCollection(string expectedId,
                                                                                SerializableTotalAssemblyResult expectedSerializableTotalAssemblyResult,
                                                                                SerializableFailureMechanismSectionCollection serializableFailureMechanismSectionCollection)
        {
            Assert.AreEqual(expectedSerializableTotalAssemblyResult.Id, serializableFailureMechanismSectionCollection.TotalAssemblyResultId);
            Assert.IsNull(serializableFailureMechanismSectionCollection.FailureMechanismId);
            Assert.AreEqual(expectedId, serializableFailureMechanismSectionCollection.Id);
        }
    }
}