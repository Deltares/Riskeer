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

        public event EventHandler CanExecuteChanged;

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
        /// <param name="canExecute">The function that determines whether command can execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is <c>null</c>.</exception>
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