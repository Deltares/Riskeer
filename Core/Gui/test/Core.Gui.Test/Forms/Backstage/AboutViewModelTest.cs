// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Globalization;
using System.Linq;
using System.Management;
using System.Windows;
using Core.Gui.Forms.Backstage;
using NUnit.Framework;

namespace Core.Gui.Test.Forms.Backstage
{
    [TestFixture]
    public class AboutViewModelTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var viewModel = new AboutViewModel();

            // Assert
            Assert.IsInstanceOf<IBackstagePageViewModel>(viewModel);
            Assert.AreEqual("Riskeer 21.2.1.1", viewModel.Version);
            
            ManagementObject processorManagementObject =
                new ManagementObjectSearcher("select * from Win32_Processor")
                    .Get()
                    .Cast<ManagementObject>()
                    .First();

            ManagementObject operatingSystemManagementObject =
                new ManagementObjectSearcher("select * from Win32_OperatingSystem")
                    .Get()
                    .Cast<ManagementObject>()
                    .First();
            
            var installedRam = $"{Math.Round(GetInstalledRam(), 2).ToString(CultureInfo.InvariantCulture)} GB";
            
            Assert.AreEqual(operatingSystemManagementObject["Caption"], viewModel.WindowsEdition);
            Assert.AreEqual(operatingSystemManagementObject["BuildNumber"], viewModel.WindowsBuild);
            Assert.AreEqual(processorManagementObject["Name"], viewModel.Processor);
            Assert.AreEqual(installedRam, viewModel.InstalledRam);
            Assert.AreEqual(GetResolution(), viewModel.Resolution);
        }

        private static string GetResolution()
        {
            return  $"{SystemParameters.PrimaryScreenWidth.ToString(CultureInfo.InvariantCulture)} " +
                    $"x {SystemParameters.PrimaryScreenHeight.ToString(CultureInfo.InvariantCulture)}";
        }

        private static double GetInstalledRam()
        {
            ManagementObject computerSystemManagementObject =
                new ManagementObjectSearcher("select * from Win32_ComputerSystem")
                    .Get()
                    .Cast<ManagementObject>()
                    .First();
            
            const double kilobyteDivider = 1024.0;
            const double gigabyteDivider = kilobyteDivider * kilobyteDivider * kilobyteDivider;
            return (ulong) computerSystemManagementObject["TotalPhysicalMemory"] / gigabyteDivider;
        }
    }
}