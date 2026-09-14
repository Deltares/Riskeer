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

namespace Application.Riskeer.Integration.Test.AssemblyResolver
{
    [TestFixture]
    public class AssemblyResolverTest
    {
        [Test]
        public void ResolveAssembly_WhenAssemblyDoesNotExist_ReturnsNull()
        {
            var assemblyName = new AssemblyName("NonExistingAssembly");

            Assembly result = global::AssemblyResolver.AssemblyResolver.ResolveAssembly(assemblyName);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveAssembly_WhenAssemblyExists_ReturnsAssembly()
        {
            const string assemblyFullName = "log4net, Version=3.3.2.0, Culture=neutral, PublicKeyToken=669e0ddf0bb1aa2a";

            var assemblyName = new AssemblyName(assemblyFullName);
            Assembly result = global::AssemblyResolver.AssemblyResolver.ResolveAssembly(assemblyName);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetName().FullName, Is.EqualTo(assemblyFullName));
        }

        [Test]
        public void ResolveAssembly_WhenAssemblyIsNative_ReturnsNull()
        {
            var assemblyName = new AssemblyName("SQLite.Interop");

            Assembly result = global::AssemblyResolver.AssemblyResolver.ResolveAssembly(assemblyName);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveAssembly_WhenVersionIsWildcard_ReturnsHighestMatchingAssembly()
        {
            var assemblyName = new AssemblyName("log4net, Version=0.0.0.0");

            Assembly result = global::AssemblyResolver.AssemblyResolver.ResolveAssembly(assemblyName);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetName().Version, Is.EqualTo(new Version(3, 3, 2, 0)));
        }

        [Test]
        public void ResolveAssembly_WhenVersionIsWildcardAndPublicKeyTokenDiffers_ReturnsNull()
        {
            var assemblyName = new AssemblyName("log4net, Version=0.0.0.0, Culture=neutral, PublicKeyToken=0123456789abcdef");

            Assembly result = global::AssemblyResolver.AssemblyResolver.ResolveAssembly(assemblyName);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveAssembly_WhenAssemblyIsSatelliteResourceAssembly_ReturnsNull()
        {
            var assemblyName = new AssemblyName("AvalonDock.resources, Version=4.74.1.0, Culture=de, PublicKeyToken=3e4669d2f30244f4");

            Assembly result = global::AssemblyResolver.AssemblyResolver.ResolveAssembly(assemblyName);

            Assert.That(result, Is.Null);
        }
    }
}