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
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO
{
    /// <summary>
    /// File reader for a plain text file in comma-separated values format (*.csv), containing
    /// data specifying characteristic points. 
    /// Expects data to be specified in the following format:
    /// <para><c>
    /// LocationID;X_Maaiveld binnenwaarts;Y_Maaiveld binnenwaarts;Z_Maaiveld binnenwaarts;X_Insteek sloot polderzijde;Y_Insteek sloot polderzijde;Z_Insteek sloot polderzijde;X_Slootbodem polderzijde;Y_Slootbodem polderzijde;Z_Slootbodem polderzijde;X_Slootbodem dijkzijde;Y_Slootbodem dijkzijde;Z_Slootbodem dijkzijde;X_Insteek sloot dijkzijde;Y_Insteek_sloot dijkzijde;Z_Insteek sloot dijkzijde;X_Teen dijk binnenwaarts;Y_Teen dijk binnenwaarts;Z_Teen dijk binnenwaarts;X_Kruin binnenberm;Y_Kruin binnenberm;Z_Kruin binnenberm;X_Insteek binnenberm;Y_Insteek binnenberm;Z_Insteek binnenberm;X_Kruin binnentalud;Y_Kruin binnentalud;Z_Kruin binnentalud;X_Verkeersbelasting kant binnenwaarts;Y_Verkeersbelasting kant binnenwaarts;Z_Verkeersbelasting kant binnenwaarts;X_Verkeersbelasting kant buitenwaarts;Y_Verkeersbelasting kant buitenwaarts;Z_Verkeersbelasting kant buitenwaarts;X_Kruin buitentalud;Y_Kruin buitentalud;Z_Kruin buitentalud;X_Insteek buitenberm;Y_Insteek buitenberm;Z_Insteek buitenberm;X_Kruin buitenberm;Y_Kruin buitenberm;Z_Kruin buitenberm;X_Teen dijk buitenwaarts;Y_Teen dijk buitenwaarts;Z_Teen dijk buitenwaarts;X_Maaiveld buitenwaarts;Y_Maaiveld buitenwaarts;Z_Maaiveld buitenwaarts;X_Dijktafelhoogte;Y_Dijktafelhoogte;Z_Dijktafelhoogte;Volgnummer
    /// </c></para>
    /// Where {LocationID} has to be a particular accepted text, {Volgnummer} is ignored, and n triplets of doubles form the
    /// 3D coordinates defining each characteristic point.
    /// </summary>
    public class PipingCharacteristicPointsCsvReader : IDisposable
    {
        private const string expectedHeader = "locationid;x_maaiveld binnenwaarts;y_maaiveld binnenwaarts;z_maaiveld binnenwaarts;x_insteek sloot polderzijde;y_insteek sloot polderzijde;z_insteek sloot polderzijde;x_slootbodem polderzijde;y_slootbodem polderzijde;z_slootbodem polderzijde;x_slootbodem dijkzijde;y_slootbodem dijkzijde;z_slootbodem dijkzijde;x_insteek sloot dijkzijde;y_insteek sloot dijkzijde;z_insteek sloot dijkzijde;x_teen dijk binnenwaarts;y_teen dijk binnenwaarts;z_teen dijk binnenwaarts;x_kruin binnenberm;y_kruin binnenberm;z_kruin binnenberm;x_insteek binnenberm;y_insteek binnenberm;z_insteek binnenberm;x_kruin binnentalud;y_kruin binnentalud;z_kruin binnentalud;x_verkeersbelasting kant binnenwaarts;y_verkeersbelasting kant binnenwaarts;z_verkeersbelasting kant binnenwaarts;x_verkeersbelasting kant buitenwaarts;y_verkeersbelasting kant buitenwaarts;z_verkeersbelasting kant buitenwaarts;x_kruin buitentalud;y_kruin buitentalud;z_kruin buitentalud;x_insteek buitenberm;y_insteek buitenberm;z_insteek buitenberm;x_kruin buitenberm;y_kruin buitenberm;z_kruin buitenberm;x_teen dijk buitenwaarts;y_teen dijk buitenwaarts;z_teen dijk buitenwaarts;x_maaiveld buitenwaarts;y_maaiveld buitenwaarts;z_maaiveld buitenwaarts;x_dijktafelhoogte;y_dijktafelhoogte;z_dijktafelhoogte;volgnummer";
        private const char separator = ';';
        private readonly string filePath;

        /// <summary>
        /// Lower case string representations of the known characteristic point types.
        /// </summary>
        private readonly string[] characteristicPointKeys =
        {
            "maaiveld binnenwaarts",
            "insteek sloot polderzijde",
            "slootbodem polderzijde",
            "slootbodem dijkzijde",
            "insteek sloot dijkzijde",
            "teen dijk binnenwaarts",
            "kruin binnenberm",
            "insteek binnenberm",
            "kruin binnentalud",
            "verkeersbelasting kant binnenwaarts",
            "verkeersbelasting kant buitenwaarts",
            "kruin buitentalud",
            "insteek buitenberm",
            "kruin buitenberm",
            "teen dijk buitenwaarts",
            "maaiveld buitenwaarts",
            "dijktafelhoogte"
        };

        private StreamReader fileReader;

        /// <summary>
        /// Initializes a new instance of <see cref="PipingCharacteristicPointsCsvReader"/> using
        /// the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to use for reading characteristic points.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is invalid.</exception>
        public PipingCharacteristicPointsCsvReader(string path)
        {
            FileUtils.ValidateFilePath(path);

            filePath = path;
        }

        /// <summary>
        /// Reads the file to determine the number of available <see cref="RingtoetsCharacteristicPointsLocation"/>
        /// data rows.
        /// </summary>
        /// <returns>A value greater than or equal to 0.</returns>
        /// <exception cref="CriticalFileReadException">A critical error has occurred, which may be caused by:
        /// <list type="bullet">
        /// <item>File cannot be found at specified path.</item>
        /// <item>The specified path is invalid, such as being on an unmapped drive.</item>
        /// <item>Some other I/O related issue occurred, such as: path includes an incorrect 
        /// or invalid syntax for file name, directory name, or volume label.</item>
        /// <item>There is insufficient memory to allocate a buffer for the returned string.</item>
        /// <item>File incompatible for importing surface lines.</item>
        /// </list>
        /// </exception>
        public int GetLocationsCount()
        {
            using (var reader = StreamReaderHelper.InitializeStreamReader(filePath))
            {
                ValidateHeader(reader, 1);

                return CountNonEmptyLines(reader, 2);
            }
        }

        public void Dispose()
        {
            if (fileReader != null)
            {
                fileReader.Dispose();
                fileReader = null;
            }
        }

        /// <summary>
        /// Validates the header of the file.
        /// </summary>
        /// <param name="reader">The reader, which is currently at the header row.</param>
        /// <param name="currentLine">Row index used in error messaging.</param>
        /// <exception cref="CriticalFileReadException">The header is not in the required format.</exception>
        private void ValidateHeader(TextReader reader, int currentLine)
        {
            var header = ReadLineAndHandleIOExceptions(reader, currentLine);
            if (header != null)
            {
                if (!IsHeaderValid(header))
                {
                    throw CreateCriticalFileReadException(currentLine, Resources.PipingCharacteristicPointsCsvReader_File_invalid_header);
                }
            }
            else
            {
                throw CreateCriticalFileReadException(currentLine, Core.Common.Utils.Properties.Resources.Error_File_empty);
            }
        }

        /// <summary>
        /// Reads the next line and handles I/O exceptions.
        /// </summary>
        /// <param name="reader">The opened text file reader.</param>
        /// <param name="currentLine">Row number for error messaging.</param>
        /// <returns>The read line, or null when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">An critical I/O exception occurred.</exception>
        private string ReadLineAndHandleIOExceptions(TextReader reader, int currentLine)
        {
            try
            {
                return reader.ReadLine();
            }
            catch (OutOfMemoryException e)
            {
                throw CreateCriticalFileReadException(currentLine, Core.Common.Utils.Properties.Resources.Error_Line_too_big_for_RAM, e);
            }
            catch (IOException e)
            {
                var message = new FileReaderErrorMessageBuilder(filePath).Build(string.Format(Core.Common.Utils.Properties.Resources.Error_General_IO_ErrorMessage_0_, e.Message));
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Throws a configured instance of <see cref="CriticalFileReadException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="criticalErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="CriticalFileReadException"/> with message and inner exception set.</returns>
        private CriticalFileReadException CreateCriticalFileReadException(int currentLine, string criticalErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(Resources.TextFile_On_LineNumber_0_, currentLine);
            var message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                     .Build(criticalErrorMessage);
            return new CriticalFileReadException(message, innerException);
        }

        private bool IsHeaderValid(string header)
        {
            return expectedHeader == header.ToLowerInvariant();
        }

        /// <summary>
        /// Counts the remaining non-empty lines.
        /// </summary>
        /// <param name="reader">The reader at the row from which counting should start.</param>
        /// <param name="currentLine">The current line, used for error messaging.</param>
        /// <returns>An integer greater than or equal to 0, being the number of surfaceline rows.</returns>
        /// <exception cref="CriticalFileReadException">An I/O exception occurred.</exception>
        private int CountNonEmptyLines(TextReader reader, int currentLine)
        {
            int count = 0, lineNumberForMessage = currentLine;
            string line;
            while ((line = ReadLineAndHandleIOExceptions(reader, lineNumberForMessage)) != null)
            {
                if (!String.IsNullOrWhiteSpace(line))
                {
                    count++;
                }
                lineNumberForMessage++;
            }
            return count;
        }
    }
}