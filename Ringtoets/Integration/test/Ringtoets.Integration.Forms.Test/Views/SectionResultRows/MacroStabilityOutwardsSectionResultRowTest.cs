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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRows
{
    [TestFixture]
    public class MacroStabilityOutwardsSectionResultRowTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new MacroStabilityOutwardsSectionResultRow(result, null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<MacroStabilityOutwardsFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
            Assert.AreEqual(result.DetailedAssessmentResult, row.DetailedAssessmentResult);
            Assert.AreEqual(result.DetailedAssessmentProbability, row.DetailedAssessmentProbability);
            Assert.AreEqual(result.TailorMadeAssessmentResult, row.TailorMadeAssessmentResult);
            Assert.AreEqual(result.TailorMadeAssessmentProbability, row.TailorMadeAssessmentProbability);

            TestHelper.AssertTypeConverter<MacroStabilityOutwardsSectionResultRow,
                NoProbabilityValueDoubleConverter>(
                nameof(MacroStabilityOutwardsSectionResultRow.DetailedAssessmentProbability));
            TestHelper.AssertTypeConverter<MacroStabilityOutwardsSectionResultRow,
                NoProbabilityValueDoubleConverter>(
                nameof(MacroStabilityOutwardsSectionResultRow.TailorMadeAssessmentProbability));
        }

        #region Registration

        [Test]
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SimpleAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            // Call
            row.SimpleAssessmentResult = newValue;

            // Assert
            Assert.AreEqual(newValue, result.SimpleAssessmentResult);
            mocks.VerifyAll();
        }

        [Test]
        public void DetailedAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            // Call
            row.DetailedAssessmentResult = newValue;

            // Assert
            Assert.AreEqual(newValue, result.DetailedAssessmentResult);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void DetailedAssessmentProbability_ValidValue_NotifyObserversAndPropertyChanged(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            // Call
            row.DetailedAssessmentProbability = value;

            // Assert
            Assert.AreEqual(value, row.DetailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void DetailedAssessmentProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            // Call
            TestDelegate test = () => row.DetailedAssessmentProbability = value;

            // Assert
            string message = Assert.Throws<ArgumentOutOfRangeException>(test).Message;
            const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            Assert.AreEqual(expectedMessage, message);
            mocks.VerifyAll();
        }

        [Test]
        public void TailorMadeAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<TailorMadeAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            // Call
            row.TailorMadeAssessmentResult = newValue;

            // Assert
            Assert.AreEqual(newValue, result.TailorMadeAssessmentResult);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void TailorMadeAssessmentProbability_ValidValue_NotifyObserversAndPropertyChanged(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            // Call
            row.TailorMadeAssessmentProbability = value;

            // Assert
            Assert.AreEqual(value, row.TailorMadeAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void TailorMadeAssessmentProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            // Call
            TestDelegate test = () => row.TailorMadeAssessmentProbability = value;

            // Assert
            string message = Assert.Throws<ArgumentOutOfRangeException>(test).Message;
            const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            Assert.AreEqual(expectedMessage, message);
            mocks.VerifyAll();
        }

        #endregion

        #region Assembly Results

        [Test]
        public void SimpleAssemblyCategoryGroup_AssemblyRan_ReturnCategoryGroup()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup simpleAssemblyCategoryGroup = row.SimpleAssemblyCategoryGroup;

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.SimpleAssessmentAssemblyOutput;
                Assert.AreEqual(calculatorOutput.Group, simpleAssemblyCategoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void SimpleAssemblyCategoryGroup_AssemblyThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup simpleAssemblyCategoryGroup;
                TestDelegate test = () => simpleAssemblyCategoryGroup = row.SimpleAssemblyCategoryGroup;

                // Assert
                Assert.Throws<AssemblyException>(test);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssemblyCategoryGroup_AssemblyRan_ReturnCategoryGroup()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup detailedAssemblyCategoryGroup = row.DetailedAssemblyCategoryGroup;

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.DetailedAssessmentAssemblyOutput;
                Assert.AreEqual(calculatorOutput.Group, detailedAssemblyCategoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssemblyCategoryGroup_AssemblyThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup detailedAssemblyCategoryGroup;
                TestDelegate test = () => detailedAssemblyCategoryGroup = row.DetailedAssemblyCategoryGroup;

                // Assert
                Assert.Throws<AssemblyException>(test);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void TailorMadeAssemblyCategoryGroup_AssemblyRan_ReturnCategoryGroup()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssemblyCategoryGroup = row.TailorMadeAssemblyCategoryGroup;

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.TailorMadeAssessmentAssemblyOutput;
                Assert.AreEqual(calculatorOutput.Group, tailorMadeAssemblyCategoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void TailorMadeAssemblyCategoryGroup_AssemblyThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssemblyCategoryGroup;
                TestDelegate test = () => tailorMadeAssemblyCategoryGroup = row.TailorMadeAssemblyCategoryGroup;

                // Assert
                Assert.Throws<AssemblyException>(test);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}