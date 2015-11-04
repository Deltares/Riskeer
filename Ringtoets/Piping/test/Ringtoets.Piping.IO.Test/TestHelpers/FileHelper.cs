using System.IO;

namespace Ringtoets.Piping.IO.Test.TestHelpers
{
    /// <summary>
    /// This static class contains helper methods which can be used in tests.
    /// </summary>
    public static class FileHelper {

        /// <summary>
        /// Checks whether the file pointed at by <paramref name="pathToFile"/> can be opened
        /// for writing.
        /// </summary>
        /// <param name="pathToFile">The location of the file to open for writing.</param>
        /// <returns><c>true</c> if the file could be opened with write permissions. <c>false</c> otherwise.</returns>
        public static bool CanOpenFileForWrite(string pathToFile)
        {
            FileStream file = null;
            try
            {
                file = File.OpenWrite(pathToFile);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }
    }
}