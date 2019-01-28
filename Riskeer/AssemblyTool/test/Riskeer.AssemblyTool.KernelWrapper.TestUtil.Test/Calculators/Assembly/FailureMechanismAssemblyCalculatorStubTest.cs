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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Test.Calculators.Assembly
{
    [TestFixture]
    public class FailureMechanismAssemblyCalculatorStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculator = new FailureMechanismAssemblyCalculatorStub();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismAssemblyCalculator>(calculator);
            Assert.AreEqual(null, calculator.FailureMechanismAssemblyCategoryGroupOutput);

            Assert.IsNull(calculator.AssemblyCategoriesInput);
            Assert.IsNull(calculator.FailureMechanismSectionAssemblies);
            Assert.IsNull(calculator.FailureMechanismAssemblyOutput);
        }

        [Test]
        public void Assemble_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub();

            // Call
            FailureMechanismAssemblyCategoryGroup category = calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.IIt, category);
        }

        [Test]
        public void Assemble_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismAssemblyCalculatorStub
            {
                FailureMechanismAssemblyCategoryGroupOutput = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismAssemblyCategoryGroup category = calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            Assert.AreEqual(calculator.FailureMechanismAssemblyCategoryGroupOutput, category);
        }

        [Test]
        public void Assemble_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub();
            IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> sectionResults = Enumerable.Empty<FailureMechanismSectionAssemblyCategoryGroup>();

            // Call
            calculator.Assemble(sectionResults);

            // Assert
            Assert.AreSame(sectionResults, calculator.FailureMechanismSectionCategories);
        }

        [Test]
        public void Assemble_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleWithProbabilities_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub();

            // Call
            FailureMechanismAssembly assembly = calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssembly>(),
                                                                    CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.IIIt, assembly.Group);
            Assert.AreEqual(1.0, assembly.Probability);
        }

        [Test]
        public void AssembleWithProbabilities_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismAssemblyCalculatorStub
            {
                FailureMechanismAssemblyOutput = new FailureMechanismAssembly(random.NextDouble(),
                                                                              random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismAssembly assembly = calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssembly>(),
                                                                    CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.FailureMechanismAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleWithProbabilities_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub();
            IEnumerable<FailureMechanismSectionAssembly> sectionResults = Enumerable.Empty<FailureMechanismSectionAssembly>();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            // Call
            calculator.Assemble(sectionResults, assemblyCategoriesInput);

            // Assert
            Assert.AreSame(sectionResults, calculator.FailureMechanismSectionAssemblies);
            Assert.AreSame(assemblyCategoriesInput, calculator.AssemblyCategoriesInput);
        }

        [Test]
        public void AssembleWithProbabilities_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssembly>(),
                                                          CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput()
        {
            var random = new Random(39);
            return new AssemblyCategoriesInput(random.NextDouble(),
                                               random.NextDouble(),
                                               random.NextDouble(),
                                               random.NextDouble());
        }
    }
}