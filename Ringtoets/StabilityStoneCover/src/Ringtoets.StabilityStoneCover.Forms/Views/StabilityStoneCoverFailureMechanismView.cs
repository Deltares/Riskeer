// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using StabilityStoneCoverDataResources = Ringtoets.StabilityStoneCover.Data.Properties.Resources;
using StabilityStoneCoverFormsResources = Ringtoets.StabilityStoneCover.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a stability stone cover failure mechanism.
    /// </summary>
    public partial class StabilityStoneCoverFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer foreshoreProfilesObserver;

        private readonly RecursiveObserver<CalculationGroup, WaveConditionsInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, StabilityStoneCoverWaveConditionsCalculation> calculationObserver;

        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryDatabaseMapData;
        private readonly MapLineData foreshoreProfilesMapData;
        private readonly MapLineData calculationsMapData;

        private StabilityStoneCoverFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverFailureMechanismView"/>.
        /// </summary>
        public StabilityStoneCoverFailureMechanismView()
        {
            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateMapData);
            assessmentSectionObserver = new Observer(UpdateMapData);
            foreshoreProfilesObserver = new Observer(UpdateMapData);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, WaveConditionsInput>(
                UpdateMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<StabilityStoneCoverWaveConditionsCalculation>().Select(pc => pc.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateMapData, pcg => pcg.Children);
            calculationObserver = new RecursiveObserver<CalculationGroup, StabilityStoneCoverWaveConditionsCalculation>(UpdateMapData, pcg => pcg.Children);

            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryDatabaseMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryDatabaseMapData();
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
            mapControl.Data.Add(foreshoreProfilesMapData);
            mapControl.Data.Add(calculationsMapData);

            mapControl.Data.Name = StabilityStoneCoverDataResources.StabilityStoneCoverFailureMechanism_DisplayName;
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
                data = value as StabilityStoneCoverFailureMechanismContext;

                if (data == null)
                {
                    failureMechanismObserver.Observable = null;
                    assessmentSectionObserver.Observable = null;
                    foreshoreProfilesObserver.Observable = null;
                    calculationInputObserver.Observable = null;
                    calculationGroupObserver.Observable = null;
                    calculationObserver.Observable = null;
                }
                else
                {
                    failureMechanismObserver.Observable = data.WrappedData;
                    assessmentSectionObserver.Observable = data.Parent;
                    foreshoreProfilesObserver.Observable = data.WrappedData.ForeshoreProfiles;
                    calculationInputObserver.Observable = data.WrappedData.WaveConditionsCalculationGroup;
                    calculationGroupObserver.Observable = data.WrappedData.WaveConditionsCalculationGroup;
                    calculationObserver.Observable = data.WrappedData.WaveConditionsCalculationGroup;
                }

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
            if (data == null)
            {
                Map.ResetMapData();
            }
            else
            {
                ReferenceLine referenceLine = data.Parent.ReferenceLine;
                IEnumerable<FailureMechanismSection> failureMechanismSections = data.WrappedData.Sections;
                HydraulicBoundaryDatabase hydraulicBoundaryDatabase = data.Parent.HydraulicBoundaryDatabase;
                IEnumerable<ForeshoreProfile> foreshoreProfiles = data.WrappedData.ForeshoreProfiles;
                IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculations = 
                    data.WrappedData.WaveConditionsCalculationGroup.GetCalculations().Cast<StabilityStoneCoverWaveConditionsCalculation>();

                referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, data.Parent.Id, data.Parent.Name);
                sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
                sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
                sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
                hydraulicBoundaryDatabaseMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeaturesWithDefaultLabels(hydraulicBoundaryDatabase);
                foreshoreProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(foreshoreProfiles);
                calculationsMapData.Features = StabilityStoneCoverMapDataFeaturesFactory.CreateCalculationFeatures(calculations);

                mapControl.Data.NotifyObservers();
            }
        }
    }
}