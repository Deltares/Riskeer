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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;
using RiskeerCommonUtilResources = Riskeer.Common.Util.Properties.Resources;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Map layer to show hydraulic boundary locations.
    /// </summary>
    public class HydraulicBoundaryLocationsMapLayer : IDisposable
    {
        private readonly IAssessmentSection assessmentSection;

        private Observer hydraulicBoundaryLocationsObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalingNormObserver;
        private RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNormObserver;

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
            SetFeatures();
        }

        /// <summary>
        /// Gets the hydraulic boundary locations map data.
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
                hydraulicBoundaryLocationsObserver.Dispose();
                waterLevelCalculationsForSignalingNormObserver.Dispose();
                waterLevelCalculationsForLowerLimitNormObserver.Dispose();
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
            hydraulicBoundaryLocationsObserver = new Observer(UpdateFeatures)
            {
                Observable = assessmentSection.HydraulicBoundaryDatabase.Locations
            };

            waterLevelCalculationsForSignalingNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                assessmentSection.WaterLevelCalculationsForSignalingNorm, UpdateFeatures);
            waterLevelCalculationsForLowerLimitNormObserver = ObserverHelper.CreateHydraulicBoundaryLocationCalculationsObserver(
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm, UpdateFeatures);

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

            MapData.Features = RiskeerMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(newLocations);

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

            foreach (KeyValuePair<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> metaDataItem in currentMetaDataItems)
            {
                if (MapData.SelectedMetaDataAttribute == metaDataItem.Value)
                {
                    if (!newMetaDataItems.ContainsKey(metaDataItem.Key))
                    {
                        MapData.SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name;
                    }
                    else
                    {
                        if (currentMetaDataItems[metaDataItem.Key] != newMetaDataItems[metaDataItem.Key])
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

            waterLevelCalculations.Add(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, assessmentSection.FailureMechanismContribution.LowerLimitNorm);
            waterLevelCalculations.Add(assessmentSection.WaterLevelCalculationsForSignalingNorm, assessmentSection.FailureMechanismContribution.SignalingNorm);

            return waterLevelCalculations.OrderByDescending(pair => pair.Value)
                                         .ToDictionary(x => x.Key, x => $"h - {TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(x.Key, assessmentSection)}");
        }

        private IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> GetWaveHeightCalculations()
        {
            Dictionary<HydraulicBoundaryLocationCalculationsForTargetProbability, double> waveHeightCalculations = assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.ToDictionary(
                tp => tp,
                tp => tp.TargetProbability);

            return waveHeightCalculations.OrderByDescending(pair => pair.Value)
                                         .ToDictionary(x => (IObservableEnumerable<HydraulicBoundaryLocationCalculation>) x.Key.HydraulicBoundaryLocationCalculations,
                                                       x => $"Hs - {TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(x.Key, assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities, y => y.TargetProbability)}");
        }
    }
}