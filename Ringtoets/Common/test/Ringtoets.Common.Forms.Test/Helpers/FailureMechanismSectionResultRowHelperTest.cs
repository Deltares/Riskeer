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
using Ringtoets.Common.Data.Calculation;
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
            TestDelegate call = () => FailureMechanismSectionResultRowHelper.ShowAssessmentLayerTwoAErrors(null, true, 0.0, calculationStub);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("dataGridViewCell", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource("ValidShowAssessmentLayerTwoAErrorsArguments")]
        public void ShowAssessmentLayerTwoAErrors_SetsErrorText(DataGridViewCell dataGridViewCell,
                                                                bool passedAssessmentLayerOne,
                                                                double assessmentLayerTwoA,
                                                                ICalculation normativeCalculation,
                                                                string expectedErrorText)
        {
            // Call
            FailureMechanismSectionResultRowHelper.ShowAssessmentLayerTwoAErrors(dataGridViewCell,
                                                                                 passedAssessmentLayerOne,
                                                                                 assessmentLayerTwoA,
                                                                                 normativeCalculation);

            // Assert
            Assert.AreEqual(expectedErrorText, dataGridViewCell.ErrorText);
        }

        private static IEnumerable ValidShowAssessmentLayerTwoAErrorsArguments()
        {
            var dataGridViewCell = new TestDataGridViewCell
            {
                ErrorText = "Default text"
            };

            yield return new TestCaseData(dataGridViewCell, true, double.NaN, new CalculationWithoutOutput(),
                                          string.Empty);
            yield return new TestCaseData(dataGridViewCell, true, 0.0, new CalculationWithoutOutput(),
                                          string.Empty);
            yield return new TestCaseData(dataGridViewCell, true, double.NaN, null, string.Empty);
            yield return new TestCaseData(dataGridViewCell, true, 0.0, null, string.Empty);
            yield return new TestCaseData(dataGridViewCell, true, double.NaN, new CalculationWithOutput(),
                                          string.Empty);
            yield return new TestCaseData(dataGridViewCell, true, 0.0, new CalculationWithOutput(),
                                          string.Empty);

            yield return new TestCaseData(dataGridViewCell, false, double.NaN, null, "Er moet een maatgevende berekening voor dit vak worden geselecteerd.");
            yield return new TestCaseData(dataGridViewCell, false, 0.0, null, "Er moet een maatgevende berekening voor dit vak worden geselecteerd.");

            yield return new TestCaseData(dataGridViewCell, false, double.NaN, new CalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.");
            yield return new TestCaseData(dataGridViewCell, false, 0.0, new CalculationWithoutOutput(),
                                          "De maatgevende berekening voor dit vak moet nog worden uitgevoerd.");

            yield return new TestCaseData(dataGridViewCell, false, double.NaN, new CalculationWithOutput(),
                                          "De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.");
            yield return new TestCaseData(dataGridViewCell, false, 0.0, new CalculationWithOutput(),
                                          string.Empty);
        }

        private class TestDataGridViewCell : DataGridViewCell {}

        private class CalculationWithOutput : ICalculation
        {
            public string Name { get; set; }
            public string Comments { get; set; }

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
            public string Comments { get; set; }

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
    }
}