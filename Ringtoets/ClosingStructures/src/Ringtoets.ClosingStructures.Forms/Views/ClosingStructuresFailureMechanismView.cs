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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Factories;
using ClosingStructuresDataResources = Ringtoets.ClosingStructures.Data.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a closing structures failure mechanism.
    /// </summary>
    public partial class ClosingStructuresFailureMechanismView : UserControl, IMapView
    {
        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;
        private readonly MapLineData foreshoreProfilesMapData;
        private readonly MapPointData structuresMapData;
        private readonly MapLineData calculationsMapData;
        private Observer failureMechanismObserver;
        private Observer assessmentSectionObserver;
        private Observer hydraulicBoundaryLocationsObserver;
        private Observer foreshoreProfilesObserver;
        private Observer structuresObserver;

        private RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation> hydraulicBoundaryLocationObserver;
        private RecursiveObserver<CalculationGroup, ClosingStructuresInput> calculationInputObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, StructuresCalculation<ClosingStructuresInput>> calculationObserver;
        private RecursiveObserver<ForeshoreProfileCollection, ForeshoreProfile> foreshoreProfileObserver;
        private RecursiveObserver<StructureCollection<ClosingStructure>, ClosingStructure> structureObserver;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClosingStructuresFailureMechanismView(ClosingStructuresFailureMechanism failureMechanism,
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

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;

            InitializeComponent();

            CreateObservers();

            var mapDataCollection = new MapDataCollection(ClosingStructuresDataResources.ClosingStructuresFailureMechanism_DisplayName);
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            foreshoreProfilesMapData = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();
            structuresMapData = RingtoetsMapDataFactory.CreateStructuresMapData();
            sectionsMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            calculationsMapData = RingtoetsMapDataFactory.CreateCalculationsMapData();

            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(sectionsMapData);
            mapDataCollection.Add(sectionsStartPointMapData);
            mapDataCollection.Add(sectionsEndPointMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);
            mapDataCollection.Add(foreshoreProfilesMapData);
            mapDataCollection.Add(structuresMapData);
            mapDataCollection.Add(calculationsMapData);

            SetMapDataFeatures();
            ringtoetsMapControl.SetAllData(mapDataCollection, AssessmentSection.BackgroundData);
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public ClosingStructuresFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

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
            foreshoreProfilesObserver.Dispose();
            foreshoreProfileObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();
            structuresObserver.Dispose();
            structureObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CreateObservers()
        {
            failureMechanismObserver = new Observer(UpdateSectionsMapData)
            {
                Observable = FailureMechanism
            };
            assessmentSectionObserver = new Observer(UpdateReferenceLineMapData)
            {
                Observable = AssessmentSection
            };
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocationsMapData)
            {
                Observable = AssessmentSection.HydraulicBoundaryDatabase.Locations
            };
            foreshoreProfilesObserver = new Observer(UpdateForeshoreProfilesMapData)
            {
                Observable = FailureMechanism.ForeshoreProfiles
            };
            structuresObserver = new Observer(UpdateStructuresMapData)
            {
                Observable = FailureMechanism.ClosingStructures
            };

            hydraulicBoundaryLocationObserver = new RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation>(
                UpdateHydraulicBoundaryLocationsMapData, hbl => hbl)
            {
                Observable = AssessmentSection.HydraulicBoundaryDatabase.Locations
            };
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ClosingStructuresInput>(
                UpdateCalculationsMapData, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<StructuresCalculation<ClosingStructuresInput>>()
                                                                     .Select(pc => pc.InputParameters)))
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            calculationObserver = new RecursiveObserver<CalculationGroup, StructuresCalculation<ClosingStructuresInput>>(UpdateCalculationsMapData, pcg => pcg.Children)
            {
                Observable = FailureMechanism.CalculationsGroup
            };
            foreshoreProfileObserver = new RecursiveObserver<ForeshoreProfileCollection, ForeshoreProfile>(UpdateForeshoreProfilesMapData, coll => coll)
            {
                Observable = FailureMechanism.ForeshoreProfiles
            };
            structureObserver = new RecursiveObserver<StructureCollection<ClosingStructure>, ClosingStructure>(UpdateStructuresMapData, coll => coll)
            {
                Observable = FailureMechanism.ClosingStructures
            };
        }

        private void SetMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetSectionsMapData();
            SetHydraulicBoundaryLocationsMapData();
            SetForeshoreProfilesMapData();
            SetStructuresMapData();
            SetCalculationsMapData();
        }

        #region Calculations MapData

        private void UpdateCalculationsMapData()
        {
            SetCalculationsMapData();
            calculationsMapData.NotifyObservers();
        }

        private void SetCalculationsMapData()
        {
            IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations =
                FailureMechanism.CalculationsGroup.GetCalculations().Cast<StructuresCalculation<ClosingStructuresInput>>();
            calculationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateStructureCalculationsFeatures<ClosingStructuresInput, ClosingStructure>(calculations);
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
            hydraulicBoundaryLocationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(AssessmentSection.HydraulicBoundaryDatabase);
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
            ReferenceLine referenceLine = AssessmentSection.ReferenceLine;
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, AssessmentSection.Id, AssessmentSection.Name);
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
            IEnumerable<FailureMechanismSection> failureMechanismSections = FailureMechanism.Sections;

            sectionsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
        }

        #endregion

        #region Structures MapData

        private void UpdateStructuresMapData()
        {
            SetStructuresMapData();
            structuresMapData.NotifyObservers();
        }

        private void SetStructuresMapData()
        {
            IEnumerable<ClosingStructure> structures = FailureMechanism.ClosingStructures;
            structuresMapData.Features = RingtoetsMapDataFeaturesFactory.CreateStructuresFeatures(structures);
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
            foreshoreProfilesMapData.Features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(foreshoreProfiles);
        }

        #endregion
    }
}