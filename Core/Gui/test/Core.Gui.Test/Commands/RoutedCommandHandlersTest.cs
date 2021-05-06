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

using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Core.Gui.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Gui.Test.Commands
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class RoutedCommandHandlersTest
    {
        [Test]
        public void GivenFrameworkElement_WhenCommandsObtainedThroughRoutedCommands_ThenEmptyRoutedCommandHandlersReturned()
        {
            // Given
            var frameworkElement = new FrameworkElement();

            // When
            RoutedCommandHandlers handlers = RoutedCommandHandlers.GetCommands(frameworkElement);

            // Then
            Assert.IsEmpty(handlers);
        }

        [Test]
        public void GivenHandlersFromFrameworkElement_WhenCommandsObtainedTwice_ThenSameInstanceReturned()
        {
            // Given
            var frameworkElement = new FrameworkElement();
            RoutedCommandHandlers handlersA = RoutedCommandHandlers.GetCommands(frameworkElement);

            // When
            RoutedCommandHandlers handlersB = RoutedCommandHandlers.GetCommands(frameworkElement);

            // Then
            Assert.AreSame(handlersA, handlersB);
        }

        [Test]
        public void
            GivenHandlersFromFrameworkElement_WhenRoutedCommandHandlerAdded_ThenPropertiesRegisteredAsCommandBindingToFrameworkElement()
        {
            // Given
            var frameworkElement = new FrameworkElement();
            RoutedCommandHandlers handlers = RoutedCommandHandlers.GetCommands(frameworkElement);

            // When
            var routedCommand = new RoutedCommand();
            var handler = new RoutedCommandHandler
            {
                RoutedCommand = routedCommand
            };
            handlers.Add(handler);

            // Then
            Assert.AreEqual(1, handlers.Count);
            Assert.AreSame(routedCommand, handlers.First().RoutedCommand);
        }

        [Test]
        public void GivenAddedRoutedCommandHandlerWithCommand_WhenRoutedCommandExecuted_ThenCommandExecuted()
        {
            // Given
            var parameter = new object();

            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.CanExecute(parameter)).Return(true);
            command.Expect(c => c.Execute(parameter));
            mocks.ReplayAll();

            var frameworkElement = new FrameworkElement();
            RoutedCommandHandlers handlers = RoutedCommandHandlers.GetCommands(frameworkElement);

            var routedCommand = new RoutedCommand();
            var handler = new RoutedCommandHandler
            {
                RoutedCommand = routedCommand,
                Command = command
            };
            handlers.Add(handler);

            // When
            routedCommand.Execute(parameter, frameworkElement);

            // Then
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAddedRoutedCommandHandlerWithCommand_WhenRoutedCommandCannotExecute_ThenCommandNotExecuted()
        {
            // Given
            var parameter = new object();

            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.CanExecute(parameter)).Return(false);
            mocks.ReplayAll();

            var frameworkElement = new FrameworkElement();
            RoutedCommandHandlers handlers = RoutedCommandHandlers.GetCommands(frameworkElement);

            var routedCommand = new RoutedCommand();
            var handler = new RoutedCommandHandler
            {
                RoutedCommand = routedCommand,
                Command = command
            };
            handlers.Add(handler);

            // When
            routedCommand.Execute(parameter, frameworkElement);

            // Then
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAddedRoutedCommandHandlerWithoutCommand_WhenRoutedCommandExecuted_ThenNoExceptionsAreThrown()
        {
            // Given
            var frameworkElement = new FrameworkElement();
            RoutedCommandHandlers handlers = RoutedCommandHandlers.GetCommands(frameworkElement);

            var parameter = new object();

            var routedCommand = new RoutedCommand();
            var handler = new RoutedCommandHandler
            {
                RoutedCommand = routedCommand
            };
            handlers.Add(handler);

            // When
            void Call() => routedCommand.Execute(parameter, frameworkElement);

            // Then
            Assert.DoesNotThrow(Call);
        }
    }
}