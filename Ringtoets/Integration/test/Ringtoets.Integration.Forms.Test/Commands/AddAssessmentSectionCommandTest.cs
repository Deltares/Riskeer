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

using System;
using System.Windows.Forms;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewHost;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Integration.Forms.Commands;

namespace Ringtoets.Integration.Forms.Test.Commands
{
    [TestFixture]
    public class AddAssessmentSectionCommandTest
    {
        [Test]
        public void Constructor_WithoutController_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new AddAssessmentSectionCommand(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithController_CreatesNewICommand()
        {
            // Setup
            var mockRepository = new MockRepository();
            var window = mockRepository.StrictMock<IWin32Window>();
            var projectOwner = mockRepository.StrictMock<IProjectOwner>();
            var documentViewController = mockRepository.StrictMock<IDocumentViewController>();
            mockRepository.ReplayAll();

            var commandHandler = new AssessmentSectionFromFileCommandHandler(window, projectOwner, documentViewController);

            // Call
            var command = new AddAssessmentSectionCommand(commandHandler);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsTrue(command.Checked);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Execute_Always_CallsCreateAssessmentSectionFromFile()
        {
            // Setup
            var mockRepository = new MockRepository();
            var commandHandler = mockRepository.StrictMock<IAssessmentSectionFromFileCommandHandler>();
            commandHandler.Expect(ch => ch.AddAssessmentSectionFromFile());
            mockRepository.ReplayAll();
            var command = new AddAssessmentSectionCommand(commandHandler);

            // Call
            command.Execute();

            // Assert
            mockRepository.VerifyAll();
        }
    }
}