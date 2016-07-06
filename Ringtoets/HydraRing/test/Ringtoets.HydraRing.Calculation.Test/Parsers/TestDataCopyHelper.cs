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

using System;
using System.IO;
using Core.Common.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    /// <summary>
    /// This class can be used to easily copy test data to a working directory.
    /// After use, the working directory is removed (together with the contents).
    /// </summary>
    public class TestDataCopyHelper : IDisposable
    {
        private readonly string testDataPath;
        private readonly string workingDirectory;
        private readonly DirectoryDisposeHelper directoryDisposeHelper;

        /// <summary>
        /// Creates a new <see cref="TestDataCopyHelper"/>. A directory is created at
        /// <paramref name="workingDirectory"/>.
        /// </summary>
        /// <param name="testDataPath">The source directory of the test files.</param>
        /// <param name="workingDirectory">The target directory.</param>
        public TestDataCopyHelper(string testDataPath, string workingDirectory)
        {
            this.testDataPath = testDataPath;
            this.workingDirectory = workingDirectory;

            directoryDisposeHelper = new DirectoryDisposeHelper(workingDirectory);
        }

        /// <summary>
        /// Copies the test file from the test directory to the working directory.
        /// </summary>
        /// <param name="testFile">The name of the test's input file.</param>
        /// <param name="outputFile">The name of the output file.</param>
        public void CopyToTemporaryOutput(string testFile, string outputFile)
        {
            var inputFilePath = Path.Combine(testDataPath, testFile);
            var outputFilePath = Path.Combine(workingDirectory, outputFile);
            File.Copy(inputFilePath, outputFilePath);
        }

        public void Dispose()
        {
            directoryDisposeHelper.Dispose();
        }
    }
}