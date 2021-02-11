// Copyright (C) Stichting Deltares 2021. All rights reserved.
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