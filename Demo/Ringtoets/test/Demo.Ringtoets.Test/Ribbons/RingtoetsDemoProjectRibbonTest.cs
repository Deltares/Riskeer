﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Demo.Ringtoets.Ribbons;
using NUnit.Framework;
using Rhino.Mocks;

namespace Demo.Ringtoets.Test.Ribbons
{
    [TestFixture]
    public class RingtoetsDemoProjectRibbonTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void DefaultConstructor_Always_CreatesControl()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var ribbon = new RingtoetsDemoProjectRibbon(projectOwner, viewCommands);

            // Assert
            Assert.IsNotNull(ribbon);
            Assert.IsInstanceOf<Control>(ribbon.GetRibbonControl());
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void OpenChartViewButton_OnClick_ExecutesOpenChartViewCommand()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var viewCommands = mocks.Stub<IViewCommands>();
            viewCommands.Expect(vc => vc.OpenView(null)).IgnoreArguments();

            mocks.ReplayAll();

            var ribbon = new RingtoetsDemoProjectRibbon(projectOwner, viewCommands);
            var button = ribbon.GetRibbonControl().FindName("OpenChartViewButton") as Button;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have an open chart view button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void OpenMapViewButton_OnClick_OpensView()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var viewCommands = mocks.Stub<IViewCommands>();
            viewCommands.Expect(vc => vc.OpenView(null)).IgnoreArguments();

            mocks.ReplayAll();

            var ribbon = new RingtoetsDemoProjectRibbon(projectOwner, viewCommands);

            var button = ribbon.GetRibbonControl().FindName("OpenMapViewButton") as Button;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have an open map view button");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
    }
}