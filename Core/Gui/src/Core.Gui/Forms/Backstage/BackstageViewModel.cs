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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Core.Gui.Commands;
using Core.Gui.Settings;

namespace Core.Gui.Forms.Backstage
{
    /// <summary>
    /// ViewModel for the <see cref="BackstageControl"/>.
    /// </summary>
    public class BackstageViewModel : INotifyPropertyChanged
    {
        private readonly GuiCoreSettings settings;
        private IBackstagePageViewModel selectedViewModel;
        private bool infoSelected;
        private bool supportSelected;
        private bool aboutSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new instance of <see cref="BackstageViewModel"/>.
        /// </summary>
        /// <param name="settings">The application settings.</param>
        /// <param name="version">The application version.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is <c>null</c>.</exception>
        public BackstageViewModel(GuiCoreSettings settings, string version)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.settings = settings;

            InfoViewModel = new InfoViewModel();
            AboutViewModel = new AboutViewModel(settings, version);
            SupportViewModel = new SupportViewModel(settings);

            OpenUserManualCommand = new RelayCommand(OnOpenUserManual, CanOpenUserManual);
            SetSelectedViewModelCommand = new RelayCommand(OnSetCurrentViewModel);

            SelectedViewModel = InfoViewModel;
        }

        /// <summary>
        /// Gets the command to open the user manual.
        /// </summary>
        public ICommand OpenUserManualCommand { get; }

        /// <summary>
        /// Gets the command to set the selected view model.
        /// </summary>
        public ICommand SetSelectedViewModelCommand { get; }

        /// <summary>
        /// Gets the <see cref="InfoViewModel"/>.
        /// </summary>
        public InfoViewModel InfoViewModel { get; }

        /// <summary>
        /// Gets the <see cref="AboutViewModel"/>.
        /// </summary>
        public AboutViewModel AboutViewModel { get; }

        /// <summary>
        /// Gets the <see cref="SupportViewModel"/>.
        /// </summary>
        public SupportViewModel SupportViewModel { get; }

        /// <summary>
        /// Gets or sets the selected view model.
        /// </summary>
        public IBackstagePageViewModel SelectedViewModel
        {
            get => selectedViewModel;
            set
            {
                if (value == selectedViewModel)
                {
                    return;
                }

                selectedViewModel = value;
                OnPropertyChanged(nameof(SelectedViewModel));

                SetButtonStates();
            }
        }

        /// <summary>
        /// Gets an indicator whether the <see cref="InfoViewModel"/>
        /// is selected.
        /// </summary>
        public bool InfoSelected
        {
            get => infoSelected;
            private set
            {
                infoSelected = value;
                OnPropertyChanged(nameof(InfoSelected));
            }
        }

        /// <summary>
        /// Gets an indicator whether the <see cref="AboutViewModel"/>
        /// is selected.
        /// </summary>
        public bool AboutSelected
        {
            get => aboutSelected;
            private set
            {
                aboutSelected = value;
                OnPropertyChanged(nameof(AboutSelected));
            }
        }

        /// <summary>
        /// Gets an indicator whether the <see cref="SupportViewModel"/>
        /// is selected.
        /// </summary>
        public bool SupportSelected
        {
            get => supportSelected;
            private set
            {
                supportSelected = value;
                OnPropertyChanged(nameof(SupportSelected));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetButtonStates()
        {
            InfoSelected = selectedViewModel is InfoViewModel;
            AboutSelected = selectedViewModel is AboutViewModel;
            SupportSelected = selectedViewModel is SupportViewModel;
        }

        private bool CanOpenUserManual(object obj)
        {
            return File.Exists(settings.ManualFilePath);
        }

        private void OnOpenUserManual(object obj)
        {
            Process.Start(settings.ManualFilePath);
        }

        private void OnSetCurrentViewModel(object obj)
        {
            SelectedViewModel = (IBackstagePageViewModel) obj;
        }
    }
}