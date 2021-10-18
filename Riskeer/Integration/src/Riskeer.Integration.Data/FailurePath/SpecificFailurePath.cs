// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Integration.Data.FailurePath
{
    /// <summary>
    /// This class is the implementation for a specific failure path.
    /// </summary>
    public class SpecificFailurePath : Observable, IFailurePath
    {
        private readonly FailureMechanismSectionCollection sectionCollection;

        /// <summary>
        /// Creates a new instance of the <see cref="SpecificFailurePath"/> class.
        /// </summary>
        public SpecificFailurePath()
        {
            Name = Resources.SpecificFailureMechanism_Name_DefaultName;

            sectionCollection = new FailureMechanismSectionCollection();
            IsRelevant = true;
            InputComments = new Comment();
            OutputComments = new Comment();
            NotRelevantComments = new Comment();
        }

        public string Name { get; set; }

        public IEnumerable<FailureMechanismSection> Sections
        {
            get
            {
                return sectionCollection;
            }
        }

        public string FailureMechanismSectionSourcePath
        {
            get
            {
                return sectionCollection.SourcePath;
            }
        }

        public Comment InputComments { get; }

        public Comment OutputComments { get; }

        public Comment NotRelevantComments { get; }

        public bool IsRelevant { get; set; }

        public void SetSections(IEnumerable<FailureMechanismSection> sections, string sourcePath)
        {
            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            if (sourcePath == null)
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            sectionCollection.SetSections(sections, sourcePath);
        }

        public void ClearAllSections()
        {
            sectionCollection.Clear();
        }
    }
}