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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Components.Gis.Data;
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
            FailureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 0, 1.0/300000);
            BackgroundMapData = new BackgroundMapDataContainer();
        }

        public string Id { get; }
        public string Name { get; set; }
        public Comment Comments { get; }
        public AssessmentSectionComposition Composition { get; }
        public ReferenceLine ReferenceLine { get; set; }
        public FailureMechanismContribution FailureMechanismContribution { get; }
        public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }
        public BackgroundMapDataContainer BackgroundMapData { get; }

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