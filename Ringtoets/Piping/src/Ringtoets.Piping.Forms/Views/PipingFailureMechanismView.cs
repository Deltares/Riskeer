// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Piping.Forms.PresentationObjects;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view showing map data for a piping failure mechanism.
    /// </summary>
    public partial class PipingFailureMechanismView : UserControl, IMapView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer surfaceLinesObserver;
        private readonly Observer stochasticSoilModelsObserver;

        private readonly MapLineData referenceLineMapData;
        private readonly MapLineData sectionsMapData;
        private readonly MapLineData stochasticSoilModelsMapData;
        private readonly MapLineData surfaceLinesMapData;
        private readonly MapPointData sectionsStartPointMapData;
        private readonly MapPointData sectionsEndPointMapData;
        private readonly MapPointData hydraulicBoundaryDatabaseMapData;

        private PipingFailureMechanismContext data;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismView"/>.
        /// </summary>
        public PipingFailureMechanismView()
        {
            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateMapData);
            assessmentSectionObserver = new Observer(UpdateMapData);
            surfaceLinesObserver = new Observer(UpdateMapData);
            stochasticSoilModelsObserver = new Observer(UpdateMapData);

            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            sectionsMapData = PipingMapDataFactory.CreateFailureMechanismSectionsMapData();
            stochasticSoilModelsMapData = PipingMapDataFactory.CreateStochasticSoilModelsMapData();
            surfaceLinesMapData = PipingMapDataFactory.CreateSurfaceLinesMapData();
            sectionsStartPointMapData = PipingMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();
            sectionsEndPointMapData = PipingMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();
            hydraulicBoundaryDatabaseMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryDatabaseMapData();

            mapControl.Data.Add(referenceLineMapData);
            mapControl.Data.Add(sectionsMapData);
            mapControl.Data.Add(stochasticSoilModelsMapData);
            mapControl.Data.Add(surfaceLinesMapData);
            mapControl.Data.Add(sectionsStartPointMapData);
            mapControl.Data.Add(sectionsEndPointMapData);
            mapControl.Data.Add(hydraulicBoundaryDatabaseMapData);

            mapControl.Data.Name = PipingDataResources.PipingFailureMechanism_DisplayName;
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as PipingFailureMechanismContext;

                if (data == null)
                {
                    failureMechanismObserver.Observable = null;
                    assessmentSectionObserver.Observable = null;
                    surfaceLinesObserver.Observable = null;
                    stochasticSoilModelsObserver.Observable = null;

                    Map.ResetMapData();
                    return;
                }

                failureMechanismObserver.Observable = data.WrappedData;
                assessmentSectionObserver.Observable = data.Parent;
                surfaceLinesObserver.Observable = data.WrappedData.SurfaceLines;
                stochasticSoilModelsObserver.Observable = data.WrappedData.StochasticSoilModels;

                UpdateMapData();
            }
        }

        public IMapControl Map
        {
            get
            {
                return mapControl;
            }
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            assessmentSectionObserver.Dispose();
            stochasticSoilModelsObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateMapData()
        {
            UpdateFeatureBasedMapData(referenceLineMapData,
                                      RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(data != null && data.Parent != null
                                                                                                      ? data.Parent.ReferenceLine
                                                                                                      : null));
            UpdateFeatureBasedMapData(sectionsMapData,
                                      PipingMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(data != null && data.WrappedData != null
                                                                                                             ? data.WrappedData.Sections
                                                                                                             : null));
            UpdateFeatureBasedMapData(stochasticSoilModelsMapData,
                                      PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(data != null && data.WrappedData != null
                                                                                                         ? data.WrappedData.StochasticSoilModels
                                                                                                         : null));
            UpdateFeatureBasedMapData(surfaceLinesMapData,
                                      PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(data != null && data.WrappedData != null
                                                                                                 ? data.WrappedData.SurfaceLines
                                                                                                 : null));
            UpdateFeatureBasedMapData(sectionsStartPointMapData,
                                      PipingMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(data != null && data.WrappedData != null
                                                                                                                       ? data.WrappedData.Sections
                                                                                                                       : null));
            UpdateFeatureBasedMapData(sectionsEndPointMapData,
                                      PipingMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(data != null && data.WrappedData != null
                                                                                                                     ? data.WrappedData.Sections
                                                                                                                     : null));
            UpdateFeatureBasedMapData(hydraulicBoundaryDatabaseMapData,
                                      RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(data != null && data.Parent != null
                                                                                                                  ? data.Parent.HydraulicBoundaryDatabase
                                                                                                                  : null));

            mapControl.Data.NotifyObservers();
        }

        private static void UpdateFeatureBasedMapData(FeatureBasedMapData mapData, MapFeature[] features)
        {
            mapData.Features = features;
            mapData.NotifyObservers();
        }
    }
}