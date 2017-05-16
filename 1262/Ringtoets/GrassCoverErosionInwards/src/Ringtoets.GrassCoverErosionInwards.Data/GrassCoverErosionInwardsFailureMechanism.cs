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

using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Model for performing grass cover erosion inwards calculations.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanism : FailureMechanismBase, ICalculatableFailureMechanism, IHasSectionResults<GrassCoverErosionInwardsFailureMechanismSectionResult>
    {
        private readonly List<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionInwardsFailureMechanism"/> class.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism()
            : base(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayName, Resources.GrassCoverErosionInwardsFailureMechanism_DisplayCode)
        {
            CalculationsGroup = new CalculationGroup(RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName, false);
            GeneralInput = new GeneralGrassCoverErosionInwardsInput();
            sectionResults = new List<GrassCoverErosionInwardsFailureMechanismSectionResult>();
            DikeProfiles = new DikeProfileCollection();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return CalculationsGroup.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>();
            }
        }

        /// <summary>
        /// Gets the general grass cover erosion inwards calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralGrassCoverErosionInwardsInput GeneralInput { get; private set; }

        /// <summary>
        /// Gets the available dike profiles for this instance.
        /// </summary>
        public DikeProfileCollection DikeProfiles { get; private set; }

        public CalculationGroup CalculationsGroup { get; }

        public IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);

            sectionResults.Add(new GrassCoverErosionInwardsFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }
    }
}