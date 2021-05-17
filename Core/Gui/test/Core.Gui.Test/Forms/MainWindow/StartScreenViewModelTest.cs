﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Gui.Forms.MainWindow;
using NUnit.Framework;

namespace Core.Gui.Test.Forms.MainWindow
{
    [TestFixture]
    public class StartScreenViewModelTest
    {
        [Test]
        public void Constructor_NewProjectActionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new StartScreenViewModel(null, () => {});

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("newProjectAction", exception.ParamName);
        }

        [Test]
        public void Constructor_OpenProjectActionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new StartScreenViewModel(() => {}, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("openProjectAction", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var viewModel = new StartScreenViewModel(() => {}, () => {});

            // Assert
            Assert.IsNotNull(viewModel.NewProjectCommand);
            Assert.IsNotNull(viewModel.OpenProjectCommand);
        }

        [Test]
        public void GivenViewModel_WhenNewProjectCommandExecuted_ThenGivenActionExecuted()
        {
            // Given
            var newProjectActionCalled = 0;

            void NewProjectAction()
            {
                newProjectActionCalled++;
            }

            var viewModel = new StartScreenViewModel(NewProjectAction, () => {});

            // When
            viewModel.NewProjectCommand.Execute(null);

            // Then
            Assert.AreEqual(1, newProjectActionCalled);
        }

        [Test]
        public void GivenViewModel_WhenOpenProjectCommandExecuted_ThenGivenActionExecuted()
        {
            // Given
            var openProjectActionCalled = 0;

            void OpenProjectAction()
            {
                openProjectActionCalled++;
            }

            var viewModel = new StartScreenViewModel(() => {}, OpenProjectAction);

            // When
            viewModel.OpenProjectCommand.Execute(null);

            // Then
            Assert.AreEqual(1, openProjectActionCalled);
        }
    }
}