// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui;
using log4net;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Forms.Merge;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.Integration.Service.Comparers;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsStorageResources = Ringtoets.Storage.Core.Properties.Resources;

namespace Ringtoets.Integration.Plugin
{
    /// <summary>
    /// Class responsible for merging <see cref="AssessmentSection"/>.
    /// </summary>
    public class AssessmentSectionMerger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionMerger));

        private readonly IInquiryHelper inquiryHandler;
        private readonly Action<string, AssessmentSectionsOwner> getAssessmentSectionsAction;
        private readonly IAssessmentSectionMergeComparer comparer;
        private readonly IMergeDataProvider mergeDataProvider;
        private readonly IAssessmentSectionMergeHandler mergeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMerger"/>,
        /// </summary>
        /// <param name="inquiryHandler">Object responsible for inquiring the required data.</param>
        /// <param name="getAssessmentSectionsAction">The action for getting the assessment sections
        /// to merge.</param>
        /// <param name="comparer">The comparer to compare the assessment sections with.</param>
        /// <param name="mergeDataProvider">The provider to get the data to merge from.</param>
        /// <param name="mergeHandler">The handler to perform the merge.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssessmentSectionMerger(IInquiryHelper inquiryHandler, Action<string, AssessmentSectionsOwner> getAssessmentSectionsAction,
                                       IAssessmentSectionMergeComparer comparer, IMergeDataProvider mergeDataProvider, IAssessmentSectionMergeHandler mergeHandler)
        {
            if (inquiryHandler == null)
            {
                throw new ArgumentNullException(nameof(inquiryHandler));
            }

            if (getAssessmentSectionsAction == null)
            {
                throw new ArgumentNullException(nameof(getAssessmentSectionsAction));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (mergeDataProvider == null)
            {
                throw new ArgumentNullException(nameof(mergeDataProvider));
            }

            if (mergeHandler == null)
            {
                throw new ArgumentNullException(nameof(mergeHandler));
            }

            this.inquiryHandler = inquiryHandler;
            this.getAssessmentSectionsAction = getAssessmentSectionsAction;
            this.comparer = comparer;
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

            string filePath = SelectProject();

            if (filePath == null)
            {
                LogCancelMessage();
                return;
            }

            IEnumerable<AssessmentSection> assessmentSections = GetAssessmentSections(filePath);

            if (assessmentSections == null)
            {
                return;
            }

            if (!assessmentSections.Any())
            {
                LogError(Resources.AssessmentSectionMerger_No_matching_AssessmentSections);
                return;
            }

            IEnumerable<AssessmentSection> matchingAssessmentSections = assessmentSections.Where(section => comparer.Compare(assessmentSection, section));

            if (!matchingAssessmentSections.Any())
            {
                LogError(Resources.AssessmentSectionMerger_No_matching_AssessmentSections);
                return;
            }

            if (!mergeDataProvider.SelectData(matchingAssessmentSections))
            {
                LogCancelMessage();
                return;
            }

            AssessmentSection assessmentSectionToMerge = mergeDataProvider.SelectedAssessmentSection;
            IEnumerable<IFailureMechanism> failureMechanismToMerge = mergeDataProvider.SelectedFailureMechanisms;

            if (assessmentSectionToMerge == null || failureMechanismToMerge == null)
            {
                LogError(Resources.AssessmentSectionMerger_No_AssessmentSection_selected);
                return;
            }

            mergeHandler.PerformMerge(assessmentSection, assessmentSectionToMerge, failureMechanismToMerge);
        }

        private string SelectProject()
        {
            return inquiryHandler.GetSourceFileLocation(RingtoetsStorageResources.Ringtoets_project_file_filter);
        }

        private IEnumerable<AssessmentSection> GetAssessmentSections(string filePath)
        {
            var assessmentSectionsOwner = new AssessmentSectionsOwner();
            getAssessmentSectionsAction(filePath, assessmentSectionsOwner);
            return assessmentSectionsOwner.AssessmentSections;
        }

        private static void LogCancelMessage()
        {
            log.Info(CoreCommonGuiResources.GuiImportHandler_ImportItemsUsingDialog_Importing_cancelled);
        }

        private static void LogError(string message)
        {
            log.Error(message);
        }
    }
}