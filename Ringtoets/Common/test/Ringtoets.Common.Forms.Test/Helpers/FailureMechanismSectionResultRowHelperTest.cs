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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionResultRowHelperTest
    {
        [Test]
        public void SetDetailedAssessmentErrorSimpleAssessmentResultType_DataGridViewCellNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            var simpleAssessmentResult = new Random(21).NextEnumValue<SimpleAssessmentResultType>();

            // Call
            TestDelegate call = () => FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(null,
                                                                                                        simpleAssessmentResult,
                                                                                                        0.0,
                                                                                                        calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("dataGridViewCell", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetSimpleAssessmentResultNotApplicableConfigurations))]
        [TestCaseSource(nameof(GetSimpleAssessmentResultProbabilityNegligibleConfigurations))]
        [TestCaseSource(nameof(GetSimpleAssessmentResultAssessFurtherOrNoneAndCalculationNullConfigurations))]
        [TestCaseSource(nameof(GetSimpleAssessmentResultAssessFurtherOrNoneAndCalculationWithoutOutputConfigurations))]
        [TestCaseSource(nameof(GetSimpleAssessmentResultAssessFurtherOrNoneAndCalculationWithOutputConfigurations))]
        public void SetDetailedAssessmentError_WithSimpleAssessmentResultConfigurations_SetsErrorText(DataGridViewCell dataGridViewCell,
                                                                                                      SimpleAssessmentResultType simpleAssessmentResult,
                                                                                                      double detailedAssessmentProbability,
                                                                                                      ICalculation normativeCalculation,
                                                                                                      string expectedErrorText)
        {
            // Call
            FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(dataGridViewCell,
                                                                              simpleAssessmentResult,
                                                                              detailedAssessmentProbability,
                                                                              normativeCalculation);

            // Assert
            Assert.AreEqual(expectedErrorText, dataGridViewCell.ErrorText);
        }

        [Test]
        public void SetDetailedAssessmentErrorSimpleAssessmentValidityOnlyType_DataGridViewCellNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            var simpleAssessmentResult = new Random(21).NextEnumValue<SimpleAssessmentResultValidityOnlyType>();

            // Call
            TestDelegate call = () => FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(null,
                                                                                                        simpleAssessmentResult,
                                                                                                        0.0,
                                                                                                        calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("dataGridViewCell", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetSimpleAssessmentResultValidityOnlyIsNotApplicableConfigurations))]
        [TestCaseSource(nameof(GetSimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationNullConfigurations))]
        [TestCaseSource(nameof(GetSimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationWithoutOutputConfigurations))]
        [TestCaseSource(nameof(SimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationWithOutput))]
        public void SetAssessmentDetailedAssessmentError_WithSimpleAssessmentValidityOnlyConfigurations_SetsErrorText(DataGridViewCell dataGridViewCell,
                                                                                                                      SimpleAssessmentResultValidityOnlyType simpleAssessmentResult,
                                                                                                                      double detailedAssessmentProbability,
                                                                                                                      ICalculation normativeCalculation,
                                                                                                                      string expectedErrorText)
        {
            // Call
            FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(dataGridViewCell,
                                                                              simpleAssessmentResult,
                                                                              detailedAssessmentProbability,
                                                                              normativeCalculation);

            // Assert
            Assert.AreEqual(expectedErrorText, dataGridViewCell.ErrorText);
        }

        private class TestDataGridViewCell : DataGridViewCell {}

        #region Test cases simple assessment (result)

        private static IEnumerable<TestCaseData> GetSimpleAssessmentResultNotApplicableConfigurations()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            string expectedErrorMessage = string.Empty;
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.NotApplicable, double.NaN, null, string.Empty)
                .SetName("SufficientWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.NotApplicable, 0.0, null, string.Empty)
                .SetName("SufficientWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.NotApplicable, double.NaN, null, expectedErrorMessage)
                .SetName("NotApplicableWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.NotApplicable, 0.0, null, expectedErrorMessage)
                .SetName("NotApplicableWithValidLayerTwoAAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.NotApplicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessage)
                .SetName("SufficientWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.NotApplicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessage)
                .SetName("SufficientWithValidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.NotApplicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("SufficientWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.NotApplicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("SufficientWithValidDetailedAssessmentAndCalculationWithoutOutput");
        }

        private static IEnumerable<TestCaseData> GetSimpleAssessmentResultProbabilityNegligibleConfigurations()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            string expectedErrorMessage = string.Empty;
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, double.NaN, null,
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, 0.0, null,
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, double.NaN, null,
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, 0.0, null, expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithValidLayerTwoAAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithValidLayerTwoAAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithValidLayerTwoAAndCalculationWithoutOutput");
        }

        private static IEnumerable<TestCaseData> GetSimpleAssessmentResultAssessFurtherOrNoneAndCalculationNullConfigurations()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            const string expectedErrorMessage = "Er moet een maatgevende berekening voor dit vak worden geselecteerd.";

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.AssessFurther, double.NaN, null,
                                          expectedErrorMessage)
                .SetName("AssessFurtherWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.AssessFurther, 0.0, null,
                                          expectedErrorMessage)
                .SetName("AssessFurtherWithValidLayerTwoAAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.None, double.NaN, null,
                                          expectedErrorMessage)
                .SetName("NoneWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.None, 0.0, null,
                                          expectedErrorMessage)
                .SetName("NoneWithValidLayerTwoAAndNoCalculation");
        }

        private static IEnumerable<TestCaseData> GetSimpleAssessmentResultAssessFurtherOrNoneAndCalculationWithoutOutputConfigurations()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            const string expectedErrorMessage = "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.";

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.AssessFurther, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("AssessFurtherWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.AssessFurther, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("AssessFurtherWithValidDetailedAssessmentAndCalculationWithoutOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.None, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("NoneWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.None, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("NoneWithValidDetailedAssessmentAndCalculationWithoutOutput");
        }

        private static IEnumerable<TestCaseData> GetSimpleAssessmentResultAssessFurtherOrNoneAndCalculationWithOutputConfigurations()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            const string expectedErrorMessageOutputInvalid = "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.";
            string expectedEmptyErrorMessage = string.Empty;

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.AssessFurther, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessageOutputInvalid)
                .SetName("AssessFurtherWithInvalidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.AssessFurther, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedEmptyErrorMessage)
                .SetName("AssessFurtherWithValidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.None, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessageOutputInvalid)
                .SetName("NoneWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.None, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedEmptyErrorMessage)
                .SetName("NoneWithValidDetailedAssessmentAndCalculationWithOutput");
        }

        #endregion

        #region Test cases simple assessment (validity only)

        private static IEnumerable GetSimpleAssessmentResultValidityOnlyIsNotApplicableConfigurations()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            string expectedErrorMessage = string.Empty;

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, double.NaN, null, expectedErrorMessage)
                .SetName("NotApplicableWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, 0.0, null, expectedErrorMessage)
                .SetName("NotApplicableWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessage)
                .SetName("NotApplicableWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessage)
                .SetName("NotApplicableWithValidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("NotApplicableWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("NotApplicableWithValidDetailedAssessmentAndCalculationWithoutOutput");
        }

        private static IEnumerable GetSimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationNullConfigurations()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            const string expectedErrorMessage = "Er moet een maatgevende berekening voor dit vak worden geselecteerd.";

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, double.NaN, null,
                                          expectedErrorMessage)
                .SetName("NoneWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, 0.0, null,
                                          expectedErrorMessage)
                .SetName("NoneWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, double.NaN, null,
                                          expectedErrorMessage)
                .SetName("ApplicableWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, 0.0, null,
                                          expectedErrorMessage)
                .SetName("ApplicableWithValidDetailedAssessmentAndNoCalculation");
        }

        private static IEnumerable GetSimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationWithoutOutputConfigurations()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            const string expectedErrorMessage = "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.";

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("NoneWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("NoneWithValidDetailedAssessmentAndCalculationWithoutOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("ApplicableWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("ApplicableValidDetailedAssessmentAndCalculationWithoutOutput");
        }

        private static IEnumerable SimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationWithOutput()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            const string expectedErrorMessageOutputInvalid = "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.";
            string expectedEmptyErrorMessage = string.Empty;

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessageOutputInvalid)
                .SetName("NoneWithInvalidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedEmptyErrorMessage)
                .SetName("NoneWithValidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessageOutputInvalid)
                .SetName("ApplicableWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedEmptyErrorMessage)
                .SetName("ApplicableWithValidDetailedAssessmentAndCalculationWithOutput");
        }

        #endregion
    }
}