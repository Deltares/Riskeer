// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Gui;
using RiskeerStorageCoreResources = Riskeer.Storage.Core.Properties.Resources;

namespace Riskeer.Integration.Forms.Merge
{
    /// <summary>
    /// Class for providing a file path of a project that might contain assessment sections that can be merged.
    /// </summary>
    public class AssessmentSectionMergeFilePathProvider : IAssessmentSectionMergeFilePathProvider
    {
        private readonly IInquiryHelper inquiryHelper;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMergeFilePathProvider"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring the required data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inquiryHelper"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionMergeFilePathProvider(IInquiryHelper inquiryHelper)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            this.inquiryHelper = inquiryHelper;
        }

        public string GetFilePath()
        {
            return inquiryHelper.GetSourceFileLocation(RiskeerStorageCoreResources.Riskeer_project_file_filter);
        }
    }
}