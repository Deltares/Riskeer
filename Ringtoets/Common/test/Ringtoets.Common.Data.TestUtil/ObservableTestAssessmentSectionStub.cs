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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// This class is a barebones implementation of <see cref="IAssessmentSection"/>.
    /// </summary>
    public class ObservableTestAssessmentSectionStub : Observable, IAssessmentSection
    {
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedSignalingNorm;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalingNorm;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNorm;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedLowerLimitNorm;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedSignalingNorm;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForSignalingNorm;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForLowerLimitNorm;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedLowerLimitNorm;

        public ObservableTestAssessmentSectionStub()
        {
            FailureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(),
                                                                            0,
                                                                            1.0 / 30000,
                                                                            1.0 / 30000);
            BackgroundData = new BackgroundData(new WmtsBackgroundDataConfiguration())
            {
                Name = "Background data"
            };

            HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            waterLevelCalculationsForFactorizedSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
            waterLevelCalculationsForSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
            waterLevelCalculationsForLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
            waterLevelCalculationsForFactorizedLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
            waveHeightCalculationsForFactorizedSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
            waveHeightCalculationsForSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
            waveHeightCalculationsForLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
            waveHeightCalculationsForFactorizedLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        }

        public string Id { get; }

        public string Name { get; set; }

        public Comment Comments { get; }

        public AssessmentSectionComposition Composition { get; }

        public ReferenceLine ReferenceLine { get; set; }

        public FailureMechanismContribution FailureMechanismContribution { get; }

        public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; }

        public BackgroundData BackgroundData { get; set; }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForFactorizedSignalingNorm
        {
            get
            {
                return waterLevelCalculationsForFactorizedSignalingNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForSignalingNorm
        {
            get
            {
                return waterLevelCalculationsForSignalingNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForLowerLimitNorm
        {
            get
            {
                return waterLevelCalculationsForLowerLimitNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForFactorizedLowerLimitNorm
        {
            get
            {
                return waterLevelCalculationsForFactorizedLowerLimitNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForFactorizedSignalingNorm
        {
            get
            {
                return waveHeightCalculationsForFactorizedSignalingNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForSignalingNorm
        {
            get
            {
                return waveHeightCalculationsForSignalingNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForLowerLimitNorm
        {
            get
            {
                return waveHeightCalculationsForLowerLimitNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForFactorizedLowerLimitNorm
        {
            get
            {
                return waveHeightCalculationsForFactorizedLowerLimitNorm;
            }
        }

        public IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield break;
        }

        public void ChangeComposition(AssessmentSectionComposition newComposition)
        {
            throw new NotImplementedException("Stub only verifies Observable and basic behaviour, use a proper stub when this function is necessary.");
        }
    }
}