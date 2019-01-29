// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Globalization;
using Core.Common.Util.Extensions;
using Core.Common.Util.Properties;

namespace Core.Common.Util.Builders
{
    /// <summary>
    /// Class to help create consistent file reader error messages.
    /// </summary>
    public class FileReaderErrorMessageBuilder
    {
        private readonly string filePath;
        private string location;
        private string subject;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileReaderErrorMessageBuilder"/> class.
        /// </summary>
        /// <param name="filePath">The file path to the file where the error occurred.</param>
        public FileReaderErrorMessageBuilder(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Builds the specified error message.
        /// </summary>
        /// <param name="errorMessage">The message about the error that has occurred.</param>
        /// <returns>The full error message.</returns>
        public string Build(string errorMessage)
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 Resources.FileReaderErrorMessageBuilder_Build_Error_while_reading_file_0_location_1_subject_2_errorMessage_3,
                                 filePath,
                                 location ?? string.Empty,
                                 subject ?? string.Empty,
                                 errorMessage.FirstToLower());
        }

        /// <summary>
        /// Adds file location information to the error message.
        /// </summary>
        /// <param name="locationDescription">The location description.</param>
        /// <returns>The builder being configured.</returns>
        /// <remarks>Call this method with a location description such as <c>"line 7"</c>.</remarks>
        public FileReaderErrorMessageBuilder WithLocation(string locationDescription)
        {
            location = " " + locationDescription;
            return this;
        }

        /// <summary>
        /// Adds the subject where the error occurred to the error message.
        /// </summary>
        /// <param name="subjectDescription">The subject description.</param>
        /// <returns>The builder being configured.</returns>
        /// <remarks>Call this method with a subject description such as <c>"soil profile 'blabla'"</c>.</remarks>
        public FileReaderErrorMessageBuilder WithSubject(string subjectDescription)
        {
            subject = string.Format(CultureInfo.CurrentCulture,
                                    " ({0})",
                                    subjectDescription);
            return this;
        }
    }
}