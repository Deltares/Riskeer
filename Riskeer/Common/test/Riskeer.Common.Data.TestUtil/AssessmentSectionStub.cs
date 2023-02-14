﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// This class is a stub implementation of <see cref="IAssessmentSection"/>.
    /// </summary>
    public class AssessmentSectionStub : Observable, IAssessmentSection
    {
        private static readonly Random random = new Random(21);
        private readonly IEnumerable<IFailureMechanism> failureMechanisms;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalFloodingProbability;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForMaximumAllowableFloodingProbability;

        public AssessmentSectionStub() : this(Array.Empty<IFailureMechanism>()) {}

        public AssessmentSectionStub(IEnumerable<IFailureMechanism> failureMechanisms)
        {
            this.failureMechanisms = failureMechanisms;
            FailureMechanismContribution = new FailureMechanismContribution(1.0 / 30000,
                                                                            1.0 / 30000);
            BackgroundData = new BackgroundData(new WmtsBackgroundDataConfiguration())
            {
                Name = "Background data"
            };

            ReferenceLine = new ReferenceLine();

            HydraulicBoundaryDatabase = new HydraulicBoundaryData();

            WaterLevelCalculationsForUserDefinedTargetProbabilities = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.0001),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.00001)
            };

            WaveHeightCalculationsForUserDefinedTargetProbabilities = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.00025),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.000025)
            };

            waterLevelCalculationsForSignalFloodingProbability = new ObservableList<HydraulicBoundaryLocationCalculation>();
            waterLevelCalculationsForMaximumAllowableFloodingProbability = new ObservableList<HydraulicBoundaryLocationCalculation>();

            SpecificFailureMechanisms = new ObservableList<SpecificFailureMechanism>();
        }

        public string Id { get; }

        public string Name { get; set; }

        public Comment Comments { get; }

        public AssessmentSectionComposition Composition { get; }

        public ReferenceLine ReferenceLine { get; set; }

        public FailureMechanismContribution FailureMechanismContribution { get; }

        public HydraulicBoundaryData HydraulicBoundaryDatabase { get; }

        public BackgroundData BackgroundData { get; set; }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForSignalFloodingProbability => waterLevelCalculationsForSignalFloodingProbability;

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForMaximumAllowableFloodingProbability => waterLevelCalculationsForMaximumAllowableFloodingProbability;

        public ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> WaterLevelCalculationsForUserDefinedTargetProbabilities { get; }

        public ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> WaveHeightCalculationsForUserDefinedTargetProbabilities { get; }

        public ObservableList<SpecificFailureMechanism> SpecificFailureMechanisms { get; }

        /// <summary>
        /// Sets the hydraulic boundary locations on the assessment section stub.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to set.</param>
        /// <param name="setCalculationOutput">Whether to set dummy output for the automatically generated
        /// hydraulic boundary location calculations.</param>
        public void SetHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, bool setCalculationOutput = false)
        {
            HydraulicBoundaryDatabase.Locations.Clear();
            waterLevelCalculationsForSignalFloodingProbability.Clear();
            waterLevelCalculationsForMaximumAllowableFloodingProbability.Clear();

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                element.HydraulicBoundaryLocationCalculations.Clear();
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                element.HydraulicBoundaryLocationCalculations.Clear();
            }

            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in hydraulicBoundaryLocations)
            {
                AddHydraulicBoundaryLocation(hydraulicBoundaryLocation, setCalculationOutput);
            }
        }

        public IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            return failureMechanisms;
        }

        public void ChangeComposition(AssessmentSectionComposition newComposition)
        {
            throw new NotImplementedException("Stub only verifies Observable and basic behavior, use a proper stub when this function is necessary.");
        }

        private void AddHydraulicBoundaryLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation, bool setCalculationOutput)
        {
            HydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            waterLevelCalculationsForSignalFloodingProbability.Add(CreateHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation, setCalculationOutput));
            waterLevelCalculationsForMaximumAllowableFloodingProbability.Add(CreateHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation, setCalculationOutput));

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                element.HydraulicBoundaryLocationCalculations.Add(CreateHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation, setCalculationOutput));
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                element.HydraulicBoundaryLocationCalculations.Add(CreateHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation, setCalculationOutput));
            }
        }

        private static HydraulicBoundaryLocationCalculation CreateHydraulicBoundaryLocationCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                                                       bool setCalculationOutput)
        {
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            if (setCalculationOutput)
            {
                hydraulicBoundaryLocationCalculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            }

            return hydraulicBoundaryLocationCalculation;
        }
    }
}