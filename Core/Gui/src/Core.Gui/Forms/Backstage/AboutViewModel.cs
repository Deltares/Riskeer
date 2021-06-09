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

namespace Core.Gui.Forms.Backstage
{
    /// <summary>
    /// ViewModel for <see cref="AboutBackstagePage"/>.
    /// </summary>
    public class AboutViewModel : IBackstagePageViewModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="AboutViewModel"/>.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="version">The application version.</param>
        public AboutViewModel(string applicationName, string version)
        {
            ApplicationName = applicationName;
            Version = version;

            WindowsEdition = (string) GetOperatingSystemValue("Caption");
            WindowsBuild = (string) GetOperatingSystemValue("BuildNumber");
            Processor = (string) GetProcessorValue("Name");
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the Windows edition.
        /// </summary>
        public string WindowsEdition { get; }

        /// <summary>
        /// Gets the Windows build.
        /// </summary>
        public string WindowsBuild { get; }

        /// <summary>
        /// Gets the processor information.
        /// </summary>
        public string Processor { get; }

        /// <summary>
        /// Gets the amount of installed RAM.
        /// </summary>
        public string InstalledRam
        {
            get
            {
                const double kilobyteDivider = 1024.0;
                const double gigabyteDivider = kilobyteDivider * kilobyteDivider * kilobyteDivider;
                double installedRam = (ulong) GetComputerSystemValue("TotalPhysicalMemory") / gigabyteDivider;

                return $"{Math.Round(installedRam, 2).ToString(CultureInfo.InvariantCulture)} GB";
            }
        }

        /// <summary>
        /// Gets the primary display resolution.
        /// </summary>
        public string Resolution =>
            $"{SystemParameters.PrimaryScreenWidth.ToString(CultureInfo.InvariantCulture)} " +
            $"x {SystemParameters.PrimaryScreenHeight.ToString(CultureInfo.InvariantCulture)}";

        private static object GetOperatingSystemValue(string propertyName)
        {
            ManagementObject managementObject =
                new ManagementObjectSearcher("select * from Win32_OperatingSystem")
                    .Get()
                    .Cast<ManagementObject>()
                    .First();
            return managementObject[propertyName];
        }

        private static object GetProcessorValue(string propertyName)
        {
            ManagementObject managementObject =
                new ManagementObjectSearcher("select * from Win32_Processor")
                    .Get()
                    .Cast<ManagementObject>()
                    .First();
            return managementObject[propertyName];
        }

        private static object GetComputerSystemValue(string propertyName)
        {
            ManagementObject managementObject =
                new ManagementObjectSearcher("select * from Win32_ComputerSystem")
                    .Get()
                    .Cast<ManagementObject>()
                    .First();
            return managementObject[propertyName];
        }
    }
}