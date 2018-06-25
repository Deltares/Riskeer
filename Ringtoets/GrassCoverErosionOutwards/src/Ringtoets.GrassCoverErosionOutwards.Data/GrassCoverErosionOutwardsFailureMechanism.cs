﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        private readonly ObservableList<GrassCoverErosionOutwardsFailureMechanismSectionResult> sectionResults;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForMechanismSpecificSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForMechanismSpecificLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForMechanismSpecificSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForMechanismSpecificLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionOutwardsFailureMechanism"/> class.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism()
            : base(Resources.GrassCoverErosionOutwardsFailureMechanism_DisplayName, Resources.GrassCoverErosionOutwardsFailureMechanism_Code, 3)
        {
            sectionResults = new ObservableList<GrassCoverErosionOutwardsFailureMechanismSectionResult>();
            GeneralInput = new GeneralGrassCoverErosionOutwardsInput();
            WaveConditionsCalculationGroup = new CalculationGroup
            {
                Name = RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName
            };
            ForeshoreProfiles = new ForeshoreProfileCollection();
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
        public GeneralGrassCoverErosionOutwardsInput GeneralInput { get; }

        /// <summary>
        /// Gets the container of all wave conditions calculations.
        /// </summary>
        public CalculationGroup WaveConditionsCalculationGroup { get; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ForeshoreProfileCollection ForeshoreProfiles { get; }

        /// <summary>
        /// Gets the water level calculations corresponding to the mechanism specific factorized signaling norm.
        /// </summary>
        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm
        {
            get
            {
                return waterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm;
            }
        }

        /// <summary>
        /// Gets the water level calculations corresponding to the mechanism specific signaling norm.
        /// </summary>
        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForMechanismSpecificSignalingNorm
        {
            get
            {
                return waterLevelCalculationsForMechanismSpecificSignalingNorm;
            }
        }

        /// <summary>
        /// Gets the water level calculations corresponding to the mechanism specific lower limit norm.
        /// </summary>
        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForMechanismSpecificLowerLimitNorm
        {
            get
            {
                return waterLevelCalculationsForMechanismSpecificLowerLimitNorm;
            }
        }

        /// <summary>
        /// Gets the wave height calculations corresponding to the mechanism specific factorized signaling norm.
        /// </summary>
        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm
        {
            get
            {
                return waveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm;
            }
        }

        /// <summary>
        /// Gets the wave height calculations corresponding to the mechanism specific signaling norm.
        /// </summary>
        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForMechanismSpecificSignalingNorm
        {
            get
            {
                return waveHeightCalculationsForMechanismSpecificSignalingNorm;
            }
        }

        /// <summary>
        /// Gets the wave height calculations corresponding to the mechanism specific lower limit norm.
        /// </summary>
        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForMechanismSpecificLowerLimitNorm
        {
            get
            {
                return waveHeightCalculationsForMechanismSpecificLowerLimitNorm;
            }
        }

        public IObservableEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        /// <summary>
        /// Sets hydraulic boundary location calculations for <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to add calculations for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/> is <c>null</c>.</exception>
        public void SetHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            ClearHydraulicBoundaryLocationCalculations();

            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in hydraulicBoundaryLocations)
            {
                AddHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocation);
            }
        }

        protected override void AddSectionResult(FailureMechanismSection section)
        {
            base.AddSectionResult(section);
            sectionResults.Add(new GrassCoverErosionOutwardsFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }

        private void ClearHydraulicBoundaryLocationCalculations()
        {
            waterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Clear();
            waterLevelCalculationsForMechanismSpecificSignalingNorm.Clear();
            waterLevelCalculationsForMechanismSpecificLowerLimitNorm.Clear();
            waveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Clear();
            waveHeightCalculationsForMechanismSpecificSignalingNorm.Clear();
            waveHeightCalculationsForMechanismSpecificLowerLimitNorm.Clear();
        }

        private void AddHydraulicBoundaryLocationCalculations(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            waterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waterLevelCalculationsForMechanismSpecificSignalingNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waterLevelCalculationsForMechanismSpecificLowerLimitNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightCalculationsForMechanismSpecificSignalingNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightCalculationsForMechanismSpecificLowerLimitNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
        }
    }
}