// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Application.Ringtoets.Integration.Test
{
    [TestFixture]
    [Explicit]
    public class ResourcesTest
    {
        [Test]
        public void UnusedResourceSearcher_Always_WritesFileWithEmbeddedResources()
        {
            // Setup
            string solution = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.Parent.FullName;
            string outputPath = Directory.GetCurrentDirectory();
            string resource = "Resources.resx";
            string sourceCode = "*.cs,*.xaml";
            string filters = "Resources.designer.cs,test";
            string searchPatterns = "\"-f-\")],\"-f-\"),Resources.-f-,Resources\\-f-";
            
            
            string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UnusedResourceSearcher");
            string executable = Path.Combine(directory, "UnusedResourceSearcher.exe");

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo(executable)
                {
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Arguments = string.Format("{0} {1} {2} {3} {4} {5}", solution, outputPath, resource, sourceCode, filters, searchPatterns)
                }
            };

            // Call
            process.Start();
            process.WaitForExit();

            // Assert
            int counter = 0;
            string outputFilePath = Path.Combine(outputPath, "UnusedResources.txt");
            using (StreamReader reader = new StreamReader(outputFilePath))
            {
                while (reader.ReadLine() != null)
                {
                    counter++;
                }
            }

            Assert.AreEqual(0, counter);

            File.Delete(outputFilePath);
        }
    }
}