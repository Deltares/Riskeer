// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Integration.Test
{
    [TestFixture]
    [Explicit("Ignored because the test is too expensive to run on every commit.")]
    public class ResourcesTest
    {
        private string outputFilePath;

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }
        }

        /// <summary>
        /// This test will determine if a resource defined in <c>Resources.resx</c> is used in the source code.
        /// </summary>
        [Test]
        public void UnusedResourceSearcher_Always_WritesFileWithEmbeddedResources()
        {
            // Setup
            string solution = TestHelper.SolutionRoot;
            string outputPath = TestHelper.GetScratchPadPath();
            const string resource = "Resources.resx";
            const string sourceCode = "*.cs;*.xaml";
            const string filters = "Resources.designer.cs;test";
            string searchPatterns = Regex.Replace("\\[.*\\(typeof\\(.*Resources\\).*\"-f-\".*\\)\\];" +
                                                  "\"-f-\"\\);Resources.-f-;Resources\\\\-f-", @"(\\*)" +
                                                                                               "\"", @"$1$1\" + "\"");

            string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UnusedResourceSearcher");
            string executable = Path.Combine(directory, "UnusedResourceSearcher.exe");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(executable)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = string.Join(" ", solution, outputPath, resource, sourceCode, filters, searchPatterns)
                }
            };

            // Call
            process.Start();
            process.WaitForExit();

            // Assert
            outputFilePath = Path.Combine(outputPath, "UnusedResources.txt");
            var lines = new List<string>();
            using (var reader = new StreamReader(outputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            string message = $"The following resources are marked as unused:{Environment.NewLine}{string.Join(Environment.NewLine, lines.OrderBy(s => s).ToList())}";
            Assert.AreEqual(0, lines.Count, message);
        }
    }
}