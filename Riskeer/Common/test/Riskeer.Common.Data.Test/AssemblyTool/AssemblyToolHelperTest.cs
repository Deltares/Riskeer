// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyToolHelperTest
    {
        [Test]
        public void AssembleFailureMechanismSection_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssemblyToolHelper.AssembleFailureMechanismSection<FailureMechanismSectionResult>(null, sr => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanismSection_PerformSectionAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => AssemblyToolHelper.AssembleFailureMechanismSection(sectionResult, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performSectionAssemblyFunc", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanismSection_WithValidData_ReturnsFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var sectionResult = new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            var random = new Random(21);
            var expectedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            // Call
            FailureMechanismSectionAssemblyResult assemblyResult = AssemblyToolHelper.AssembleFailureMechanismSection(
                sectionResult, sr => expectedAssemblyResult);

            // Assert
            Assert.AreSame(expectedAssemblyResult, assemblyResult);
        }

        [Test]
        public void AssembleFailureMechanismSection_PerformSectionAssemblyFuncThrowsAssemblyException_ReturnsDefaultFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var sectionResult = new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            FailureMechanismSectionAssemblyResult assemblyResult = AssemblyToolHelper.AssembleFailureMechanismSection(
                sectionResult, sr => throw new AssemblyException());

            // Assert
            Assert.IsInstanceOf<DefaultFailureMechanismSectionAssemblyResult>(assemblyResult);
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssemblyToolHelper.AssemblyFailureMechanism<FailureMechanismSectionResult>(
                null, sr => null, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_PerformSectionAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailureMechanism<FailureMechanismSectionResult>>();
            mocks.ReplayAll();

            // Call
            void Call() => AssemblyToolHelper.AssemblyFailureMechanism(
                failurePath, null, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performSectionAssemblyFunc", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_WithFailurePathInAssemblyFalse_ReturnsNaN()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailureMechanism<FailureMechanismSectionResult>>();
            mocks.ReplayAll();

            failurePath.InAssembly = false;

            // Call
            double assemblyResult = AssemblyToolHelper.AssemblyFailureMechanism(
                failurePath, sr => null, random.NextDouble());

            // Assert
            Assert.IsNaN(assemblyResult);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithFailurePathInAssemblyAndProbabilityResultTypeManual_ReturnsExpectedAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            double expectedAssemblyResult = random.NextDouble();

            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailureMechanism<FailureMechanismSectionResult>>();
            failurePath.Stub(fp => fp.AssemblyResult)
                       .Return(new FailureMechanismAssemblyResult
                       {
                           ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual,
                           ManualFailurePathAssemblyProbability = expectedAssemblyResult
                       });
            mocks.ReplayAll();

            failurePath.InAssembly = true;

            // Call
            double assemblyResult = AssemblyToolHelper.AssemblyFailureMechanism(failurePath, sr => null, double.NaN);

            // Assert
            Assert.AreEqual(expectedAssemblyResult, assemblyResult);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithFailurePathInAssemblyAndProbabilityResultTypeAutomatic_InputCorrectlySetOnCalculator()
        {
            // Setup
            var random = new Random(21);
            double failureMechanismN = random.NextDouble();

            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailureMechanism<TestFailureMechanismSectionResult>>();
            failurePath.Stub(fp => fp.AssemblyResult)
                       .Return(new FailureMechanismAssemblyResult
                       {
                           ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Automatic,
                           ManualFailurePathAssemblyProbability = double.NaN
                       });
            failurePath.Stub(fp => fp.SectionResults)
                       .Return(new ObservableList<TestFailureMechanismSectionResult>
                       {
                           new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
                       });
            mocks.ReplayAll();

            failurePath.InAssembly = true;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var failureMechanismSectionAssemblyResult = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                // Call
                AssemblyToolHelper.AssemblyFailureMechanism(failurePath, sr => failureMechanismSectionAssemblyResult, failureMechanismN);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                CollectionAssert.AreEqual(new[]
                {
                    failureMechanismSectionAssemblyResult
                }, failureMechanismAssemblyCalculator.SectionAssemblyResultsInput);
                Assert.AreEqual(failureMechanismN, failureMechanismAssemblyCalculator.FailureMechanismN);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithFailurePathInAssemblyAndProbabilityResultTypeAutomaticAndFailureMechanismSectionAssemblyThrowsException_InputCorrectlySetOnCalculator()
        {
            // Setup
            var random = new Random(21);
            double failureMechanismN = random.NextDouble();

            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailureMechanism<TestFailureMechanismSectionResult>>();
            failurePath.Stub(fp => fp.AssemblyResult)
                       .Return(new FailureMechanismAssemblyResult
                       {
                           ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Automatic,
                           ManualFailurePathAssemblyProbability = double.NaN
                       });
            failurePath.Stub(fp => fp.SectionResults)
                       .Return(new ObservableList<TestFailureMechanismSectionResult>
                       {
                           new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
                       });
            mocks.ReplayAll();

            failurePath.InAssembly = true;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                AssemblyToolHelper.AssemblyFailureMechanism(failurePath, sr => throw new AssemblyException(), failureMechanismN);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                Assert.AreEqual(1, failureMechanismAssemblyCalculator.SectionAssemblyResultsInput.Count());
                Assert.IsInstanceOf<DefaultFailureMechanismSectionAssemblyResult>(failureMechanismAssemblyCalculator.SectionAssemblyResultsInput.First());
                Assert.AreEqual(failureMechanismN, failureMechanismAssemblyCalculator.FailureMechanismN);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithFailurePathInAssemblyAndProbabilityResultTypeAutomatic_ReturnsExpectedAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            double expectedAssemblyResult = random.NextDouble();

            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailureMechanism<FailureMechanismSectionResult>>();
            failurePath.Stub(fp => fp.AssemblyResult)
                       .Return(new FailureMechanismAssemblyResult
                       {
                           ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Automatic,
                           ManualFailurePathAssemblyProbability = double.NaN
                       });
            failurePath.Stub(fp => fp.SectionResults)
                       .Return(new ObservableList<TestFailureMechanismSectionResult>
                       {
                           new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
                       });
            mocks.ReplayAll();

            failurePath.InAssembly = true;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                failureMechanismAssemblyCalculator.AssemblyResult = expectedAssemblyResult;

                // Call
                double assemblyResult = AssemblyToolHelper.AssemblyFailureMechanism(failurePath, sr => null, double.NaN);

                // Assert
                Assert.AreEqual(expectedAssemblyResult, assemblyResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithFailurePathInAssemblyAndProbabilityResultTypeAutomaticAndFailureMechanismAssemblyThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailureMechanism<FailureMechanismSectionResult>>();
            failurePath.Stub(fp => fp.AssemblyResult)
                       .Return(new FailureMechanismAssemblyResult
                       {
                           ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Automatic,
                           ManualFailurePathAssemblyProbability = double.NaN
                       });
            failurePath.Stub(fp => fp.SectionResults)
                       .Return(new ObservableList<TestFailureMechanismSectionResult>
                       {
                           new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
                       });
            mocks.ReplayAll();

            failurePath.InAssembly = true;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                failureMechanismAssemblyCalculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => AssemblyToolHelper.AssemblyFailureMechanism(failurePath, sr => null, double.NaN);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }

            mocks.VerifyAll();
        }
    }
}