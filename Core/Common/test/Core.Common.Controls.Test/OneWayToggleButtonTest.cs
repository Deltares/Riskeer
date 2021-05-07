// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Controls.Test
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class OneWayToggleButtonTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var toggleButton = new OneWayToggleButton();

            // Assert
            Assert.IsInstanceOf<ToggleButton>(toggleButton);
        }

        [Test]
        public void OnClick_Always_ClickEventRaised()
        {
            // Setup
            var toggleButton = new TestOneWayToggleButton();

            var clickCount = 0;
            object actualSender = null;
            toggleButton.Click += (sender, args) =>
            {
                clickCount++;
                actualSender = sender;
            };

            // Call
            toggleButton.PerformClick();

            // Assert
            Assert.AreEqual(1, clickCount);
            Assert.AreSame(toggleButton, actualSender);
        }

        [Test]
        public void GivenToggleButtonWithCommandAndCanExecuteTrue_WhenButtonClicked_ThenCommandExecuted()
        {
            // Given
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.CanExecuteChanged += null).IgnoreArguments();
            command.Expect(c => c.CanExecute(null))
                   .Return(true)
                   .Repeat.Any();
            command.Expect(c => c.Execute(null));
            mocks.ReplayAll();

            var toggleButton = new TestOneWayToggleButton
            {
                Command = command
            };

            // When
            toggleButton.PerformClick();

            // Then
            mocks.VerifyAll();
        }

        [Test]
        public void GivenToggleButtonWithCommandAndCanExecuteFalse_WhenButtonClicked_ThenCommandNotExecuted()
        {
            // Given
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.CanExecuteChanged += null).IgnoreArguments();
            command.Expect(c => c.CanExecute(null))
                   .Return(false)
                   .Repeat.Any();
            mocks.ReplayAll();

            var toggleButton = new TestOneWayToggleButton
            {
                Command = command
            };

            // When
            toggleButton.PerformClick();

            // Then
            mocks.VerifyAll();
        }

        private class TestOneWayToggleButton : OneWayToggleButton
        {
            public void PerformClick()
            {
                OnClick();
            }
        }
    }
}