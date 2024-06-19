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
        [SetCulture("nl-NL")]
        public void Constructor_ControlsCorrectlyInitialized()
        {
            // Setup
            const double b = 0.4;

            // Call
            ShowFailureMechanismSectionConfigurationControl(b);

            // Assert
            Label parameterALabel = GetParameterALabel();
            Assert.AreEqual("Mechanismegevoelige fractie a [-]", parameterALabel.Text);

            Label parameterBLabel = GetParameterBLabel();
            Assert.AreEqual("Equivalente onafhankelijke lengte b [m]", parameterBLabel.Text);

            Label lengthEffectNRoundedLabel = GetLengthEffectNRoundedLabel();
            Assert.AreEqual("Lengte-effect parameter Nvak* [-]", lengthEffectNRoundedLabel.Text);

            TextBox parameterATextBox = GetParameterATextBox();
            Assert.IsFalse(parameterATextBox.Enabled);

            TextBox parameterBTextBox = GetParameterBTextBox();
            Assert.IsFalse(parameterBTextBox.Enabled);
            Assert.AreEqual("0,4", parameterBTextBox.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.IsFalse(lengthEffectNRoundedTextBox.Enabled);
        }

        [Test]
        public void Constructor_ToolTipsCorrectlyInitialized()
        {
            // Call
            FailureMechanismSectionConfigurationControl view = ShowFailureMechanismSectionConfigurationControl();

            // Assert
            Label parameterALabel = GetParameterALabel();
            var parameterAToolTip = TypeUtils.GetField<ToolTip>(view, "parameterAToolTip");
            AssertToolTip("Mechanismegevoelige fractie van het dijkvak.",
                          parameterAToolTip,
                          parameterALabel);

            Label parameterBLabel = GetParameterBLabel();
            var parameterBToolTip = TypeUtils.GetField<ToolTip>(view, "parameterBToolTip");
            AssertToolTip("Lengtemaat van de intensiteit van het lengte-effect binnen het mechanismegevoelige gedeelte van het dijkvak.",
                          parameterBToolTip,
                          parameterBLabel);

            Label lengthEffectNRoundedLabel = GetLengthEffectNRoundedLabel();
            var lengthEffectNRoundedToolTip = TypeUtils.GetField<ToolTip>(view, "lengthEffectNRoundedToolTip");
            AssertToolTip("De parameter 'Nvak*' die het lengte-effect beschrijft in de berekening van de faalkans per vak in de semi-probabilistische toets.",
                          lengthEffectNRoundedToolTip,
                          lengthEffectNRoundedLabel);
        }

        [Test]
        public void SetData_SectionConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSectionConfigurationControl control = ShowFailureMechanismSectionConfigurationControl();

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
            var scenarioConfiguration = new FailureMechanismSectionConfiguration(section)
            {
                A = (RoundedDouble) a
            };

            FailureMechanismSectionConfigurationControl control = ShowFailureMechanismSectionConfigurationControl(b);

            // Precondition
            TextBox parameterATextBox = GetParameterATextBox();
            Assert.IsEmpty(parameterATextBox.Text);
            
            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);

            // When
            control.SetData(scenarioConfiguration);

            // Then
            Assert.AreEqual("0,700", parameterATextBox.Text);
            Assert.AreEqual("1,23", lengthEffectNRoundedTextBox.Text);
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
            var sectionConfiguration = new FailureMechanismSectionConfiguration(section)
            {
                A = (RoundedDouble) a
            };

            FailureMechanismSectionConfigurationControl control = ShowFailureMechanismSectionConfigurationControl(b);
            control.SetData(sectionConfiguration);

            // Precondition
            TextBox parameterATextBox = GetParameterATextBox();
            Assert.AreEqual("0,700", parameterATextBox.Text);
            
            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual("1,23", lengthEffectNRoundedTextBox.Text);

            // When
            sectionConfiguration.A = (RoundedDouble) 0.4;
            sectionConfiguration.NotifyObservers();

            // Then
            Assert.AreEqual("0,400", parameterATextBox.Text);
            Assert.AreEqual("1,13", lengthEffectNRoundedTextBox.Text);
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
            var oldConfiguration = new FailureMechanismSectionConfiguration(section)
            {
                A = (RoundedDouble) a
            };

            FailureMechanismSectionConfigurationControl control = ShowFailureMechanismSectionConfigurationControl(b);
            control.SetData(oldConfiguration);

            // Precondition
            TextBox parameterATextBox = GetParameterATextBox();
            Assert.AreEqual("0,700", parameterATextBox.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual("1,23", lengthEffectNRoundedTextBox.Text);

            // When
            var newConfiguration = new FailureMechanismSectionConfiguration(section)
            {
                A = (RoundedDouble) 0.4
            };
            control.SetData(newConfiguration);
            oldConfiguration.NotifyObservers();

            // Then
            Assert.AreEqual("0,400", parameterATextBox.Text);
            Assert.AreEqual("1,13", lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenControlWithConfigurationSet_WhenClearingData_ThenControlUpdated()
        {
            // Given
            const double a = 0.7;
            const double b = 300.0;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var scenarioConfiguration = new FailureMechanismSectionConfiguration(section)
            {
                A = (RoundedDouble) a
            };

            FailureMechanismSectionConfigurationControl control = ShowFailureMechanismSectionConfigurationControl(b);
            control.SetData(scenarioConfiguration);

            // Precondition
            TextBox parameterATextBox = GetParameterATextBox();
            Assert.AreEqual("0,700", parameterATextBox.Text);
            
            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.AreEqual("1,23", lengthEffectNRoundedTextBox.Text);

            // When
            control.ClearData();

            // Then
            Assert.IsEmpty(parameterATextBox.Text);
            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenControlWithConfigurationSet_WhenClearingDataAndOldConfigurationNotifiesObservers_ThenControlNotUpdated()
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });
            var configuration = new FailureMechanismSectionConfiguration(section);

            FailureMechanismSectionConfigurationControl control = ShowFailureMechanismSectionConfigurationControl();
            control.SetData(configuration);

            // When
            control.ClearData();
            configuration.NotifyObservers();

            // Then
            TextBox parameterATextBox = GetParameterATextBox();
            Assert.IsEmpty(parameterATextBox.Text);

            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox();
            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);
        }

        private static void AssertToolTip(string toolTipText, ToolTip toolTip, Control label)
        {
            Assert.AreEqual(toolTipText, toolTip.GetToolTip(label));
            Assert.AreEqual(5000, toolTip.AutoPopDelay);
            Assert.AreEqual(100, toolTip.InitialDelay);
            Assert.AreEqual(100, toolTip.ReshowDelay);
        }

        private static Label GetParameterALabel()
        {
            return (Label) new LabelTester("parameterALabel").TheObject;
        }

        private static Label GetParameterBLabel()
        {
            return (Label) new LabelTester("parameterBLabel").TheObject;
        }

        private static Label GetLengthEffectNRoundedLabel()
        {
            return (Label) new LabelTester("lengthEffectNRoundedLabel").TheObject;
        }

        private static TextBox GetParameterATextBox()
        {
            return (TextBox) new TextBoxTester("parameterATextBox").TheObject;
        }

        private static TextBox GetParameterBTextBox()
        {
            return (TextBox) new ControlTester("parameterBTextBox").TheObject;
        }

        private static TextBox GetLengthEffectNRoundedTextBox()
        {
            return (TextBox) new ControlTester("lengthEffectNRoundedTextBox").TheObject;
        }

        private FailureMechanismSectionConfigurationControl ShowFailureMechanismSectionConfigurationControl()
        {
            var random = new Random(21);
            return ShowFailureMechanismSectionConfigurationControl(random.NextDouble());
        }

        private FailureMechanismSectionConfigurationControl ShowFailureMechanismSectionConfigurationControl(double b)
        {
            var control = new FailureMechanismSectionConfigurationControl(b);

            testForm.Controls.Add(control);
            testForm.Show();

            return control;
        }
    }
}