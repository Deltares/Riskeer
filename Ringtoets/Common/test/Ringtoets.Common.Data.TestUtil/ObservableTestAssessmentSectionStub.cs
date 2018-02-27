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
    /// This class is a barebones implementation of <see cref="IAssessmentSection"/> and
    /// supports <see cref="IObservable"/> behavior.
    /// </summary>
    public class ObservableTestAssessmentSectionStub : Observable, IAssessmentSection
    {
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

            DesignWaterLevelLocationCalculations1 = new HydraulicBoundaryLocationCalculation[0];
            DesignWaterLevelLocationCalculations2 = new HydraulicBoundaryLocationCalculation[0];
            DesignWaterLevelLocationCalculations3 = new HydraulicBoundaryLocationCalculation[0];
            DesignWaterLevelLocationCalculations4 = new HydraulicBoundaryLocationCalculation[0];
            WaveHeightLocationCalculations1 = new HydraulicBoundaryLocationCalculation[0];
            WaveHeightLocationCalculations2 = new HydraulicBoundaryLocationCalculation[0];
            WaveHeightLocationCalculations3 = new HydraulicBoundaryLocationCalculation[0];
            WaveHeightLocationCalculations4 = new HydraulicBoundaryLocationCalculation[0];
        }

        public string Id { get; }
        public string Name { get; set; }
        public Comment Comments { get; }
        public AssessmentSectionComposition Composition { get; }
        public ReferenceLine ReferenceLine { get; set; }
        public FailureMechanismContribution FailureMechanismContribution { get; }
        public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; }
        public BackgroundData BackgroundData { get; set; }
        public IEnumerable<HydraulicBoundaryLocationCalculation> DesignWaterLevelLocationCalculations1 { get; }
        public IEnumerable<HydraulicBoundaryLocationCalculation> DesignWaterLevelLocationCalculations2 { get; }
        public IEnumerable<HydraulicBoundaryLocationCalculation> DesignWaterLevelLocationCalculations3 { get; }
        public IEnumerable<HydraulicBoundaryLocationCalculation> DesignWaterLevelLocationCalculations4 { get; }
        public IEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightLocationCalculations1 { get; }
        public IEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightLocationCalculations2 { get; }
        public IEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightLocationCalculations3 { get; }
        public IEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightLocationCalculations4 { get; }

        public IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield break;
        }

        public void ChangeComposition(AssessmentSectionComposition newComposition)
        {
            throw new NotImplementedException("Stub only verifies Observable and basic behaviour, use a proper stub when this function is necessary.");
        }

        public void SetHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            throw new NotImplementedException("Stub only verifies Observable and basic behaviour, use a proper stub when this function is necessary.");
        }
    }
}