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
    public class BackstageViewModel : INotifyPropertyChanged
    {
        private IViewModel currentViewModel;
        public event PropertyChangedEventHandler PropertyChanged;

        public BackstageViewModel()
        {
            InfoViewModel = new InfoViewModel();
            OpenViewModel = new OpenViewModel();
            AboutViewModel = new AboutViewModel();

            SetCurrentViewModelCommand = new RelayCommand(OnSetCurrentViewModel);
        }

        public IViewModel CurrentViewModel
        {
            get => currentViewModel;
            set
            {
                currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public ICommand SetCurrentViewModelCommand { get; }

        public IViewModel InfoViewModel { get; }
        public IViewModel OpenViewModel { get; }
        public IViewModel AboutViewModel { get; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnSetCurrentViewModel(object obj)
        {
            CurrentViewModel = (IViewModel) obj;
        }
    }
}