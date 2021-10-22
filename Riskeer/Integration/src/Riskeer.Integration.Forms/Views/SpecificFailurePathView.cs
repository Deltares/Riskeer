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
using Core.Components.Gis.Forms;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.Views;
using Riskeer.Integration.Data.FailurePath;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// View for <see cref="SpecificFailurePath"/>.
    /// </summary>
    public partial class SpecificFailurePathView : CloseForFailurePathView, IMapView
    {
        private readonly SpecificFailurePath failurePath;

        private MapDataCollection failurePathMapDataCollection;

        private HydraulicBoundaryLocationsMapLayer hydraulicBoundaryLocationsMapLayer;
        
        private MapLineData referenceLineMapData;

        private MapLineData sectionsMapData;
        private MapPointData sectionsStartPointMapData;
        private MapPointData sectionsEndPointMapData;

        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;
        private Observer failurePathObserver;

        /// <summary>
        /// Creates a new instance of <see cref="SpecificFailurePathView"/>.
        /// </summary>
        /// <param name="failurePath">The <see cref="SpecificFailurePath"/> to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SpecificFailurePathView(SpecificFailurePath failurePath, IAssessmentSection assessmentSection) : base(failurePath)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }


            InitializeComponent();

            this.failurePath = failurePath;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the <see cref="IAssessmentSection"/>.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        public IMapControl Map => riskeerMapControl.MapControl;

        protected override void OnLoad(EventArgs e)
        {
            CreateObservers();

            CreateMapData();
            SetAllMapDataFeatures();

            riskeerMapControl.SetAllData(failurePathMapDataCollection, AssessmentSection.BackgroundData);

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            referenceLineObserver.Dispose();
            failurePathObserver.Dispose();
            hydraulicBoundaryLocationsMapLayer.Dispose();

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
                Observable = AssessmentSection
            };

            referenceLineObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection.ReferenceLine
            };

            failurePathObserver = new Observer(UpdateFailurePathMapData)
            {
                Observable = failurePath
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
            failurePathMapDataCollection = new MapDataCollection(failurePath.Name);
            hydraulicBoundaryLocationsMapLayer = new HydraulicBoundaryLocationsMapLayer(AssessmentSection);
            
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();

            MapDataCollection sectionsMapDataCollection = RiskeerMapDataFactory.CreateSectionsMapDataCollection();
            sectionsMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            sectionsMapDataCollection.Add(sectionsMapData);
            sectionsMapDataCollection.Add(sectionsStartPointMapData);
            sectionsMapDataCollection.Add(sectionsEndPointMapData);

            failurePathMapDataCollection.Add(referenceLineMapData);
            failurePathMapDataCollection.Add(sectionsMapData);
            failurePathMapDataCollection.Add(hydraulicBoundaryLocationsMapLayer.MapData);
        }

        #region ReferenceLine MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            referenceLineMapData.Features = RiskeerMapDataFeaturesFactory.CreateReferenceLineFeatures(AssessmentSection.ReferenceLine,
                                                                                                      AssessmentSection.Id,
                                                                                                      AssessmentSection.Name);
        }

        #endregion

        #region FailurePath MapData

        private void UpdateFailurePathMapData()
        {
            UpdateFailurePathMapDataCollectionData();

            SetSectionsMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();
        }

        private void UpdateFailurePathMapDataCollectionData()
        {
            failurePathMapDataCollection.Name = failurePath.Name;
            failurePathMapDataCollection.NotifyObservers();
        }

        private void SetSectionsMapData()
        {
            IEnumerable<FailureMechanismSection> failureMechanismSections = failurePath.Sections;
            sectionsMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
        }

        #endregion
    }
}