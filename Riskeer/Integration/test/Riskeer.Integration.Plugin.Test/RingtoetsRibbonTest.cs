// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using Core.Common.Controls.Commands;
using Fluent;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Integration.Plugin;

namespace Riskeer.Integration.Plugin.Test
{
    [TestFixture]
    public class RingtoetsRibbonTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetRibbonControl_Always_ReturnRibbon()
        {
            // Setup
            var ribbon = new RingtoetsRibbon();

            // Call
            Ribbon control = ribbon.GetRibbonControl();

            // Assert
            Assert.IsInstanceOf<Ribbon>(control);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AddAssessmentSectionButtonCommand_AddAssessmentSectionButtonClicked_ExecutesCommand()
        {
            // Setup
            var mockRepository = new MockRepository();
            var command = mockRepository.StrictMock<ICommand>();
            command.Expect(c => c.Execute());
            mockRepository.ReplayAll();

            var ribbon = new RingtoetsRibbon
            {
                AddAssessmentSectionButtonCommand = command
            };

            var addAssessmentSectionButton = ribbon.GetRibbonControl().FindName("AddAssessmentSectionButton") as Button;

            // Precondition
            Assert.IsNotNull(addAssessmentSectionButton, "RingtoetsRibbon should have an add assessment section button.");

            // Call
            addAssessmentSectionButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mockRepository.VerifyAll();
        }
    }
}