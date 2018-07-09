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
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Plugin.Properties;
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
        private readonly AssessmentSectionsOwner assessmentSectionsOwner;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMerger"/>,
        /// </summary>
        /// <param name="inquiryHandler">Object responsible for inquiring the required data.</param>
        /// <param name="getAssessmentSectionsAction">The action for getting the assessment sections
        /// to merge.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inquiryHandler"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionMerger(IInquiryHelper inquiryHandler, Action<string, AssessmentSectionsOwner> getAssessmentSectionsAction)
        {
            if (inquiryHandler == null)
            {
                throw new ArgumentNullException(nameof(inquiryHandler));
            }

            if (getAssessmentSectionsAction == null)
            {
                throw new ArgumentNullException(nameof(getAssessmentSectionsAction));
            }

            this.inquiryHandler = inquiryHandler;
            this.getAssessmentSectionsAction = getAssessmentSectionsAction;

            assessmentSectionsOwner = new AssessmentSectionsOwner();
        }

        public void StartMerge()
        {
            SelectProject();
        }

        private void SelectProject()
        {
            string filePath = inquiryHandler.GetSourceFileLocation(RingtoetsStorageResources.Ringtoets_project_file_filter);
            if (filePath == null)
            {
                CancelMergeAndLog();
                return;
            }

            if (!GetAssessmentSections(filePath))
            {
                return;
            }
        }

        private bool GetAssessmentSections(string filePath)
        {
            getAssessmentSectionsAction(filePath, assessmentSectionsOwner);
            IEnumerable<AssessmentSection> assessmentSections = assessmentSectionsOwner.AssessmentSections;

            if (assessmentSections == null)
            {
                return false;
            }

            if (!assessmentSections.Any())
            {
                LogError(Resources.AssessmentSectionMerger_No_matching_AssessmentSections);
                return false;
            }

            return true;
        }

        private static void CancelMergeAndLog()
        {
            log.Info(CoreCommonGuiResources.GuiImportHandler_ImportItemsUsingDialog_Importing_cancelled);
        }

        private static void LogError(string message)
        {
            log.Error(message);
        }
    }
}