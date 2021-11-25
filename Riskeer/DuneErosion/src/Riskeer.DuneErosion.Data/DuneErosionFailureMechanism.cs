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
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.DuneErosion.Data.Properties;

namespace Riskeer.DuneErosion.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Dune Erosion failure mechanism.
    /// </summary>
    public class DuneErosionFailureMechanism : FailureMechanismBase, IHasSectionResults<DuneErosionFailureMechanismSectionResultOld>
    {
        private readonly ObservableList<DuneErosionFailureMechanismSectionResultOld> sectionResults;
        private readonly ObservableList<DuneLocation> duneLocationCollection = new ObservableList<DuneLocation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DuneErosionFailureMechanism"/> class.
        /// </summary>
        public DuneErosionFailureMechanism()
            : base(Resources.DuneErosionFailureMechanism_DisplayName, Resources.DuneErosionFailureMechanism_Code, 3)
        {
            sectionResults = new ObservableList<DuneErosionFailureMechanismSectionResultOld>();
            GeneralInput = new GeneralDuneErosionInput();
            DuneLocationCalculationsForUserDefinedTargetProbabilities = new ObservableList<DuneLocationCalculationsForTargetProbability>();
        }

        /// <summary>
        /// Gets the general dune erosion calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralDuneErosionInput GeneralInput { get; }

        /// <summary>
        /// Gets the dune locations.
        /// </summary>
        public IObservableEnumerable<DuneLocation> DuneLocations
        {
            get
            {
                return duneLocationCollection;
            }
        }

        /// <summary>
        /// Gets the dune location calculations corresponding to the user defined target probabilities.
        /// </summary>
        public ObservableList<DuneLocationCalculationsForTargetProbability> DuneLocationCalculationsForUserDefinedTargetProbabilities { get; }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        public IObservableEnumerable<DuneErosionFailureMechanismSectionResultOld> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        /// <summary>
        /// Sets dune locations and calculations for the failure mechanism.
        /// </summary>
        /// <param name="duneLocations">The dune locations to add to the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocations"/> is <c>null</c>.</exception>
        public void SetDuneLocations(IEnumerable<DuneLocation> duneLocations)
        {
            if (duneLocations == null)
            {
                throw new ArgumentNullException(nameof(duneLocations));
            }

            duneLocationCollection.Clear();
            duneLocationCollection.AddRange(duneLocations);

            DuneLocationCalculationsForUserDefinedTargetProbabilities.ForEach(dlc =>
            {
                dlc.DuneLocationCalculations.Clear();
                dlc.DuneLocationCalculations.AddRange(duneLocations.Select(dl => new DuneLocationCalculation(dl)));
            });
        }

        protected override void AddSectionDependentData(FailureMechanismSection section)
        {
            base.AddSectionDependentData(section);

            sectionResults.Add(new DuneErosionFailureMechanismSectionResultOld(section));
        }

        protected override void ClearSectionDependentData()
        {
            sectionResults.Clear();
        }
    }
}