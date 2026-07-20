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
using System.IO;
using System.Linq;
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
            var args = new ResolveEventArgs("NonExistingAssembly");

            Assembly result = Riskeer.AssemblyResolver.ResolveAssembly(null, args);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveAssembly_WhenAssemblyExists_ReturnsAssembly()
        {
            var assemblyPath = FindAssemblyFile("System.Memory.dll");

            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            var args = new ResolveEventArgs(assemblyName.FullName);
            Assembly result = Riskeer.AssemblyResolver.ResolveAssembly(null, args);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetName().FullName, Is.EqualTo(assemblyName.FullName));
        }

        [Test]
        public void TryCreateAssemblyPath_WhenNativeDll_ReturnsNull()
        {
            var result = Riskeer.AssemblyResolver.TryCreateAssemblyPath(FindAssemblyFile("SQLite.Interop.dll"));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void NoDuplicateAssemblyIdentitiesExist()
        {
            var duplicateAssemblyIdentities = Directory
                                              .EnumerateFiles(Riskeer.AssemblyResolver.Root, "*.dll", SearchOption.AllDirectories)
                                              .Select(Riskeer.AssemblyResolver.TryCreateAssemblyPath)
                                              .Where(identity => identity != null)
                                              .GroupBy(
                                                  identity => identity.AssemblyName.FullName,
                                                  StringComparer.OrdinalIgnoreCase)
                                              .Where(group => group.Count() > 1)
                                              .ToList();

            foreach (var duplicateAssemblyIdentity in duplicateAssemblyIdentities)
            {
                TestContext.WriteLine($"Duplicate assembly identity: {duplicateAssemblyIdentity.Key}");

                foreach (Riskeer.AssemblyResolver.AssemblyPath identity in duplicateAssemblyIdentity)
                {
                    TestContext.WriteLine($"  {identity.Path}");
                }

                TestContext.WriteLine(string.Empty);
            }

            Assert.That(
                duplicateAssemblyIdentities,
                Is.Empty,
                "Duplicate assembly identities were found. See test output for details.");
        }

        private static string FindAssemblyFile(string assemblyName)
        {
            string path = Directory.EnumerateFiles(Riskeer.AssemblyResolver.Root,
                                                   assemblyName, SearchOption.AllDirectories).First();
            Assert.IsNotEmpty(path);
            return path;
        }
    }
}