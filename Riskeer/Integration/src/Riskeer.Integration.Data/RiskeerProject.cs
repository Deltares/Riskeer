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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Properties;

namespace Riskeer.Integration.Data
{
    /// <summary>
    /// Class which defines a project for the application.
    /// </summary>
    public class RiskeerProject : Observable, IProject
    {
        /// <summary>
        /// Constructs a new <see cref="RiskeerProject"/>. 
        /// </summary>
        public RiskeerProject() : this(Resources.Project_Constructor_Default_name) {}

        /// <summary>
        /// Constructs a new <see cref="RiskeerProject"/>. 
        /// </summary>
        /// <param name="name">The name of the <see cref="RiskeerProject"/>.</param>
        public RiskeerProject(string name)
        {
            Name = name;
            Description = "";

            AssessmentSections = new List<AssessmentSection>();
        }

        /// <summary>
        /// Gets or sets the assessment sections of the <see cref="RiskeerProject"/>.
        /// </summary>
        public List<AssessmentSection> AssessmentSections { get; }

        /// <summary>
        /// Gets or sets the name of the <see cref="RiskeerProject"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the <see cref="RiskeerProject"/>.
        /// </summary>
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((RiskeerProject) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        private bool Equals(IProject other)
        {
            var otherProject = other as RiskeerProject;
            if (otherProject == null)
            {
                return false;
            }

            return string.Equals(Name, otherProject.Name) &&
                   string.Equals(Description, otherProject.Description) &&
                   AssessmentSections.SequenceEqual(otherProject.AssessmentSections);
        }
    }
}