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

using System.Windows.Input;
using Core.Gui.Commands;
using NUnit.Framework;

namespace Core.Gui.Test.Commands
{
    [TestFixture]
    public class MainWindowCommandsTest
    {
        [Test]
        public void MainWindowCommands_Always_CorrectlyInitialized()
        {
            // Call
            ICommand newProjectCommand = MainWindowCommands.NewProjectCommand;
            ICommand saveProjectCommand = MainWindowCommands.SaveProjectCommand;
            ICommand saveProjectAsCommand = MainWindowCommands.SaveProjectAsCommand;
            ICommand openProjectCommand = MainWindowCommands.OpenProjectCommand;
            ICommand closeApplicationCommand = MainWindowCommands.CloseApplicationCommand;
            ICommand toggleBackstageCommand = MainWindowCommands.ToggleBackstageCommand;

            // Assert
            Assert.IsInstanceOf<RoutedCommand>(newProjectCommand);
            Assert.IsInstanceOf<RoutedCommand>(saveProjectCommand);
            Assert.IsInstanceOf<RoutedCommand>(saveProjectAsCommand);
            Assert.IsInstanceOf<RoutedCommand>(openProjectCommand);
            Assert.IsInstanceOf<RoutedCommand>(closeApplicationCommand);
            Assert.IsInstanceOf<RoutedCommand>(toggleBackstageCommand);
        }
    }
}