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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Calculators.Assembly
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
        public void AssembleFailureMechanism_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub();

            // Call
            FailureMechanismAssemblyCategoryGroup category = calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.IIt, category);
        }

        [Test]
        public void AssembleFailureMechanism_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismAssemblyCalculatorStub
            {
                FailureMechanismAssemblyCategoryGroupOutput = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismAssemblyCategoryGroup category = calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            Assert.AreEqual(calculator.FailureMechanismAssemblyCategoryGroupOutput, category);
        }

        [Test]
        public void AssembleFailureMechanism_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub();
            var sectionResults = new List<FailureMechanismSectionAssemblyCategoryGroup>();

            // Call
            calculator.AssembleFailureMechanism(sectionResults);

            // Assert
            Assert.AreEqual(sectionResults, calculator.FailureMechanismSectionAssemblies);
        }

        [Test]
        public void AssembleFailureMechanism_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleFailureMechanismWithProbabilities_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub();

            // Call
            FailureMechanismAssembly assembly = calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssembly>(),
                                                                                    CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.IIIt, assembly.Group);
            Assert.AreEqual(1.0, assembly.Probability);
        }

        [Test]
        public void AssembleFailureMechanismWithProbabilities_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismAssemblyCalculatorStub
            {
                FailureMechanismAssemblyOutput = new FailureMechanismAssembly(random.NextDouble(),
                                                                              random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismAssembly assembly = calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssembly>(),
                                                                                    CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.FailureMechanismAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleFailureMechanismWithProbabilities_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub();
            var sectionResults = new List<FailureMechanismSectionAssembly>();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            // Call
            calculator.AssembleFailureMechanism(sectionResults, assemblyCategoriesInput);

            // Assert
            Assert.AreEqual(sectionResults, calculator.FailureMechanismSectionAssemblies);
            Assert.AreEqual(assemblyCategoriesInput, calculator.AssemblyCategoriesInput);
        }

        [Test]
        public void AssembleFailureMechanismWithProbabilities_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssembly>(),
                                                                          CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        private AssemblyCategoriesInput CreateAssemblyCategoriesInput()
        {
            var random = new Random(39);
            return new AssemblyCategoriesInput(random.NextDouble(),
                                               random.NextDouble(),
                                               random.NextDouble(),
                                               random.NextDouble());
        }
    }
}