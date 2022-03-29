﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Components.Gis.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.MapLayers;
using Riskeer.StabilityPointStructures.Data;
using StabilityPointStructuresDataResources = Riskeer.StabilityPointStructures.Data.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Forms.Views.RegistrationState
{
    /// <summary>
    /// Registration state view showing map data for a stability point structures failure mechanism.
    /// </summary>
    public class StabilityPointStructuresFailureMechanismView : CalculationsState.StabilityPointStructuresFailureMechanismView
    {
        private MapLineData sectionsMapData;
        private MapPointData sectionsStartPointMapData;
        private MapPointData sectionsEndPointMapData;

        private CalculatableFailureMechanismSectionResultsMapLayer<StabilityPointStructuresFailureMechanism, AdoptableFailureMechanismSectionResult, StabilityPointStructuresInput> assemblyResultsMapLayer;

        private Observer failureMechanismObserver;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityPointStructuresFailureMechanismView(StabilityPointStructuresFailureMechanism failureMechanism,
                                                            IAssessmentSection assessmentSection)
            : base(failureMechanism, assessmentSection) {}

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            assemblyResultsMapLayer.Dispose();

            base.Dispose(disposing);
        }

        protected override void CreateMapData()
        {
            base.CreateMapData();

            MapDataCollection sectionsMapDataCollection = RiskeerMapDataFactory.CreateSectionsMapDataCollection();
            sectionsMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            assemblyResultsMapLayer = new CalculatableFailureMechanismSectionResultsMapLayer<StabilityPointStructuresFailureMechanism, AdoptableFailureMechanismSectionResult, StabilityPointStructuresInput>(
                FailureMechanism, sr => StructuresFailureMechanismAssemblyFactory.AssembleSection<StabilityPointStructuresInput>(sr, FailureMechanism, AssessmentSection));

            sectionsMapDataCollection.Add(sectionsMapData);
            sectionsMapDataCollection.Add(sectionsStartPointMapData);
            sectionsMapDataCollection.Add(sectionsEndPointMapData);
            MapDataCollection.Insert(1, sectionsMapDataCollection);

            MapDataCollection.Insert(2, assemblyResultsMapLayer.MapData);
        }

        protected override void CreateObservers()
        {
            base.CreateObservers();

            failureMechanismObserver = new Observer(UpdateFailureMechanismMapData)
            {
                Observable = FailureMechanism
            };
        }

        protected override void SetAllMapDataFeatures()
        {
            base.SetAllMapDataFeatures();

            SetSectionsMapData();
        }

        #region FailureMechanism MapData

        private void UpdateFailureMechanismMapData()
        {
            SetSectionsMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();
        }

        private void SetSectionsMapData()
        {
            IEnumerable<FailureMechanismSection> failureMechanismSections = FailureMechanism.Sections;
            sectionsMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
        }

        #endregion
    }
}