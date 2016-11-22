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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a grass cover erosion inwards failure mechanism.
    /// </summary>
    public partial class GrassCoverErosionInwardsFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer dikeProfilesObserver;

        private readonly RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculation> calculationObserver;

        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryDatabaseMapData;
        private readonly MapLineData dikeProfilesMapData;
        private readonly MapLineData foreshoreProfilesMapData;
        private readonly MapLineData calculationsMapData;

        private GrassCoverErosionInwardsFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismView"/>.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanismView()
        {
            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateMapData);
            assessmentSectionObserver = new Observer(UpdateMapData);
            dikeProfilesObserver = new Observer(UpdateMapData);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsInput>(
                UpdateMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<GrassCoverErosionInwardsCalculation>().Select(pc => pc.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateMapData, pcg => pcg.Children);
            calculationObserver = new RecursiveObserver<CalculationGroup, GrassCoverErosionInwardsCalculation>(UpdateMapData, pcg => pcg.Children);

            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryDatabaseMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryDatabaseMapData();
            dikeProfilesMapData = RingtoetsMapDataFactory.CreateDikeProfileMapData();
            foreshoreProfilesMapData = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();

            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            calculationsMapData = RingtoetsMapDataFactory.CreateCalculationsMapData();

            mapControl.Data.Add(referenceLineMapData);
            mapControl.Data.Add(sectionsMapData);
            mapControl.Data.Add(sectionsStartPointMapData);
            mapControl.Data.Add(sectionsEndPointMapData);
            mapControl.Data.Add(hydraulicBoundaryDatabaseMapData);
            mapControl.Data.Add(dikeProfilesMapData);
            mapControl.Data.Add(foreshoreProfilesMapData);
            mapControl.Data.Add(calculationsMapData);

            mapControl.Data.Name = GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsFailureMechanism_DisplayName;
            mapControl.Data.NotifyObservers();
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
                    dikeProfilesObserver.Observable = null;
                    calculationInputObserver.Observable = null;
                    calculationGroupObserver.Observable = null;
                    calculationObserver.Observable = null;

                    Map.ResetMapData();
                    return;
                }

                failureMechanismObserver.Observable = data.WrappedData;
                assessmentSectionObserver.Observable = data.Parent;
                dikeProfilesObserver.Observable = data.WrappedData.DikeProfiles;
                calculationInputObserver.Observable = data.WrappedData.CalculationsGroup;
                calculationGroupObserver.Observable = data.WrappedData.CalculationsGroup;
                calculationObserver.Observable = data.WrappedData.CalculationsGroup;

                UpdateMapData();
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
            dikeProfilesObserver.Dispose();
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
            ReferenceLine referenceLine = null;
            IEnumerable<FailureMechanismSection> failureMechanismSections = null;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = null;
            IEnumerable<DikeProfile> dikeProfiles = null;
            CalculationGroup calculationGroup = null;

            if (data != null)
            {
                referenceLine = data.Parent.ReferenceLine;
                failureMechanismSections = data.WrappedData.Sections;
                hydraulicBoundaryDatabase = data.Parent.HydraulicBoundaryDatabase;
                dikeProfiles = data.WrappedData.DikeProfiles;
                calculationGroup = data.WrappedData.CalculationsGroup;
            }

            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, data.Parent.Id, data.Parent.Name);
            sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
            hydraulicBoundaryDatabaseMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeaturesWithDefaultLabels(hydraulicBoundaryDatabase);
            dikeProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateDikeProfilesFeatures(dikeProfiles);
            foreshoreProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(dikeProfiles.Select(dp => dp.ForeshoreProfile));
            calculationsMapData.Features = GrassCoverErosionInwardsMapDataFeaturesFactory.CreateCalculationFeatures(calculationGroup.GetCalculations().Cast<GrassCoverErosionInwardsCalculation>());

            mapControl.Data.NotifyObservers();
        }
    }
}