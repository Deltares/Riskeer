// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Piping.Data.SoilProfile;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using PipingDataResources = Riskeer.Piping.Data.Properties.Resources;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Model for performing piping calculations.
    /// </summary>
    public class PipingFailureMechanism : FailureMechanismBase<AdoptableFailureMechanismSectionResult>, ICalculatableFailureMechanism
    {
        private readonly ObservableList<PipingFailureMechanismSectionConfiguration> sectionConfigurations;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanism"/>.
        /// </summary>
        public PipingFailureMechanism()
            : base(PipingDataResources.PipingFailureMechanism_DisplayName, PipingDataResources.PipingFailureMechanism_DisplayCode)
        {
            GeneralInput = new GeneralPipingInput();
            SurfaceLines = new PipingSurfaceLineCollection();
            StochasticSoilModels = new PipingStochasticSoilModelCollection();
            CalculationsGroup = new CalculationGroup
            {
                Name = RiskeerCommonDataResources.FailureMechanism_Calculations_DisplayName
            };

            ScenarioConfigurationType = PipingFailureMechanismScenarioConfigurationType.SemiProbabilistic;
            sectionConfigurations = new ObservableList<PipingFailureMechanismSectionConfiguration>();
            CalculationsInputComments = new Comment();
        }

        /// <summary>
        /// Gets the available surface lines within the scope of the piping failure mechanism.
        /// </summary>
        public PipingSurfaceLineCollection SurfaceLines { get; }

        /// <summary>
        /// Gets the available stochastic soil models within the scope of the piping failure mechanism.
        /// </summary>
        public PipingStochasticSoilModelCollection StochasticSoilModels { get; }

        /// <summary>
        /// Gets the general calculation input parameters that apply to each piping calculation.
        /// </summary>
        public GeneralPipingInput GeneralInput { get; }

        /// <summary>
        /// Gets or sets the <see cref="PipingFailureMechanismScenarioConfigurationType"/>.
        /// </summary>
        public PipingFailureMechanismScenarioConfigurationType ScenarioConfigurationType { get; set; }

        /// <summary>
        /// Gets an <see cref="IObservableEnumerable{T}"/> of <see cref="PipingFailureMechanismSectionConfiguration"/>.
        /// </summary>
        public IObservableEnumerable<PipingFailureMechanismSectionConfiguration> SectionConfigurations => sectionConfigurations;

        public IEnumerable<ICalculation> Calculations => CalculationsGroup.GetCalculations();

        public CalculationGroup CalculationsGroup { get; }

        public Comment CalculationsInputComments { get; }

        protected override void AddSectionDependentData(FailureMechanismSection section)
        {
            base.AddSectionDependentData(section);
            sectionConfigurations.Add(new PipingFailureMechanismSectionConfiguration(section));
        }

        protected override void ClearSectionDependentData()
        {
            base.ClearSectionDependentData();
            sectionConfigurations.Clear();
        }
    }
}