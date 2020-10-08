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
using System.IO;
using NUnit.Framework;

namespace Core.Common.Assembly.Test
{
    [TestFixture]
    public class AssemblyResolverTest
    {
        private string validAssemblyDirectory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AssemblyResolver.Reset(); // Reset any initialization by other test projects

            validAssemblyDirectory = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
        }

        [TearDown]
        public void TearDown()
        {
            AssemblyResolver.Reset();
        }

        [Test]
        public void RequiresInitialization_AssemblyResolverNotInitialized_ReturnsTrue()
        {
            // Call
            bool requiresInitialization = AssemblyResolver.RequiresInitialization;

            // Assert
            Assert.IsTrue(requiresInitialization);
        }

        [Test]
        public void RequiresInitialization_AssemblyResolverInitialized_ReturnsFalse()
        {
            // Setup
            AssemblyResolver.Initialize(validAssemblyDirectory);

            // Call
            bool requiresInitialization = AssemblyResolver.RequiresInitialization;

            // Assert
            Assert.IsFalse(requiresInitialization);
        }

        [Test]
        public void Initialize_AssemblyResolverAlreadyInitialized_ThrowsInvalidOperationExceptionAndLeavesAssemblyResolverInitialized()
        {
            // Setup
            AssemblyResolver.Initialize(validAssemblyDirectory);

            // Call
            void Call() => AssemblyResolver.Initialize(validAssemblyDirectory);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(Call);
            Assert.AreEqual("Cannot initialize the assembly resolver more than once.", exception.Message);
            Assert.IsFalse(AssemblyResolver.RequiresInitialization);
        }

        [Test]
        public void Initialize_InvalidAssemblyDirectory_ThrowsDirectoryNotFoundExceptionAndLeavesAssemblyResolverStillRequiringInitialization()
        {
            // Call
            void Call() => AssemblyResolver.Initialize("Invalid");

            // Assert
            var exception = Assert.Throws<DirectoryNotFoundException>(Call);
            Assert.AreEqual("Cannot find the directory 'Invalid'.", exception.Message);
            Assert.IsTrue(AssemblyResolver.RequiresInitialization);
        }

        [Test]
        public void GetApplicationDirectory_Always_ReturnsApplicationDirectory()
        {
            // Call
            string applicationDirectory = AssemblyResolver.GetApplicationDirectory();

            // Assert
            StringAssert.EndsWith("Application", applicationDirectory);
        }
    }
}