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
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.Factories;
using DuneErosionDataResources = Riskeer.DuneErosion.Data.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a dune erosion failure path.
    /// </summary>
    public class DuneErosionFailurePathView : DuneErosionFailureMechanismView
    {
        private MapLineData sectionsMapData;
        private MapPointData sectionsStartPointMapData;
        private MapPointData sectionsEndPointMapData;

        private MapLineData simpleAssemblyMapData;
        private MapLineData detailedAssemblyMapData;
        private MapLineData tailorMadeAssemblyMapData;
        private MapLineData combinedAssemblyMapData;

        private Observer failureMechanismObserver;

        private RecursiveObserver<IObservableEnumerable<DuneErosionFailureMechanismSectionResult>, DuneErosionFailureMechanismSectionResult> sectionResultObserver;

        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionFailurePathView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section in which the <paramref name="failureMechanism"/>
        /// belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneErosionFailurePathView(DuneErosionFailureMechanism failureMechanism,
                                          IAssessmentSection assessmentSection) : base(failureMechanism, assessmentSection) {}

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();

            sectionResultObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void CreateObservers()
        {
            base.CreateObservers();

            failureMechanismObserver = new Observer(UpdateFailureMechanismMapData)
            {
                Observable = FailureMechanism
            };

            sectionResultObserver = new RecursiveObserver<IObservableEnumerable<DuneErosionFailureMechanismSectionResult>,
                DuneErosionFailureMechanismSectionResult>(UpdateAssemblyMapData, sr => sr)
            {
                Observable = FailureMechanism.SectionResults
            };
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

        protected override void SetAllMapDataFeatures()
        {
            base.SetAllMapDataFeatures();

            SetFailureMechanismMapData();
            SetAssemblyMapData();
        }

        #region FailureMechanism MapData

        private void UpdateFailureMechanismMapData()
        {
            SetFailureMechanismMapData();
            sectionsMapData.NotifyObservers();
            sectionsStartPointMapData.NotifyObservers();
            sectionsEndPointMapData.NotifyObservers();

            UpdateAssemblyMapData();
        }

        private void SetFailureMechanismMapData()
        {
            IEnumerable<FailureMechanismSection> failureMechanismSections = FailureMechanism.Sections;
            sectionsMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(failureMechanismSections);
            sectionsStartPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(failureMechanismSections);
            sectionsEndPointMapData.Features = RiskeerMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(failureMechanismSections);
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
            simpleAssemblyMapData.Features = DuneErosionAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(FailureMechanism);
            detailedAssemblyMapData.Features = DuneErosionAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(FailureMechanism);
            tailorMadeAssemblyMapData.Features = DuneErosionAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(FailureMechanism);
            combinedAssemblyMapData.Features = DuneErosionAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(FailureMechanism);
        }

        #endregion
    }
}