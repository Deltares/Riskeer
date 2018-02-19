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
using System.Collections;
using System.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionResultRowHelperTest
    {
        [Test]
        public void SetDetailedAssessmentError_WithAssessmentLayerOneAndDataGridViewCellNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(null,
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
        public void SetDetailedAssessmentError_WithAssessmentLayerOne_SetsErrorText(DataGridViewCell dataGridViewCell,
                                                                                     AssessmentLayerOneState passedAssessmentLayerOne,
                                                                                     double detailedAssessmentProbability,
                                                                                     ICalculation normativeCalculation,
                                                                                     string expectedErrorText)
        {
            // Call
            FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(dataGridViewCell,
                                                                               passedAssessmentLayerOne,
                                                                               detailedAssessmentProbability,
                                                                               normativeCalculation);

            // Assert
            Assert.AreEqual(expectedErrorText, dataGridViewCell.ErrorText);
        }

        [Test]
        public void SetDetailedAssessmentError_WithSimpleAssessmentValidityOnlyAndDataGridViewCellNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(null,
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
        public void SetAssessmentDetailedAssessmentError_WithSimpleAssessmentValidityOnly_SetsErrorText(DataGridViewCell dataGridViewCell,
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

        #region Test cases assessment layer one

        private static IEnumerable AssessmentLayerOneStateIsSufficient()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, double.NaN, null, string.Empty)
                .SetName("SufficientWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, 0.0, null, string.Empty)
                .SetName("SufficientWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("SufficientWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("SufficientWithValidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          string.Empty)
                .SetName("SufficientWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          string.Empty)
                .SetName("SufficientWithValidDetailedAssessmentAndCalculationWithoutOutput");
        }

        private static IEnumerable AssessmentLayerOneStateIsNotSufficientAndCalculationNull()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, double.NaN, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NotAssessedWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, 0.0, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NotAssessedWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, double.NaN, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NeedsDetailedAssessmentWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, 0.0, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NeedsDetailedAssessmentWithValidDetailedAssessmentAndNoCalculation");
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
                .SetName("NotAssessedWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NotAssessedWithValidDetailedAssessmentAndCalculationWithoutOutput");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NeedsDetailedAssessmentWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NeedsDetailedAssessmentValidDetailedAssessmentAndCalculationWithoutOutput");
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
                .SetName("NotAssessedWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NotAssessedWithValidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.")
                .SetName("NeedsDetailedAssessmentWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NeedsDetailedAssessmentWithValidDetailedAssessmentAndCalculationWithOutput");
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
                .SetName("NotApplicableWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, 0.0, null, string.Empty)
                .SetName("NotApplicableWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NotApplicableWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NotApplicableWithValidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          string.Empty)
                .SetName("NotApplicableWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.NotApplicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          string.Empty)
                .SetName("NotApplicableWithValidDetailedAssessmentAndCalculationWithoutOutput");
        }

        private static IEnumerable SimpleAssessmentResultValidityOnlyIsNotApplicableAndCalculationNull()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, double.NaN, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NoneWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, 0.0, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("NoneWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, double.NaN, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("ApplicableWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, 0.0, null,
                                          "Er moet een maatgevende berekening voor dit vak worden geselecteerd.")
                .SetName("ApplicableWithValidDetailedAssessmentAndNoCalculation");
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
                .SetName("NoneWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NoneWithValidDetailedAssessmentAndCalculationWithoutOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("ApplicableWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("ApplicableValidDetailedAssessmentAndCalculationWithoutOutput");
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
                .SetName("NoneWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.None, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("NoneWithValidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.")
                .SetName("ApplicableWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultValidityOnlyType.Applicable, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          string.Empty)
                .SetName("ApplicableWithValidDetailedAssessmentAndCalculationWithOutput");
        }

        #endregion
    }
}