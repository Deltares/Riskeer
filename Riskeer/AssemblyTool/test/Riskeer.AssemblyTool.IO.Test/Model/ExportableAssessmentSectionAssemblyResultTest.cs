// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class ExportableAssessmentSectionAssemblyResultTest
    {
        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Call
            void Call() => new ExportableAssessmentSectionAssemblyResult(
                invalidId, ExportableAssessmentSectionAssemblyGroup.A, double.NaN,
                ExportableAssemblyMethod.Manual, ExportableAssemblyMethod.Manual);

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            const string id = "id";
            var assemblyGroup = random.NextEnumValue<ExportableAssessmentSectionAssemblyGroup>();
            double probability = random.NextDouble();
            var assemblyGroupAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();
            var probabilityAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            var assembly = new ExportableAssessmentSectionAssemblyResult(
                id, assemblyGroup, probability, assemblyGroupAssemblyMethod, probabilityAssemblyMethod);

            // Assert
            Assert.AreEqual(id, assembly.Id);
            Assert.AreEqual(assemblyGroup, assembly.AssemblyGroup);
            Assert.AreEqual(probability, assembly.Probability);
            Assert.AreEqual(assemblyGroupAssemblyMethod, assembly.AssemblyGroupAssemblyMethod);
            Assert.AreEqual(probabilityAssemblyMethod, assembly.ProbabilityAssemblyMethod);
        }
    }
}