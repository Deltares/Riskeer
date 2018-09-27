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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.Exceptions;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableCombinedFailureMechanismSectionAssemblyResultCreatorTest
    {
        [Test]
        public void Create_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableCombinedFailureMechanismSectionAssemblyResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void Create_WithExportableFailureMechanismCombinedSectionAssemblyResult_ReturnsSerializableCombinedFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var sectionResult = new ExportableFailureMechanismCombinedSectionAssemblyResult(CreateSectionAssemblyResult(),
                                                                                            random.NextEnumValue<ExportableFailureMechanismType>());

            // Call
            SerializableCombinedFailureMechanismSectionAssemblyResult serializableResult =
                SerializableCombinedFailureMechanismSectionAssemblyResultCreator.Create(sectionResult);

            // Assert
            Assert.AreEqual(SerializableFailureMechanismTypeCreator.Create(sectionResult.Code),
                            serializableResult.FailureMechanismType);
            ExportableSectionAssemblyResult expectedSectionAssemblyResult = sectionResult.SectionAssemblyResult;
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedSectionAssemblyResult.AssemblyMethod),
                            serializableResult.AssemblyMethod);
            Assert.AreEqual(SerializableFailureMechanismSectionCategoryGroupCreator.Create(expectedSectionAssemblyResult.AssemblyCategory),
                            serializableResult.CategoryGroup);
        }

        [Test]
        public void Create_WithExportableFailureMechanismCombinedSectionAssemblyResultAndResultNone_ThrowsAssemblyCreatorException()
        {
            // Setup
            var random = new Random(21);
            var sectionResult = new ExportableFailureMechanismCombinedSectionAssemblyResult(new ExportableSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                                                                                FailureMechanismSectionAssemblyCategoryGroup.None),
                                                                                            random.NextEnumValue<ExportableFailureMechanismType>());

            // Call
            TestDelegate call = () => SerializableCombinedFailureMechanismSectionAssemblyResultCreator.Create(sectionResult);

            // Assert
            var exception = Assert.Throws<AssemblyCreatorException>(call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        private static ExportableSectionAssemblyResult CreateSectionAssemblyResult()
        {
            var random = new Random(21);
            return new ExportableSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                       random.NextEnumValue(new[]
                                                       {
                                                           FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                                                           FailureMechanismSectionAssemblyCategoryGroup.Iv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.IIv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.IIIv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.IVv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.Vv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.VIv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.VIIv
                                                       }));
        }
    }
}