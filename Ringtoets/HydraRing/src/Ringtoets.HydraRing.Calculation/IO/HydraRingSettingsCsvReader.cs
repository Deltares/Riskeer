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
    /// An abstract base for Hydra-Ring settings readers.
    /// </summary>
    /// <typeparam name="TOutput">The output format of the read settings.</typeparam>
    internal abstract class HydraRingSettingsCsvReader<TOutput>
    {
        private const char separator = ';';

        private readonly string fileContents;
        private readonly TOutput settings;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingSettingsCsvReader{T}"/>.
        /// </summary>
        /// <param name="fileContents">The file contents to read.</param>
        /// <param name="settings">The provided settings object to add the read settings to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileContents"/> or <paramref name="settings"/> is <c>null</c>.</exception>
        protected HydraRingSettingsCsvReader(string fileContents, TOutput settings)
        {
            if (string.IsNullOrEmpty(fileContents))
            {
                throw new ArgumentNullException("fileContents", @"File contents must be set.");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings", @"The settings object must be provided.");
            }

            this.fileContents = fileContents;
            this.settings = settings;
        }

        /// <summary>
        /// Reads the settings from the file contents.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> with the settings.</returns>
        public TOutput ReadSettings()
        {
            var lines = fileContents.Split('\n');

            foreach (var line in lines.Skip(1).Where(s => !string.IsNullOrEmpty(s)))
            {
                CreateSetting(TokenizeString(line));
            }

            return settings;
        }

        /// <summary>
        /// Gets the read settings.
        /// </summary>
        protected TOutput Settings
        {
            get
            {
                return settings;
            }
        }

        /// <summary>
        // Creates a setting object from the provided line and adds it to <see cref="Settings"/>.
        /// </summary>
        /// <param name="line">The line to create the setting for.</param>
        protected abstract void CreateSetting(IList<string> line);

        /// <summary>
        /// Gets a string value from the given <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element to extract the string value from.</param>
        /// <returns>The extracted string value.</returns>
        protected static string GetStringValueFromElement(string element)
        {
            return element.Trim().Replace("\"", "");
        }

        /// <summary>
        /// Gets an int value from the given <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element to extract the int value from.</param>
        /// <returns>The extracted int value.</returns>
        protected static int GetIntValueFromElement(string element)
        {
            return int.Parse(GetStringValueFromElement(element), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets a double value from the given <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element to extract the double value from.</param>
        /// <returns>The extracted double value.</returns>
        protected static double GetDoubleValueFromElement(string element)
        {
            return double.Parse(GetStringValueFromElement(element), CultureInfo.InvariantCulture);
        }

        private static string[] TokenizeString(string readText)
        {
            return !readText.Contains(separator)
                       ? new string[0]
                       : readText.Split(separator)
                                 .TakeWhile(text => !string.IsNullOrEmpty(text))
                                 .ToArray();
        }
    }
}