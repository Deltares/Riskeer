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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Creators;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableAssessmentSectionAssemblyResultCreatorTest
    {
        [Test]
        public void Create_AssessmentSectionAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableAssessmentSectionAssemblyResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_WithAssessmentSectionAssemblyResult_ReturnsSerializableAssessmentSectionAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var result = new ExportableAssessmentSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                       random.NextEnumValue(new[]
                                                                       {
                                                                           AssessmentSectionAssemblyCategoryGroup.NotAssessed,
                                                                           AssessmentSectionAssemblyCategoryGroup.A,
                                                                           AssessmentSectionAssemblyCategoryGroup.B,
                                                                           AssessmentSectionAssemblyCategoryGroup.C,
                                                                           AssessmentSectionAssemblyCategoryGroup.D
                                                                       }));

            // Call
            SerializableAssessmentSectionAssemblyResult serializableResult =
                SerializableAssessmentSectionAssemblyResultCreator.Create(result);

            // Assert
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(result.AssemblyMethod),
                            serializableResult.AssemblyMethod);
            Assert.AreEqual(SerializableAssessmentSectionCategoryGroupCreator.Create(result.AssemblyCategory),
                            serializableResult.CategoryGroup);
        }
    }
}