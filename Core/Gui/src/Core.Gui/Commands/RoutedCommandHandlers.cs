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