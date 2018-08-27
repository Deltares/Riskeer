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
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismResultCreatorTest
    {
        [Test]
        public void CreateWithFailureMechanismAssemblyResult_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_WithFailureMechanismAssemblyResult_ReturnsSerializableFailureMechanismAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var result = new ExportableFailureMechanismAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                      random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            // Call
            SerializableFailureMechanismAssemblyResult serializableAssemblyResult = SerializableFailureMechanismResultCreator.Create(result);

            // Assert
            Assert.AreEqual(SerializableFailureMechanismCategoryGroupCreator.Create(result.AssemblyCategory), serializableAssemblyResult.CategoryGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(result.AssemblyMethod), serializableAssemblyResult.AssemblyMethod);
            Assert.IsNull(serializableAssemblyResult.Probability);
        }

        [Test]
        public void CreateWithFailureMechanismAssemblyResultWithProbability_ResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var result = (ExportableFailureMechanismAssemblyResultWithProbability) null;

            // Call
            TestDelegate call = () => SerializableFailureMechanismResultCreator.Create(result);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_WithFailureMechanismAssemblyResultWithProbability_ReturnsSerializableFailureMechanismAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var result = new ExportableFailureMechanismAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                                     random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>(),
                                                                                     random.NextDouble());

            // Call
            SerializableFailureMechanismAssemblyResult serializableAssemblyResult = SerializableFailureMechanismResultCreator.Create(result);

            // Assert
            Assert.AreEqual(SerializableFailureMechanismCategoryGroupCreator.Create(result.AssemblyCategory), serializableAssemblyResult.CategoryGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(result.AssemblyMethod), serializableAssemblyResult.AssemblyMethod);
            Assert.AreEqual(result.Probability, serializableAssemblyResult.Probability);
        }
    }
}