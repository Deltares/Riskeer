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
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.DuneErosion.Data.Properties;

namespace Ringtoets.DuneErosion.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Dune Erosion failure mechanism.
    /// </summary>
    public class DuneErosionFailureMechanism : FailureMechanismBase, IHasSectionResults<DuneErosionFailureMechanismSectionResult>
    {
        private readonly ObservableList<DuneErosionFailureMechanismSectionResult> sectionResults;
        private readonly ObservableList<DuneLocationCalculation> calculationsForMechanismSpecificFactorizedSignalingNorm = new ObservableList<DuneLocationCalculation>();
        private readonly ObservableList<DuneLocationCalculation> calculationsForMechanismSpecificSignalingNorm = new ObservableList<DuneLocationCalculation>();
        private readonly ObservableList<DuneLocationCalculation> calculationsForMechanismSpecificLowerLimitNorm = new ObservableList<DuneLocationCalculation>();
        private readonly ObservableList<DuneLocationCalculation> calculationsForLowerLimitNorm = new ObservableList<DuneLocationCalculation>();
        private readonly ObservableList<DuneLocationCalculation> calculationsForFactorizedLowerLimitNorm = new ObservableList<DuneLocationCalculation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DuneErosionFailureMechanism"/> class.
        /// </summary>
        public DuneErosionFailureMechanism()
            : base(Resources.DuneErosionFailureMechanism_DisplayName, Resources.DuneErosionFailureMechanism_Code, 3)
        {
            sectionResults = new ObservableList<DuneErosionFailureMechanismSectionResult>();
            GeneralInput = new GeneralDuneErosionInput();
            DuneLocations = new ObservableList<DuneLocation>();

            CalculationsForMechanismSpecificFactorizedSignalingNorm = calculationsForMechanismSpecificFactorizedSignalingNorm;
            CalculationsForMechanismSpecificSignalingNorm = calculationsForMechanismSpecificSignalingNorm;
            CalculationsForMechanismSpecificLowerLimitNorm = calculationsForMechanismSpecificLowerLimitNorm;
            CalculationsForLowerLimitNorm = calculationsForLowerLimitNorm;
            CalculationsForFactorizedLowerLimitNorm = calculationsForFactorizedLowerLimitNorm;
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Gets the general dune erosion calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralDuneErosionInput GeneralInput { get; }

        /// <summary>
        /// Gets the dune locations.
        /// </summary>
        public ObservableList<DuneLocation> DuneLocations { get; }

        /// <summary>
        /// Gets the calculations corresponding to the mechanism specific factorized signaling norm.
        /// </summary>
        public IObservableEnumerable<DuneLocationCalculation> CalculationsForMechanismSpecificFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the calculations corresponding to the mechanism specific signaling norm.
        /// </summary>
        public IObservableEnumerable<DuneLocationCalculation> CalculationsForMechanismSpecificSignalingNorm { get; }

        /// <summary>
        /// Gets the calculations corresponding to the mechanism specific lower limit norm.
        /// </summary>
        public IObservableEnumerable<DuneLocationCalculation> CalculationsForMechanismSpecificLowerLimitNorm { get; }

        /// <summary>
        /// Gets the calculations corresponding to the lower limit norm.
        /// </summary>
        public IObservableEnumerable<DuneLocationCalculation> CalculationsForLowerLimitNorm { get; }

        /// <summary>
        /// Gets the calculations corresponding to the factorized lower limit norm.
        /// </summary>
        public IObservableEnumerable<DuneLocationCalculation> CalculationsForFactorizedLowerLimitNorm { get; set; }

        public IObservableEnumerable<DuneErosionFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        /// <summary>
        /// Sets dune location calculations for <paramref name="duneLocations"/>.
        /// </summary>
        /// <param name="duneLocations">The dune locations to add calculations for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocations"/> is <c>null</c>.</exception>
        public void SetDuneLocationCalculations(IEnumerable<DuneLocation> duneLocations)
        {
            if (duneLocations == null)
            {
                throw new ArgumentNullException(nameof(duneLocations));
            }

            ClearAllCalculations();
            foreach (DuneLocation duneLocation in duneLocations)
            {
                AddCalculationsForDuneLocation(duneLocation);
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);

            sectionResults.Add(new DuneErosionFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }

        private void ClearAllCalculations()
        {
            calculationsForMechanismSpecificFactorizedSignalingNorm.Clear();
            calculationsForMechanismSpecificSignalingNorm.Clear();
            calculationsForMechanismSpecificLowerLimitNorm.Clear();
            calculationsForLowerLimitNorm.Clear();
            calculationsForFactorizedLowerLimitNorm.Clear();
        }

        private void AddCalculationsForDuneLocation(DuneLocation duneLocation)
        {
            calculationsForMechanismSpecificFactorizedSignalingNorm.Add(new DuneLocationCalculation(duneLocation));
            calculationsForMechanismSpecificSignalingNorm.Add(new DuneLocationCalculation(duneLocation));
            calculationsForMechanismSpecificLowerLimitNorm.Add(new DuneLocationCalculation(duneLocation));
            calculationsForLowerLimitNorm.Add(new DuneLocationCalculation(duneLocation));
            calculationsForFactorizedLowerLimitNorm.Add(new DuneLocationCalculation(duneLocation));
        }
    }
}