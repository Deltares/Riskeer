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

using System.IO;
using Core.Common.Base.IO;
using Core.Common.Util.Builders;
using Core.Common.Util.Properties;

namespace Core.Common.IO.Readers
{
    /// <summary>
    /// This class provides helper functions for reading UTF8 encoded files.
    /// </summary>
    public static class StreamReaderHelper
    {
        /// <summary>
        /// Initializes the stream reader for a UTF8 encoded file.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <returns>A UTF8 encoding configured stream reader opened on <paramref name="path"/>.</returns>
        /// <exception cref="CriticalFileReadException">File/directory cannot be found or 
        /// some other I/O related problem occurred.</exception>
        public static StreamReader InitializeStreamReader(string path)
        {
            try
            {
                return new StreamReader(path);
            }
            catch (FileNotFoundException e)
            {
                string message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message, e);
            }
            catch (DirectoryNotFoundException e)
            {
                string message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Directory_missing);
                throw new CriticalFileReadException(message, e);
            }
            catch (IOException e)
            {
                string message = new FileReaderErrorMessageBuilder(path).Build(string.Format(Resources.Error_General_IO_ErrorMessage_0_, e.Message));
                throw new CriticalFileReadException(message, e);
            }
        }
    }
}