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
using Riskeer.Common.Forms.Factories;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.Factories;

namespace Riskeer.DuneErosion.Forms.Views
{
    /// <summary>
    /// Map layer to show dune erosion locations.
    /// </summary>
    public class DuneErosionLocationsMapLayer : IDisposable
    {
        private readonly DuneErosionFailureMechanism failureMechanism;

        private Observer duneLocationsObserver;
        private Observer userDefinedTargetProbabilitiesListObserver;

        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionLocationsMapLayer"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/>
        /// to get the data from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public DuneErosionLocationsMapLayer(DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            this.failureMechanism = failureMechanism;

            CreateObservers();

            MapData = RiskeerMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            SetFeatures();
        }

        /// <summary>
        /// Gets the dune erosion locations map data.
        /// </summary>
        public MapPointData MapData { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                duneLocationsObserver.Dispose();
                userDefinedTargetProbabilitiesListObserver.Dispose();
            }
        }

        private void CreateObservers()
        {
            duneLocationsObserver = new Observer(UpdateFeatures)
            {
                Observable = failureMechanism.DuneLocations
            };
            userDefinedTargetProbabilitiesListObserver = new Observer(UpdateFeatures)
            {
                Observable = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities
            };
        }

        private void UpdateFeatures()
        {
            SetFeatures();
            MapData.NotifyObservers();
        }

        private void SetFeatures()
        {
            MapData.Features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(failureMechanism);
        }
    }
}