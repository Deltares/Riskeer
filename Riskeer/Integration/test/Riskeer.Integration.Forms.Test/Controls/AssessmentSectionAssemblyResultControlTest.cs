// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.Forms.Controls;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Test.Controls
{
    [TestFixture]
    public class AssessmentSectionAssemblyResultControlTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var form = new Form())
            using (var resultControl = new AssessmentSectionAssemblyResultControl())
            {
                form.Controls.Add(resultControl);
                form.Show();

                // Assert
                Assert.IsInstanceOf<UserControl>(resultControl);
                Assert.IsTrue(resultControl.AutoSize);

                TableLayoutPanel panel = GetResultPanel(resultControl);
                Assert.AreEqual(2, panel.ColumnCount);
                Assert.AreEqual(2, panel.RowCount);

                var assemblyGroupDisplayNameLabel = (Label) panel.GetControlFromPosition(0, 0);
                Assert.AreEqual("Veiligheidsoordeel", assemblyGroupDisplayNameLabel.Text);
                Assert.IsTrue(assemblyGroupDisplayNameLabel.AutoSize);
                Assert.AreEqual(DockStyle.Left, assemblyGroupDisplayNameLabel.Dock);
                Assert.AreEqual(new Padding(5, 0, 5, 0), assemblyGroupDisplayNameLabel.Padding);
                Assert.AreEqual(new Padding(3), assemblyGroupDisplayNameLabel.Margin);

                var failureProbabilityDisplayNameLabel = (Label) panel.GetControlFromPosition(0, 1);
                Assert.AreEqual("Faalkans van het traject [1/jaar]", failureProbabilityDisplayNameLabel.Text);
                Assert.IsTrue(failureProbabilityDisplayNameLabel.AutoSize);
                Assert.AreEqual(DockStyle.Left, failureProbabilityDisplayNameLabel.Dock);
                Assert.AreEqual(new Padding(5, 0, 5, 0), failureProbabilityDisplayNameLabel.Padding);
                Assert.AreEqual(new Padding(3), failureProbabilityDisplayNameLabel.Margin);

                var groupLabel = (BorderedLabel) panel.GetControlFromPosition(1, 0);
                Assert.IsTrue(groupLabel.AutoSize);
                Assert.AreEqual(DockStyle.Left, groupLabel.Dock);
                Assert.AreEqual(new Padding(5, 0, 5, 0), groupLabel.Padding);
                Assert.AreEqual(new Padding(3), groupLabel.Margin);

                var probabilityLabel = (BorderedLabel) panel.GetControlFromPosition(1, 1);
                Assert.IsTrue(probabilityLabel.AutoSize);
                Assert.AreEqual(DockStyle.Left, probabilityLabel.Dock);
                Assert.AreEqual(new Padding(5, 0, 5, 0), probabilityLabel.Padding);
                Assert.AreEqual(new Padding(3), probabilityLabel.Margin);

                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ErrorIcon.ToBitmap(), errorProvider.Icon.ToBitmap());
                Assert.AreEqual(ErrorBlinkStyle.NeverBlink, errorProvider.BlinkStyle);

                Assert.IsEmpty(errorProvider.GetError(groupLabel));
                Assert.IsEmpty(errorProvider.GetError(probabilityLabel));
            }
        }

        [Test]
        public void SetAssemblyResult_ResultNull_ThrowsArgumentNullException()
        {
            using (var resultControl = new AssessmentSectionAssemblyResultControl())
            {
                // Call
                void Call() => resultControl.SetAssemblyResult(null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("result", exception.ParamName);
            }
        }

        [Test]
        public void SetAssemblyResult_WithResult_SetsValues()
        {
            // Setup
            var random = new Random(39);
            var result = new AssessmentSectionAssemblyResult(random.NextDouble(),
                                                             random.NextEnumValue<AssessmentSectionAssemblyGroup>());
            using (var resultControl = new AssessmentSectionAssemblyResultControl())
            {
                // Call
                resultControl.SetAssemblyResult(result);

                // Assert
                BorderedLabel groupLabel = GetGroupLabel(resultControl);
                AssertGroupLabel(result.AssemblyGroup, groupLabel);

                BorderedLabel probabilityLabel = GetProbabilityLabel(resultControl);
                AssertProbabilityLabel(result.Probability, probabilityLabel);
            }
        }

        [Test]
        public void ClearAssemblyResult_Always_ClearsResultOnControl()
        {
            // Setup
            var random = new Random(39);

            using (var resultControl = new AssessmentSectionAssemblyResultControl())
            {
                var result = new AssessmentSectionAssemblyResult(random.NextDouble(),
                                                                 random.NextEnumValue<AssessmentSectionAssemblyGroup>());
                resultControl.SetAssemblyResult(result);

                // Precondition
                BorderedLabel groupLabel = GetGroupLabel(resultControl);
                AssertGroupLabel(result.AssemblyGroup, groupLabel);

                BorderedLabel probabilityLabel = GetProbabilityLabel(resultControl);
                AssertProbabilityLabel(result.Probability, probabilityLabel);

                // Call
                resultControl.ClearAssemblyResult();

                // Assert
                Assert.AreEqual("-", groupLabel.Text);
                Assert.AreEqual(Color.White, groupLabel.BackColor);

                Assert.AreEqual("-", probabilityLabel.Text);
            }
        }

        [Test]
        public void SetError_ErrorMessageNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var resultControl = new AssessmentSectionAssemblyResultControl())
            {
                // Call
                TestDelegate test = () => resultControl.SetError(null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("errorMessage", exception.ParamName);
            }
        }

        [Test]
        [TestCase("random error 123")]
        [TestCase("")]
        public void SetError_WithErrorMessage_SetsErrorMessageOnControl(string errorMessage)
        {
            // Setup
            using (var resultControl = new AssessmentSectionAssemblyResultControl())
            {
                // Call
                resultControl.SetError(errorMessage);

                // Assert
                ErrorProvider errorProvider = GetErrorProvider(resultControl);

                BorderedLabel groupLabel = GetGroupLabel(resultControl);
                Assert.AreEqual(errorMessage, errorProvider.GetError(groupLabel));

                BorderedLabel probabilityLabel = GetProbabilityLabel(resultControl);
                Assert.AreEqual(errorMessage, errorProvider.GetError(probabilityLabel));
            }
        }

        [Test]
        public void GivenControlWithErrors_WhenErrorsCleared_ThenErrorsCleared()
        {
            // Given
            using (var resultControl = new AssessmentSectionAssemblyResultControl())
            {
                resultControl.SetError("Error");

                // Precondition
                ErrorProvider errorProvider = GetErrorProvider(resultControl);

                BorderedLabel groupLabel = GetGroupLabel(resultControl);
                Assert.IsNotEmpty(errorProvider.GetError(groupLabel));

                BorderedLabel probabilityLabel = GetProbabilityLabel(resultControl);
                Assert.IsNotEmpty(errorProvider.GetError(probabilityLabel));

                // When
                resultControl.ClearErrors();

                // Then
                Assert.IsEmpty(errorProvider.GetError(groupLabel));
                Assert.IsEmpty(errorProvider.GetError(probabilityLabel));
            }
        }

        private static void AssertGroupLabel(AssessmentSectionAssemblyGroup result, BorderedLabel groupLabel)
        {
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result), groupLabel.Text);
            Assert.AreEqual(AssessmentSectionAssemblyGroupColorHelper.GetAssessmentSectionAssemblyGroupColor(result),
                            groupLabel.BackColor);
        }

        private static void AssertProbabilityLabel(double probability, BorderedLabel probabilityLabel)
        {
            Assert.AreEqual(ProbabilityFormattingHelper.FormatWithDiscreteNumbers(probability),
                            probabilityLabel.Text);
        }

        private static BorderedLabel GetGroupLabel(AssessmentSectionAssemblyResultControl resultControl)
        {
            return (BorderedLabel) (GetResultPanel(resultControl)).GetControlFromPosition(1, 0);
        }

        private static BorderedLabel GetProbabilityLabel(AssessmentSectionAssemblyResultControl resultControl)
        {
            return (BorderedLabel) (GetResultPanel(resultControl)).GetControlFromPosition(1, 1);
        }

        private static TableLayoutPanel GetResultPanel(AssessmentSectionAssemblyResultControl resultControl)
        {
            return (TableLayoutPanel) resultControl.Controls["resultLayoutPanel"];
        }

        private static ErrorProvider GetErrorProvider(AssessmentSectionAssemblyResultControl resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "errorProvider");
        }
    }
}