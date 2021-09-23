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
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Factories;
using GrassCoverErosionInwardsDataResources = Riskeer.GrassCoverErosionInwards.Data.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a grass cover erosion inwards failure mechanism.
    /// </summary>
    public partial class GrassCoverErosionInwardsFailureMechanismView : UserControl, IMapView
    {
        private HydraulicBoundaryLocationsMapLayer hydraulicBoundaryLocationsMapLayer;

        private MapLineData referenceLineMapData;
        private MapLineData dikeProfilesMapData;
        private MapLineData foreshoreProfilesMapData;
        private MapLineData calculationsMapData;

        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;
        private Observer dikeProfilesObserver;

        private RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput> calculationInputObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculation> calculationObserver;
        private RecursiveObserver<DikeProfileCollection, DikeProfile> dikeProfileObserver;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsFailureMechanismView(GrassCoverErosionInwardsFailureMechanism failureMechanism,
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
        public GrassCoverErosionInwardsFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the <see cref="IAssessmentSection"/>.
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
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();
            dikeProfilesObserver.Dispose();
            dikeProfileObserver.Dispose();

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

            MapDataCollection = new MapDataCollection(GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsFailureMechanism_DisplayName);
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();
            dikeProfilesMapData = RiskeerMapDataFactory.CreateDikeProfileMapData();
            foreshoreProfilesMapData = RiskeerMapDataFactory.CreateForeshoreProfileMapData();
            calculationsMapData = RiskeerMapDataFactory.CreateCalculationsMapData();

            MapDataCollection.Add(referenceLineMapData);
            MapDataCollection.Add(hydraulicBoundaryLocationsMapLayer.MapData);
            MapDataCollection.Add(dikeProfilesMapData);
            MapDataCollection.Add(foreshoreProfilesMapData);
            MapDataCollection.Add(calculationsMapData);
        }

        /// <summary>
        /// Creates the observers.
        /// </summary>        
        protected virtual void CreateObservers()
        {
            assessmentSectionObserver = new Observer(UpdateUpdateReferenceLineMapData)
            {
                Observable = AssessmentSection
            };
            referenceLineObserver = new Observer(UpdateUpdateReferenceLineMapData)
            {
                Observable = AssessmentSection.ReferenceLine
            };
            dikeProfilesObserver = new Observer(UpdateDikeProfilesMapData)
            {
                Observable = FailureMechanism.DikeProfiles
            };

            calculationInputObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<GrassCoverErosionInwardsCalculation>()
                                                                                 .Select(pc => pc.InputParameters)))
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            calculationObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculation>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            dikeProfileObserver = new RecursiveObserver<DikeProfileCollection, DikeProfile>(UpdateDikeProfilesMapData, dpc => dpc)
            {
                Observable = FailureMechanism.DikeProfiles
            };
        }

        /// <summary>
        /// Sets all map data features.
        /// </summary>        
        protected virtual void SetAllMapDataFeatures()
        {
            SetCalculationsMapData();
            SetReferenceLineMapData();

            SetDikeProfilesMapData();
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
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations =
                FailureMechanism.CalculationsGroup.GetCalculations().Cast<GrassCoverErosionInwardsCalculation>();

            calculationsMapData.Features = GrassCoverErosionInwardsMapDataFeaturesFactory.CreateCalculationFeatures(calculations);
        }

        #endregion

        #region AssessmentSection MapData

        private void UpdateUpdateReferenceLineMapData()
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

        #region DikeProfiles MapData

        private void UpdateDikeProfilesMapData()
        {
            SetDikeProfilesMapData();
            dikeProfilesMapData.NotifyObservers();
            foreshoreProfilesMapData.NotifyObservers();
        }

        private void SetDikeProfilesMapData()
        {
            IEnumerable<DikeProfile> dikeProfiles = FailureMechanism.DikeProfiles;

            dikeProfilesMapData.Features = RiskeerMapDataFeaturesFactory.CreateDikeProfilesFeatures(dikeProfiles);
            foreshoreProfilesMapData.Features = RiskeerMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(dikeProfiles.Select(dp => dp.ForeshoreProfile));
        }

        #endregion
    }
}