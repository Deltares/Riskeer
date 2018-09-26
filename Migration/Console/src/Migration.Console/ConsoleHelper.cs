// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.IO;
using SystemConsole = System.Console;

namespace Migration.Console
{
    /// <summary>
    /// This class defines methods that help writing to the <see cref="SystemConsole"/>.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Writes <paramref name="format"/> as an error text to the <see cref="SystemConsole"/>.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurred.</exception>
        /// <exception cref="FormatException">Thrown when the format specification in <paramref name="format"/> is invalid.</exception>
        /// <seealso cref="SystemConsole.WriteLine(string, object[])"/>
        public static void WriteErrorLine(string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            SystemConsole.ForegroundColor = ConsoleColor.Red;
            SystemConsole.WriteLine(format, args);
            SystemConsole.ResetColor();
        }

        /// <summary>
        /// Writes <paramref name="format"/> as a description text to the <see cref="SystemConsole"/>.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurred.</exception>
        /// <exception cref="FormatException">Thrown when the format specification in <paramref name="format"/> is invalid.</exception>
        /// <seealso cref="SystemConsole.WriteLine(string, object[])"/>
        public static void WriteCommandDescriptionLine(string format, params object[] args)
        {
            const int paddingLeft = 10;
            const int paddingRight = 1;
            WriteLineWithPadding(format, args, paddingLeft, paddingRight);
            SystemConsole.WriteLine();
        }

        private static void WriteLineWithPadding(string format, object[] args, int paddingLeft, int paddingRight)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            int bufferSize = GetLineWidth() - paddingRight - paddingLeft;
            var paddingString = new string(' ', paddingLeft);
            foreach (string line in format.SplitByLength(bufferSize))
            {
                SystemConsole.WriteLine($@"{paddingString}{line.TrimStart()}", args);
            }
        }

        private static int GetLineWidth()
        {
            try
            {
                return SystemConsole.WindowWidth;
            }
            catch (IOException)
            {
                return 80;
            }
        }

        private static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            for (var index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }
    }
}