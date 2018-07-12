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
using Core.Common.Base.Service;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Service.Properties;

namespace Ringtoets.Integration.Service.Merge
{
    /// <summary>
    /// Activity to load a collection of <see cref="AssessmentSection"/> from a file.
    /// </summary>
    internal class LoadAssessmentSectionsActivity : Activity
    {
        private readonly AssessmentSectionsOwner assessmentSectionsOwner;
        private readonly IAssessmentSectionProvider assessmentSectionProvider;
        private readonly string filePath;

        private bool canceled;

        /// <summary>
        /// Creates a new instance of <see cref="LoadAssessmentSectionsActivity"/>.
        /// </summary>
        /// <param name="assessmentSectionsOwner">The owner to set the retrieved collection
        /// of <see cref="AssessmentSection"/> on.</param>
        /// <param name="assessmentSectionProvider">The provider defining how to
        /// retrieve the collection of <see cref="AssessmentSection"/> from a file.</param>
        /// <param name="filePath">The file path to retrieve the collection of
        /// <see cref="AssessmentSection"/> from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the arguments is <c>null</c>.</exception>
        public LoadAssessmentSectionsActivity(AssessmentSectionsOwner assessmentSectionsOwner,
                                              IAssessmentSectionProvider assessmentSectionProvider,
                                              string filePath)
        {
            if (assessmentSectionsOwner == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionsOwner));
            }

            if (assessmentSectionProvider == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionProvider));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            this.assessmentSectionsOwner = assessmentSectionsOwner;
            this.assessmentSectionProvider = assessmentSectionProvider;
            this.filePath = filePath;

            Description = Resources.LoadAssessmentSectionsActivity_Description;
        }

        protected override void OnRun()
        {
            assessmentSectionsOwner.AssessmentSections = assessmentSectionProvider.GetAssessmentSections(filePath);
        }

        protected override void OnCancel()
        {
            canceled = true;
        }

        protected override void OnFinish()
        {
            if (canceled)
            {
                assessmentSectionsOwner.AssessmentSections = null;
            }
        }
    }
}