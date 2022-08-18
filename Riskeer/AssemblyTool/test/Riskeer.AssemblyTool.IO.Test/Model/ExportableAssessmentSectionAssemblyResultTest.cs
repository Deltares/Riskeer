// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class ExportableAssessmentSectionAssemblyResultTest
    {
        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assemblyGroup = random.NextEnumValue<AssessmentSectionAssemblyGroup>();
            double probability = random.NextDouble();
            var assemblyGroupAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();
            var probabilityAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            var assembly = new ExportableAssessmentSectionAssemblyResult(
                assemblyGroup, probability, assemblyGroupAssemblyMethod, probabilityAssemblyMethod);

            // Assert
            Assert.AreEqual(assemblyGroup, assembly.AssemblyGroup);
            Assert.AreEqual(probability, assembly.Probability);
            Assert.AreEqual(assemblyGroupAssemblyMethod, assembly.AssemblyGroupAssemblyMethod);
            Assert.AreEqual(probabilityAssemblyMethod, assembly.ProbabilityAssemblyMethod);
        }
    }
}