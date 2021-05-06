using System.Collections;
using System.Collections.Specialized;
using System.Windows;

namespace Core.Gui.Commands
{
    /// <summary>
    ///  Holds a collection of <see cref="RoutedCommandHandler"/> that should be
    ///  turned into CommandBindings.
    /// </summary>
    public class RoutedCommandHandlers : FreezableCollection<RoutedCommandHandler>
    {
        private static readonly DependencyProperty commandsProperty = DependencyProperty.RegisterAttached(
            "CommandsPrivate",
            typeof(RoutedCommandHandlers),
            typeof(RoutedCommandHandlers),
            new PropertyMetadata(default(RoutedCommandHandlers)));

        private readonly FrameworkElement owner;

        /// <summary>
        /// Creates a new instance of <see cref="RoutedCommandHandlers"/>.
        /// </summary>
        /// <param name="owner"> The element for which this collection is created. </param>
        private RoutedCommandHandlers(FrameworkElement owner)
        {
            this.owner = owner;

            ((INotifyCollectionChanged) this).CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Gets the collection of RoutedCommandHandler for a given element, creating
        ///  it if it doesn't already exist.
        /// </summary>
        /// <param name="element">The element to which <see cref="RoutedCommandHandlers"/>
        /// was added.</param>
        public static RoutedCommandHandlers GetCommands(FrameworkElement element)
        {
            var handlers = (RoutedCommandHandlers) element.GetValue(commandsProperty);
            if (handlers == null)
            {
                handlers = new RoutedCommandHandlers(element);
                element.SetValue(commandsProperty, handlers);
            }

            return handlers;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new RoutedCommandHandlers(owner);
        }

        private static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            ((RoutedCommandHandlers) sender).OnAdd(args.NewItems);
        }

        private void OnAdd(IEnumerable newItems)
        {
            if (newItems == null)
            {
                return;
            }

            foreach (RoutedCommandHandler routedHandler in newItems)
            {
                routedHandler.Register(owner);
            }
        }
    }
}