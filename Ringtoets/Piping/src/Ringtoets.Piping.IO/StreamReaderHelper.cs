using System;
using System.IO;
using Core.Common.Utils.Builders;
using Core.Common.Utils.Properties;
using Ringtoets.Piping.IO.Exceptions;

namespace Ringtoets.Piping.IO
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
                var message = new FileReaderErrorMessageBuilder(path).Build(String.Format(Resources.Error_General_IO_ErrorMessage_0_, e.Message));
                throw new CriticalFileReadException(message, e);
            }
        }
    }
}