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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Gui.Forms.Backstage;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Gui.Test.Forms.Backstage
{
    [TestFixture]
    public class InfoViewModelTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var viewModel = new InfoViewModel();

            // Assert
            Assert.IsInstanceOf<IBackstagePageViewModel>(viewModel);
            Assert.IsInstanceOf<INotifyPropertyChanged>(viewModel);
            Assert.IsNull(viewModel.ProjectName);
            Assert.IsNull(viewModel.ProjectDescription);
        }

        [Test]
        public void SetProject_ProjectNull_ThrowsArgumentNullException()
        {
            // Setup
            var viewModel = new InfoViewModel();
            
            // Call
            void Call() => viewModel.SetProject(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("projectToSet", exception.ParamName);
        }

        [Test]
        public void SetProject_WithProject_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            mocks.ReplayAll();

            project.Name = "Test";
            project.Description = "Test description";
            
            var viewModel = new InfoViewModel();
            
            // Call
            viewModel.SetProject(project);

            // Assert
            Assert.AreEqual(project.Name, viewModel.ProjectName);
            Assert.AreEqual(project.Description, viewModel.ProjectDescription);
            mocks.VerifyAll();
        }
    }
}