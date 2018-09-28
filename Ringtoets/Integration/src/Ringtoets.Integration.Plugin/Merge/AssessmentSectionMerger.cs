// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.Linq;
using log4net;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Forms.Merge;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.Integration.Service.Comparers;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Merge
{
    /// <summary>
    /// Class responsible for merging <see cref="AssessmentSection"/>.
    /// </summary>
    public class AssessmentSectionMerger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionMerger));

        private readonly IAssessmentSectionMergeFilePathProvider filePathProvider;
        private readonly IAssessmentSectionProvider assessmentSectionProvider;
        private readonly IAssessmentSectionMergeComparer mergeComparer;
        private readonly IAssessmentSectionMergeDataProvider mergeDataProvider;
        private readonly IAssessmentSectionMergeHandler mergeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMerger"/>.
        /// </summary>
        /// <param name="filePathProvider">The provider to get the file path of the file to merge.</param>
        /// <param name="assessmentSectionProvider">The provider to get the assessment sections to merge.</param>
        /// <param name="mergeComparer">The comparer to compare the assessment sections with.</param>
        /// <param name="mergeDataProvider">The provider to get the data to merge from.</param>
        /// <param name="mergeHandler">The handler to perform the merge.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssessmentSectionMerger(IAssessmentSectionMergeFilePathProvider filePathProvider, IAssessmentSectionProvider assessmentSectionProvider,
                                       IAssessmentSectionMergeComparer mergeComparer, IAssessmentSectionMergeDataProvider mergeDataProvider, IAssessmentSectionMergeHandler mergeHandler)
        {
            if (filePathProvider == null)
            {
                throw new ArgumentNullException(nameof(filePathProvider));
            }

            if (assessmentSectionProvider == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionProvider));
            }

            if (mergeComparer == null)
            {
                throw new ArgumentNullException(nameof(mergeComparer));
            }

            if (mergeDataProvider == null)
            {
                throw new ArgumentNullException(nameof(mergeDataProvider));
            }

            if (mergeHandler == null)
            {
                throw new ArgumentNullException(nameof(mergeHandler));
            }

            this.filePathProvider = filePathProvider;
            this.assessmentSectionProvider = assessmentSectionProvider;
            this.mergeComparer = mergeComparer;
            this.mergeDataProvider = mergeDataProvider;
            this.mergeHandler = mergeHandler;
        }

        /// <summary>
        /// Performs the merge of <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to perform the merge on.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public void StartMerge(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            string filePath = filePathProvider.GetFilePath();

            if (filePath == null)
            {
                LogCancelMessage();
                return;
            }

            IEnumerable<AssessmentSection> assessmentSections;

            try
            {
                assessmentSections = assessmentSectionProvider.GetAssessmentSections(filePath);
            }
            catch (AssessmentSectionProviderException)
            {
                return;
            }

            if (!assessmentSections.Any())
            {
                log.Error(Resources.AssessmentSectionMerger_No_matching_AssessmentSections);
                return;
            }

            IEnumerable<AssessmentSection> matchingAssessmentSections = assessmentSections.Where(section => mergeComparer.Compare(assessmentSection, section));

            if (!matchingAssessmentSections.Any())
            {
                log.Error(Resources.AssessmentSectionMerger_No_matching_AssessmentSections);
                return;
            }

            AssessmentSectionMergeData mergeData = mergeDataProvider.GetMergeData(matchingAssessmentSections);

            if (mergeData == null)
            {
                LogCancelMessage();
                return;
            }

            PerformMerge(assessmentSection, mergeData);
        }

        private void PerformMerge(AssessmentSection assessmentSection, AssessmentSectionMergeData mergeData)
        {
            log.Info(Resources.AssessmentSectionMerger_PerformMerge_Merging_AssessmentSections_started);

            try
            {
                mergeHandler.PerformMerge(assessmentSection, mergeData);
                log.Info(Resources.AssessmentSectionMerger_PerformMerge_Merging_AssessmentSections_successful);
            }
            catch (Exception e)
            {
                log.Error(Resources.AssessmentSectionMerger_PerformMerge_Unexpected_error_occurred_during_merge, e);
                log.Error(Resources.AssessmentSectionMerger_PerformMerge_Merging_AssessmentSections_failed);
            }
        }

        private static void LogCancelMessage()
        {
            log.Warn(CoreCommonGuiResources.GuiImportHandler_ImportItemsUsingDialog_Importing_cancelled);
        }
    }
}