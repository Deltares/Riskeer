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

using System.Collections.Generic;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HeightStructures.Data.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Failure mechanism for Height structures.
    /// </summary>
    public class HeightStructuresFailureMechanism : FailureMechanismBase, IHasSectionResults<CustomFailureMechanismSectionResult>
    {
        private readonly IList<CustomFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Creates a new instance of the <see cref="HeightStructuresFailureMechanism"/> class.
        /// </summary>
        public HeightStructuresFailureMechanism()
            : base(Resources.HeightStructuresFailureMechanism_DisplayName, Resources.HeightStructuresFailureMechanism_Code)
        {
            sectionResults = new List<CustomFailureMechanismSectionResult>();
            CalculationsGroup = new CalculationGroup(RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName, false);
            NormProbabilityInput = new GeneralNormProbabilityInput();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);

            sectionResults.Add(new CustomFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }

        public IEnumerable<CustomFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }
        /// <summary>
        /// Gets the length-effect parameters.
        /// </summary>
        public GeneralNormProbabilityInput NormProbabilityInput { get; private set; }

        public CalculationGroup CalculationsGroup { get; private set; }
    }
}