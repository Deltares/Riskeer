using System;
using Core.Common.Util;

namespace Core.Common.Gui.Helpers
{
    /// <summary>
    /// Class with helper methods that can be used during export.
    /// </summary>
    public static class ExportHelper
    {
        /// <summary>
        /// Gets the file path to export to.
        /// </summary>
        /// <param name="inquiryHelper">Helper responsible for performing information inquiries.</param>
        /// <param name="fileFilterGenerator">The file filter generator to use.</param>
        /// <returns>A path to a file, which may or may not exist yet, or <c>null</c> if no location
        /// was chosen.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static string GetFilePath(IInquiryHelper inquiryHelper, FileFilterGenerator fileFilterGenerator)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            if (fileFilterGenerator == null)
            {
                throw new ArgumentNullException(nameof(fileFilterGenerator));
            }

            return inquiryHelper.GetTargetFileLocation(fileFilterGenerator.Filter, null);
        }

        /// <summary>
        /// Gets the folder path to export to.
        /// </summary>
        /// <param name="inquiryHelper">Helper responsible for performing information inquiries.</param>
        /// <returns>A path to a folder, or <c>null</c> if no location was chosen.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inquiryHelper"/>
        /// is <c>null</c>.</exception>
        public static string GetFolderPath(IInquiryHelper inquiryHelper)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            return inquiryHelper.GetTargetFolderLocation();
        }
    }
}