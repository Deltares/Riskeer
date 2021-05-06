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
using System.Collections.Generic;
using Core.Gui.Commands;
using NUnit.Framework;

namespace Core.Gui.Test.Commands
{
    [TestFixture]
    public class RelayCommandTest
    {
        [Test]
        public void WhenCreatingRelayCommandWithoutAction_ThenArgumentNullExceptionIsThrown()
        {
            // When
            void Call() => new RelayCommand(null);

            // Then
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("action", exception.ParamName);
        }

        [Test]
        public void GivenRelayCommand_WhenCanExecuteIsDetermined_ThenReturnsTrue()
        {
            // Given
            var command = new RelayCommand(p => {});

            // When
            bool canExecute = command.CanExecute(null);

            // Then
            Assert.IsTrue(canExecute);
        }

        [Test]
        public void GivenRelayCommandWithoutCanExecuteFunction_ThenArgumentNullExceptionIsThrown()
        {
            // When
            void Call() => new RelayCommand(p => {}, null);

            // Then
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("canExecute", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenRelayCommandWithCanExecuteFunction_WhenCanExecuteIsDetermined_ThenFunctionCalled(
            bool valueToReturn)
        {
            // Given
            var obj = new object();
            object parameter = null;
            var command = new RelayCommand(p => {}, p =>
            {
                parameter = p;
                return valueToReturn;
            });

            // When
            bool canExecute = command.CanExecute(obj);

            // Then
            Assert.AreEqual(valueToReturn, canExecute);
            Assert.AreEqual(obj, parameter);
        }

        [Test]
        public void GivenRelayCommand_WhenExecuted_ThenGivenActionIsExecuted()
        {
            // Given
            var parameter = new object();
            var actionsCalled = new List<object>();
            var command = new RelayCommand(p => actionsCalled.Add(p));

            // When
            command.Execute(parameter);

            // Then
            CollectionAssert.AreEqual(new[]
            {
                parameter
            }, actionsCalled);
        }
    }
}