using System.IO;

namespace Ringtoets.Piping.IO.Test.TestHelpers
{
    public static class FileHelper {

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