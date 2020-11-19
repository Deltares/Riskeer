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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Riskeer.Integration.Test.Assembly
{
    /// <summary>
    /// Class containing a method for generating a 'privatePath" value.
    ///
    /// This 'privatePath' value should go into all of our app.config files like this:
    ///
    ///  <runtime>
    ///    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
    ///      <probing privatePath="generated_privatePath_value" />
    ///    </assemblyBinding>
    ///  </runtime>
    ///
    /// Note: be sure to perform a full cleanup and rebuild before running this test! 
    /// </summary>
    [TestFixture]
    [Explicit("Test that should only be run manually")]
    public class RuntimeProbingPrivatePathGenerator
    {
        [Test]
        public void WritePrivatePathStringToConsole()
        {
            var privatePath = string.Empty;
            string assemblyDirectory = TestHelper.GetApplicationDirectory();
            string rootDirectory = Directory.GetParent(assemblyDirectory).FullName;

            AddAssemblyPathsRecursively(assemblyDirectory, ref privatePath);
            MakeAssemblyPathsRelativeToRootDirectory(rootDirectory, ref privatePath);

            Console.WriteLine(privatePath);
        }

        private static void AddAssemblyPathsRecursively(string assemblyDirectory, ref string privatePath)
        {
            privatePath += assemblyDirectory + ";";

            foreach (string directory in Directory.GetDirectories(assemblyDirectory))
            {
                AddAssemblyPathsRecursively(directory, ref privatePath);
            }
        }

        private static void MakeAssemblyPathsRelativeToRootDirectory(string rootDirectory, ref string assemblyPaths)
        {
            assemblyPaths = assemblyPaths.Replace(rootDirectory + @"\", string.Empty);
        }
    }
}