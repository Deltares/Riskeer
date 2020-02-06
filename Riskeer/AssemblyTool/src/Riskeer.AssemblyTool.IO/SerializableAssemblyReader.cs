using System;
using System.IO;
using System.Xml.Serialization;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Riskeer.AssemblyTool.IO.Model;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.AssemblyTool.IO
{
    /// <summary>
    /// Reader for reading data from a .gml file.
    /// </summary>
    public static class SerializableAssemblyReader
    {
        /// <summary>
        /// Reads a <see cref="SerializableAssembly"/> from a .gml file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The read <see cref="SerializableAssembly"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="filePath"/> points to a file that does not exist.</item>
        /// <item>An unexpected error occurred when reading the file.</item>
        /// </list>
        /// </exception>
        public static SerializableAssembly Read(string filePath)
        {
            IOUtils.ValidateFilePath(filePath);
            if (!File.Exists(filePath))
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            StreamReader reader = null;
            var serializer = new XmlSerializer(typeof(SerializableAssembly));
            
            try
            {
                reader = new StreamReader(filePath);
                return (SerializableAssembly) serializer.Deserialize(reader);
            }
            catch (Exception e) when(e is IOException || e is InvalidOperationException)
            {
                string message = new FileReaderErrorMessageBuilder(filePath).Build(CoreCommonUtilResources.Error_General_IO_Import_ErrorMessage);
                throw new CriticalFileReadException(message, e);
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
