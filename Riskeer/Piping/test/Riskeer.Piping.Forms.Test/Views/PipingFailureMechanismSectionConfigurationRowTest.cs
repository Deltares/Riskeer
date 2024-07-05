// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismSectionConfigurationRowTest
    {
        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            double b = random.NextDouble();

            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => new PipingFailureMechanismSectionConfigurationRow(sectionConfiguration, sectionStart, sectionEnd, b, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var random = new Random(39);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            double b = random.NextDouble();

            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            var sectionRow = new PipingFailureMechanismSectionConfigurationRow(sectionConfiguration, sectionStart, sectionEnd, b, handler);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionConfigurationRow>(sectionRow);

            FailureMechanismSection section = sectionConfiguration.Section;
            Assert.AreEqual(section.Name, sectionRow.Name);

            Assert.AreEqual(sectionStart, sectionRow.SectionStart, sectionRow.SectionStart.GetAccuracy());
            Assert.AreEqual(sectionEnd, sectionRow.SectionEnd, sectionRow.SectionEnd.GetAccuracy());

            Assert.AreEqual(section.Length, sectionRow.Length, sectionRow.Length.GetAccuracy());

            Assert.AreEqual(sectionConfiguration.A.NumberOfDecimalPlaces, sectionRow.A.NumberOfDecimalPlaces);
            Assert.AreEqual(sectionConfiguration.A, sectionRow.A);

            Assert.AreEqual(2, sectionRow.N.NumberOfDecimalPlaces);
            Assert.AreEqual(sectionConfiguration.GetN(b), sectionRow.N, sectionRow.N.GetAccuracy());

            Assert.AreEqual(2, sectionRow.FailureMechanismSensitiveSectionLength.NumberOfDecimalPlaces);
            Assert.AreEqual(sectionConfiguration.GetFailureMechanismSensitiveSectionLength(), sectionRow.FailureMechanismSensitiveSectionLength,
                            sectionRow.FailureMechanismSensitiveSectionLength.GetAccuracy());
        }

        [Test]
        public void A_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call & Assert
            SetPropertyAndVerifyNotifications(row => row.A = random.NextRoundedDouble(), sectionConfiguration);
        }

        [Test]
        public void A_ChangeToEqualValue_NoNotificationsAndChangeHandlerNotCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.StrictMock<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var random = new Random(21);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            double b = random.NextDouble();

            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            var sectionRow = new PipingFailureMechanismSectionConfigurationRow(sectionConfiguration, sectionStart, sectionEnd, b, changeHandler);
            RoundedDouble originalValue = sectionRow.A;

            // Call
            sectionRow.A = originalValue;

            // Assert
            mocks.VerifyAll();
        }

        private static void SetPropertyAndVerifyNotifications(
            Action<PipingFailureMechanismSectionConfigurationRow> setProperty,
            PipingFailureMechanismSectionConfiguration sectionConfiguration)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var random = new Random(21);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            double b = random.NextDouble();

            var handler = new SetPropertyValueAfterConfirmationParameterTester(
                new[]
                {
                    observable
                });

            var row = new PipingFailureMechanismSectionConfigurationRow(sectionConfiguration, sectionStart, sectionEnd, b, handler);

            // Call
            setProperty(row);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}