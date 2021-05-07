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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Core.Gui.Commands;

namespace Core.Gui.Forms.Backstage
{
    /// <summary>
    /// ViewModel for the <see cref="BackstageControl"/>.
    /// </summary>
    public class BackstageViewModel : INotifyPropertyChanged
    {
        private IBackstagePageViewModel selectedViewModel;
        private bool infoSelected;
        private bool openSelected;
        private bool aboutSelected;
        
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new instance of <see cref="BackstageViewModel"/>.
        /// </summary>
        public BackstageViewModel()
        {
            InfoViewModel = new InfoViewModel();
            OpenViewModel = new OpenViewModel();
            AboutViewModel = new AboutViewModel();

            SetSelectedViewModelCommand = new RelayCommand(OnSetCurrentViewModel);

            SelectedViewModel = InfoViewModel;
        }

        /// <summary>
        /// Gets the command to set the selected view model.
        /// </summary>
        public ICommand SetSelectedViewModelCommand { get; }
        
        /// <summary>
        /// Gets the <see cref="InfoViewModel"/>.
        /// </summary>
        public InfoViewModel InfoViewModel { get; }
        
        /// <summary>
        /// Gets the <see cref="OpenViewModel"/>.
        /// </summary>
        public OpenViewModel OpenViewModel { get; }
        
        /// <summary>
        /// Gets the <see cref="AboutViewModel"/>.
        /// </summary>
        public AboutViewModel AboutViewModel { get; }

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
        /// Gets an indicator whether the <see cref="OpenViewModel"/>
        /// is selected.
        /// </summary>
        public bool OpenSelected
        {
            get => openSelected;
            private set
            {
                openSelected = value;
                OnPropertyChanged(nameof(OpenSelected));
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetButtonStates()
        {
            InfoSelected = selectedViewModel is InfoViewModel;
            OpenSelected = selectedViewModel is OpenViewModel;
            AboutSelected = selectedViewModel is AboutViewModel;
        }

        private void OnSetCurrentViewModel(object obj)
        {
            SelectedViewModel = (IBackstagePageViewModel) obj;
        }
    }
}