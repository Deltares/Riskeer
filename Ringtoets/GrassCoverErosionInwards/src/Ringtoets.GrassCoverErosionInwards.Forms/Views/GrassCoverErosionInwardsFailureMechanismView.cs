﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Factories;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a grass cover erosion inwards failure mechanism.
    /// </summary>
    public partial class GrassCoverErosionInwardsFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryLocationsObserver;
        private readonly Observer dikeProfilesObserver;

        private readonly RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation> hydraulicBoundaryLocationObserver;
        private readonly RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculation> calculationObserver;
        private readonly RecursiveObserver<DikeProfileCollection, DikeProfile> dikeProfileObserver;

        private readonly MapDataCollection mapDataCollection;
        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;
        private readonly MapLineData dikeProfilesMapData;
        private readonly MapLineData foreshoreProfilesMapData;
        private readonly MapLineData calculationsMapData;

        private GrassCoverErosionInwardsFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show data for.</param>
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

            failureMechanismObserver = new Observer(UpdateAllMapData);
            assessmentSectionObserver = new Observer(UpdateAllMapData);
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData);
            dikeProfilesObserver = new Observer(UpdateDikeProfilesMapData);

            hydraulicBoundaryLocationObserver = new RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation>(
                UpdateHydraulicBoundaryLocationsMapData, hbl => hbl);
            calculationInputObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<GrassCoverErosionInwardsCalculation>()
                                                                                 .Select(pc => pc.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children);
            calculationObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculation>(UpdateCalculationsMapData, pcg => pcg.Children);
            dikeProfileObserver = new RecursiveObserver<DikeProfileCollection, DikeProfile>(UpdateDikeProfilesMapData, dpc => dpc);

            mapDataCollection = new MapDataCollection(GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            dikeProfilesMapData = RingtoetsMapDataFactory.CreateDikeProfileMapData();
            foreshoreProfilesMapData = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();
            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            calculationsMapData = RingtoetsMapDataFactory.CreateCalculationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(sectionsMapData);
            mapDataCollection.Add(sectionsStartPointMapData);
            mapDataCollection.Add(sectionsEndPointMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);
            mapDataCollection.Add(dikeProfilesMapData);
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
                data = value as GrassCoverErosionInwardsFailureMechanismContext;

                if (data == null)
                {
                    failureMechanismObserver.Observable = null;
                    assessmentSectionObserver.Observable = null;
                    hydraulicBoundaryLocationsObserver.Observable = null;
                    hydraulicBoundaryLocationObserver.Observable = null;

                    calculationInputObserver.Observable = null;
                    calculationGroupObserver.Observable = null;
                    calculationObserver.Observable = null;

                    dikeProfilesObserver.Observable = null;
                    dikeProfileObserver.Observable = null;

                    ringtoetsMapControl.RemoveAllData();
                }
                else
                {
                    failureMechanismObserver.Observable = data.WrappedData;
                    assessmentSectionObserver.Observable = data.Parent;
                    hydraulicBoundaryLocationsObserver.Observable = data.Parent.HydraulicBoundaryDatabase.Locations;
                    hydraulicBoundaryLocationObserver.Observable = data.Parent.HydraulicBoundaryDatabase.Locations;
                    calculationInputObserver.Observable = data.WrappedData.CalculationsGroup;
                    calculationGroupObserver.Observable = data.WrappedData.CalculationsGroup;
                    calculationObserver.Observable = data.WrappedData.CalculationsGroup;

                    dikeProfilesObserver.Observable = data.WrappedData.DikeProfiles;
                    dikeProfileObserver.Observable = data.WrappedData.DikeProfiles;

                    SetAllMapDataFeatures();

                    ringtoetsMapControl.SetAllData(mapDataCollection, data.Parent.BackgroundData);
                }
            }
        }

        public IMapControl Map
        {
            get
            {
                return ringtoetsMapControl.MapControl;
            }
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            assessmentSectionObserver.Dispose();
            hydraulicBoundaryLocationsObserver.Dispose();
            hydraulicBoundaryLocationObserver.Dispose();
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

        private void UpdateAllMapData()
        {
            UpdateCalculationsMapData();
            UpdateHydraulicBoundaryLocationsMapData();
            UpdateReferenceLineMapData();

            UpdateSectionsMapData();
            UpdateDikeProfilesMapData();
        }

        private void SetAllMapDataFeatures()
        {
            SetCalculationsMapData();
            SetHydraulicBoundaryLocationsMapData();
            SetReferenceLineMapData();

            SetSectionsMapData();
            SetDikeProfilesMapData();
        }

        #region Calculations MapData

        private void UpdateCalculationsMapData()
        {
            SetCalculationsMapData();
            calculationsMapData.NotifyObservers();
        }

        private void SetCalculationsMapData()
        {
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations =
                data.WrappedData.CalculationsGroup.GetCalculations().Cast<GrassCoverErosionInwardsCalculation>();

            calculationsMapData.Features = GrassCoverErosionInwardsMapDataFeaturesFactory.CreateCalculationFeatures(calculations);
        }

        #endregion

        #region HydraulicBoundaryLocations MapData

        private void UpdateHydraulicBoundaryLocationsMapData()
        {
            SetHydraulicBoundaryLocationsMapData();
            hydraulicBoundaryLocationsMapData.NotifyObservers();
        }

        private void SetHydraulicBoundaryLocationsMapData()
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = data.Parent.HydraulicBoundaryDatabase;
            hydraulicBoundaryLocationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(hydraulicBoundaryDatabase);
        }

        #endregion

        #region ReferenceLine MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            ReferenceLine referenceLine = data.Parent.ReferenceLine;
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, data.Parent.Id, data.Parent.Name);
        }

        #endregion

        #region Sections MapData

        private void UpdateSectionsMapData()
        {
            SetSectionsMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();
        }

        private void SetSectionsMapData()
        {
            IEnumerable<FailureMechanismSection> failureMechanismSections = data.WrappedData.Sections;
            sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
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
            IEnumerable<DikeProfile> dikeProfiles = data.WrappedData.DikeProfiles;

            dikeProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateDikeProfilesFeatures(dikeProfiles);
            foreshoreProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(dikeProfiles.Select(dp => dp.ForeshoreProfile));
        }

        #endregion
    }
}