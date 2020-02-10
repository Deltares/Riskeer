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
    public class SerializableAssemblyReader : IDisposable
    {
        private bool disposed;
        private StreamReader reader;
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssemblyReader"/>.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <exception cref="CriticalFileReadException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="filePath"/> is invalid.</item>
        /// <item><paramref name="filePath"/> points to a file that does not exist.</item>
        /// </list>
        /// </exception>
        public SerializableAssemblyReader(string filePath)
        {
            try
            {
                IOUtils.ValidateFilePath(filePath);

                if (!File.Exists(filePath))
                {
                    string message = new FileReaderErrorMessageBuilder(filePath)
                        .Build(CoreCommonUtilResources.Error_File_does_not_exist);
                    throw new CriticalFileReadException(message);
                }

                this.filePath = filePath;

                reader = new StreamReader(filePath);
            }
            catch (ArgumentException e)
            {
                throw new CriticalFileReadException(e.Message, e);
            }
            catch (IOException e)
            {
                string message = new FileReaderErrorMessageBuilder(filePath).Build(CoreCommonUtilResources.Error_General_IO_Import_ErrorMessage);
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Reads a <see cref="SerializableAssembly"/> from a .gml file.
        /// </summary>
        /// <returns>The read <see cref="SerializableAssembly"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when an unexpected error
        /// occurred when reading the file.</exception>
        public SerializableAssembly Read()
        {
            var serializer = new XmlSerializer(typeof(SerializableAssembly));

            try
            {
                return (SerializableAssembly) serializer.Deserialize(reader);
            }
            catch (InvalidOperationException e)
            {
                string message = new FileReaderErrorMessageBuilder(filePath).Build(CoreCommonUtilResources.Error_General_IO_Import_ErrorMessage);
                throw new CriticalFileReadException(message, e);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
                reader = null;
            }

            disposed = true;
        }
    }
}