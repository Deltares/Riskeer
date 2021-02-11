﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Creators;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Creators
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
        public void Create_WithExportableFailureMechanismAssemblyResultAndResultNone_ThrowsAssemblyCreatorException()
        {
            // Setup
            var random = new Random(21);
            var result = new ExportableFailureMechanismAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                      FailureMechanismAssemblyCategoryGroup.None);

            // Call
            TestDelegate call = () => SerializableFailureMechanismResultCreator.Create(result);

            // Assert
            var exception = Assert.Throws<AssemblyCreatorException>(call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        [Test]
        public void Create_WithExportableFailureMechanismAssemblyResult_ReturnsSerializableFailureMechanismAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var result = new ExportableFailureMechanismAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                      random.NextEnumValue(new[]
                                                                      {
                                                                          FailureMechanismAssemblyCategoryGroup.NotApplicable,
                                                                          FailureMechanismAssemblyCategoryGroup.It,
                                                                          FailureMechanismAssemblyCategoryGroup.IIt,
                                                                          FailureMechanismAssemblyCategoryGroup.IIIt,
                                                                          FailureMechanismAssemblyCategoryGroup.IVt,
                                                                          FailureMechanismAssemblyCategoryGroup.Vt,
                                                                          FailureMechanismAssemblyCategoryGroup.VIt,
                                                                          FailureMechanismAssemblyCategoryGroup.VIIt
                                                                      }));

            // Call
            SerializableFailureMechanismAssemblyResult serializableAssemblyResult = SerializableFailureMechanismResultCreator.Create(result);

            // Assert
            SerializableFailureMechanismAssemblyResultTestHelper.AssertSerializableFailureMechanismAssemblyResult(result, serializableAssemblyResult);
        }

        [Test]
        public void CreateWithFailureMechanismAssemblyResultWithProbability_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_WithExportableFailureMechanismAssemblyResultWithProbabilityAndResultNone_ThrowsAssemblyCreatorException()
        {
            // Setup
            var random = new Random(21);
            var result = new ExportableFailureMechanismAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                                     FailureMechanismAssemblyCategoryGroup.None,
                                                                                     random.NextDouble());

            // Call
            TestDelegate call = () => SerializableFailureMechanismResultCreator.Create(result);

            // Assert
            var exception = Assert.Throws<AssemblyCreatorException>(call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        [Test]
        public void Create_WithExportableFailureMechanismAssemblyResultWithProbability_ReturnsSerializableFailureMechanismAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var result = new ExportableFailureMechanismAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                                     random.NextEnumValue(new[]
                                                                                     {
                                                                                         FailureMechanismAssemblyCategoryGroup.NotApplicable,
                                                                                         FailureMechanismAssemblyCategoryGroup.It,
                                                                                         FailureMechanismAssemblyCategoryGroup.IIt,
                                                                                         FailureMechanismAssemblyCategoryGroup.IIIt,
                                                                                         FailureMechanismAssemblyCategoryGroup.IVt,
                                                                                         FailureMechanismAssemblyCategoryGroup.Vt,
                                                                                         FailureMechanismAssemblyCategoryGroup.VIt,
                                                                                         FailureMechanismAssemblyCategoryGroup.VIIt
                                                                                     }),
                                                                                     random.NextDouble());

            // Call
            SerializableFailureMechanismAssemblyResult serializableAssemblyResult = SerializableFailureMechanismResultCreator.Create(result);

            // Assert
            SerializableFailureMechanismAssemblyResultTestHelper.AssertSerializableFailureMechanismAssemblyResult(result, serializableAssemblyResult);
        }
    }
}