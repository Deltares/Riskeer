// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionResultRowHelperTest
    {
        [Test]
        public void GetDetailedAssessmentProbabilityError_RelevantScenariosNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultRowHelper.GetDetailedAssessmentProbabilityError<ICalculationScenario>(
                null, scenarios => double.NaN, scenarios => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("relevantScenarios", exception.ParamName);
        }

        [Test]
        public void GetDetailedAssessmentProbabilityError_GetTotalContributionFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultRowHelper.GetDetailedAssessmentProbabilityError(
                new ICalculationScenario[0], null, scenarios => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getTotalContributionFunc", exception.ParamName);
        }

        [Test]
        public void GetDetailedAssessmentProbabilityError_GetDetailedAssessmentProbabilityFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultRowHelper.GetDetailedAssessmentProbabilityError(
                new ICalculationScenario[0], scenarios => double.NaN, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getDetailedAssessmentProbabilityFunc", exception.ParamName);
        }

        [Test]
        public void GetDetailedAssessmentProbabilityError_RelevantScenariosEmpty_ReturnsErrorMessage()
        {
            // Setup
            var random = new Random(21);

            // Call
            string errorMessage = FailureMechanismSectionResultRowHelper.GetDetailedAssessmentProbabilityError(
                new ICalculationScenario[0], scenarios => random.NextDouble(), scenarios => random.NextDouble());

            // Assert
            Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden gedefinieerd.", errorMessage);
        }

        [Test]
        public void GetDetailedAssessmentProbabilityError_TotalContributionNotOne_ReturnsErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenario = mocks.Stub<ICalculationScenario>();
            mocks.ReplayAll();

            var random = new Random(21);
            ICalculationScenario[] calculationScenarios = 
            {
                calculationScenario
            };

            // Call
            string errorMessage = FailureMechanismSectionResultRowHelper.GetDetailedAssessmentProbabilityError(
                calculationScenarios, scenarios => random.NextDouble(0, 0.99), scenarios => random.NextDouble());

            // Assert
            Assert.AreEqual("De bijdragen van de maatgevende scenario's voor dit vak moeten opgeteld gelijk zijn aan 100%.", errorMessage);
        }

        [Test]
        public void GetDetailedAssessmentProbabilityError_CalculationScenariosWithoutOutput_ReturnsErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenario1 = mocks.Stub<ICalculationScenario>();
            calculationScenario1.Stub(cs => cs.HasOutput).Return(true);
            var calculationScenario2 = mocks.Stub<ICalculationScenario>();
            mocks.ReplayAll();

            var random = new Random(21);
            ICalculationScenario[] calculationScenarios = 
            {
                calculationScenario1,
                calculationScenario2
            };

            // Call
            string errorMessage = FailureMechanismSectionResultRowHelper.GetDetailedAssessmentProbabilityError(
                calculationScenarios, scenarios => 1, scenarios => random.NextDouble());

            // Assert
            Assert.AreEqual("Alle maatgevende berekeningen voor dit vak moeten uitgevoerd zijn.", errorMessage);
        }

        [Test]
        public void GetDetailedAssessmentProbabilityError_DetailedAssessmentProbabilityNaN_ReturnsErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenario = mocks.Stub<ICalculationScenario>();
            calculationScenario.Stub(cs => cs.HasOutput).Return(true);
            mocks.ReplayAll();

            ICalculationScenario[] calculationScenarios = 
            {
                calculationScenario
            };

            // Call
            string errorMessage = FailureMechanismSectionResultRowHelper.GetDetailedAssessmentProbabilityError(
                calculationScenarios, scenarios => 1, scenarios => double.NaN);

            // Assert
            Assert.AreEqual("Alle maatgevende berekeningen voor dit vak moeten een geldige uitkomst hebben.", errorMessage);
        }

        [Test]
        public void GetDetailedAssessmentProbabilityError_ValidData_ReturnsEmptyMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenario = mocks.Stub<ICalculationScenario>();
            calculationScenario.Stub(cs => cs.HasOutput).Return(true);
            mocks.ReplayAll();

            var random = new Random(21);
            ICalculationScenario[] calculationScenarios = 
            {
                calculationScenario
            };

            // Call
            string errorMessage = FailureMechanismSectionResultRowHelper.GetDetailedAssessmentProbabilityError(
                calculationScenarios, scenarios => 1, scenarios => random.NextDouble());

            // Assert
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, false)]
        [TestCase(SimpleAssessmentResultType.NotApplicable, true)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible, true)]
        [TestCase(SimpleAssessmentResultType.AssessFurther, false)]
        public void SimpleAssessmentIsSufficient_WithSimpleAssessmentResultType_ReturnsExpectedResult(
            SimpleAssessmentResultType simpleAssessmentResult, bool expectedSufficiency)
        {
            // Call
            bool isSufficient = FailureMechanismSectionResultRowHelper.SimpleAssessmentIsSufficient(simpleAssessmentResult);

            // Assert
            Assert.AreEqual(expectedSufficiency, isSufficient);
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None, false)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable, false)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.NotApplicable, true)]
        public void SimpleAssessmentIsSufficient_WithSimpleAssessmentValidityOnlyResultType_ReturnsExpectedResult(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult, bool expectedSufficiency)
        {
            // Call
            bool isSufficient = FailureMechanismSectionResultRowHelper.SimpleAssessmentIsSufficient(simpleAssessmentResult);

            // Assert
            Assert.AreEqual(expectedSufficiency, isSufficient);
        }

        [Test]
        [TestCase(DetailedAssessmentProbabilityOnlyResultType.NotAssessed, false)]
        [TestCase(DetailedAssessmentProbabilityOnlyResultType.Probability, true)]
        public void DetailedAssessmentResultIsProbability_WithDetailedAssessmentProbabilityOnlyResultType_ReturnsExpectedResult(
            DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult, bool expectedIsProbability)
        {
            // Call
            bool isProbability = FailureMechanismSectionResultRowHelper.DetailedAssessmentResultIsProbability(detailedAssessmentResult);

            // Assert
            Assert.AreEqual(expectedIsProbability, isProbability);
        }

        [Test]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed, false)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible, false)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.Probability, true)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed, false)]
        public void TailorMadeAssessmentResultIsProbability_WithTailorMadeAssessmentProbabilityCalculationResultType_ReturnsExpectedResult(
            TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult, bool expectedIsProbability)
        {
            // Call
            bool isProbability = FailureMechanismSectionResultRowHelper.TailorMadeAssessmentResultIsProbability(tailorMadeAssessmentResult);

            // Assert
            Assert.AreEqual(expectedIsProbability, isProbability);
        }

        [Test]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.None, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.NotAssessed, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.ProbabilityNegligible, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability, true)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Sufficient, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Insufficient, false)]
        public void TailorMadeAssessmentResultIsProbability_WithTailorMadeAssessmentProbabilityAndDetailedCalculationResultType_ReturnsExpectedResult(
            TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult, bool expectedIsProbability)
        {
            // Call
            bool isProbability = FailureMechanismSectionResultRowHelper.TailorMadeAssessmentResultIsProbability(tailorMadeAssessmentResult);

            // Assert
            Assert.AreEqual(expectedIsProbability, isProbability);
        }

        [Test]
        public void SetAssemblyCategoryGroupStyle_DataGridViewColumnStateDefinitionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(
                null, new Random(39).NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("columnStateDefinition", exception.ParamName);
        }

        [Test]
        public void SetAssemblyCategoryGroupStyle_WithInvalidFailureMechanismSectionAssemblyCategoryGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition
            {
                Style = new CellStyle(Color.FromKnownColor(KnownColor.Transparent), Color.FromKnownColor(KnownColor.Transparent))
            };

            // Call
            void Call() => FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(
                columnStateDefinition, (FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'assemblyCategoryGroup' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.FailureMechanismSectionAssemblyCategoryGroupColorCases))]
        public void SetAssemblyCategoryGroupStyle_WithFailureMechanismSectionAssemblyCategoryGroup_UpdatesColumnStyle(
            FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup, Color expectedColor)
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();

            // Call
            FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(columnStateDefinition, assemblyCategoryGroup);

            // Assert
            Assert.AreEqual(expectedColor, columnStateDefinition.Style.BackgroundColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), columnStateDefinition.Style.TextColor);
        }
    }
}