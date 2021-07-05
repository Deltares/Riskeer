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
using Core.Common.Base;
using Core.Components.Gis.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Factories;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.Factories;
using PipingDataResources = Riskeer.Piping.Data.Properties.Resources;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a piping failure path.
    /// </summary>
    public class PipingFailurePathView : PipingFailureMechanismView
    {
        private MapLineData simpleAssemblyMapData;
        private MapLineData detailedAssemblyMapData;
        private MapLineData tailorMadeAssemblyMapData;
        private MapLineData combinedAssemblyMapData;

        private RecursiveObserver<IObservableEnumerable<PipingFailureMechanismSectionResult>, PipingFailureMechanismSectionResult> sectionResultObserver;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailurePathView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailurePathView(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection) : base(failureMechanism, assessmentSection) {}

        protected override void Dispose(bool disposing)
        {
            sectionResultObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void CreateMapData()
        {
            base.CreateMapData();

            MapDataCollection assemblyMapDataCollection = AssemblyMapDataFactory.CreateAssemblyMapDataCollection();
            tailorMadeAssemblyMapData = AssemblyMapDataFactory.CreateTailorMadeAssemblyMapData();
            detailedAssemblyMapData = AssemblyMapDataFactory.CreateDetailedAssemblyMapData();
            simpleAssemblyMapData = AssemblyMapDataFactory.CreateSimpleAssemblyMapData();
            combinedAssemblyMapData = AssemblyMapDataFactory.CreateCombinedAssemblyMapData();

            assemblyMapDataCollection.Add(tailorMadeAssemblyMapData);
            assemblyMapDataCollection.Add(detailedAssemblyMapData);
            assemblyMapDataCollection.Add(simpleAssemblyMapData);
            assemblyMapDataCollection.Add(combinedAssemblyMapData);
            MapDataCollection.Insert(4, assemblyMapDataCollection);
        }

        protected override void CreateObservers()
        {
            base.CreateObservers();

            sectionResultObserver = new RecursiveObserver<IObservableEnumerable<PipingFailureMechanismSectionResult>, PipingFailureMechanismSectionResult>(UpdateAssemblyMapData, sr => sr)
            {
                Observable = FailureMechanism.SectionResults
            };
        }

        protected override void SetAllMapDataFeatures()
        {
            base.SetAllMapDataFeatures();

            SetAssemblyMapData();
        }

        protected override void UpdateSemiProbabilisticCalculationsMapData()
        {
            base.UpdateSemiProbabilisticCalculationsMapData();

            UpdateAssemblyMapData();
        }

        protected override void UpdateFailureMechanismMapData()
        {
            base.UpdateFailureMechanismMapData();

            UpdateAssemblyMapData();
        }

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
            simpleAssemblyMapData.Features = PipingAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(FailureMechanism);
            detailedAssemblyMapData.Features = PipingAssemblyMapDataFeaturesFactory.CreateDetailedAssemblyFeatures(FailureMechanism, AssessmentSection);
            tailorMadeAssemblyMapData.Features = PipingAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(FailureMechanism, AssessmentSection);
            combinedAssemblyMapData.Features = PipingAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(FailureMechanism, AssessmentSection);
        }

        #endregion
    }
}