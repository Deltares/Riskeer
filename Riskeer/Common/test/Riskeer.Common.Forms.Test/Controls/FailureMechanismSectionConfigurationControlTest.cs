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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Controls;

namespace Riskeer.Common.Forms.Test.Controls
{
    [TestFixture]
    public class FailureMechanismSectionConfigurationControlTest
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

            Label parameterBLabel = GetParameterBLabel();
            Assert.AreEqual("Equivalente onafhankelijke lengte b [m]", parameterBLabel.Text);

            Label roundedNSectionLabel = GetRoundedNSectionLabel();
            Assert.AreEqual("Lengte-effect parameter Nvak* [-]", roundedNSectionLabel.Text);

            TextBox parameterATextBox = GetParameterATextBoxTester();
            Assert.IsFalse(parameterATextBox.Enabled);

            TextBox parameterBTextBox = GetParameterBTextBox();
            Assert.IsFalse(parameterBTextBox.Enabled);

            TextBox roundedNSectionTextBox = GetRoundedNSectionTextBox();
            Assert.IsFalse(roundedNSectionTextBox.Enabled);
        }

        [Test]
        public void Constructor_ToolTipsCorrectlyInitialized()
        {
            // Call
            FailureMechanismSectionConfigurationControl view = ShowScenarioConfigurationPerFailureMechanismSectionControl();

            // Assert
            Label parameterALabel = GetParameterALabel();
            var parameterAToolTip = TypeUtils.GetField<ToolTip>(view, "parameterAToolTip");
            Assert.AreEqual("Mechanismegevoelige fractie van het dijkvak.",
                            parameterAToolTip.GetToolTip(parameterALabel));
            Assert.AreEqual(5000, parameterAToolTip.AutoPopDelay);
            Assert.AreEqual(100, parameterAToolTip.InitialDelay);
            Assert.AreEqual(100, parameterAToolTip.ReshowDelay);

            Label parameterBLabel = GetParameterBLabel();
            var parameterBToolTip = TypeUtils.GetField<ToolTip>(view, "parameterBToolTip");
            Assert.AreEqual("Lengtemaat van de intensiteit van het lengte-effect binnen het mechanismegevoelige gedeelte van het dijkvak.",
                            parameterBToolTip.GetToolTip(parameterBLabel));
            Assert.AreEqual(5000, parameterBToolTip.AutoPopDelay);
            Assert.AreEqual(100, parameterBToolTip.InitialDelay);
            Assert.AreEqual(100, parameterBToolTip.ReshowDelay);

            Label roundedNSectionLabel = GetRoundedNSectionLabel();
            var roundedNSectionToolTip = TypeUtils.GetField<ToolTip>(view, "roundedNSectionToolTip");
            Assert.AreEqual("De parameter 'Nvak*' die het lengte-effect beschrijft in de berekening van de faalkans per vak in de semi-probabilistische toets.",
                            roundedNSectionToolTip.GetToolTip(roundedNSectionLabel));
            Assert.AreEqual(5000, roundedNSectionToolTip.AutoPopDelay);
            Assert.AreEqual(100, roundedNSectionToolTip.InitialDelay);
            Assert.AreEqual(100, roundedNSectionToolTip.ReshowDelay);
        }

        [Test]
        public void SetData_SectionConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSectionConfigurationControl control = ShowScenarioConfigurationPerFailureMechanismSectionControl();

            // Call
            void Call() => control.SetData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionConfiguration", exception.ParamName);
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
            var scenarioConfiguration = new TestFailureMechanismSectionConfiguration(section, (RoundedDouble) a);

            FailureMechanismSectionConfigurationControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(b);

            // Precondition
            TextBox parameterATextBox = GetParameterATextBoxTester();
            Assert.IsEmpty(parameterATextBox.Text);

            TextBox parameterBTextBox = GetParameterBTextBox();
            Assert.IsEmpty(parameterBTextBox.Text);

            TextBox roundedNSectionTextBox = GetRoundedNSectionTextBox();
            Assert.IsEmpty(roundedNSectionTextBox.Text);

            // When
            settingsControl.SetData(scenarioConfiguration);

            // Then
            Assert.AreEqual("0,700", parameterATextBox.Text);
            Assert.AreEqual("300", parameterBTextBox.Text);
            Assert.AreEqual("1,23", roundedNSectionTextBox.Text);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenControlWithConfigurationSet_WhenConfigurationNotifiesObservers_ThenControlUpdated()
        {
            // Given
            const double a = 0.7;
            const double b = 300.0;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var scenarioConfiguration = new TestFailureMechanismSectionConfiguration(section, (RoundedDouble) a);

            FailureMechanismSectionConfigurationControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(b);
            settingsControl.SetData(scenarioConfiguration);

            // Precondition
            TextBox parameterATextBox = GetParameterATextBoxTester();
            Assert.AreEqual("0,700", parameterATextBox.Text);

            TextBox parameterBTextBox = GetParameterBTextBox();
            Assert.AreEqual("300", parameterBTextBox.Text);

            TextBox roundedNSectionTextBox = GetRoundedNSectionTextBox();
            Assert.AreEqual("1,23", roundedNSectionTextBox.Text);

            // When
            scenarioConfiguration.A = (RoundedDouble) 0.4;
            scenarioConfiguration.NotifyObservers();

            // Then
            Assert.AreEqual("0,400", parameterATextBox.Text);
            Assert.AreEqual("300", parameterBTextBox.Text);
            Assert.AreEqual("1,13", roundedNSectionTextBox.Text);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenControlWithConfigurationSet_WhenNewConfigurationSetAndOldConfigurationNotifiesObservers_ThenControlNotUpdated()
        {
            // Given
            const double a = 0.7;
            const double b = 300.0;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var oldConfiguration = new TestFailureMechanismSectionConfiguration(section, (RoundedDouble) a);

            FailureMechanismSectionConfigurationControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(b);
            settingsControl.SetData(oldConfiguration);

            // Precondition
            TextBox parameterATextBox = GetParameterATextBoxTester();
            Assert.AreEqual("0,700", parameterATextBox.Text);

            TextBox parameterBTextBox = GetParameterBTextBox();
            Assert.AreEqual("300", parameterBTextBox.Text);

            TextBox roundedNSectionTextBox = GetRoundedNSectionTextBox();
            Assert.AreEqual("1,23", roundedNSectionTextBox.Text);

            // When
            var newConfiguration = new TestFailureMechanismSectionConfiguration(section, (RoundedDouble) 0.4);
            settingsControl.SetData(newConfiguration);
            oldConfiguration.NotifyObservers();

            // Then
            Assert.AreEqual("0,400", parameterATextBox.Text);
            Assert.AreEqual("300", parameterBTextBox.Text);
            Assert.AreEqual("1,13", roundedNSectionTextBox.Text);
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
            var scenarioConfiguration = new TestFailureMechanismSectionConfiguration(section, (RoundedDouble) a);

            FailureMechanismSectionConfigurationControl settingsControl = ShowScenarioConfigurationPerFailureMechanismSectionControl(b);
            settingsControl.SetData(scenarioConfiguration);

            // Precondition
            TextBox parameterATextBox = GetParameterATextBoxTester();
            Assert.AreEqual("0,700", parameterATextBox.Text);

            TextBox parameterBTextBox = GetParameterBTextBox();
            Assert.AreEqual("300", parameterBTextBox.Text);

            TextBox roundedNSectionTextBox = GetRoundedNSectionTextBox();
            Assert.AreEqual("1,23", roundedNSectionTextBox.Text);

            // When
            settingsControl.ClearData();

            // Then
            Assert.IsEmpty(parameterATextBox.Text);
            Assert.IsEmpty(parameterBTextBox.Text);
            Assert.IsEmpty(roundedNSectionTextBox.Text);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenControlWithConfigurationSet_WhenClearingDataAndOldConfigurationNotifiesObservers_ThenControlNotUpdated()
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

        private static Label GetParameterBLabel()
        {
            return (Label) new LabelTester("parameterBLabel").TheObject;
        }

        private static Label GetRoundedNSectionLabel()
        {
            return (Label) new LabelTester("roundedNSectionLabel").TheObject;
        }

        private static TextBox GetParameterATextBoxTester()
        {
            return (TextBox) new TextBoxTester("parameterATextBox").TheObject;
        }

        private static TextBox GetParameterBTextBox()
        {
            return (TextBox) new ControlTester("parameterBTextBox").TheObject;
        }

        private static TextBox GetRoundedNSectionTextBox()
        {
            return (TextBox) new ControlTester("roundedNSectionTextBox").TheObject;
        }

        private FailureMechanismSectionConfigurationControl ShowScenarioConfigurationPerFailureMechanismSectionControl()
        {
            var random = new Random(21);
            return ShowScenarioConfigurationPerFailureMechanismSectionControl(random.NextDouble());
        }

        private FailureMechanismSectionConfigurationControl ShowScenarioConfigurationPerFailureMechanismSectionControl(double b)
        {
            var control = new FailureMechanismSectionConfigurationControl(b);

            testForm.Controls.Add(control);
            testForm.Show();

            return control;
        }
    }
}