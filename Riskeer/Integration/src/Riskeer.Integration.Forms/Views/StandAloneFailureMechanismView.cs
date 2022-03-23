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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.MapLayers;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a stand alone failure mechanism.
    /// </summary>
    public partial class StandAloneFailureMechanismView<TFailureMechanism, TSectionResult> : UserControl, IMapView
        where TFailureMechanism : IFailureMechanism<TSectionResult>
        where TSectionResult : FailureMechanismSectionResult
    {
        private readonly Func<TSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc;

        private HydraulicBoundaryLocationsMapLayer hydraulicBoundaryLocationsMapLayer;
        private NonCalculatableFailureMechanismSectionResultsMapLayer<TSectionResult> assemblyResultMapLayer;

        private MapDataCollection mapDataCollection;
        private MapLineData referenceLineMapData;

        private MapLineData sectionsMapData;
        private MapPointData sectionsStartPointMapData;
        private MapPointData sectionsEndPointMapData;

        private Observer failureMechanismObserver;
        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;

        /// <summary>
        /// Creates a new instance of <see cref="StandAloneFailureMechanismView{TFailureMechanism,TSectionResult}"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <param name="performAssemblyFunc">The <see cref="Func{T1,TResult}"/> used to assemble the result of a section result.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StandAloneFailureMechanismView(TFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
                                              Func<TSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (performAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(performAssemblyFunc));
            }

            InitializeComponent();

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
            this.performAssemblyFunc = performAssemblyFunc;

            CreateObservers();

            CreateMapData();
            SetAllMapDataFeatures();
            riskeerMapControl.SetAllData(mapDataCollection, assessmentSection.BackgroundData);
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public TFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        public IMapControl Map => riskeerMapControl.MapControl;

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            assessmentSectionObserver.Dispose();
            referenceLineObserver.Dispose();
            hydraulicBoundaryLocationsMapLayer.Dispose();
            assemblyResultMapLayer.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CreateMapData()
        {
            mapDataCollection = new MapDataCollection(FailureMechanism.Name);
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapLayer = new HydraulicBoundaryLocationsMapLayer(AssessmentSection);

            MapDataCollection sectionsMapDataCollection = RiskeerMapDataFactory.CreateSectionsMapDataCollection();
            sectionsMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            assemblyResultMapLayer = new NonCalculatableFailureMechanismSectionResultsMapLayer<TSectionResult>(
                FailureMechanism, performAssemblyFunc);

            mapDataCollection.Add(referenceLineMapData);

            sectionsMapDataCollection.Add(sectionsMapData);
            sectionsMapDataCollection.Add(sectionsStartPointMapData);
            sectionsMapDataCollection.Add(sectionsEndPointMapData);
            mapDataCollection.Add(sectionsMapDataCollection);

            mapDataCollection.Add(assemblyResultMapLayer.MapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapLayer.MapData);
        }

        private void CreateObservers()
        {
            failureMechanismObserver = new Observer(UpdateFailureMechanismMapData)
            {
                Observable = FailureMechanism
            };
            assessmentSectionObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection
            };
            referenceLineObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection.ReferenceLine
            };
        }

        private void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetSectionsMapData();
        }

        #region AssessmentSection MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            referenceLineMapData.Features = RiskeerMapDataFeaturesFactory.CreateReferenceLineFeatures(
                AssessmentSection.ReferenceLine, AssessmentSection.Id, AssessmentSection.Name);
        }

        #endregion

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