// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Reflection;
using NUnit.Framework;

namespace Application.Riskeer.Integration.Test.AssemblyResolverTest
{
    [TestFixture]
    public class AssemblyResolverTest
    {
        [Test]
        public void ResolveAssembly_WhenAssemblyDoesNotExist_ReturnsNull()
        {
            var args = new ResolveEventArgs("NonExistingAssembly");

            Assembly result = AssemblyResolver.ResolveAssembly(null, args);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveAssembly_WhenAssemblyExists_ReturnsAssembly()
        {
            var args = new ResolveEventArgs("System.Memory, Version=4.0.5.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");
            Assembly result = AssemblyResolver.ResolveAssembly(null, args);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetName().FullName, Is.EqualTo("System.Memory, Version=4.0.5.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"));
        }

        [Test]
        public void ResolveAssembly_WhenNativeDll_ReturnsNull()
        {
            var args = new ResolveEventArgs("SQLite.Interop");

            Assembly result = AssemblyResolver.ResolveAssembly(null, args);

            Assert.That(result, Is.Null);
        }
    }
}