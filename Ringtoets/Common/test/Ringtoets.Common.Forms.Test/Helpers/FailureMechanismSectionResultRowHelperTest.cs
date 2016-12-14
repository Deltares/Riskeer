// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionResultRowHelperTest
    {
        [Test]
        public void ShowAssessmentLayerTwoAErrors_DataGridViewCellNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationStub = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => FailureMechanismSectionResultRowHelper.SetAssessmentLayerTwoAError(null,
                                                                                                         AssessmentLayerOneState.Sufficient,
                                                                                                         0.0, calculationStub);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("dataGridViewCell", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource("AssessmentLayerOneStateIsSufficient")]
        [TestCaseSource("AssessmentLayerOneStateIsNotSufficientAndCalculationNull")]
        [TestCaseSource("AssessmentLayerOneStateIsNotSufficientAndCalculationWithoutOutput")]
        [TestCaseSource("AssessmentLayerOneStateIsNotSufficientAndCalculationWithOutput")]
        public void SetAssessmentLayerTwoAError_SetsErrorText(DataGridViewCell dataGridViewCell,
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

        private class TestDataGridViewCell : DataGridViewCell {}

        private class CalculationWithOutput : ICalculation
        {
            public string Name { get; set; }
            public Comment Comments { get; private set; }

            public bool HasOutput
            {
                get
                {
                    return true;
                }
            }

            public void Attach(IObserver observer) {}
            public void Detach(IObserver observer) {}
            public void NotifyObservers() {}
            public void ClearOutput() {}
        }

        private class CalculationWithoutOutput : ICalculation
        {
            public string Name { get; set; }
            public Comment Comments { get; private set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void Attach(IObserver observer) {}
            public void Detach(IObserver observer) {}
            public void NotifyObservers() {}
            public void ClearOutput() {}
        }

        #region Testcases

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
                                          new CalculationWithOutput(), string.Empty)
                .SetName("SufficientWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, 0.0,
                                          new CalculationWithOutput(), string.Empty)
                .SetName("SufficientWithValidLayerTwoAAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, double.NaN,
                                          new CalculationWithoutOutput(), string.Empty)
                .SetName("SufficientWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.Sufficient, 0.0,
                                          new CalculationWithoutOutput(), string.Empty)
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
                                          new CalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NotAssessedWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, 0.0, new CalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NotAssessedWithValidLayerTwoAAndCalculationWithoutOutput");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, double.NaN,
                                          new CalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.")
                .SetName("NeedsDetailedAssessmentWithInvalidLayerTwoAAndCalculationWithoutOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, 0.0,
                                          new CalculationWithoutOutput(),
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
                                          new CalculationWithOutput(),
                                          "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.")
                .SetName("NotAssessedWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NotAssessed, 0.0, new CalculationWithOutput(),
                                          string.Empty)
                .SetName("NotAssessedWithValidLayerTwoAAndCalculationWithOutput");

            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, double.NaN,
                                          new CalculationWithOutput(),
                                          "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.")
                .SetName("NeedsDetailedAssessmentWithInvalidLayerTwoAAndCalculationWithOutput");
            yield return new TestCaseData(dataGridViewCell, AssessmentLayerOneState.NoVerdict, 0.0,
                                          new CalculationWithOutput(), string.Empty)
                .SetName("NeedsDetailedAssessmentWithValidLayerTwoAAndCalculationWithOutput");
        }

        #endregion
    }
}