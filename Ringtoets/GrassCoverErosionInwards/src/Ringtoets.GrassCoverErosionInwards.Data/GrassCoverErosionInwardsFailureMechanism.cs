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
using Core.Common.Base;
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
    public class GrassCoverErosionInwardsFailureMechanism : FailureMechanismBase,
                                                            ICalculatableFailureMechanism,
                                                            IHasSectionResults<GrassCoverErosionInwardsFailureMechanismSectionResult>
    {
        private readonly ObservableList<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionInwardsFailureMechanism"/> class.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism()
            : base(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayName, Resources.GrassCoverErosionInwardsFailureMechanism_DisplayCode, 1)
        {
            CalculationsGroup = new CalculationGroup
            {
                Name = RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName
            };
            GeneralInput = new GeneralGrassCoverErosionInwardsInput();
            sectionResults = new ObservableList<GrassCoverErosionInwardsFailureMechanismSectionResult>();
            DikeProfiles = new DikeProfileCollection();
        }

        /// <summary>
        /// Gets the general grass cover erosion inwards calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralGrassCoverErosionInwardsInput GeneralInput { get; }

        /// <summary>
        /// Gets the available dike profiles for this instance.
        /// </summary>
        public DikeProfileCollection DikeProfiles { get; }

        public CalculationGroup CalculationsGroup { get; }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return CalculationsGroup.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>();
            }
        }

        public IObservableEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionResult(FailureMechanismSection section)
        {
            base.AddSectionResult(section);

            sectionResults.Add(new GrassCoverErosionInwardsFailureMechanismSectionResult(section));
        }

        protected override void ClearSectionResults()
        {
            sectionResults.Clear();
        }
    }
}