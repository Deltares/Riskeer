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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.Exceptions;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismSectionAssemblyResultCreatorTest
    {
        [Test]
        public void CreateWithExportableSectionAssemblyResult_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentType = new Random(21).NextEnumValue<SerializableAssessmentType>();

            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionAssemblyResultCreator.Create(assessmentType, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void Create_WithExportableSectionAssemblyResultAndResultNone_ReturnsSerializableFailureMechanismAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var assessmentType = random.NextEnumValue<SerializableAssessmentType>();
            var sectionResult = new ExportableSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                    FailureMechanismSectionAssemblyCategoryGroup.None);

            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionAssemblyResultCreator.Create(assessmentType, sectionResult);

            // Assert
            var exception = Assert.Throws<AssemblyCreatorException>(call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        [Test]
        public void Create_WithExportableSectionAssemblyResult_ReturnsSerializableFailureMechanismAssemblyResult()
        {
            // Setup
            var assessmentType = new Random(21).NextEnumValue<SerializableAssessmentType>();
            ExportableSectionAssemblyResult sectionResult = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();

            // Call
            SerializableFailureMechanismSectionAssemblyResult serializableResult =
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(assessmentType, sectionResult);

            // Assert
            Assert.AreEqual(assessmentType, serializableResult.AssessmentType);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(sectionResult.AssemblyMethod), serializableResult.AssemblyMethod);
            Assert.AreEqual(SerializableFailureMechanismSectionCategoryGroupCreator.Create(sectionResult.AssemblyCategory), serializableResult.CategoryGroup);
            Assert.IsNull(serializableResult.Probability);
        }

        [Test]
        public void CreateWithExportableSectionAssemblyResultWithProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentType = new Random(21).NextEnumValue<SerializableAssessmentType>();

            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionAssemblyResultCreator.Create(assessmentType, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void Create_WithExportableSectionAssemblyResultWithProbabilityAndResultNone_ReturnsSerializableFailureMechanismAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var assessmentType = random.NextEnumValue<SerializableAssessmentType>();
            var sectionResult = new ExportableSectionAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                                   FailureMechanismSectionAssemblyCategoryGroup.None,
                                                                                   random.NextDouble());

            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionAssemblyResultCreator.Create(assessmentType, sectionResult);

            // Assert
            var exception = Assert.Throws<AssemblyCreatorException>(call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        [Test]
        public void Create_WithExportableSectionAssemblyResultWithProbability_ReturnsSerializableFailureMechanismAssemblyResult()
        {
            // Setup
            var assessmentType = new Random(21).NextEnumValue<SerializableAssessmentType>();
            ExportableSectionAssemblyResultWithProbability sectionResult = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();

            // Call
            SerializableFailureMechanismSectionAssemblyResult serializableResult =
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(assessmentType, sectionResult);

            // Assert
            Assert.AreEqual(assessmentType, serializableResult.AssessmentType);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(sectionResult.AssemblyMethod), serializableResult.AssemblyMethod);
            Assert.AreEqual(SerializableFailureMechanismSectionCategoryGroupCreator.Create(sectionResult.AssemblyCategory), serializableResult.CategoryGroup);
            Assert.AreEqual(sectionResult.Probability, serializableResult.Probability);
        }
    }
}