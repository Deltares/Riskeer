// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Globalization;
using Core.Common.Util.Extensions;
using Core.Common.Util.Properties;

namespace Core.Common.Util.Builders
{
    /// <summary>
    /// Class to help create consistent folder writer error messages.
    /// </summary>
    public class DirectoryWriterErrorMessageBuilder
    {
        private readonly string folderPath;

        /// <summary>
        /// Initializes a new instance of <see cref="DirectoryWriterErrorMessageBuilder"/> class.
        /// </summary>
        /// <param name="folderPath">The file path to the directory where the error occurred.</param>
        public DirectoryWriterErrorMessageBuilder(string folderPath)
        {
            this.folderPath = folderPath;
        }

        /// <summary>
        /// Builds the specified error message.
        /// </summary>
        /// <param name="errorMessage">The message about the error that occurred.</param>
        /// <returns>The full error message.</returns>
        public string Build(string errorMessage)
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 Resources.Error_Writing_to_Directory_0_CustomMessage_1_,
                                 folderPath,
                                 errorMessage.FirstToLower());
        }
    }
}