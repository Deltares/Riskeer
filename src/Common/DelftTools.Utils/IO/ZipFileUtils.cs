using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace DelftTools.Utils.IO
{
    public static class ZipFileUtils
    {
        //todo add callback function to report when extraction is finished. use separate thread?
        public static void Extract(string zipFile, string destinationPath)
        {
            var fastZip=new FastZip();
            fastZip.ExtractZip(zipFile,destinationPath,"");
        }

        /// <summary>
        /// Creates a zip file containing the provided filePaths. (File will be overwritten when it already exits)
        /// </summary>
        /// <param name="zipFileName">Name and path of the zipfile to create</param>
        /// <param name="filePaths">List with files to add to the zip file</param>
        /// <exception cref="IOException">Throws IOExecptions</exception>
        public static void Create(string zipFileName, List<string> filePaths)
        {
            Create(zipFileName, filePaths, true);
        }

        /// <summary>
        /// Creates a zip file containing the provided filePaths. (File will be overwritten when it already exits)
        /// The zip entries will be stored relative if dirContainingZipFiles is specified, and absolute otherwise.
        /// </summary>
        /// <param name="zipFileName">Name and path of the zipfile to create</param>
        /// <param name="dirContainingZipFiles">The directory that containings the files to be zipped (absolute path)</param>
        /// <param name="filePaths">List with files to add to the zip file</param>
        /// <exception cref="IOException">Throws IOExecptions</exception>
        public static void Create(string zipFileName, string dirContainingZipFiles, List<string> filePaths)
        {
            Create(zipFileName, dirContainingZipFiles, filePaths, true);
        }

        /// <summary>
        /// Creates a zip file containing the provided filePaths.
        /// </summary>
        /// <param name="zipFileName">Name and path of the zipfile to create</param>
        /// <param name="filePaths">List with files to add to the zip file</param>
        /// <param name="overwriteIfExits">Determents if the file will be overwritten if it already exists</param>
        /// <exception cref="IOException">Throws IOExecptions</exception>
        public static void Create(string zipFileName, List<string> filePaths, bool overwriteIfExits)
        {
            Create(zipFileName, null, filePaths, overwriteIfExits);
        }


        /// <summary>
        /// Creates a zip file containing the provided filePaths. The zip entries will be stored relative if dirContainingZipFiles
        /// is specified, and absolute otherwise.
        /// </summary>
        /// <param name="zipFileName">Name and path of the zipfile to create</param>
        /// <param name="dirContainingZipFiles">The directory that containings the files to be zipped (absolute path)</param>
        /// <param name="filePaths">List with files to add to the zip file</param>
        /// <param name="overwriteIfExits">Determents if the file will be overwritten if it already exists</param>
        /// <param name="fileNamesInZipFile">Optinal: set of files that should be named different in the zip file (only works for non-absolute paths)</param>
        /// <exception cref="IOException">Throws IOExecptions</exception>
        public static void Create(string zipFileName, string dirContainingZipFiles, List<string> filePaths, bool overwriteIfExits, IDictionary<string, string> fileNamesInZipFile = null)
        {
            if (!overwriteIfExits && File.Exists(zipFileName))
            {
                throw new IOException("File already exists.");
            }

            ZipOutputStream zipStream = null;

            try
            {
                FileStream fsOut = File.Create(zipFileName);
                zipStream = new ZipOutputStream(fsOut);
                if (filePaths != null)
                {
                    foreach (var incomingFilePath in filePaths)
                    {
                        string actualFilePath = incomingFilePath;
                        string filePathInZipFileKey = incomingFilePath;
                        string filePathInZipFile = incomingFilePath;

                        if (dirContainingZipFiles != null)
                        {
                            if (Path.IsPathRooted(incomingFilePath))
                            {
                                filePathInZipFileKey = FileUtils.GetRelativePath(dirContainingZipFiles, incomingFilePath);
                            }
                            else
                            {
                                actualFilePath = Path.Combine(dirContainingZipFiles, incomingFilePath);
                            }
                        }

                        if (fileNamesInZipFile != null && fileNamesInZipFile.ContainsKey(filePathInZipFileKey))
                        {
                            filePathInZipFile = fileNamesInZipFile[filePathInZipFileKey];
                        }

                        if (Path.IsPathRooted(filePathInZipFile))
                        {
                            throw new InvalidOperationException("");
                        }

                        FileInfo fi = new FileInfo(actualFilePath);

                        string entryName = ZipEntry.CleanName(filePathInZipFile); // Removes drive from name and fixes slash direction;
                        ZipEntry newEntry = new ZipEntry(entryName);
                        newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                        newEntry.Size = fi.Length;

                        zipStream.PutNextEntry(newEntry);

                        // Zip the file in buffered chunks
                        // the "using" will close the stream even if an exception occurs
                        byte[] buffer = new byte[4096];
                        using (FileStream streamReader = File.OpenRead(actualFilePath))
                        {
                            StreamUtilsCopy(streamReader, zipStream, buffer);
                        }
                        zipStream.CloseEntry();
                    }
                }
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
                    zipStream.Close();
                }
            }
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
        /// <param name="source">The stream to source data from.</param>
        /// <param name="destination">The stream to write data to.</param>
        /// <param name="buffer">The buffer to use during copying.</param>
        static public void StreamUtilsCopy(Stream source, Stream destination, byte[] buffer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < 128)
            {
                throw new ArgumentException("Buffer is too small", "buffer");
            }

            bool copying = true;

            while (copying)
            {
                int bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                }
                else
                {
                    destination.Flush();
                    copying = false;
                }
            }
        }

        public static IList<string> GetFilePathsInZip(string zippedStateFilePath, string dirContainingModelStateFiles)
        {
            IList<string> filePathsInZip = new List<string>();

            using (Stream stream = File.Open(zippedStateFilePath, FileMode.Open))
            {
                using (ZipInputStream zipInputStream = new ZipInputStream(stream))
                {
                    zipInputStream.IsStreamOwner = false;

                    ZipEntry zipEntry = zipInputStream.GetNextEntry();
                    while (zipEntry != null)
                    {
                        string filePath = zipEntry.Name;
                        if (Path.IsPathRooted(filePath) && dirContainingModelStateFiles != null)
                        {
                            filePath = FileUtils.GetRelativePath(dirContainingModelStateFiles, filePath);
                        }
                        filePathsInZip.Add(filePath);

                        zipEntry = zipInputStream.GetNextEntry();
                    }
                }
            }

            return filePathsInZip;
        }
    }
}
