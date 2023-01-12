// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.MapLayers;
using Riskeer.Common.Forms.Views;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// View for <see cref="SpecificFailureMechanism"/>.
    /// </summary>
    public partial class SpecificFailureMechanismView : CloseForFailureMechanismView, IMapView
    {
        private readonly SpecificFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        private MapDataCollection failureMechanismMapDataCollection;

        private HydraulicBoundaryLocationsMapLayer hydraulicBoundaryLocationsMapLayer;

        private NonCalculatableFailureMechanismSectionResultsMapLayer<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> assemblyResultMapLayer;

        private MapLineData referenceLineMapData;

        private MapLineData sectionsMapData;
        private MapPointData sectionsStartPointMapData;
        private MapPointData sectionsEndPointMapData;

        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;
        private Observer failureMechanismObserver;

        /// <summary>
        /// Creates a new instance of <see cref="SpecificFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="SpecificFailureMechanism"/> to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SpecificFailureMechanismView(SpecificFailureMechanism failureMechanism, IAssessmentSection assessmentSection) : base(failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            InitializeComponent();

            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;
        }

        public IMapControl Map => riskeerMapControl.MapControl;

        protected override void OnLoad(EventArgs e)
        {
            CreateObservers();

            CreateMapData();
            SetAllMapDataFeatures();

            riskeerMapControl.SetAllData(failureMechanismMapDataCollection, assessmentSection.BackgroundData);

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            referenceLineObserver.Dispose();
            failureMechanismObserver.Dispose();
            hydraulicBoundaryLocationsMapLayer.Dispose();
            assemblyResultMapLayer.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates observers for the related data.
        /// </summary>
        protected virtual void CreateObservers()
        {
            assessmentSectionObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = assessmentSection
            };

            referenceLineObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = assessmentSection.ReferenceLine
            };

            failureMechanismObserver = new Observer(UpdateFailureMechanismMapData)
            {
                Observable = failureMechanism
            };
        }

        /// <summary>
        /// Sets all map data features.
        /// </summary>
        protected virtual void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetSectionsMapData();
        }

        private void CreateMapData()
        {
            failureMechanismMapDataCollection = new MapDataCollection(failureMechanism.Name);
            hydraulicBoundaryLocationsMapLayer = new HydraulicBoundaryLocationsMapLayer(assessmentSection);

            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();

            MapDataCollection sectionsMapDataCollection = RiskeerMapDataFactory.CreateSectionsMapDataCollection();
            sectionsMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            sectionsMapDataCollection.Add(sectionsMapData);
            sectionsMapDataCollection.Add(sectionsStartPointMapData);
            sectionsMapDataCollection.Add(sectionsEndPointMapData);

            assemblyResultMapLayer = new NonCalculatableFailureMechanismSectionResultsMapLayer<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                failureMechanism, sr => FailureMechanismAssemblyFactory.AssembleSection(sr, failureMechanism, assessmentSection).AssemblyResult);

            failureMechanismMapDataCollection.Add(referenceLineMapData);
            failureMechanismMapDataCollection.Add(sectionsMapDataCollection);
            failureMechanismMapDataCollection.Add(assemblyResultMapLayer.MapData);
            failureMechanismMapDataCollection.Add(hydraulicBoundaryLocationsMapLayer.MapData);
        }

        #region ReferenceLine MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            referenceLineMapData.Features = RiskeerMapDataFeaturesFactory.CreateReferenceLineFeatures(assessmentSection.ReferenceLine,
                                                                                                      assessmentSection.Id,
                                                                                                      assessmentSection.Name);
        }

        #endregion

        #region FailureMechanism MapData

        private void UpdateFailureMechanismMapData()
        {
            UpdateFailureMechanismMapDataCollectionData();

            SetSectionsMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();
        }

        private void UpdateFailureMechanismMapDataCollectionData()
        {
            failureMechanismMapDataCollection.Name = failureMechanism.Name;
            failureMechanismMapDataCollection.NotifyObservers();
        }

        private void SetSectionsMapData()
        {
            IEnumerable<FailureMechanismSection> failureMechanismSections = failureMechanism.Sections.ToArray();
            sectionsMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
        }

        #endregion
    }
}