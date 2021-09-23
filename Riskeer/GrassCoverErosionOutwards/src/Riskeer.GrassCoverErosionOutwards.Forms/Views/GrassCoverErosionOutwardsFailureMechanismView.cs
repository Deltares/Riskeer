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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.Factories;
using Riskeer.Revetment.Data;
using GrassCoverErosionOutwardsDataResources = Riskeer.GrassCoverErosionOutwards.Data.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a grass cover erosion outwards failure mechanism.
    /// </summary>
    public partial class GrassCoverErosionOutwardsFailureMechanismView : UserControl, IMapView
    {
        private HydraulicBoundaryLocationsMapLayer hydraulicBoundaryLocationsMapLayer;

        private MapLineData referenceLineMapData;
        private MapLineData foreshoreProfilesMapData;
        private MapLineData calculationsMapData;

        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;
        private Observer foreshoreProfilesObserver;

        private RecursiveObserver<CalculationGroup, WaveConditionsInput> calculationInputObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, GrassCoverErosionOutwardsWaveConditionsCalculation> calculationObserver;
        private RecursiveObserver<ForeshoreProfileCollection, ForeshoreProfile> foreshoreProfileObserver;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsFailureMechanismView(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                             IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            InitializeComponent();

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        public IMapControl Map
        {
            get
            {
                return riskeerMapControl.MapControl;
            }
        }

        /// <summary>
        /// Gets the <see cref="MapDataCollection"/>.
        /// </summary>
        protected MapDataCollection MapDataCollection { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            CreateObservers();

            CreateMapData();

            SetAllMapDataFeatures();

            riskeerMapControl.SetAllData(MapDataCollection, AssessmentSection.BackgroundData);

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            hydraulicBoundaryLocationsMapLayer.Dispose();

            assessmentSectionObserver.Dispose();
            referenceLineObserver.Dispose();
            foreshoreProfilesObserver.Dispose();
            foreshoreProfileObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates the map data.
        /// </summary>
        protected virtual void CreateMapData()
        {
            hydraulicBoundaryLocationsMapLayer = new HydraulicBoundaryLocationsMapLayer(AssessmentSection);

            MapDataCollection = new MapDataCollection(GrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsFailureMechanism_DisplayName);
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();
            foreshoreProfilesMapData = RiskeerMapDataFactory.CreateForeshoreProfileMapData();
            calculationsMapData = RiskeerMapDataFactory.CreateCalculationsMapData();

            MapDataCollection.Add(referenceLineMapData);
            MapDataCollection.Add(hydraulicBoundaryLocationsMapLayer.MapData);
            MapDataCollection.Add(foreshoreProfilesMapData);
            MapDataCollection.Add(calculationsMapData);
        }

        /// <summary>
        /// Creates the observers.
        /// </summary>        
        protected virtual void CreateObservers()
        {
            assessmentSectionObserver = new Observer(UpdateReferenceLineData)
            {
                Observable = AssessmentSection
            };
            referenceLineObserver = new Observer(UpdateReferenceLineData)
            {
                Observable = AssessmentSection.ReferenceLine
            };
            foreshoreProfilesObserver = new Observer(UpdateForeshoreProfilesMapData)
            {
                Observable = FailureMechanism.ForeshoreProfiles
            };

            calculationInputObserver = new RecursiveObserver<CalculationGroup, WaveConditionsInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                                 .Select(pc => pc.InputParameters)))
            {
                Observable = FailureMechanism.WaveConditionsCalculationGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.WaveConditionsCalculationGroup
            };
            calculationObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionOutwardsWaveConditionsCalculation>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.WaveConditionsCalculationGroup
            };
            foreshoreProfileObserver = new RecursiveObserver<ForeshoreProfileCollection, ForeshoreProfile>(UpdateForeshoreProfilesMapData, coll => coll)
            {
                Observable = FailureMechanism.ForeshoreProfiles
            };
        }

        /// <summary>
        /// Sets all map data features.
        /// </summary>        
        protected virtual void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetForeshoreProfilesMapData();
            SetCalculationsMapData();
        }

        #region Calculations MapData

        /// <summary>
        /// Updates the calculations map data.
        /// </summary>
        protected virtual void UpdateCalculationsMapData()
        {
            SetCalculationsMapData();
            calculationsMapData.NotifyObservers();
        }

        private void SetCalculationsMapData()
        {
            IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations =
                FailureMechanism.WaveConditionsCalculationGroup.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();
            calculationsMapData.Features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(calculations);
        }

        #endregion

        #region AssessmentSection MapData

        private void UpdateReferenceLineData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            ReferenceLine referenceLine = AssessmentSection.ReferenceLine;
            referenceLineMapData.Features = RiskeerMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, AssessmentSection.Id, AssessmentSection.Name);
        }

        #endregion

        #region Foreshore Profiles MapData

        private void UpdateForeshoreProfilesMapData()
        {
            SetForeshoreProfilesMapData();
            foreshoreProfilesMapData.NotifyObservers();
        }

        private void SetForeshoreProfilesMapData()
        {
            IEnumerable<ForeshoreProfile> foreshoreProfiles = FailureMechanism.ForeshoreProfiles;
            foreshoreProfilesMapData.Features = RiskeerMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(foreshoreProfiles);
        }

        #endregion
    }
}