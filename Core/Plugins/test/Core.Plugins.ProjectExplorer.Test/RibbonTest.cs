// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using Core.Common.Controls.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using ToggleButton = Fluent.ToggleButton;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class RibbonTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void ValidateItems_ShowProjectExplorerCommandNotSet_ThrowsNullReferenceException()
        {
            // Setup
            var ribbon = new Ribbon();

            // Call
            TestDelegate test = () => ribbon.ValidateItems();

            // Assert
            Assert.Throws<NullReferenceException>(test);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_ShowProjectExplorerCommandSet_ShowProjectButtonIsCheckedEqualToCommandChecked(bool isChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Checked).Return(isChecked);
            mocks.ReplayAll();

            var ribbon = new Ribbon
            {
                ToggleExplorerCommand = command
            };

            var toggleProjectExplorerButton = ribbon.GetRibbonControl().FindName("ToggleProjectExplorerButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(toggleProjectExplorerButton, "Ribbon should have a toggle project explorer button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(isChecked, toggleProjectExplorerButton.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ToggleExplorerCommand_ButtonShowProjectExplorerToolWindowOnClick_ExecutesCommandAndUpdatesButtonState()
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Execute());
            command.Expect(c => c.Checked).Return(false);
            mocks.ReplayAll();

            var ribbon = new Ribbon
            {
                ToggleExplorerCommand = command
            };

            var toggleProjectExplorerButton = ribbon.GetRibbonControl().FindName("ToggleProjectExplorerButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(toggleProjectExplorerButton, "Ribbon should have a toggle project explorer button.");

            // Call
            toggleProjectExplorerButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
    }
}