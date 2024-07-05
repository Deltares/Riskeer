﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Reflection;
using Core.Gui.Forms.Backstage;
using Core.Gui.Settings;
using NUnit.Framework;

namespace Core.Gui.Test.Forms.Backstage
{
    [TestFixture]
    public class BackstageViewModelTest
    {
        [Test]
        public void Constructor_SettingsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new BackstageViewModel(null, "0");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("settings", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var settings = new GuiCoreSettings
            {
                ApplicationName = "Test application",
                SupportHeader = "Support",
                SupportText = "Some text"
            };
            const string version = "1.0";

            // Call
            var viewModel = new BackstageViewModel(settings, version);

            // Assert
            Assert.IsInstanceOf<INotifyPropertyChanged>(viewModel);

            Assert.IsNotNull(viewModel.InfoViewModel);
            Assert.IsNull(viewModel.InfoViewModel.ProjectName);

            Assert.IsNotNull(viewModel.AboutViewModel);
            Assert.AreEqual(settings.ApplicationName, viewModel.AboutViewModel.ApplicationName);
            Assert.AreEqual(version, viewModel.AboutViewModel.Version);

            Assert.IsNotNull(viewModel.SupportViewModel);
            Assert.AreEqual(settings.SupportHeader, viewModel.SupportViewModel.SupportHeader);
            Assert.AreEqual(settings.SupportText, viewModel.SupportViewModel.SupportText);

            Assert.IsNotNull(viewModel.OpenUserManualCommand);
            Assert.IsNotNull(viewModel.SetSelectedViewModelCommand);

            Assert.AreSame(viewModel.InfoViewModel, viewModel.SelectedViewModel);
            Assert.IsTrue(viewModel.InfoSelected);
            Assert.IsFalse(viewModel.AboutSelected);
            Assert.IsFalse(viewModel.SupportSelected);
        }

        [Test]
        [TestCaseSource(nameof(GetBackstagePageViewModels))]
        public void GivenViewModel_WhenExecutingSetSelectedViewModelCommand_ThenExpectedValuesSetAndPropertyChangedEventsFired(
            Func<BackstageViewModel, IBackstagePageViewModel> getBackstagePageViewModelFunc,
            bool infoSelected, bool openSelected, bool aboutSelected)
        {
            // Given
            var viewModel = new BackstageViewModel(new GuiCoreSettings(), "1.0");
            IBackstagePageViewModel backstagePageViewModel = getBackstagePageViewModelFunc(viewModel);

            if (backstagePageViewModel is InfoViewModel)
            {
                viewModel.SelectedViewModel = viewModel.AboutViewModel;
            }

            var propertyNames = new List<string>();
            viewModel.PropertyChanged += (sender, args) =>
            {
                propertyNames.Add(args.PropertyName);
            };

            // When
            viewModel.SetSelectedViewModelCommand.Execute(backstagePageViewModel);

            // Then
            Assert.AreSame(backstagePageViewModel, viewModel.SelectedViewModel);
            Assert.AreEqual(infoSelected, viewModel.InfoSelected);
            Assert.AreEqual(aboutSelected, viewModel.AboutSelected);
            Assert.AreEqual(openSelected, viewModel.SupportSelected);
            CollectionAssert.AreEqual(new[]
            {
                nameof(viewModel.SelectedViewModel),
                nameof(viewModel.InfoSelected),
                nameof(viewModel.AboutSelected),
                nameof(viewModel.SupportSelected)
            }, propertyNames);
        }

        [Test]
        [TestCaseSource(nameof(GetBackstagePageViewModels))]
        public void GivenViewModel_WhenExecutingSetSelectedViewModelCommandAndSetToSameViewModel_ThenNothingHappens(
            Func<BackstageViewModel, IBackstagePageViewModel> getBackstagePageViewModelFunc,
            bool infoSelected, bool openSelected, bool aboutSelected)
        {
            // Given
            var viewModel = new BackstageViewModel(new GuiCoreSettings(), "1.0");
            IBackstagePageViewModel backstagePageViewModel = getBackstagePageViewModelFunc(viewModel);

            viewModel.SelectedViewModel = backstagePageViewModel;

            // Precondition
            Assert.AreSame(backstagePageViewModel, viewModel.SelectedViewModel);
            Assert.AreEqual(infoSelected, viewModel.InfoSelected);
            Assert.AreEqual(aboutSelected, viewModel.AboutSelected);
            Assert.AreEqual(openSelected, viewModel.SupportSelected);

            var propertyChangedCount = 0;
            viewModel.PropertyChanged += (sender, args) => propertyChangedCount++;

            // When
            viewModel.SetSelectedViewModelCommand.Execute(backstagePageViewModel);

            // Then
            Assert.AreSame(backstagePageViewModel, viewModel.SelectedViewModel);
            Assert.AreEqual(infoSelected, viewModel.InfoSelected);
            Assert.AreEqual(aboutSelected, viewModel.AboutSelected);
            Assert.AreEqual(openSelected, viewModel.SupportSelected);
            Assert.AreEqual(0, propertyChangedCount);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenViewModelWithSettingsWithOrWithoutUserManualPathSet_WhenCanExecuteOpenUserManualCommand_ThenExpectedValue(
            bool userManualPresent)
        {
            // Given
            string path = Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path);

            var settings = new GuiCoreSettings
            {
                ManualFilePath = userManualPresent ? path : null
            };

            var viewModel = new BackstageViewModel(settings, "1.0");

            // When
            bool canExecute = viewModel.OpenUserManualCommand.CanExecute(null);

            // Then
            Assert.AreEqual(userManualPresent, canExecute);
        }

        private static IEnumerable<TestCaseData> GetBackstagePageViewModels()
        {
            yield return new TestCaseData(new Func<BackstageViewModel, IBackstagePageViewModel>(
                                              viewModel => viewModel.InfoViewModel), true, false, false);
            yield return new TestCaseData(new Func<BackstageViewModel, IBackstagePageViewModel>(
                                              viewModel => viewModel.AboutViewModel), false, false, true);
            yield return new TestCaseData(new Func<BackstageViewModel, IBackstagePageViewModel>(
                                              viewModel => viewModel.SupportViewModel), false, true, false);
        }
    }
}