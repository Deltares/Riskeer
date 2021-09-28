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

        private ISet<MetaDataItemsLookup> currentMetaDataItems;

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
            currentMetaDataItems = new HashSet<MetaDataItemsLookup>();
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
            IReadOnlyDictionary<IObservableEnumerable<DuneLocationCalculation>, double> calculationsForTargetProbabilities =
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities
                                .OrderByDescending(tp => tp.TargetProbability)
                                .ToDictionary(tp => (IObservableEnumerable<DuneLocationCalculation>) tp.DuneLocationCalculations,
                                              tp => tp.TargetProbability);

            IEnumerable<AggregatedDuneLocation> locations = AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(
                failureMechanism.DuneLocations, calculationsForTargetProbabilities);

            MapData.Features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(locations);

            if (MapData.Features.Any())
            {
                UpdateMetaData(calculationsForTargetProbabilities);
            }
        }

        private void UpdateMetaData(IReadOnlyDictionary<IObservableEnumerable<DuneLocationCalculation>, double> calculationsForTargetProbabilities)
        {
            var newMetaDataItems = new HashSet<MetaDataItemsLookup>();

            var waterLevelMetaDataItemsCounter = 0;
            var waveHeightMetaDataItemsCounter = 0;
            var wavePeriodMetaDataItemsCounter = 0;
            foreach (string metaData in MapData.MetaData)
            {
                if (metaData.Contains(string.Format(Resources.MetaData_WaterLevel_TargetProbability_0, string.Empty)))
                {
                    AddMetaDataItemToLookup(newMetaDataItems, calculationsForTargetProbabilities.ElementAt(waterLevelMetaDataItemsCounter).Key,
                                            lookupItem => lookupItem.WaterLevelMetaDataItem = metaData);
                    waterLevelMetaDataItemsCounter++;
                }
                else if (metaData.Contains(string.Format(Resources.MetaData_WaveHeight_TargetProbability_0, string.Empty)))
                {
                    AddMetaDataItemToLookup(newMetaDataItems, calculationsForTargetProbabilities.ElementAt(waveHeightMetaDataItemsCounter).Key,
                                            lookupItem => lookupItem.WaveHeightMetaDataItem = metaData);
                    waveHeightMetaDataItemsCounter++;
                }
                else if (metaData.Contains(string.Format(Resources.MetaData_WavePeriod_TargetProbability_0, string.Empty)))
                {
                    AddMetaDataItemToLookup(newMetaDataItems, calculationsForTargetProbabilities.ElementAt(wavePeriodMetaDataItemsCounter).Key,
                                            lookupItem => lookupItem.WavePeriodMetaDataItem = metaData);
                    wavePeriodMetaDataItemsCounter++;
                }
            }

            foreach (MetaDataItemsLookup currentMetaDataItem in currentMetaDataItems)
            {
                if (MapData.SelectedMetaDataAttribute == currentMetaDataItem.WaterLevelMetaDataItem
                    || MapData.SelectedMetaDataAttribute == currentMetaDataItem.WaveHeightMetaDataItem
                    || MapData.SelectedMetaDataAttribute == currentMetaDataItem.WavePeriodMetaDataItem)
                {
                    MetaDataItemsLookup newMetaDataItem = newMetaDataItems.FirstOrDefault(i => i.Calculations.Equals(currentMetaDataItem.Calculations));

                    if (newMetaDataItem == null)
                    {
                        MapData.SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name;
                    }
                    else
                    {
                        SetSelectedMetaDataAttribute(currentMetaDataItem.WaterLevelMetaDataItem, newMetaDataItem.WaterLevelMetaDataItem);
                        SetSelectedMetaDataAttribute(currentMetaDataItem.WaveHeightMetaDataItem, newMetaDataItem.WaveHeightMetaDataItem);
                        SetSelectedMetaDataAttribute(currentMetaDataItem.WavePeriodMetaDataItem, newMetaDataItem.WavePeriodMetaDataItem);
                    }
                }
            }

            currentMetaDataItems = newMetaDataItems;
        }

        private static void AddMetaDataItemToLookup(ISet<MetaDataItemsLookup> lookup, IObservableEnumerable<DuneLocationCalculation> calculations,
                                                    Action<MetaDataItemsLookup> setMetaDataAction)
        {
            MetaDataItemsLookup lookupItem = lookup.FirstOrDefault(i => i.Calculations.Equals(calculations));

            if (lookupItem == null)
            {
                lookupItem = new MetaDataItemsLookup
                {
                    Calculations = calculations
                };
                lookup.Add(lookupItem);
            }

            setMetaDataAction(lookupItem);
        }

        private void SetSelectedMetaDataAttribute(string metaDataItem, string newMetaDataItem)
        {
            if (metaDataItem != newMetaDataItem && metaDataItem == MapData.SelectedMetaDataAttribute)
            {
                MapData.SelectedMetaDataAttribute = newMetaDataItem;
            }
        }

        private class MetaDataItemsLookup
        {
            public IObservableEnumerable<DuneLocationCalculation> Calculations { get; set; }

            public string WaterLevelMetaDataItem { get; set; }

            public string WaveHeightMetaDataItem { get; set; }

            public string WavePeriodMetaDataItem { get; set; }
        }
    }
}