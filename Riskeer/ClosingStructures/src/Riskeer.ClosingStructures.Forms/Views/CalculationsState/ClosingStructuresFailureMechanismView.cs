﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.MapLayers;
using ClosingStructuresDataResources = Riskeer.ClosingStructures.Data.Properties.Resources;

namespace Riskeer.ClosingStructures.Forms.Views.CalculationsState
{
    /// <summary>
    /// Calculations state view showing map data for a closing structures failure mechanism.
    /// </summary>
    public partial class ClosingStructuresFailureMechanismView : UserControl, IMapView
    {
        private HydraulicBoundaryLocationsMapLayer hydraulicBoundaryLocationsMapLayer;

        private MapLineData referenceLineMapData;
        private MapLineData foreshoreProfilesMapData;
        private MapPointData structuresMapData;
        private MapLineData calculationsMapData;

        private Observer assessmentSectionObserver;
        private Observer referenceLineObserver;
        private Observer foreshoreProfilesObserver;
        private Observer structuresObserver;

        private RecursiveObserver<CalculationGroup, ClosingStructuresInput> calculationInputObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, StructuresCalculation<ClosingStructuresInput>> calculationObserver;
        private RecursiveObserver<ForeshoreProfileCollection, ForeshoreProfile> foreshoreProfileObserver;
        private RecursiveObserver<StructureCollection<ClosingStructure>, ClosingStructure> structureObserver;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
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

            InitializeComponent();

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
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

        public IMapControl Map => riskeerMapControl.MapControl;

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
            structuresObserver.Dispose();
            structureObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void CreateMapData()
        {
            hydraulicBoundaryLocationsMapLayer = new HydraulicBoundaryLocationsMapLayer(AssessmentSection);

            MapDataCollection = new MapDataCollection(ClosingStructuresDataResources.ClosingStructuresFailureMechanism_DisplayName);
            referenceLineMapData = RiskeerMapDataFactory.CreateReferenceLineMapData();
            foreshoreProfilesMapData = RiskeerMapDataFactory.CreateForeshoreProfileMapData();
            calculationsMapData = RiskeerMapDataFactory.CreateCalculationsMapData();
            structuresMapData = RiskeerMapDataFactory.CreateStructuresMapData();

            MapDataCollection.Add(referenceLineMapData);
            MapDataCollection.Add(hydraulicBoundaryLocationsMapLayer.MapData);
            MapDataCollection.Add(foreshoreProfilesMapData);
            MapDataCollection.Add(structuresMapData);
            MapDataCollection.Add(calculationsMapData);
        }

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
            foreshoreProfilesObserver = new Observer(UpdateForeshoreProfilesMapData)
            {
                Observable = FailureMechanism.ForeshoreProfiles
            };
            structuresObserver = new Observer(UpdateStructuresMapData)
            {
                Observable = FailureMechanism.ClosingStructures
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

        protected virtual void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
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
            calculationsMapData.Features = RiskeerMapDataFeaturesFactory.CreateStructureCalculationsFeatures<ClosingStructuresInput, ClosingStructure>(calculations);
        }

        #endregion

        #region AssessmentSection MapData

        private void UpdateReferenceLineMapData()
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

        #region Structures MapData

        private void UpdateStructuresMapData()
        {
            SetStructuresMapData();
            structuresMapData.NotifyObservers();
        }

        private void SetStructuresMapData()
        {
            IEnumerable<ClosingStructure> structures = FailureMechanism.ClosingStructures;
            structuresMapData.Features = RiskeerMapDataFeaturesFactory.CreateStructuresFeatures(structures);
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