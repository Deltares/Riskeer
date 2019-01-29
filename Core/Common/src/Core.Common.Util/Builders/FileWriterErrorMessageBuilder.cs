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
using Core.Common.Util.Properties;

namespace Core.Common.Util.Builders
{
    /// <summary>
    /// Class to help create consistent file writer error messages.
    /// </summary>
    public class FileWriterErrorMessageBuilder
    {
        private readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWriterErrorMessageBuilder"/> class.
        /// </summary>
        /// <param name="filePath">The file path to the file where the error occurred.</param>
        public FileWriterErrorMessageBuilder(string filePath)
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
                                 Resources.Error_Writing_to_File_0_CustomMessage_1_,
                                 filePath,
                                 errorMessage);
        }
    }
}