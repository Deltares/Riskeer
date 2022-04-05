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
using System.Windows.Media.Imaging;
using Core.Gui.Properties;
using Core.Gui.Settings;

namespace Core.Gui.Forms.Backstage
{
    /// <summary>
    /// ViewModel for <see cref="AboutBackstagePage"/>.
    /// </summary>
    public class AboutViewModel : IBackstagePageViewModel
    {
        private readonly GuiCoreSettings settings;

        /// <summary>
        /// Creates a new instance of <see cref="AboutViewModel"/>.
        /// </summary>
        /// <param name="settings">The application settings.</param>
        /// <param name="version">The application version.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is <c>null</c>.</exception>
        public AboutViewModel(GuiCoreSettings settings, string version)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.settings = settings;

            Version = version;

            WindowsEdition = (string) GetManagementObjectProperty("Win32_OperatingSystem", "Caption")
                             ?? Resources.AboutViewModel_Unknown_value;
            WindowsBuild = (string) GetManagementObjectProperty("Win32_OperatingSystem", "BuildNumber")
                           ?? Resources.AboutViewModel_Unknown_value;
            Processor = (string) GetManagementObjectProperty("Win32_Processor", "Name")
                        ?? Resources.AboutViewModel_Unknown_value;
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName => settings.ApplicationName;

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
        public static string InstalledRam
        {
            get
            {
                const double kilobyteDivider = 1024.0;
                const double gigabyteDivider = kilobyteDivider * kilobyteDivider * kilobyteDivider;
                object totalPhysicalMemory = GetManagementObjectProperty("Win32_ComputerSystem", "TotalPhysicalMemory");

                return totalPhysicalMemory != null
                           ? string.Format(Resources.AboutViewModel_InstalledRam_0_GB,
                                           Math.Round((ulong) totalPhysicalMemory / gigabyteDivider, 2)
                                               .ToString(CultureInfo.InvariantCulture))
                           : Resources.AboutViewModel_Unknown_value;
            }
        }

        /// <summary>
        /// Gets the primary display resolution.
        /// </summary>
        public static string Resolution =>
            $"{SystemParameters.PrimaryScreenWidth.ToString(CultureInfo.InvariantCulture)} " +
            $"x {SystemParameters.PrimaryScreenHeight.ToString(CultureInfo.InvariantCulture)}";

        /// <summary>
        /// Gets the made by bitmap image.
        /// </summary>
        public BitmapImage MadeByBitmapImage => new BitmapImage(new Uri("pack://application:,,,/Core.Common.Controls;component/Resources/MadeBy.png"));

        private static object GetManagementObjectProperty(string managementObjectName, string propertyName)
        {
            try
            {
                ManagementObject managementObject =
                    new ManagementObjectSearcher($"select * from {managementObjectName}")
                        .Get()
                        .Cast<ManagementObject>()
                        .FirstOrDefault();
                return managementObject?[propertyName];
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}