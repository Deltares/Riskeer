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

using Core.Common.Gui.Commands;
using Demo.Riskeer.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Demo.Riskeer.Test.Commands
{
    [TestFixture]
    public class OpenStackChartViewCommandTest
    {
        [Test]
        public void Execute_Always_OpensViewForIChartData()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(g => g.OpenView(Arg<object>.Is.NotNull));
            mocks.ReplayAll();

            var command = new OpenStackChartViewCommand(viewCommands);

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
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var command = new OpenStackChartViewCommand(viewCommands);

            // Call
            bool isChecked = command.Checked;

            // Assert
            Assert.IsFalse(isChecked);
            mocks.VerifyAll();
        }
    }
}