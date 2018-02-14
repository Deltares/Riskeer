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
using System.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionResultRowHelperTest
    {
        [Test]
        public void ShowAssessmentLayerTwoAErrors_WithAssessmentLayerOneAndDataGridViewCellNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => FailureMechanismSectionResultRowHelper.SetAssessmentLayerTwoAError(null,
                                                                                                         AssessmentLayerOneState.Sufficient,
                                                                                                         0.0,
                                                                                                         calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("dataGridViewCell", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(AssessmentLayerOneStateIsSufficient))]
        [TestCaseSource(nameof(AssessmentLayerOneStateIsNotSufficientAndCalculationNull))]
        [TestCaseSource(nameof(AssessmentLayerOneStateIsNotSufficientAndCalculationWithoutOutput))]
        [TestCaseSource(nameof(AssessmentLayerOneStateIsNotSufficientAndCalculationWithOutput))]
        public void SetAssessmentLayerTwoAError_WithAssessmentLayerOne_SetsErrorText(DataGridViewCell dataGridViewCell,
                                                                                     AssessmentLayerOneState passedAssessmentLayerOne,
                                                                                     double assessmentLayerTwoA,
                                                                                     ICalculation normativeCalculation,
                                                                                     string expectedErrorText)
        {
            // Call
            FailureMechanismSectionResultRowHelper.SetAssessmentLayerTwoAError(dataGridViewCell,
                                                                               passedAssessmentLayerOne,
                                                                               assessmentLayerTwoA,
                                                                               normativeCalculation);

            // Assert
            Assert.AreEqual(expectedErrorText, dataGridViewCell.ErrorText);
        }

        [Test]
        public void ShowAssessmentLayerTwoAErrors_WithSimpleAssessmentValidityOnlyAndDataGridViewCellNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => FailureMechanismSectionResultRowHelper.SetAssessmentLayerTwoAError(null,
                                                                                                         SimpleAssessmentResultValidityOnlyType.NotApplicable,
                                                                                                         0.0,
                                                                                                         calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("dataGridViewCell", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(SimpleAssessmentResultValidityOnlyIsNotApplicable))]
        [TestCaseSource(nameof(SimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationNull))]
        [TestCaseSource(nameof(SimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationWithoutOutput))]
        [TestCaseSource(nameof(SimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationWithOutput))]
        public void SetAssessmentLayerTwoAError_WithSimpleAssessmentValidityOnly_SetsErrorText(DataGridViewCell dataGridViewCell,
                                                                                               SimpleAssessmentResultValidityOnlyType simpleAssessmentResult,
                                                                                               double assessmentLayerTwoA,
                                                                                               ICalculation normativeCalculation,
                                                                                               string expectedErrorText)
        {
            // Call
            FailureMechanismSectionResultRowHelper.SetAssessmentLayerTwoAError(dataGridViewCell,
                                                                               simpleAssessmentResult,
                                                                               assessmentLayerTwoA,
                                                                               normativeCalculation);

            // Assert
            Assert.AreEqual(expectedErrorText, dataGridViewCell.ErrorText);
        }

        private class TestDataGridViewCell : DataGridViewCell {}

        #region Test cases assessment layer one

        private static IEnumerable AssessmentLayerOneStateIsSufficient()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, double.NaN, null, string.Empty)
                .SetName("SufficientWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, 0.0, null, string.Empty)
                .SetName("SufficientWithValidLayerTwoAAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("SufficientWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("SufficientWithValidLayerTwoAAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          string.Empty)
                .SetName("SufficientWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          string.Empty)
                .SetName("SufficientWithValidLayerTwoAAndCalculationWithoutOutput");
        }

        private static IEnumerable AssessmentLayerOneStateIsNotSufficientAndCalculationNull()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, double.NaN, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NotAssessedWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, 0.0, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NotAssessedWithValidLayerTwoAAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, double.NaN, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NeedsDetailedAssessmentWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, 0.0, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NeedsDetailedAssessmentWithValidLayerTwoAAndNoCalculation");
        }

        private static IEnumerable AssessmentLayerOneStateIsNotSufficientAndCalculationWithoutOutput()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NotAssessedWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NotAssessedWithValidLayerTwoAAndCalculationWithoutOutput");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NeedsDetailedAssessmentWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NeedsDetailedAssessmentValidLayerTwoAAndCalculationWithoutOutput");
        }

        private static IEnumerable AssessmentLayerOneStateIsNotSufficientAndCalculationWithOutput()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.")
                .SetName("NotAssessedWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NotAssessedWithValidLayerTwoAAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.")
                .SetName("NeedsDetailedAssessmentWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NeedsDetailedAssessmentWithValidLayerTwoAAndCalculationWithOutput");
        }

        #endregion

        #region Test cases simple assessment (validity only)

        private static IEnumerable SimpleAssessmentResultValidityOnlyIsNotApplicable()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, double.NaN, null, string.Empty)
                .SetName("NotApplicableWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, 0.0, null, string.Empty)
                .SetName("NotApplicableWithValidLayerTwoAAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NotApplicableWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NotApplicableWithValidLayerTwoAAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          string.Empty)
                .SetName("NotApplicableWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          string.Empty)
                .SetName("NotApplicableWithValidLayerTwoAAndCalculationWithoutOutput");
        }

        private static IEnumerable SimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationNull()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, double.NaN, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NoneWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, 0.0, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NoneWithValidLayerTwoAAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, double.NaN, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("ApplicableWithInvalidLayerTwoAAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, 0.0, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("ApplicableWithValidLayerTwoAAndNoCalculation");
        }

        private static IEnumerable SimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationWithoutOutput()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NoneWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NoneWithValidLayerTwoAAndCalculationWithoutOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("ApplicableWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("ApplicableValidLayerTwoAAndCalculationWithoutOutput");
        }

        private static IEnumerable SimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationWithOutput()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.")
                .SetName("NoneWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NoneWithValidLayerTwoAAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.")
                .SetName("ApplicableWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("ApplicableWithValidLayerTwoAAndCalculationWithOutput");
        }

        #endregion
    }
}