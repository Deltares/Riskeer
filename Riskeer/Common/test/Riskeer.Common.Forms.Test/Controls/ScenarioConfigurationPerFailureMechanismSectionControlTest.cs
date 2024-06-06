// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Controls;

namespace Riskeer.Common.Forms.Test.Controls
{
    [TestFixture]
    public class ScenarioConfigurationPerFailureMechanismSectionControlTest
    {
        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void Constructor_ControlsCorrectlyInitialized()
        {
            // Call
            ShowScenarioConfigurationPerFailureMechanismSectionControl();

            // Assert
            Label parameterALabel = GetParameterALabel();
            Assert.AreEqual("Mechanismegevoelige fractie a [-]", parameterALabel.Text);

            Label lengthEffectNRoundedLabel = GetLengthEffectNRoundedLabel();
            Assert.AreEqual("Lengte-effect parameter Nvak* [-]", lengthEffectNRoundedLabel.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.IsFalse(lengthEffectNRoundedTextBox.Enabled);
        }

        [Test]
        public void Constructor_ToolTipsCorrectlyInitialized()
        {
            // Call
            ScenarioConfigurationPerFailureMechanismSectionControl view = ShowScenarioConfigurationPerFailureMechanismSectionControl();

            // Assert
            Label parameterALabel = GetParameterALabel();
            var parameterAToolTip = TypeUtils.GetField<ToolTip>(view, "parameterAToolTip");

            Assert.AreEqual("Mechanismegevoelige fractie van het dijkvak.",
                            parameterAToolTip.GetToolTip(parameterALabel));
            Assert.AreEqual(5000, parameterAToolTip.AutoPopDelay);
            Assert.AreEqual(100, parameterAToolTip.InitialDelay);
            Assert.AreEqual(100, parameterAToolTip.ReshowDelay);

            Label lengthEffectNRoundedLabel = GetLengthEffectNRoundedLabel();
            var lengthEffectNRoundedToolTip = TypeUtils.GetField<ToolTip>(view, "lengthEffectNRoundedToolTip");
            Assert.AreEqual("De parameter 'Nvak*' die het lengte-effect beschrijft in de berekening van de faalkans per vak in de semi-probabilistische toets.",
                            lengthEffectNRoundedToolTip.GetToolTip(lengthEffectNRoundedLabel));
            Assert.AreEqual(5000, lengthEffectNRoundedToolTip.AutoPopDelay);
            Assert.AreEqual(100, lengthEffectNRoundedToolTip.InitialDelay);
            Assert.AreEqual(100, lengthEffectNRoundedToolTip.ReshowDelay);
        }

        [Test]
        public void SetScenarioConfiguration_ScenarioConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            ScenarioConfigurationPerFailureMechanismSectionControl control = ShowScenarioConfigurationPerFailureMechanismSectionControl();

            // Call
            void Call() => control.SetData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("scenarioConfiguration", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenControl_WhenSettingConfiguration_ThenControlUpdated()
        {
            // Given
            const double a = 0.7;
            const double b = 300.0;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, (RoundedDouble) a);

            ScenarioConfigurationPerFailureMechanismSectionControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(b);

            // Precondition
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            Assert.IsEmpty(parameterATextBox.Text);
            Assert.IsFalse(parameterATextBox.Enabled);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);

            // When
            settingsControl.SetData(scenarioConfiguration);

            // Then
            Assert.AreEqual("0,700", parameterATextBox.Text);
            Assert.IsTrue(parameterATextBox.Enabled);

            Assert.AreEqual("1,23", lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenControlWithConfiguration_WhenClearingData_ThenControlUpdated()
        {
            // Given
            const double a = 0.7;
            const double b = 300.0;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, (RoundedDouble) a);

            ScenarioConfigurationPerFailureMechanismSectionControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(b);
            settingsControl.SetData(scenarioConfiguration);

            // Precondition
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            Assert.AreEqual("0,700", parameterATextBox.Text);
            Assert.IsTrue(parameterATextBox.Enabled);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual("1,23", lengthEffectNRoundedTextBox.Text);

            // When
            settingsControl.ClearData();

            // Then
            Assert.IsEmpty(parameterATextBox.Text);
            Assert.IsFalse(parameterATextBox.Enabled);

            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        public void GivenControlWithError_WhenClearingData_ThenErrorCleared()
        {
            // Given
            var random = new Random(21);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, random.NextRoundedDouble());

            ScenarioConfigurationPerFailureMechanismSectionControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(random.NextDouble());
            settingsControl.SetData(scenarioConfiguration);

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            parameterATextBoxTester.Enter("NotADouble");

            // Precondition
            ErrorProvider errorProvider = GetErrorProvider(settingsControl);
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            string errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsNotEmpty(errorMessage);

            // When
            settingsControl.ClearData();

            // Then
            errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        public void GivenControlWithError_WhenEnteringValidData_ThenErrorClearedAndControlsUpdated()
        {
            // Given
            var random = new Random(21);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, random.NextRoundedDouble());

            ScenarioConfigurationPerFailureMechanismSectionControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(random.NextDouble());
            settingsControl.SetData(scenarioConfiguration);

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            parameterATextBoxTester.Enter("NotADouble");

            // Precondition
            ErrorProvider errorProvider = GetErrorProvider(settingsControl);
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            string errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsNotEmpty(errorMessage);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);

            // When
            parameterATextBoxTester.Enter("0,7");

            // Then
            errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsEmpty(errorMessage);
            Assert.IsNotEmpty(lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        public void GivenControlWithError_WhenSettingData_ThenErrorCleared()
        {
            // Given
            var random = new Random(21);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, random.NextRoundedDouble());

            ScenarioConfigurationPerFailureMechanismSectionControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(random.NextDouble());
            settingsControl.SetData(scenarioConfiguration);

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            parameterATextBoxTester.Enter("NotADouble");

            // Precondition
            ErrorProvider errorProvider = GetErrorProvider(settingsControl);
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            string errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsNotEmpty(errorMessage);

            // When
            var newConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(),
                                                                                           random.NextRoundedDouble());
            settingsControl.SetData(newConfiguration);

            // Then
            errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        public void GivenControlWithoutError_WhenEnteringInvalidData_ThenErrorSetAndControlsUpdated()
        {
            // Given
            var random = new Random(21);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, random.NextRoundedDouble());

            ScenarioConfigurationPerFailureMechanismSectionControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(random.NextDouble());
            settingsControl.SetData(scenarioConfiguration);

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();

            // Precondition
            ErrorProvider errorProvider = GetErrorProvider(settingsControl);
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            string errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsEmpty(errorMessage);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.IsNotEmpty(lengthEffectNRoundedTextBox.Text);

            // When
            parameterATextBoxTester.Enter("NotADouble");

            // Then
            errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsNotEmpty(errorMessage);
            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        public void GivenControlWithConfiguration_WhenSettingNewAValue_ThenControlsUpdatedAndObserversNotified()
        {
            // Given
            const double a = 0.4;
            const double b = 300;

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, (RoundedDouble) a);
            scenarioConfiguration.Attach(observer);

            ScenarioConfigurationPerFailureMechanismSectionControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(b);
            settingsControl.SetData(scenarioConfiguration);

            // Precondition
            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            Assert.AreEqual("0,400", parameterATextBoxTester.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual("1,13", lengthEffectNRoundedTextBox.Text);

            // When
            parameterATextBoxTester.Enter("0,7");

            // Then
            Assert.AreEqual("1,23", lengthEffectNRoundedTextBox.Text);
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenControlWithConfiguration_WhenSettingInvalidAValueAndEscPressed_ThenControlsSetToInitialValue()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            const double a = 0.4;
            const string initialAValue = "0,400";
            const double b = 300;
            const string lengthEffectNRoundedValue = "1,13";

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, (RoundedDouble) a);
            scenarioConfiguration.Attach(observer);

            ScenarioConfigurationPerFailureMechanismSectionControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(b);
            settingsControl.SetData(scenarioConfiguration);

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            const Keys keyData = Keys.Escape;

            var parameterATextBox = (TextBox) parameterATextBoxTester.TheObject;
            parameterATextBox.TextChanged += (sender, args) =>
            {
                parameterATextBoxTester.FireEvent("KeyDown", new KeyEventArgs(keyData));
            };

            // Precondition
            Assert.AreEqual(initialAValue, parameterATextBoxTester.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual(lengthEffectNRoundedValue, lengthEffectNRoundedTextBox.Text);

            // When
            parameterATextBox.Text = "NotADouble";

            // Then
            Assert.AreEqual(initialAValue, parameterATextBoxTester.Text);
            Assert.AreEqual(lengthEffectNRoundedValue, lengthEffectNRoundedTextBox.Text);
            mocks.VerifyAll();
        }

        private static Label GetParameterALabel()
        {
            return (Label) new LabelTester("parameterALabel").TheObject;
        }

        private static Label GetLengthEffectNRoundedLabel()
        {
            return (Label) new LabelTester("lengthEffectNRoundedLabel").TheObject;
        }

        private static TextBoxTester GetParameterATextBoxTester()
        {
            return new TextBoxTester("parameterATextBox");
        }

        private static TextBox GetLengthEffectNRoundedTextBox()
        {
            return (TextBox) new ControlTester("lengthEffectNRoundedTextBox").TheObject;
        }

        private static ErrorProvider GetErrorProvider(ScenarioConfigurationPerFailureMechanismSectionControl settingsControl)
        {
            return TypeUtils.GetField<ErrorProvider>(settingsControl, "errorProvider");
        }

        private ScenarioConfigurationPerFailureMechanismSectionControl ShowScenarioConfigurationPerFailureMechanismSectionControl()
        {
            var random = new Random(21);
            return ShowScenarioConfigurationPerFailureMechanismSectionControl(random.NextDouble());
        }

        private ScenarioConfigurationPerFailureMechanismSectionControl ShowScenarioConfigurationPerFailureMechanismSectionControl(double b)
        {
            var control = new ScenarioConfigurationPerFailureMechanismSectionControl(b);

            testForm.Controls.Add(control);
            testForm.Show();

            return control;
        }
    }
}