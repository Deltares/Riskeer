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
using Ringtoets.Common.Data;
using Ringtoets.Common.Placeholder;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Model for performing piping calculations.
    /// </summary>
    public class PipingFailureMechanism : BaseFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingFailureMechanism"/> class.
        /// </summary>
        public PipingFailureMechanism()
            : base(PipingDataResources.PipingFailureMechanism_DisplayName)
        {
            SemiProbabilisticInput = new SemiProbabilisticPipingInput();
            GeneralInput = new GeneralPipingInput();
            SurfaceLines = new ObservableList<RingtoetsPipingSurfaceLine>();
            StochasticSoilModels = new ObservableList<StochasticSoilModel>();
            var pipingCalculationGroup = new PipingCalculationGroup(PipingDataResources.PipingFailureMechanism_Calculations_DisplayName, false);
            pipingCalculationGroup.Children.Add(new PipingCalculation(GeneralInput, SemiProbabilisticInput));
            CalculationsGroup = pipingCalculationGroup;
            AssessmentResult = new PipingFailureMechanismResult();
        }

        public override IEnumerable<ICalculationItem> CalculationItems
        {
            get
            {
                return CalculationsGroup.GetPipingCalculations();
            }
        }

        public override double Contribution
        {
            get
            {
                return SemiProbabilisticInput.Contribution;
            }
            set
            {
                SemiProbabilisticInput.Contribution = value;
            }
        }

        /// <summary>
        /// Gets the available <see cref="RingtoetsPipingSurfaceLine"/> within the scope of the piping failure mechanism.
        /// </summary>
        public ICollection<RingtoetsPipingSurfaceLine> SurfaceLines { get; private set; }

        /// <summary>
        /// Gets the available profiles within the scope of the piping failure mechanism.
        /// </summary>
        public PipingSoilProfile[] SoilProfiles
        {
            get
            {
                return StochasticSoilModels.SelectMany(ssm => ssm.StochasticSoilProfiles.Select(ssp => ssp.SoilProfile)).Distinct().ToArray();
            }
        }

        /// <summary>
        /// Gets the available stochastic soil models within the scope of the piping failure mechanism.
        /// </summary>
        public ObservableList<StochasticSoilModel> StochasticSoilModels { get; private set; }

        /// <summary>
        /// Gets all available piping calculations.
        /// </summary>
        public PipingCalculationGroup CalculationsGroup { get; private set; }

        /// <summary>
        /// Gets the calculation results for this failure mechanism.
        /// </summary>
        public PipingFailureMechanismResult AssessmentResult { get; private set; }

        /// <summary>
        /// Gets the general piping calculation input parameters that apply to each piping calculation.
        /// </summary>
        public GeneralPipingInput GeneralInput { get; private set; }

        /// <summary>
        /// Gets the general semi-probabilistic calculation input parameters that apply to each calculation 
        /// in a semi-probabilistic assessment.
        /// </summary>
        public SemiProbabilisticPipingInput SemiProbabilisticInput { get; set; }
    }
}