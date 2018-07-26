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
using System.Windows.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Service.Merge;
using Ringtoets.Storage.Core;

namespace Ringtoets.Integration.Plugin.Merge
{
    /// <summary>
    /// Class for providing <see cref="AssessmentSection"/> instances.
    /// </summary>
    public class AssessmentSectionProvider : IAssessmentSectionProvider
    {
        private readonly IWin32Window viewParent;

        /// <summary>
        /// Initializes a new instance of <see cref="AssessmentSectionProvider"/>.
        /// </summary>
        /// <param name="viewParent">The parent of the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewParent"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionProvider(IWin32Window viewParent)
        {
            if (viewParent == null)
            {
                throw new ArgumentNullException(nameof(viewParent));
            }

            this.viewParent = viewParent;
        }

        public IEnumerable<AssessmentSection> GetAssessmentSections(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var assessmentSectionsOwner = new AssessmentSectionsOwner();

            ActivityProgressDialogRunner.Run(viewParent,
                                             LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(
                                                 assessmentSectionsOwner, new LoadAssessmentSectionService(new StorageSqLite()),
                                                 filePath));

            if (assessmentSectionsOwner.AssessmentSections == null)
            {
                throw new AssessmentSectionProviderException();
            }

            return assessmentSectionsOwner.AssessmentSections;
        }
    }
}