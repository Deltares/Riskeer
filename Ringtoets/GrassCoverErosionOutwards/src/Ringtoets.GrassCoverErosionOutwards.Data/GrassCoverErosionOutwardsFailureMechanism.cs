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
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.GrassCoverErosionOutwards.Data.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Grass Cover Erosion Outwards failure mechanism.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanism : FailureMechanismBase,
                                                             IHasSectionResults<GrassCoverErosionOutwardsFailureMechanismSectionResult>
    {
        private readonly IList<GrassCoverErosionOutwardsFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionOutwardsFailureMechanism"/> class.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism()
            : base(Resources.GrassCoverErosionOutwardsFailureMechanism_DisplayName, Resources.GrassCoverErosionOutwardsFailureMechanism_Code)
        {
            sectionResults = new List<GrassCoverErosionOutwardsFailureMechanismSectionResult>();
            GeneralInput = new GeneralGrassCoverErosionOutwardsInput();
            WaveConditionsCalculationGroup = new CalculationGroup(RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName, false);
            HydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>();
            ForeshoreProfiles = new ObservableList<ForeshoreProfile>();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return WaveConditionsCalculationGroup.GetCalculations().OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>();
            }
        }

        /// <summary>
        /// Gets the general grass cover erosion outwards calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralGrassCoverErosionOutwardsInput GeneralInput { get; private set; }

        /// <summary>
        /// Gets the hydraulic boundary locations.
        /// </summary>
        public ObservableList<HydraulicBoundaryLocation> HydraulicBoundaryLocations { get; private set; }

        /// <summary>
        /// Gets the container of all wave conditions calculations.
        /// </summary>
        public CalculationGroup WaveConditionsCalculationGroup { get; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ObservableList<ForeshoreProfile> ForeshoreProfiles { get; private set; }

        public IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);
            sectionResults.Add(new GrassCoverErosionOutwardsFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }
    }
}