﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Factories;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.Factories;
using HeightStructuresDataResources = Riskeer.HeightStructures.Data.Properties.Resources;

namespace Riskeer.HeightStructures.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a height structures failure path.
    /// </summary>
    public class HeightStructuresFailurePathView : HeightStructuresFailureMechanismView
    {
        private MapLineData sectionsMapData;
        private MapPointData sectionsStartPointMapData;
        private MapPointData sectionsEndPointMapData;

        private MapLineData simpleAssemblyMapData;
        private MapLineData detailedAssemblyMapData;
        private MapLineData tailorMadeAssemblyMapData;
        private MapLineData combinedAssemblyMapData;

        private Observer failureMechanismObserver;

        private RecursiveObserver<IObservableEnumerable<HeightStructuresFailureMechanismSectionResultOld>, HeightStructuresFailureMechanismSectionResultOld> sectionResultObserver;

        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresFailurePathView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HeightStructuresFailurePathView(HeightStructuresFailureMechanism failureMechanism,
                                               IAssessmentSection assessmentSection) : base(failureMechanism, assessmentSection) {}

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            sectionResultObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void CreateMapData()
        {
            base.CreateMapData();
            
            MapDataCollection sectionsMapDataCollection = RiskeerMapDataFactory.CreateSectionsMapDataCollection();
            sectionsMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsMapData();
            sectionsStartPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = RiskeerMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            MapDataCollection assemblyMapDataCollection = AssemblyMapDataFactory.CreateAssemblyMapDataCollection();
            tailorMadeAssemblyMapData = AssemblyMapDataFactory.CreateTailorMadeAssemblyMapData();
            detailedAssemblyMapData = AssemblyMapDataFactory.CreateDetailedAssemblyMapData();
            simpleAssemblyMapData = AssemblyMapDataFactory.CreateSimpleAssemblyMapData();
            combinedAssemblyMapData = AssemblyMapDataFactory.CreateCombinedAssemblyMapData();

            sectionsMapDataCollection.Add(sectionsMapData);
            sectionsMapDataCollection.Add(sectionsStartPointMapData);
            sectionsMapDataCollection.Add(sectionsEndPointMapData);
            MapDataCollection.Insert(1, sectionsMapDataCollection);

            assemblyMapDataCollection.Add(tailorMadeAssemblyMapData);
            assemblyMapDataCollection.Add(detailedAssemblyMapData);
            assemblyMapDataCollection.Add(simpleAssemblyMapData);
            assemblyMapDataCollection.Add(combinedAssemblyMapData);
            MapDataCollection.Insert(2, assemblyMapDataCollection);
        }

        protected override void CreateObservers()
        {
            base.CreateObservers();

            failureMechanismObserver = new Observer(UpdateFailureMechanismMapData)
            {
                Observable = FailureMechanism
            };

            sectionResultObserver = new RecursiveObserver<IObservableEnumerable<HeightStructuresFailureMechanismSectionResultOld>,
                HeightStructuresFailureMechanismSectionResultOld>(UpdateAssemblyMapData, sr => sr)
            {
                Observable = FailureMechanism.SectionResults
            };
        }

        protected override void SetAllMapDataFeatures()
        {
            base.SetAllMapDataFeatures();

            SetSectionsMapData();
            SetAssemblyMapData();
        }

        #region Calculations MapData

        protected override void UpdateCalculationsMapData()
        {
            base.UpdateCalculationsMapData();

            UpdateAssemblyMapData();
        }

        #endregion

        #region Assembly MapData

        private void UpdateAssemblyMapData()
        {
            SetAssemblyMapData();
            simpleAssemblyMapData.NotifyObservers();
            detailedAssemblyMapData.NotifyObservers();
            tailorMadeAssemblyMapData.NotifyObservers();
            combinedAssemblyMapData.NotifyObservers();
        }

        private void SetAssemblyMapData()
        {
            simpleAssemblyMapData.Features = HeightStructuresAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(FailureMechanism);
            detailedAssemblyMapData.Features = HeightStructuresAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(FailureMechanism, AssessmentSection);
            tailorMadeAssemblyMapData.Features = HeightStructuresAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(FailureMechanism, AssessmentSection);
            combinedAssemblyMapData.Features = HeightStructuresAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(FailureMechanism, AssessmentSection);
        }

        #endregion

        #region FailureMechanism MapData

        private void UpdateFailureMechanismMapData()
        {
            SetSectionsMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();

            UpdateAssemblyMapData();
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