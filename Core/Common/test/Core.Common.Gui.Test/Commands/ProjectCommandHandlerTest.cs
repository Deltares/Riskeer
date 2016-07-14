// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Selection;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class ProjectCommandHandlerTest
    {
        [Test]
        public void AddItemToProject_AddToProjectItemsAndNotifyObservers()
        {
            // Setup
            var project = new Project();
            var childData = new object();
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = project;

            var dialogParent = mocks.Stub<IWin32Window>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var viewController = mocks.Stub<IViewController>();

            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            project.Attach(observer);
            var commandHandler = new ProjectCommandHandler(projectOwner, dialogParent, null, applicationSelection, viewController);

            // Call
            commandHandler.AddItemToProject(childData);

            // Assert
            CollectionAssert.Contains(project.Items, childData);
            mocks.VerifyAll();
        }
    }
}