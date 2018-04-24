﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.Test.StandAlone.AssemblyFactories
{
    [TestFixture]
    public class StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactoryTest
    {
        #region Simple Assembly

        [Test]
        public void AssembleSimpleAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleSimpleAssessment_WithSectionResult_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(failureMechanismSection)
            {
                SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.SimpleAssessmentResult, calculator.SimpleAssessmentInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.SimpleAssessmentAssemblyOutput;
                Assert.AreEqual(calculatorOutput.Group, actualOutput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Tailor Made Assembly

        [Test]
        public void AssembleTailorMadeAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.TailorMadeAssessmentResult, calculator.TailorMadeAssessmentResultInput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup? calculatorOutput = calculator.TailorMadeAssemblyCategoryOutput;
                Assert.AreEqual(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Combined Assembly

        [Test]
        public void AssembleCombinedAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleCombinedAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedSimpleAssembly = StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(
                    sectionResult);
                FailureMechanismSectionAssemblyCategoryGroup expectedTailorMadeAssembly = StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssessment(
                    sectionResult);

                Assert.AreEqual(expectedSimpleAssembly, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual(expectedTailorMadeAssembly, calculator.CombinedTailorMadeAssemblyGroupInput);
            }
        }

        [Test]
        public void AssembleCombinedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup? calculatorOutput = calculator.CombinedAssemblyCategoryOutput;
                Assert.AreEqual(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleCombinedAssessment_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Failure Mechanism Assembly

        [Test]
        public void AssembleFailureMechanism_FailureMechanismSectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResults", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResults = new[]
            {
                new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(sectionResults);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup assemblyCategory =
                    StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssessment(sectionResults.Single());
                Assert.AreEqual(assemblyCategory, calculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInputConsiderManualAssemblyTrue_SetsInputOnCalculator()
        {
            // Setup
            var sectionResults = new[]
            {
                new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
                {
                    UseManualAssemblyCategoryGroup = true,
                    ManualAssemblyCategoryGroup = new Random(39).NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
                }
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(sectionResults);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup assemblyCategory =
                    StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssessment(sectionResults.Single());
                Assert.AreEqual(assemblyCategory, calculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInputConsiderManualAssemblyFalse_SetsInputOnCalculator()
        {
            // Setup
            var sectionResults = new[]
            {
                new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
                {
                    UseManualAssemblyCategoryGroup = true,
                    ManualAssemblyCategoryGroup = new Random(39).NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
                }
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(sectionResults, false);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup assemblyCategory =
                    StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssessment(sectionResults.Single());
                Assert.AreEqual(assemblyCategory, calculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void AssembleFailureMechanism_AssemblyRan_ReturnsOutput()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                FailureMechanismAssemblyCategoryGroup actualOutput =
                    StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(
                        Enumerable.Empty<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>());

                // Assert
                Assert.AreEqual(calculator.FailureMechanismAssemblyCategoryGroupOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(
                    Enumerable.Empty<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismSectionCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(
                    new[]
                    {
                        new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
                    });

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion
    }
}