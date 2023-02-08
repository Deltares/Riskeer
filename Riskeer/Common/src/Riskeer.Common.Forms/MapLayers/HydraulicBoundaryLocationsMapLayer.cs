﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Components.Gis.Features;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;
using RiskeerCommonUtilResources = Riskeer.Common.Util.Properties.Resources;

namespace Riskeer.Common.Forms.MapLayers
{
    /// <summary>
    /// Map layer to show hydraulic boundary locations.
    /// </summary>
    public class HydraulicBoundaryLocationsMapLayer : IDisposable
    {
        private readonly IAssessmentSection assessmentSection;

        private Observer failureMechanismContributionObserver;

        private Observer hydraulicBoundaryLocationsObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalFloodingProbabilityObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForMaximumAllowableFloodingProbabilityObserver;

        private Observer waterLevelForUserDefinedTargetProbabilitiesCollectionObserver;
        private Observer waveHeightForUserDefinedTargetProbabilitiesCollectionObserver;

        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability>, HydraulicBoundaryLocationCalculationsForTargetProbability> waterLevelForUserDefinedTargetProbabilitiesObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability>, HydraulicBoundaryLocationCalculationsForTargetProbability> waveHeightForUserDefinedTargetProbabilitiesObserver;

        private List<RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>> waterLevelCalculationsForTargetProbabilityObservers;
        private List<RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>> waveHeightCalculationsForTargetProbabilityObservers;

        private IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> currentMetaDataItems;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsMapLayer"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to get the data from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public HydraulicBoundaryLocationsMapLayer(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;

            CreateObservers();

            MapData = RiskeerMapDataFactory.CreateHydraulicBoundaryLocationsMapData();
            currentMetaDataItems = new Dictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string>();
            CurrentFeatures = new Dictionary<MapFeature, AggregatedHydraulicBoundaryLocation>();
            SetFeatures();
        }

        /// <summary>
        /// Gets the hydraulic boundary locations map data.
        /// </summary>
        public MapPointData MapData { get; }

        public IReadOnlyDictionary<MapFeature, AggregatedHydraulicBoundaryLocation> CurrentFeatures { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                failureMechanismContributionObserver.Dispose();
                hydraulicBoundaryLocationsObserver.Dispose();
                waterLevelCalculationsForSignalFloodingProbabilityObserver.Dispose();
                waterLevelCalculationsForMaximumAllowableFloodingProbabilityObserver.Dispose();
                waterLevelForUserDefinedTargetProbabilitiesCollectionObserver.Dispose();
                waveHeightForUserDefinedTargetProbabilitiesCollectionObserver.Dispose();
                waterLevelForUserDefinedTargetProbabilitiesObserver.Dispose();
                waveHeightForUserDefinedTargetProbabilitiesObserver.Dispose();

                DeleteTargetProbabilitiesObservers(waterLevelCalculationsForTargetProbabilityObservers);
                DeleteTargetProbabilitiesObservers(waveHeightCalculationsForTargetProbabilityObservers);
            }
        }

        private void CreateObservers()
        {
            failureMechanismContributionObserver = new Observer(UpdateFeatures)
            {
                Observable = assessmentSection.FailureMechanismContribution
            };

            hydraulicBoundaryLocationsObserver = new Observer(UpdateFeatures)
            {
                Observable = assessmentSection.HydraulicBoundaryDatabase.Locations
            };

            waterLevelCalculationsForSignalFloodingProbabilityObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                assessmentSection.WaterLevelCalculationsForSignalFloodingProbability, UpdateFeatures);
            waterLevelCalculationsForMaximumAllowableFloodingProbabilityObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability, UpdateFeatures);

            waterLevelForUserDefinedTargetProbabilitiesCollectionObserver = new Observer(() =>
            {
                DeleteTargetProbabilitiesObservers(waterLevelCalculationsForTargetProbabilityObservers);
                CreateTargetProbabilitiesObservers(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities, waterLevelCalculationsForTargetProbabilityObservers);
                UpdateFeatures();
            })
            {
                Observable = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
            };
            waveHeightForUserDefinedTargetProbabilitiesCollectionObserver = new Observer(() =>
            {
                DeleteTargetProbabilitiesObservers(waveHeightCalculationsForTargetProbabilityObservers);
                CreateTargetProbabilitiesObservers(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities, waveHeightCalculationsForTargetProbabilityObservers);
                UpdateFeatures();
            })
            {
                Observable = assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
            };

            waterLevelForUserDefinedTargetProbabilitiesObserver = new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability>, HydraulicBoundaryLocationCalculationsForTargetProbability>(
                UpdateFeatures, tp => tp)
            {
                Observable = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
            };

            waveHeightForUserDefinedTargetProbabilitiesObserver = new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability>, HydraulicBoundaryLocationCalculationsForTargetProbability>(
                UpdateFeatures, tp => tp)
            {
                Observable = assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
            };

            waterLevelCalculationsForTargetProbabilityObservers = new List<RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>>();
            CreateTargetProbabilitiesObservers(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities, waterLevelCalculationsForTargetProbabilityObservers);

            waveHeightCalculationsForTargetProbabilityObservers = new List<RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>>();
            CreateTargetProbabilitiesObservers(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities, waveHeightCalculationsForTargetProbabilityObservers);
        }

        private void CreateTargetProbabilitiesObservers(IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> calculationsForUserDefinedTargetProbabilities,
                                                        List<RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>> observers)
        {
            observers.AddRange(calculationsForUserDefinedTargetProbabilities.Select(calculationsForTargetProbability => ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                                                                                        calculationsForTargetProbability.HydraulicBoundaryLocationCalculations, UpdateFeatures)));
        }

        private static void DeleteTargetProbabilitiesObservers(List<RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>> observers)
        {
            foreach (RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> observer in observers)
            {
                observer.Dispose();
            }

            observers.Clear();
        }

        private void UpdateFeatures()
        {
            SetFeatures();
            MapData.NotifyObservers();
        }

        private void SetFeatures()
        {
            IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> waterLevelCalculations = GetWaterLevelCalculations();
            IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> waveHeightsCalculations = GetWaveHeightCalculations();

            IEnumerable<AggregatedHydraulicBoundaryLocation> newLocations = AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(
                assessmentSection.HydraulicBoundaryDatabase.Locations, waterLevelCalculations, waveHeightsCalculations);

            IEnumerable<MapFeature> features = RiskeerMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(newLocations);

            var newFeatures = new Dictionary<MapFeature, AggregatedHydraulicBoundaryLocation>();
            
            for (var i = 0; i < features.Count(); i++)
            {
                newFeatures.Add(features.ElementAt(i), newLocations.ElementAt(i));
            }

            CurrentFeatures = newFeatures;

            MapData.Features = features;
            
            if (MapData.Features.Any())
            {
                UpdateMetaData(waterLevelCalculations, waveHeightsCalculations);
            }
        }

        private void UpdateMetaData(IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> waterLevelCalculations,
                                    IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> waveHeightsCalculations)
        {
            var newMetaDataItems = new Dictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string>();

            var waterLevelMetaDataItemsCounter = 0;
            var waveHeightMetaDataItemsCounter = 0;
            foreach (string metaData in MapData.MetaData)
            {
                if (metaData.Contains(string.Format(Resources.MetaData_WaterLevel_TargetProbability_0, string.Empty)))
                {
                    newMetaDataItems.Add(waterLevelCalculations.ElementAt(waterLevelMetaDataItemsCounter).Key, metaData);
                    waterLevelMetaDataItemsCounter++;
                }
                else if (metaData.Contains(string.Format(Resources.MetaData_WaveHeight_TargetProbability_0, string.Empty)))
                {
                    newMetaDataItems.Add(waveHeightsCalculations.ElementAt(waveHeightMetaDataItemsCounter).Key, metaData);
                    waveHeightMetaDataItemsCounter++;
                }
            }

            string currentSelectedMetaDataAttribute = MapData.SelectedMetaDataAttribute;
            foreach (KeyValuePair<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> metaDataItem in currentMetaDataItems)
            {
                if (currentSelectedMetaDataAttribute == metaDataItem.Value)
                {
                    if (!newMetaDataItems.ContainsKey(metaDataItem.Key))
                    {
                        MapData.SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name;
                    }
                    else
                    {
                        if (currentSelectedMetaDataAttribute != newMetaDataItems[metaDataItem.Key])
                        {
                            MapData.SelectedMetaDataAttribute = newMetaDataItems[metaDataItem.Key];
                        }
                    }
                }
            }

            currentMetaDataItems = newMetaDataItems;
        }

        private IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> GetWaterLevelCalculations()
        {
            Dictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, double> waterLevelCalculations =
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.ToDictionary(
                    tp => (IObservableEnumerable<HydraulicBoundaryLocationCalculation>) tp.HydraulicBoundaryLocationCalculations,
                    tp => tp.TargetProbability);

            waterLevelCalculations.Add(assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability, assessmentSection.FailureMechanismContribution.MaximumAllowableFloodingProbability);
            waterLevelCalculations.Add(assessmentSection.WaterLevelCalculationsForSignalFloodingProbability, assessmentSection.FailureMechanismContribution.SignalFloodingProbability);

            return waterLevelCalculations.OrderByDescending(pair => pair.Value)
                                         .ToDictionary(x => x.Key, x => string.Format(Resources.MetaData_WaterLevel_TargetProbability_0,
                                                                                      TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(x.Key,
                                                                                                                                                                                   assessmentSection)));
        }

        private IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> GetWaveHeightCalculations()
        {
            return assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                    .OrderByDescending(tp => tp.TargetProbability)
                                    .ToDictionary(x => (IObservableEnumerable<HydraulicBoundaryLocationCalculation>) x.HydraulicBoundaryLocationCalculations,
                                                  x => string.Format(Resources.MetaData_WaveHeight_TargetProbability_0,
                                                                     TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(x,
                                                                                                                                                        assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                                                                                                                                        y => y.TargetProbability)));
        }
    }
}