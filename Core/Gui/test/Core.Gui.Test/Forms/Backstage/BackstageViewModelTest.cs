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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Gui.Forms.Backstage;
using NUnit.Framework;

namespace Core.Gui.Test.Forms.Backstage
{
    [TestFixture]
    public class BackstageViewModelTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var viewModel = new BackstageViewModel();

            // Assert
            Assert.IsInstanceOf<INotifyPropertyChanged>(viewModel);
            
            Assert.IsNotNull(viewModel.InfoViewModel);
            Assert.IsNotNull(viewModel.OpenViewModel);
            Assert.IsNotNull(viewModel.AboutViewModel);
            
            Assert.IsNotNull(viewModel.SetSelectedViewModelCommand);
            
            Assert.AreSame(viewModel.InfoViewModel, viewModel.SelectedViewModel);
            Assert.IsTrue(viewModel.InfoSelected);
            Assert.IsFalse(viewModel.OpenSelected);
            Assert.IsFalse(viewModel.AboutSelected);
        }

        [Test]
        [TestCaseSource(nameof(GetBackstagePageViewModels))]
        public void GivenViewModel_WhenExecutingSetSelectedViewModelCommand_ThenExpectedValuesSet(
            Func<BackstageViewModel, IBackstagePageViewModel> getBackstagePageViewModelFunc,
            bool infoSelected, bool openSelected, bool aboutSelected)
        {
            // Given
            var viewModel = new BackstageViewModel();
            IBackstagePageViewModel backstagePageViewModel = getBackstagePageViewModelFunc(viewModel);

            if (backstagePageViewModel is InfoViewModel)
            {
                viewModel.SelectedViewModel = viewModel.AboutViewModel;
            }
            
            // When
            viewModel.SetSelectedViewModelCommand.Execute(backstagePageViewModel);
            
            // Assert
            Assert.AreSame(backstagePageViewModel, viewModel.SelectedViewModel);
            Assert.AreEqual(infoSelected, viewModel.InfoSelected);
            Assert.AreEqual(openSelected, viewModel.OpenSelected);
            Assert.AreEqual(aboutSelected, viewModel.AboutSelected);
        }

        [Test]
        [TestCaseSource(nameof(GetBackstagePageViewModels))]
        public void GivenViewModel_WhenExecutingSetSelectedViewModelCommandAndSetToSameViewModel_ThenNothingHappens(
            Func<BackstageViewModel, IBackstagePageViewModel> getBackstagePageViewModelFunc,
            bool infoSelected, bool openSelected, bool aboutSelected)
        {
            // Given
            var viewModel = new BackstageViewModel();
            IBackstagePageViewModel backstagePageViewModel = getBackstagePageViewModelFunc(viewModel);

            viewModel.SelectedViewModel = backstagePageViewModel;

            // Precondition
            Assert.AreSame(backstagePageViewModel, viewModel.SelectedViewModel);
            Assert.AreEqual(infoSelected, viewModel.InfoSelected);
            Assert.AreEqual(openSelected, viewModel.OpenSelected);
            Assert.AreEqual(aboutSelected, viewModel.AboutSelected);

            // When
            viewModel.SetSelectedViewModelCommand.Execute(backstagePageViewModel);

            // Assert
            Assert.AreSame(backstagePageViewModel, viewModel.SelectedViewModel);
            Assert.AreEqual(infoSelected, viewModel.InfoSelected);
            Assert.AreEqual(openSelected, viewModel.OpenSelected);
            Assert.AreEqual(aboutSelected, viewModel.AboutSelected);
        }

        private static IEnumerable<TestCaseData> GetBackstagePageViewModels()
        {
            yield return new TestCaseData(new Func<BackstageViewModel, IBackstagePageViewModel>(
                                              viewModel => viewModel.InfoViewModel), true, false ,false);
            yield return new TestCaseData(new Func<BackstageViewModel, IBackstagePageViewModel>(
                                              viewModel => viewModel.OpenViewModel), false, true, false);
            yield return new TestCaseData(new Func<BackstageViewModel, IBackstagePageViewModel>(
                                              viewModel => viewModel.AboutViewModel), false, false, true);
        }
    }
}