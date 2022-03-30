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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Factories;

namespace Riskeer.Common.Forms.MapLayers
{
    /// <summary>
    /// Map layer to show section results.
    /// </summary>
    /// <typeparam name="TSectionResult">The type of section result.</typeparam>
    public class NonCalculatableFailureMechanismSectionResultsMapLayer<TSectionResult> : IDisposable
        where TSectionResult : FailureMechanismSectionResult
    {
        private readonly Func<TSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc;

        private readonly IFailureMechanism<TSectionResult> failureMechanism;

        private Observer failureMechanismObserver;
        private RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult> sectionResultObserver;

        /// <summary>
        /// Creates a new instance of <see cref="NonCalculatableFailureMechanismSectionResultsMapLayer{TSectionResult}"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to get the data from.</param>
        /// <param name="performAssemblyFunc">The <see cref="Func{T1,TResult}"/> used to assemble the result of a section result.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public NonCalculatableFailureMechanismSectionResultsMapLayer(
            IFailureMechanism<TSectionResult> failureMechanism,
            Func<TSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (performAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(performAssemblyFunc));
            }

            this.failureMechanism = failureMechanism;
            this.performAssemblyFunc = performAssemblyFunc;

            CreateObservers();

            MapData = AssemblyMapDataFactory.CreateFailureMechanismSectionAssemblyMapData();
            SetFeatures();
        }

        /// <summary>
        /// Gets the section results map data.
        /// </summary>
        public MapLineData MapData { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                failureMechanismObserver.Dispose();
                sectionResultObserver.Dispose();
            }
        }

        protected void UpdateFeatures()
        {
            SetFeatures();
            MapData.NotifyObservers();
        }

        private void CreateObservers()
        {
            failureMechanismObserver = new Observer(UpdateFeatures)
            {
                Observable = failureMechanism
            };
            sectionResultObserver = new RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult>(UpdateFeatures, sr => sr)
            {
                Observable = failureMechanism.SectionResults
            };
        }

        private void SetFeatures()
        {
            MapData.Features = AssemblyMapDataFeaturesFactory.CreateAssemblyGroupFeatures(failureMechanism, performAssemblyFunc);
        }
    }
}