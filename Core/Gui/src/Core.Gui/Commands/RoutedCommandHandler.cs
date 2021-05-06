using System.Windows;
using System.Windows.Input;

namespace Core.Gui.Commands
{
     /// <summary>
    /// Allows for binding of <see cref="RoutedCommand"/> to the execution of a <see cref="ICommand"/>.
    /// </summary>
    public class RoutedCommandHandler : Freezable
    {
        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(RoutedCommandHandler),
            new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// Gets or sets the command that should be executed when the RoutedCommand fires.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// The command that triggers the <see cref="ICommand"/>.
        /// </summary>
        public ICommand RoutedCommand { get; set; }

        protected override Freezable CreateInstanceCore()
        {
            return new RoutedCommandHandler();
        }

        /// <summary>
        /// Registers this handler to respond to the registered RoutedCommand for the
        /// given element.
        /// </summary>
        /// <param name="owner"> The element for which we should register the command
        /// binding for the current routed command. </param>
        internal void Register(FrameworkElement owner)
        {
            var binding = new CommandBinding(RoutedCommand, HandleExecute, HandleCanExecute);
            owner.CommandBindings.Add(binding);
        }

        /// <summary>
        /// Executes <see cref="ICommand.CanExecute(object)"/> with the 
        /// <see cref="CanExecuteRoutedEventArgs.Parameter"/> from <paramref name="e"/>.
        /// </summary>
        /// <param name="sender">The owner of the routed command.</param>
        /// <param name="e">The event arguments given by the routed event.</param>
        private void HandleCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Command?.CanExecute(e.Parameter) == true;
            e.Handled = true;
        }

        /// <summary>
        /// Executes <see cref="ICommand.Execute(object)"/> with the 
        /// <see cref="ExecutedRoutedEventArgs.Parameter"/> from <paramref name="e"/>.
        /// </summary>
        /// <param name="sender">The owner of the routed command.</param>
        /// <param name="e">The event arguments given by the routed event.</param>
        private void HandleExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Command?.Execute(e.Parameter);
            e.Handled = true;
        }
    }
}