﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using GrassCoverErosionOutwardsDataResources = Ringtoets.GrassCoverErosionOutwards.Data.Properties.Resources;
using GrassCoverErosionOutwardsFormsResources = Ringtoets.GrassCoverErosionOutwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a grass cover erosion outwards failure mechanism.
    /// </summary>
    public partial class GrassCoverErosionOutwardsFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;
        private readonly Observer foreshoreProfilesObserver;

        private readonly RecursiveObserver<CalculationGroup, WaveConditionsInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, GrassCoverErosionOutwardsWaveConditionsCalculation> calculationObserver;

        private readonly MapDataCollection mapDataCollection;
        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryDatabaseMapData;
        private readonly MapLineData foreshoreProfilesMapData;
        private readonly MapLineData calculationsMapData;

        private GrassCoverErosionOutwardsFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsFailureMechanismView"/>.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanismView()
        {
            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateMapData);
            assessmentSectionObserver = new Observer(() =>
            {
                if (hydraulicBoundaryDatabaseObserver.Observable == null && data.Parent.HydraulicBoundaryDatabase != null)
                {
                    hydraulicBoundaryDatabaseObserver.Observable = data.Parent.HydraulicBoundaryDatabase;
                }
                UpdateMapData();
            });
            hydraulicBoundaryDatabaseObserver = new Observer(UpdateMapData);
            foreshoreProfilesObserver = new Observer(UpdateMapData);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, WaveConditionsInput>(
                UpdateMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>().Select(pc => pc.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateMapData, pcg => pcg.Children);
            calculationObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionOutwardsWaveConditionsCalculation>(UpdateMapData, pcg => pcg.Children);

            mapDataCollection = new MapDataCollection(GrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryDatabaseMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryDatabaseMapData();
            foreshoreProfilesMapData = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();
            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            calculationsMapData = RingtoetsMapDataFactory.CreateCalculationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(sectionsMapData);
            mapDataCollection.Add(sectionsStartPointMapData);
            mapDataCollection.Add(sectionsEndPointMapData);
            mapDataCollection.Add(hydraulicBoundaryDatabaseMapData);
            mapDataCollection.Add(foreshoreProfilesMapData);
            mapDataCollection.Add(calculationsMapData);
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as GrassCoverErosionOutwardsFailureMechanismContext;

                if (data == null)
                {
                    failureMechanismObserver.Observable = null;
                    assessmentSectionObserver.Observable = null;
                    hydraulicBoundaryDatabaseObserver.Observable = null;
                    foreshoreProfilesObserver.Observable = null;
                    calculationInputObserver.Observable = null;
                    calculationGroupObserver.Observable = null;
                    calculationObserver.Observable = null;

                    Map.Data = null;
                }
                else
                {
                    failureMechanismObserver.Observable = data.WrappedData;
                    assessmentSectionObserver.Observable = data.Parent;
                    hydraulicBoundaryDatabaseObserver.Observable = data.Parent.HydraulicBoundaryDatabase;
                    foreshoreProfilesObserver.Observable = data.WrappedData.ForeshoreProfiles;
                    calculationInputObserver.Observable = data.WrappedData.WaveConditionsCalculationGroup;
                    calculationGroupObserver.Observable = data.WrappedData.WaveConditionsCalculationGroup;
                    calculationObserver.Observable = data.WrappedData.WaveConditionsCalculationGroup;

                    SetMapDataFeatures();

                    Map.Data = mapDataCollection;
                }
            }
        }

        public IMapControl Map
        {
            get
            {
                return mapControl;
            }
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            assessmentSectionObserver.Dispose();
            hydraulicBoundaryDatabaseObserver.Dispose();
            foreshoreProfilesObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateMapData()
        {
            SetMapDataFeatures();

            referenceLineMapData.NotifyObservers();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();
            hydraulicBoundaryDatabaseMapData.NotifyObservers();
            foreshoreProfilesMapData.NotifyObservers();
            calculationsMapData.NotifyObservers();
        }

        private void SetMapDataFeatures()
        {
            ReferenceLine referenceLine = data.Parent.ReferenceLine;
            IEnumerable<FailureMechanismSection> failureMechanismSections = data.WrappedData.Sections;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = data.Parent.HydraulicBoundaryDatabase;
            IEnumerable<ForeshoreProfile> foreshoreProfiles = data.WrappedData.ForeshoreProfiles;
            IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations =
                data.WrappedData.WaveConditionsCalculationGroup.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();

            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, data.Parent.Id, data.Parent.Name);
            sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
            hydraulicBoundaryDatabaseMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeaturesWithOptionalLabels(hydraulicBoundaryDatabase);
            foreshoreProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(foreshoreProfiles);
            calculationsMapData.Features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(calculations);
        }
    }
}