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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ringtoets.HydraRing.Calculation.IO
{
    /// <summary>
    /// An abstract reader base for HydraRing settings readers.
    /// </summary>
    /// <typeparam name="TOuput">The output format of the read settings.</typeparam>
    internal abstract class HydraRingSettingsCsvReader<TOuput>
    {
        private const char separator = ';';

        private readonly string fileContents;
        private readonly TOuput settings;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingSettingsCsvReader{T}"/>.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="settings">The provided settings object to import the read settings on.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="file"/> or <paramref name="settings"/> is <c>null</c>.</exception>
        protected HydraRingSettingsCsvReader(string file, TOuput settings)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException("file", @"A file must be set.");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings", @"The settinsg object must be provided.");
            }

            fileContents = file;
            this.settings = settings;
        }

        /// <summary>
        /// Reads the settings from the file.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> with the settings.</returns>
        public TOuput ReadSettings()
        {
            var lines = fileContents.Split('\n');

            foreach (var line in lines.Skip(1).Where(s => !string.IsNullOrEmpty(s)))
            {
                CreateSetting(TokenizeString(line));
            }

            return settings;
        }

        protected TOuput Settings
        {
            get
            {
                return settings;
            }
        }

        /// <summary>
        /// Creates a setting from one line in the file.
        /// </summary>
        /// <param name="line">The line to create the setting for.</param>
        protected abstract void CreateSetting(IList<string> line);

        protected static string GetStringValueFromElement(string element)
        {
            return element.Trim().Replace("\"", "");
        }

        protected static int GetIntValueFromElement(string element)
        {
            return int.Parse(GetStringValueFromElement(element));
        }

        protected static double GetDoubleValueFromElement(string element)
        {
            return double.Parse(GetStringValueFromElement(element), CultureInfo.InvariantCulture);
        }

        private static string[] TokenizeString(string readText)
        {
            if (!readText.Contains(separator))
            {
                return new string[]
                {};
            }
            return readText.Split(separator)
                           .TakeWhile(text => !string.IsNullOrEmpty(text))
                           .ToArray();
        }
    }
}