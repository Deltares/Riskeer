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
using Core.Common.Base;
using Core.Components.Gis.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.MapLayers;
using Riskeer.StabilityStoneCover.Data;
using StabilityStoneCoverDataResources = Riskeer.StabilityStoneCover.Data.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a stability stone cover failure path.
    /// </summary>
    public class StabilityStoneCoverFailurePathView : StabilityStoneCoverFailureMechanismView
    {
        private MapLineData sectionsMapData;
        private MapPointData sectionsStartPointMapData;
        private MapPointData sectionsEndPointMapData;

        private NonCalculatableFailureMechanismSectionResultsMapLayer<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> assemblyResultMapLayer;

        private Observer failureMechanismObserver;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverFailurePathView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityStoneCoverFailurePathView(StabilityStoneCoverFailureMechanism failureMechanism,
                                                  IAssessmentSection assessmentSection)
            : base(failureMechanism, assessmentSection) {}

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            assemblyResultMapLayer.Dispose();

            base.Dispose(disposing);
        }

        protected override void CreateMapData()
        {
            base.CreateMapData();

            MapDataCollection sectionsMapDataCollection = RiskeerMapDataFactory.CreateSectionsMapDataCollection();
            sectionsMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            assemblyResultMapLayer = new NonCalculatableFailureMechanismSectionResultsMapLayer<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                FailureMechanism, sr => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection(sr, FailureMechanism, AssessmentSection));

            sectionsMapDataCollection.Add(sectionsMapData);
            sectionsMapDataCollection.Add(sectionsStartPointMapData);
            sectionsMapDataCollection.Add(sectionsEndPointMapData);
            MapDataCollection.Insert(1, sectionsMapDataCollection);

            MapDataCollection.Insert(2, assemblyResultMapLayer.MapData);
        }

        protected override void CreateObservers()
        {
            base.CreateObservers();

            failureMechanismObserver = new Observer(UpdateFailureMechanismData)
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

        private void UpdateFailureMechanismData()
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