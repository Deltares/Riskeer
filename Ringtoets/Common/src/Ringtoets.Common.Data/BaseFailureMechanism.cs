// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// This class is the base implementation for a failure mechanism. Classes which want
    /// to implement IFailureMechanism can and should most likely inherit from this class.
    /// </summary>
    public abstract class BaseFailureMechanism : Observable, IFailureMechanism
    {
        private readonly List<FailureMechanismSection> sections;
        private readonly List<FailureMechanismSectionResult> sectionResults;
        private double contribution;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFailureMechanism"/> class.
        /// </summary>
        /// <param name="failureMechanismName">The name of the failure mechanism.</param>
        protected BaseFailureMechanism(string failureMechanismName)
        {
            Name = failureMechanismName;
            sections = new List<FailureMechanismSection>();
            sectionResults = new List<FailureMechanismSectionResult>();
        }

        public IEnumerable<FailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        /// <summary>
        /// Gets the amount of contribution as a percentage (0-100) for the <see cref="IFailureMechanism"/>
        /// as part of the overall verdict.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="value"/> is not in interval (0-100].</exception>
        public virtual double Contribution
        {
            get
            {
                return contribution;
            }
            set
            {
                if (value <= 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.Contribution_Value_should_be_in_interval_0_100);
                }
                contribution = value;
            }
        }

        public string Name { get; private set; }

        public abstract IEnumerable<ICalculationItem> CalculationItems { get; }

        public IEnumerable<FailureMechanismSection> Sections
        {
            get
            {
                return sections;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for the storage of the class.
        /// </summary>
        public long StorageId { get; set; }

        public void AddSection(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }

            if (!sections.Any())
            {
                sections.Add(section);
            }
            else
            {
                InsertSectionWhileMaintainingConnectivityOrder(section);
            }

            sectionResults.Add(new FailureMechanismSectionResult(section));
        }

        public void ClearAllSections()
        {
            sections.Clear();
            sectionResults.Clear();
        }

        /// <summary>
        /// Inserts the section to <see cref="Sections"/> while maintaining connectivity
        /// order (Neighboring <see cref="FailureMechanismSection"/> have same start- and 
        /// endpoints).
        /// </summary>
        /// <param name="sectionToInsert">The new section.</param>
        /// <exception cref="System.ArgumentException">When <paramref name="sectionToInsert"/> cannot
        /// be connected to elements already defined in <see cref="Sections"/>.</exception>
        private void InsertSectionWhileMaintainingConnectivityOrder(FailureMechanismSection sectionToInsert)
        {
            if (sections[0].GetStart().Equals(sectionToInsert.GetLast()))
            {
                sections.Insert(0, sectionToInsert);
            }
            else if (sections[sections.Count - 1].GetLast().Equals(sectionToInsert.GetStart()))
            {
                sections.Add(sectionToInsert);
            }
            else
            {
                string message = string.Format(Resources.BaseFailureMechanism_AddSection_Section_0_must_connect_to_existing_sections,
                                               sectionToInsert.Name);
                throw new ArgumentException(message, "sectionToInsert");
            }
        }
    }
}