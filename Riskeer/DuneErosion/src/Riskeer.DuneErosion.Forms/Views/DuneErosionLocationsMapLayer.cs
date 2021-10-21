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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Riskeer.Common.Forms.Factories;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.Factories;
using Riskeer.DuneErosion.Forms.Properties;
using RiskeerCommonUtilResources = Riskeer.Common.Util.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.Views
{
    /// <summary>
    /// Map layer to show dune erosion locations.
    /// </summary>
    public class DuneErosionLocationsMapLayer : IDisposable
    {
        private readonly DuneErosionFailureMechanism failureMechanism;

        private Observer duneLocationsObserver;
        private Observer userDefinedTargetProbabilitiesCollectionObserver;
        private RecursiveObserver<IObservableEnumerable<DuneLocationCalculationsForTargetProbability>, DuneLocationCalculationsForTargetProbability> userDefinedTargetProbabilitiesObserver;

        private List<RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>> calculationsForTargetProbabilityObservers;

        private IReadOnlyDictionary<DuneLocationCalculationsForTargetProbability, MetaDataItemsLookup> currentMetaDataItemsLookups;

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
            currentMetaDataItemsLookups = new Dictionary<DuneLocationCalculationsForTargetProbability, MetaDataItemsLookup>();
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
                userDefinedTargetProbabilitiesCollectionObserver.Dispose();
                userDefinedTargetProbabilitiesObserver.Dispose();

                DeleteTargetProbabilitiesObservers();
            }
        }

        private void DeleteTargetProbabilitiesObservers()
        {
            foreach (RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> observer in calculationsForTargetProbabilityObservers)
            {
                observer.Dispose();
            }

            calculationsForTargetProbabilityObservers.Clear();
        }

        private void CreateObservers()
        {
            duneLocationsObserver = new Observer(UpdateFeatures)
            {
                Observable = failureMechanism.DuneLocations
            };
            userDefinedTargetProbabilitiesCollectionObserver = new Observer(() =>
            {
                DeleteTargetProbabilitiesObservers();
                CreateTargetProbabilitiesObservers();
                UpdateFeatures();
            })
            {
                Observable = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities
            };

            userDefinedTargetProbabilitiesObserver = new RecursiveObserver<IObservableEnumerable<DuneLocationCalculationsForTargetProbability>, DuneLocationCalculationsForTargetProbability>(
                UpdateFeatures, tp => tp)
            {
                Observable = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities
            };

            calculationsForTargetProbabilityObservers = new List<RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>>();
            CreateTargetProbabilitiesObservers();
        }

        private void CreateTargetProbabilitiesObservers()
        {
            calculationsForTargetProbabilityObservers.AddRange(failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities
                                                                               .Select(calculationsForTargetProbability => CreateDuneLocationCalculationsObserver(
                                                                                           calculationsForTargetProbability.DuneLocationCalculations))
                                                                               .ToArray());
        }

        private RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> CreateDuneLocationCalculationsObserver(
            IObservableEnumerable<DuneLocationCalculation> calculations)
        {
            return new RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>(
                UpdateFeatures, calc => calc)
            {
                Observable = calculations
            };
        }

        private void UpdateFeatures()
        {
            SetFeatures();
            MapData.NotifyObservers();
        }

        private void SetFeatures()
        {
            DuneLocationCalculationsForTargetProbability[] orderedCalculationsForTargetProbabilities = failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities
                                                                                                                       .OrderByDescending(tp => tp.TargetProbability)
                                                                                                                       .ToArray();
            IEnumerable<AggregatedDuneLocation> locations = AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(
                failureMechanism.DuneLocations, orderedCalculationsForTargetProbabilities);

            MapData.Features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(locations);

            if (MapData.Features.Any())
            {
                UpdateMetaData(orderedCalculationsForTargetProbabilities);
            }
        }

        private void UpdateMetaData(IEnumerable<DuneLocationCalculationsForTargetProbability> calculationsForTargetProbabilities)
        {
            var newMetaDataItemsLookups = new Dictionary<DuneLocationCalculationsForTargetProbability, MetaDataItemsLookup>();

            var waterLevelMetaDataItemsCounter = 0;
            var waveHeightMetaDataItemsCounter = 0;
            var wavePeriodMetaDataItemsCounter = 0;
            foreach (string metaData in MapData.MetaData)
            {
                if (metaData.Contains(string.Format(Resources.MetaData_WaterLevel_TargetProbability_0, string.Empty)))
                {
                    AddMetaDataItemToLookup(newMetaDataItemsLookups, calculationsForTargetProbabilities.ElementAt(waterLevelMetaDataItemsCounter),
                                            lookupItem => lookupItem.WaterLevelMetaDataItem = metaData);
                    waterLevelMetaDataItemsCounter++;
                }
                else if (metaData.Contains(string.Format(Resources.MetaData_WaveHeight_TargetProbability_0, string.Empty)))
                {
                    AddMetaDataItemToLookup(newMetaDataItemsLookups, calculationsForTargetProbabilities.ElementAt(waveHeightMetaDataItemsCounter),
                                            lookupItem => lookupItem.WaveHeightMetaDataItem = metaData);
                    waveHeightMetaDataItemsCounter++;
                }
                else if (metaData.Contains(string.Format(Resources.MetaData_WavePeriod_TargetProbability_0, string.Empty)))
                {
                    AddMetaDataItemToLookup(newMetaDataItemsLookups, calculationsForTargetProbabilities.ElementAt(wavePeriodMetaDataItemsCounter),
                                            lookupItem => lookupItem.WavePeriodMetaDataItem = metaData);
                    wavePeriodMetaDataItemsCounter++;
                }
            }

            string currentSelectedMetaDataAttribute = MapData.SelectedMetaDataAttribute;
            foreach (KeyValuePair<DuneLocationCalculationsForTargetProbability, MetaDataItemsLookup> currentMetaDataItemLookup in currentMetaDataItemsLookups)
            {
                MetaDataItemsLookup currentMetaDataItem = currentMetaDataItemLookup.Value;
                if (currentSelectedMetaDataAttribute == currentMetaDataItem.WaterLevelMetaDataItem
                    || currentSelectedMetaDataAttribute == currentMetaDataItem.WaveHeightMetaDataItem
                    || currentSelectedMetaDataAttribute == currentMetaDataItem.WavePeriodMetaDataItem)
                {
                    DuneLocationCalculationsForTargetProbability calculationsForTargetProbability = currentMetaDataItemLookup.Key;
                    if (!newMetaDataItemsLookups.ContainsKey(calculationsForTargetProbability))
                    {
                        MapData.SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name;
                    }
                    else
                    {
                        MetaDataItemsLookup newMetaDataItem = newMetaDataItemsLookups[calculationsForTargetProbability];
                        SetSelectedMetaDataAttribute(currentSelectedMetaDataAttribute, currentMetaDataItem.WaterLevelMetaDataItem, newMetaDataItem.WaterLevelMetaDataItem);
                        SetSelectedMetaDataAttribute(currentSelectedMetaDataAttribute, currentMetaDataItem.WaveHeightMetaDataItem, newMetaDataItem.WaveHeightMetaDataItem);
                        SetSelectedMetaDataAttribute(currentSelectedMetaDataAttribute, currentMetaDataItem.WavePeriodMetaDataItem, newMetaDataItem.WavePeriodMetaDataItem);
                    }
                }
            }

            currentMetaDataItemsLookups = newMetaDataItemsLookups;
        }

        private static void AddMetaDataItemToLookup(IDictionary<DuneLocationCalculationsForTargetProbability, MetaDataItemsLookup> lookup,
                                                    DuneLocationCalculationsForTargetProbability calculationsForTargetProbability,
                                                    Action<MetaDataItemsLookup> setMetaDataAction)
        {
            if (!lookup.ContainsKey(calculationsForTargetProbability))
            {
                lookup.Add(calculationsForTargetProbability, new MetaDataItemsLookup());
            }

            setMetaDataAction(lookup[calculationsForTargetProbability]);
        }

        private void SetSelectedMetaDataAttribute(string currentSelectedMetaDataItem,
                                                  string currentMetaDataItem,
                                                  string newMetaDataItem)
        {
            if (currentMetaDataItem == currentSelectedMetaDataItem && currentMetaDataItem != newMetaDataItem)
            {
                MapData.SelectedMetaDataAttribute = newMetaDataItem;
            }
        }

        private class MetaDataItemsLookup
        {
            public string WaterLevelMetaDataItem { get; set; }

            public string WaveHeightMetaDataItem { get; set; }

            public string WavePeriodMetaDataItem { get; set; }
        }
    }
}