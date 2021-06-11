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
using System.Windows.Input;

namespace Core.Gui.Commands
{
    /// <summary>
    /// Defines a simple command that executes an <see cref="Action"/>.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> action;
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is <c>null</c>.</exception>
        public RelayCommand(Action<object> action) : this(action, o => true) {}

        /// <summary>
        /// Creates a new instance of <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="canExecute">The function that determines whether the command can execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public RelayCommand(Action<object> action, Func<object, bool> canExecute)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException(nameof(canExecute));
            }

            this.action = action;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }
    }
}