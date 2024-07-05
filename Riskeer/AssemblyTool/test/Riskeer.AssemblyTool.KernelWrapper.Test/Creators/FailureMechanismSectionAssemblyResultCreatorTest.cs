// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyResultCreatorTest
    {
        [Test]
        public void CreateForProbabilityAndCategory_WithValidData_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            double sectionProbability = random.NextDouble();
            var category = random.NextEnumValue<EInterpretationCategory>();

            // Call
            FailureMechanismSectionAssemblyResult result = FailureMechanismSectionAssemblyResultCreator.Create(
                new Probability(sectionProbability), category);

            // Assert
            Assert.AreEqual(sectionProbability, result.SectionProbability);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(category),
                            result.FailureMechanismSectionAssemblyGroup);
        }
    }
}