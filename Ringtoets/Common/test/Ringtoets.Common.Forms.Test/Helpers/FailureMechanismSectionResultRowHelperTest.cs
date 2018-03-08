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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionResultRowHelperTest
    {
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
        [TestCaseSource(typeof(FailureMechanismSectionResultRowTestHelper), nameof(FailureMechanismSectionResultRowTestHelper.CategoryGroupColorCases))]
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

        [Test]
        public void GetCategoryGroupDisplayname_InvalidValue_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname((FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'assemblyCategoryGroup' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, "")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, "-")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, "Iv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, "IIv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, "IIIv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, "IVv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, "Vv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, "VIv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, "VIIv")]
        public void GetCategoryGroupDisplayname_ValidValue_ReturnsDisplayName(FailureMechanismSectionAssemblyCategoryGroup categoryGroup,
                                                                              string expectedDisplayName)
        {
            // Call
            string displayName = FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(categoryGroup);

            // Assert
            Assert.AreEqual(expectedDisplayName, displayName);
        }

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
    }
}