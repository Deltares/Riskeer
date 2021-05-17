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

using System;
using System.Threading;
using Core.Gui.Forms.StartScreen;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace Core.Gui.Test.Forms.StartScreen
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class StartScreenTest
    {
        [Test]
        public void Constructor_ViewModelNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new Gui.Forms.StartScreen.StartScreen(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("viewModel", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var viewModel = new StartScreenViewModel(() => {}, () => {});

            // Call
            var startScreen = new Gui.Forms.StartScreen.StartScreen(viewModel);

            // Assert
            Assert.IsInstanceOf<MetroWindow>(startScreen);
            Assert.AreSame(viewModel, startScreen.DataContext);
        }
    }
}