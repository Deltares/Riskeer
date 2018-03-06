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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
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
                                                                                                      double detailedAssessmentResult,
                                                                                                      ICalculation normativeCalculation,
                                                                                                      string expectedErrorText)
        {
            // Call
            FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(dataGridViewCell,
                                                                              simpleAssessmentResult,
                                                                              detailedAssessmentResult,
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
                                                                                                                      double detailedAssessmentResult,
                                                                                                                      ICalculation normativeCalculation,
                                                                                                                      string expectedErrorText)
        {
            // Call
            FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(dataGridViewCell,
                                                                              simpleAssessmentResult,
                                                                              detailedAssessmentResult,
                                                                              normativeCalculation);

            // Assert
            Assert.AreEqual(expectedErrorText, dataGridViewCell.ErrorText);
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationNullConfigurations))]
        [TestCaseSource(nameof(GetCalculationWithoutOutputConfigurations))]
        [TestCaseSource(nameof(GetCalculationWithOutputConfigurations))]
        public void GetAssessmentDetailedAssessmentError_WithVariousConfigurations_GetsExpectedErrorText(double detailedAssessmentResult,
                                                                                                         ICalculation normativeCalculation,
                                                                                                         string expectedErrorText)
        {
            // Call
            string errorText = FailureMechanismSectionResultRowHelper.GetDetailedAssessmentError(detailedAssessmentResult,
                                                                                                 normativeCalculation);

            // Assert
            Assert.AreEqual(expectedErrorText, errorText);
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
        [TestCase(SimpleAssessmentResultValidityOnlyType.None, false)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable, false)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.NotApplicable, true)]
        public void SimpleAssessmentIsSufficient_WithSimpleAssessmentResultValidityOnlyType_ReturnsExpectedResult(
            SimpleAssessmentResultValidityOnlyType simpleAssessmentResult, bool expectedSufficiency)
        {
            // Call
            bool isSufficient = FailureMechanismSectionResultRowHelper.SimpleAssessmentIsSufficient(simpleAssessmentResult);

            // Assert
            Assert.AreEqual(expectedSufficiency, isSufficient);
        }

        [Test]
        [TestCase(DetailedAssessmentResultType.NotAssessed, false)]
        [TestCase(DetailedAssessmentResultType.Probability, true)]
        public void DetailedAssessmentResultIsProbability_WithDetailedAssessmentResultType_ReturnsExpectedResult(
            DetailedAssessmentResultType detailedAssessmentResult, bool expectedIsProbability)
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
            TestDelegate test = () => FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(
                null,
                new Random(39).NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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
            TestDelegate test = () => FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(columnStateDefinition,
                                                                                                           (FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'assemblyCategoryGroup' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, 255, 255, 255)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, 255, 255, 255)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, 255, 255, 255)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, 255, 0, 0)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, 255, 153, 0)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, 204, 192, 218)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, 255, 255, 0)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, 118, 147, 60)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, 0, 255, 0)]
        public void SetAssemblyCategoryGroupStyle_WithFailureMechanismSectionAssemblyCategoryGroup_UpdatesColumnStyle(
            FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup, int r, int g, int b)
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();

            // Call
            FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(columnStateDefinition, assemblyCategoryGroup);

            // Assert
            Assert.AreEqual(r, columnStateDefinition.Style.BackgroundColor.R);
            Assert.AreEqual(g, columnStateDefinition.Style.BackgroundColor.G);
            Assert.AreEqual(b, columnStateDefinition.Style.BackgroundColor.B);
            Assert.AreEqual(255, columnStateDefinition.Style.BackgroundColor.A);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), columnStateDefinition.Style.TextColor);
        }

        [Test]
        public void SetColumnState_DataGridViewColumnStateDefinitionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultRowHelper.SetColumnState(
                null,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("columnStateDefinition", exception.ParamName);
        }

        [Test]
        public void SetColumnState_ShouldDisableFalse_UpdatesColumnState()
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();

            // Call
            FailureMechanismSectionResultRowHelper.SetColumnState(
                columnStateDefinition,
                false);

            // Assert
            Assert.IsFalse(columnStateDefinition.ReadOnly);
            Assert.AreEqual(CellStyle.Enabled, columnStateDefinition.Style);
        }

        [Test]
        public void SetColumnState_ShouldDisableTrue_UpdatesColumnState()
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();

            // Call
            FailureMechanismSectionResultRowHelper.SetColumnState(
                columnStateDefinition,
                true);

            // Assert
            Assert.IsTrue(columnStateDefinition.ReadOnly);
            Assert.AreEqual(CellStyle.Disabled, columnStateDefinition.Style);
        }

        [Test]
        public void EnableColumn_DataGridViewColumnStateDefinitionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultRowHelper.EnableColumn(
                null,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("columnStateDefinition", exception.ParamName);
        }

        [Test]
        public void EnableColumn_WithValidData_UpdatesColumnState()
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();
            bool readOnly = new Random(39).NextBoolean();

            // Call
            FailureMechanismSectionResultRowHelper.EnableColumn(
                columnStateDefinition,
                readOnly);

            // Assert
            Assert.AreEqual(readOnly, columnStateDefinition.ReadOnly);
            Assert.AreEqual(CellStyle.Enabled, columnStateDefinition.Style);
        }

        [Test]
        public void DisableColumn_DataGridViewColumnStateDefinitionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultRowHelper.DisableColumn(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("columnStateDefinition", exception.ParamName);
        }

        [Test]
        public void DisableColumn_WithValidData_UpdatesColumnState()
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();

            // Call
            FailureMechanismSectionResultRowHelper.DisableColumn(columnStateDefinition);

            // Assert
            Assert.IsTrue(columnStateDefinition.ReadOnly);
            Assert.AreEqual(CellStyle.Disabled, columnStateDefinition.Style);
        }

        private class TestDataGridViewCell : DataGridViewCell {}

        #region Test cases

        private static IEnumerable<TestCaseData> GetCalculationNullConfigurations()
        {
            const string expectedErrorMessage = "Er moet een maatgevende berekening voor dit vak worden geselecteerd.";

            yield return new TestCaseData(double.NaN, null,
                                          expectedErrorMessage)
                .SetName("InvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(0.0, null,
                                          expectedErrorMessage)
                .SetName("ValidDetailedAssessmentAndNoCalculation");
        }

        private static IEnumerable<TestCaseData> GetCalculationWithoutOutputConfigurations()
        {
            const string expectedErrorMessage = "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.";

            yield return new TestCaseData(double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("InvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("ValidDetailedAssessmentAndCalculationWithoutOutput");
        }

        private static IEnumerable<TestCaseData> GetCalculationWithOutputConfigurations()
        {
            const string expectedErrorMessageOutputInvalid = "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.";
            string expectedEmptyErrorMessage = string.Empty;

            yield return new TestCaseData(double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessageOutputInvalid)
                .SetName("InvalidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedEmptyErrorMessage)
                .SetName("ValidDetailedAssessmentAndCalculationWithOutput");
        }

        #endregion

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
                .SetName("NotApplicableWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.NotApplicable, 0.0, null, expectedErrorMessage)
                .SetName("NotApplicableWithValidDetailedAssessmentAndNoCalculation");

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
                .SetName("ProbabilityNegligibleWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, 0.0, null, expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithInvalidDetailedAssessmentAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithOutput(),
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithValidDetailedAssessmentAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, double.NaN,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithInvalidDetailedAssessmentAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.ProbabilityNegligible, 0.0,
                                          CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                                          expectedErrorMessage)
                .SetName("ProbabilityNegligibleWithValidDetailedAssessmentAndCalculationWithoutOutput");
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
                .SetName("AssessFurtherWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.AssessFurther, 0.0, null,
                                          expectedErrorMessage)
                .SetName("AssessFurtherWithValidDetailedAssessmentAndNoCalculation");

            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.None, double.NaN, null,
                                          expectedErrorMessage)
                .SetName("NoneWithInvalidDetailedAssessmentAndNoCalculation");
            yield return new TestCaseData(dataGridViewCell, SimpleAssessmentResultType.None, 0.0, null,
                                          expectedErrorMessage)
                .SetName("NoneWithValidDetailedAssessmentAndNoCalculation");
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
                .SetName("ApplicableWithValidDetailedAssessmentAndCalculationWithoutOutput");
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