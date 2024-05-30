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
using Riskeer.Common.Data.Test.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Controls;

namespace Riskeer.Common.Forms.Test.Controls
{
    [TestFixture]
    public class LengthEffectSettingsControlTest
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
        public void Constructor_LengthEffectControlsCorrectlyInitialized()
        {
            // Call
            ShowLengthEffectSettingsControl();

            // Assert
            var parameterALabel = (Label) new LabelTester("parameterALabel").TheObject;
            Assert.AreEqual("Mechanismegevoelige fractie a [-]", parameterALabel.Text);

            var lengthEffectNRoundedLabel = (Label) new LabelTester("lengthEffectNRoundedLabel").TheObject;
            Assert.AreEqual("Lengte-effect parameter Nvak* [-]", lengthEffectNRoundedLabel.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.IsFalse(lengthEffectNRoundedTextBox.Enabled);
        }

        [Test]
        public void SetScenarioConfiguration_ScenarioConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            LengthEffectSettingsControl control = ShowLengthEffectSettingsControl();

            // Call
            void Call() => control.SetData(null, random.NextDouble());

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

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();

            // Precondition
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            Assert.IsEmpty(parameterATextBox.Text);
            Assert.IsFalse(parameterATextBox.Enabled);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);

            // When
            settingsControl.SetData(scenarioConfiguration, b);

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

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(scenarioConfiguration, b);

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

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(scenarioConfiguration, random.NextDouble());

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            parameterATextBoxTester.Enter("NotADouble");

            // Precondition
            ErrorProvider errorProvider = GetLengthEffectErrorProvider(settingsControl);
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

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(scenarioConfiguration, random.NextDouble());

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            parameterATextBoxTester.Enter("NotADouble");

            // Precondition
            ErrorProvider errorProvider = GetLengthEffectErrorProvider(settingsControl);
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

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(scenarioConfiguration, random.NextDouble());

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            parameterATextBoxTester.Enter("NotADouble");

            // Precondition
            ErrorProvider errorProvider = GetLengthEffectErrorProvider(settingsControl);
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            string errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsNotEmpty(errorMessage);

            // When
            var newConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(),
                                                                                           random.NextRoundedDouble());
            settingsControl.SetData(newConfiguration, random.NextDouble());

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

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(scenarioConfiguration, random.NextDouble());

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();

            // Precondition
            ErrorProvider errorProvider = GetLengthEffectErrorProvider(settingsControl);
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

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(scenarioConfiguration, b);

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

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(scenarioConfiguration, b);

            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            const Keys keyData = Keys.Escape;

            var parameterTextBox = (TextBox) parameterATextBoxTester.TheObject;
            parameterTextBox.TextChanged += (sender, args) =>
            {
                parameterATextBoxTester.FireEvent("KeyDown", new KeyEventArgs(keyData));
            };

            // Precondition
            Assert.AreEqual(initialAValue, parameterATextBoxTester.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual(lengthEffectNRoundedValue, lengthEffectNRoundedTextBox.Text);

            // When
            parameterTextBox.Text = "NotAProbability";

            // Then
            Assert.AreEqual(initialAValue, parameterATextBoxTester.Text);
            Assert.AreEqual(lengthEffectNRoundedValue, lengthEffectNRoundedTextBox.Text);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenControlWithConfiguration_WhenConfigurationNotifiesObservers_ThenControlsUpdated()
        {
            // Given
            const double a = 0.4;
            const double b = 300;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, (RoundedDouble) a);

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(scenarioConfiguration, b);

            // Precondition
            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            Assert.AreEqual("0,400", parameterATextBoxTester.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual("1,13", lengthEffectNRoundedTextBox.Text);

            // When
            scenarioConfiguration.A = (RoundedDouble) 0.7;
            scenarioConfiguration.NotifyObservers();

            // Then
            Assert.AreEqual("0,700", parameterATextBoxTester.Text);
            Assert.AreEqual("1,23", lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        public void GivenControlWithConfiguration_WhenDataClearedAndConfigurationNotifiesObservers_ThenControlsNotUpdated()
        {
            // Given
            const double a = 0.4;
            const double b = 300;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var scenarioConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, (RoundedDouble) a);

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(scenarioConfiguration, b);

            // Precondition
            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            Assert.AreEqual("0,400", parameterATextBoxTester.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual("1,13", lengthEffectNRoundedTextBox.Text);

            // When
            settingsControl.ClearData();
            scenarioConfiguration.A = (RoundedDouble) 0.7;
            scenarioConfiguration.NotifyObservers();

            // Then
            Assert.IsEmpty(parameterATextBoxTester.Text);
            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        public void GivenControlWithConfiguration_WhenSettingNewConfigurationAndOldConfigurationNotifiesObservers_ThenControlsNotUpdated()
        {
            // Given
            const double a = 0.4;
            const double b = 300;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var oldConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, (RoundedDouble) a);

            LengthEffectSettingsControl settingsControl = ShowLengthEffectSettingsControl();
            settingsControl.SetData(oldConfiguration, b);

            // Precondition
            TextBoxTester parameterATextBoxTester = GetParameterATextBoxTester();
            Assert.AreEqual("0,400", parameterATextBoxTester.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual("1,13", lengthEffectNRoundedTextBox.Text);

            // When
            var newConfiguration = new TestScenarioConfigurationPerFailureMechanismSection(section, (RoundedDouble) 0.7);
            settingsControl.SetData(newConfiguration, b);
            oldConfiguration.NotifyObservers();

            // Then
            Assert.AreEqual("0,700", parameterATextBoxTester.Text);
            Assert.AreEqual("1,23", lengthEffectNRoundedTextBox.Text);
        }

        private static TextBoxTester GetParameterATextBoxTester()
        {
            return new TextBoxTester("parameterATextBox");
        }

        private static TextBox GetLengthEffectNRoundedTextBox()
        {
            return (TextBox) new ControlTester("lengthEffectNRoundedTextBox").TheObject;
        }

        private static ErrorProvider GetLengthEffectErrorProvider(LengthEffectSettingsControl settingsControl)
        {
            return TypeUtils.GetField<ErrorProvider>(settingsControl, "lengthEffectErrorProvider");
        }

        private LengthEffectSettingsControl ShowLengthEffectSettingsControl()
        {
            var control = new LengthEffectSettingsControl();

            testForm.Controls.Add(control);
            testForm.Show();

            return control;
        }
    }
}