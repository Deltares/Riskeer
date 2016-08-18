// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewHost;
using Demo.Ringtoets.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Demo.Ringtoets.Test.Commands
{
    [TestFixture]
    public class OpenMapViewCommandTest
    {
        [Test]
        public void ParameteredConstructor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.Stub<IViewController>();
            mocks.ReplayAll();

            // Call
            var command = new OpenMapViewCommand(viewController);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_Always_OpensViewForMapData()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            viewController.Expect(g => g.DocumentViewController).Return(documentViewController);
            documentViewController.Expect(vr => vr.OpenViewForData(null)).IgnoreArguments().Return(true);

            mocks.ReplayAll();

            var command = new OpenMapViewCommand(viewController);

            // Call
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Checked_Always_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.Stub<IViewController>();
            mocks.ReplayAll();

            // Call
            var command = new OpenMapViewCommand(viewController);

            // Assert
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }
    }
}