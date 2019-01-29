// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Components.GraphSharp.Commands;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.TestUtil;
using NUnit.Framework;

namespace Core.Components.GraphSharp.Test.Commands
{
    [TestFixture]
    public class VertexSelectedCommandTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var command = new VertexSelectedCommand(PointedTreeTestDataFactory.CreatePointedTreeElementVertex());

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
        }

        [Test]
        public void Constructor_VertexNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new VertexSelectedCommand(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("vertex", exception.ParamName);
        }

        [Test]
        public void CanExecute_VertexIsSelectableFalse_ReturnFalse()
        {
            // Setup
            var command = new VertexSelectedCommand(PointedTreeTestDataFactory.CreatePointedTreeElementVertex());

            // Call
            bool canExecute = command.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [Test]
        public void CanExecute_VertexIsSelectableTrue_ReturnTrue()
        {
            // Setup
            var command = new VertexSelectedCommand(PointedTreeTestDataFactory.CreatePointedTreeElementVertex(true));

            // Call
            bool canExecute = command.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [Test]
        public void Execute_Always_SetVertexSelectionToTrue()
        {
            // Setup
            PointedTreeElementVertex vertex = PointedTreeTestDataFactory.CreatePointedTreeElementVertex(true);
            var command = new VertexSelectedCommand(vertex);

            // Precondition
            Assert.IsFalse(vertex.IsSelected);

            // Call
            command.Execute(null);

            // Assert
            Assert.IsTrue(vertex.IsSelected);
        }
    }
}