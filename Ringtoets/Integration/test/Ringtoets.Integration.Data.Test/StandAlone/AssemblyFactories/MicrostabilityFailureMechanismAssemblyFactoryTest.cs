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
    public class MicrostabilityFailureMechanismAssemblyFactoryTest
    {
        #region Simple Assembly

        [Test]
        public void AssembleSimpleAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleSimpleAssessment(null);

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
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(failureMechanismSection)
            {
                SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MicrostabilityFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.SimpleAssessmentResult, calculator.SimpleAssessmentInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MicrostabilityFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, actualOutput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

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
            TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleDetailedAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleDetailedAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MicrostabilityFailureMechanismAssemblyFactory.AssembleDetailedAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.DetailedAssessmentResult, calculator.DetailedAssessmentResultInput);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MicrostabilityFailureMechanismAssemblyFactory.AssembleDetailedAssessment(sectionResult);

                // Assert
                Assert.AreEqual(calculator.DetailedAssessmentAssemblyGroupOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleDetailedAssessment(sectionResult);

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
            TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(failureMechanismSection)
            {
                TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MicrostabilityFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.TailorMadeAssessmentResult, calculator.TailorMadeAssessmentResultInput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MicrostabilityFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

                // Assert
                Assert.AreEqual(calculator.TailorMadeAssemblyCategoryOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(sectionResult);

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
            TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleCombinedAssessment(null);

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
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MicrostabilityFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual(calculator.DetailedAssessmentAssemblyGroupOutput, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual(calculator.TailorMadeAssemblyCategoryOutput, calculator.CombinedTailorMadeAssemblyGroupInput);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.NotApplicable)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible)]
        public void AssembleCombinedAssessment_WithVariousSimpleAssessmentInputAssemblesWithOnlySimpleAssessmentInput_SetsInputOnCalculator(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MicrostabilityFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedTailorMadeAssemblyGroupInput);
            }
        }

        [Test]
        public void AssembleCombinedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MicrostabilityFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup? calculatorOutput = calculator.CombinedAssemblyCategoryOutput;
                Assert.AreEqual(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleCombinedAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult);

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
            TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                null,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MicrostabilityFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual(calculator.DetailedAssessmentAssemblyGroupOutput, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual(calculator.TailorMadeAssemblyCategoryOutput, calculator.CombinedTailorMadeAssemblyGroupInput);
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_ReturnsOutput()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = MicrostabilityFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(calculator.CombinedAssemblyCategoryOutput, categoryGroup);
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInputAndUseManualTrue_ReturnsOutput()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = new Random(39).NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismSectionAssemblyCategoryGroup categoryGroup = MicrostabilityFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                sectionResult,
                true);

            // Assert
            Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                            categoryGroup);
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInputAndUseManualFalse_ReturnsOutput()
        {
            // Setup
            var random = new Random(39);
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = MicrostabilityFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    false);

                // Assert
                Assert.AreEqual(calculator.CombinedAssemblyCategoryOutput, categoryGroup);
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInputAndCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    false);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithUseManualAndUseManualInputTrueAndInvalidManualFailureMechanismSectionAssemblyCategoryGroup_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new MicrostabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = (ManualFailureMechanismSectionAssemblyCategoryGroup) 99
            };

            // Call
            TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(sectionResult,
                                                                                                                    true);

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
            TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(null, new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismIsNotRelevant_ReturnsNotApplicableCategory()
        {
            // Setup
            var failureMechanism = new MicrostabilityFailureMechanism
            {
                IsRelevant = false
            };

            // Call
            FailureMechanismAssemblyCategoryGroup category = MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, new Random(39).NextBoolean());

            // Assert
            Assert.AreEqual(FailureMechanismAssemblyResultFactory.CreateNotApplicableCategory(), category);
        }

        [Test]
        public void AssembleFailureMechanism_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new MicrostabilityFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(sectionCalculator.CombinedAssemblyCategoryOutput, calculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInputAndUseManualTrue_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new MicrostabilityFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = new Random(39).NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, true);

                // Assert
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                calculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInputAndUseManualFalse_SetsCombinedInputOnCalculator()
        {
            // Setup
            var failureMechanism = new MicrostabilityFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.IIv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, false);

                // Assert
                Assert.AreEqual(sectionCalculator.CombinedAssemblyCategoryOutput, calculator.FailureMechanismSectionCategories.Single());
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
                    MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(new MicrostabilityFailureMechanism(),
                                                                                           new Random(39).NextBoolean());

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
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                    new MicrostabilityFailureMechanism(),
                    new Random(39).NextBoolean());

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
            var failureMechanism = new MicrostabilityFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism,
                                                                                                                 new Random(39).NextBoolean());

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