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

using System.Diagnostics;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Services;

namespace Riskeer.HydraRing.Calculation.Test.Services
{
    [TestFixture]
    public class HydraRingProcessFactoryTest
    {
        [Test]
        public void Create_ReturnsExpectedProcess()
        {
            // Call
            const string iniFilePath = "D:\\iniFile.text";
            const string workingDirectory = "D:\\workingDirectory";
            const string mechanismComputationExeFilePath = "D:\\mechanismComputation.exe";

            Process process = HydraRingProcessFactory.Create(mechanismComputationExeFilePath, iniFilePath, workingDirectory);

            // Assert
            Assert.AreEqual(mechanismComputationExeFilePath, process.StartInfo.FileName);
            Assert.AreEqual(iniFilePath, process.StartInfo.Arguments);
            Assert.AreEqual(workingDirectory, process.StartInfo.WorkingDirectory);
            Assert.IsFalse(process.StartInfo.UseShellExecute);
            Assert.IsTrue(process.StartInfo.CreateNoWindow);
            Assert.IsTrue(process.StartInfo.RedirectStandardInput);
            Assert.IsTrue(process.StartInfo.RedirectStandardOutput);
            Assert.IsTrue(process.StartInfo.RedirectStandardError);
        }
    }
}