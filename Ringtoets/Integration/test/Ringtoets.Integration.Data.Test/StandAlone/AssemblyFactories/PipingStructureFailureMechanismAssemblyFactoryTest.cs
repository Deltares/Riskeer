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
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.Test.StandAlone.AssemblyFactories
{
    [TestFixture]
    public class PipingStructureFailureMechanismAssemblyFactoryTest
    {
        #region Simple Assembly

        [Test]
        public void AssembleSimpleAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleSimpleAssessment(null);

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
            var sectionResult = new PipingStructureFailureMechanismSectionResult(failureMechanismSection)
            {
                SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingStructureFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.SimpleAssessmentResult, calculator.SimpleAssessmentInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingStructureFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    PipingStructureFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

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
            var sectionResult = new PipingStructureFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Detailed Assembly

        [Test]
        public void AssembleDetailedAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleDetailedAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleDetailedAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingStructureFailureMechanismAssemblyFactory.AssembleDetailedAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.DetailedAssessmentResult, calculator.DetailedAssessmentResultInput);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    PipingStructureFailureMechanismAssemblyFactory.AssembleDetailedAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup? calculatorOutput = calculator.DetailedAssessmentAssemblyGroupOutput;
                Assert.AreEqual(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleDetailedAssessment(sectionResult);

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
            TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingStructureFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.TailorMadeAssessmentResult, calculator.TailorMadeAssessmentResultInput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    PipingStructureFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup? calculatorOutput = calculator.TailorMadeAssemblyCategoryOutput;
                Assert.AreEqual(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

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
            TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleCombinedAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void AssembleCombinedAssessment_WithVariousSimpleAssessmentInputAssemblesWithAllInformation_SetsInputOnCalculator(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingStructureFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedSimpleAssembly =
                    PipingStructureFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);
                FailureMechanismSectionAssemblyCategoryGroup expectedDetailedAssembly =
                    PipingStructureFailureMechanismAssemblyFactory.AssembleDetailedAssessment(sectionResult);
                FailureMechanismSectionAssemblyCategoryGroup expectedTailorMadeAssembly =
                    PipingStructureFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

                Assert.AreEqual(expectedSimpleAssembly, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual(expectedDetailedAssembly, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual(expectedTailorMadeAssembly, calculator.CombinedTailorMadeAssemblyGroupInput);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.NotApplicable)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible)]
        public void AssembleCombinedAssessment_WithVariousSimpleAssessmentInputAssemblesWithOnlySimpleAssessmentInput_SetsInputOnCalculator(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingStructureFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedSimpleAssembly =
                    PipingStructureFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                Assert.AreEqual(expectedSimpleAssembly, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedTailorMadeAssemblyGroupInput);
            }
        }

        [Test]
        public void AssembleCombinedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    PipingStructureFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup? calculatorOutput = calculator.CombinedAssemblyCategoryOutput;
                Assert.AreEqual(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleCombinedAssessment_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region GetSectionAssemblyCategoryGroup

        [Test]
        public void GetSectionAssemblyCategoryGroup_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingStructureFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedSimpleAssembly = PipingStructureFailureMechanismAssemblyFactory.AssembleSimpleAssessment(
                    sectionResult);
                FailureMechanismSectionAssemblyCategoryGroup expectedDetailedAssembly = PipingStructureFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                    sectionResult);
                FailureMechanismSectionAssemblyCategoryGroup expectedTailorMadeAssembly = PipingStructureFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                    sectionResult);

                Assert.AreEqual(expectedSimpleAssembly, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual(expectedDetailedAssembly, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual(expectedTailorMadeAssembly, calculator.CombinedTailorMadeAssemblyGroupInput);
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_ReturnsOutput()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = PipingStructureFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedAssembly = PipingStructureFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                    sectionResult);
                Assert.AreEqual(expectedAssembly, categoryGroup);
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInput_ReturnsOutput()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssemblyCategoryGroup = true,
                ManualAssemblyCategoryGroup = new Random(39).NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismSectionAssemblyCategoryGroup categoryGroup = PipingStructureFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                sectionResult);

            // Assert
            Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                            categoryGroup);
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInputAndCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInputAndInvalidManualFailureMechanismSectionAssemblyCategoryGroup_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new PipingStructureFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssemblyCategoryGroup = true,
                ManualAssemblyCategoryGroup = (ManualFailureMechanismSectionAssemblyCategoryGroup) 99
            };

            // Call
            TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(sectionResult);

            // Assert
            var exception = Assert.Throws<AssemblyException>(call);
            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
            Assert.AreEqual(innerException.Message, exception.Message);
        }

        #endregion

        #region Failure Mechanism Assembly

        [Test]
        public void AssembleFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismIsNotRelevant_ReturnsNotApplicableCategory()
        {
            // Setup
            var failureMechanism = new PipingStructureFailureMechanism
            {
                IsRelevant = false
            };

            // Call
            FailureMechanismAssemblyCategoryGroup category = PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism);

            // Assert
            Assert.AreEqual(FailureMechanismAssemblyResultFactory.CreateNotApplicableCategory(), category);
        }

        [Test]
        public void AssembleFailureMechanism_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new PipingStructureFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup assemblyCategory =
                    PipingStructureFailureMechanismAssemblyFactory.AssembleCombinedAssessment(failureMechanism.SectionResults.Single());
                Assert.AreEqual(assemblyCategory, calculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new PipingStructureFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = new Random(39).NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism);

                // Assert
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                calculator.FailureMechanismSectionCategories.Single());
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
                    PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(new PipingStructureFailureMechanism());

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
                TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(new PipingStructureFailureMechanism());

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
            var failureMechanism = new PipingStructureFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere vakken kan geen resultaat worden bepaald.", exception.Message);
            }
        }

        #endregion
    }
}