// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Util.Extensions;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class is a view to show the macro stability inwards input.
    /// </summary>
    public partial class MacroStabilityInwardsInputView : UserControl, IChartView
    {
        private readonly Observer calculationObserver;
        private readonly Observer calculationInputObserver;
        private readonly Observer hydraulicLocationCalculationObserver;
        private readonly Observer failureMechanismContributionObserver;

        private readonly ChartDataCollection chartDataCollection;
        private readonly ChartDataCollection soilProfileChartData;
        private readonly ChartDataCollection waternetZonesExtremeChartData;
        private readonly ChartDataCollection waternetZonesDailyChartData;
        private readonly ChartLineData surfaceLineChartData;
        private readonly ChartPointData surfaceLevelInsideChartData;
        private readonly ChartPointData ditchPolderSideChartData;
        private readonly ChartPointData bottomDitchPolderSideChartData;
        private readonly ChartPointData bottomDitchDikeSideChartData;
        private readonly ChartPointData ditchDikeSideChartData;
        private readonly ChartPointData dikeToeAtPolderChartData;
        private readonly ChartPointData shoulderTopInsideChartData;
        private readonly ChartPointData shoulderBaseInsideChartData;
        private readonly ChartPointData dikeTopAtPolderChartData;
        private readonly ChartPointData dikeToeAtRiverChartData;
        private readonly ChartPointData dikeTopAtRiverChartData;
        private readonly ChartPointData surfaceLevelOutsideChartData;
        private readonly ChartPointData leftGridChartData;
        private readonly ChartPointData rightGridChartData;
        private readonly ChartMultipleLineData tangentLinesData;

        private readonly Func<HydraulicBoundaryLocationCalculation> getHydraulicBoundaryLocationCalculationFunc;

        private readonly List<ChartMultipleAreaData> soilLayerChartDataLookup;

        private readonly MacroStabilityInwardsCalculationScenario data;

        private IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> currentSoilProfile;
        private MacroStabilityInwardsSurfaceLine currentSurfaceLine;

        private MacroStabilityInwardsWaternet currentWaternetExtreme;
        private MacroStabilityInwardsWaternet currentWaternetDaily;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsInputView"/>.
        /// </summary>
        /// <param name="data">The calculation to show the input for.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <param name="getHydraulicBoundaryLocationCalculationFunc">The <see cref="Func{TResult}"/> for
        /// obtaining the hydraulic boundary location calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsInputView(MacroStabilityInwardsCalculationScenario data,
                                              IAssessmentSection assessmentSection,
                                              Func<HydraulicBoundaryLocationCalculation> getHydraulicBoundaryLocationCalculationFunc)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getHydraulicBoundaryLocationCalculationFunc == null)
            {
                throw new ArgumentNullException(nameof(getHydraulicBoundaryLocationCalculationFunc));
            }

            this.data = data;
            this.getHydraulicBoundaryLocationCalculationFunc = getHydraulicBoundaryLocationCalculationFunc;

            InitializeComponent();

            calculationObserver = new Observer(UpdateChartTitle)
            {
                Observable = data
            };

            calculationInputObserver = new Observer(UpdateViewData)
            {
                Observable = data.InputParameters
            };

            hydraulicLocationCalculationObserver = new Observer(UpdateViewData)
            {
                Observable = getHydraulicBoundaryLocationCalculationFunc()
            };

            failureMechanismContributionObserver = new Observer(UpdateViewData)
            {
                Observable = assessmentSection.FailureMechanismContribution
            };

            chartDataCollection = new ChartDataCollection(RingtoetsCommonFormsResources.Calculation_Input);
            soilProfileChartData = RingtoetsChartDataFactory.CreateSoilProfileChartData();
            surfaceLineChartData = RingtoetsChartDataFactory.CreateSurfaceLineChartData();
            surfaceLevelInsideChartData = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelInsideChartData();
            ditchPolderSideChartData = RingtoetsChartDataFactory.CreateDitchPolderSideChartData();
            bottomDitchPolderSideChartData = RingtoetsChartDataFactory.CreateBottomDitchPolderSideChartData();
            bottomDitchDikeSideChartData = RingtoetsChartDataFactory.CreateBottomDitchDikeSideChartData();
            ditchDikeSideChartData = RingtoetsChartDataFactory.CreateDitchDikeSideChartData();
            dikeToeAtPolderChartData = RingtoetsChartDataFactory.CreateDikeToeAtPolderChartData();
            shoulderTopInsideChartData = MacroStabilityInwardsChartDataFactory.CreateShoulderTopInsideChartData();
            shoulderBaseInsideChartData = MacroStabilityInwardsChartDataFactory.CreateShoulderBaseInsideChartData();
            dikeTopAtPolderChartData = MacroStabilityInwardsChartDataFactory.CreateDikeTopAtPolderChartData();
            dikeToeAtRiverChartData = RingtoetsChartDataFactory.CreateDikeToeAtRiverChartData();
            dikeTopAtRiverChartData = MacroStabilityInwardsChartDataFactory.CreateDikeTopAtRiverChartData();
            surfaceLevelOutsideChartData = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelOutsideChartData();
            tangentLinesData = MacroStabilityInwardsChartDataFactory.CreateTangentLinesChartData();
            leftGridChartData = MacroStabilityInwardsChartDataFactory.CreateLeftGridChartData();
            rightGridChartData = MacroStabilityInwardsChartDataFactory.CreateRightGridChartData();
            waternetZonesExtremeChartData = MacroStabilityInwardsChartDataFactory.CreateWaternetZonesExtremeChartDataCollection();
            waternetZonesDailyChartData = MacroStabilityInwardsChartDataFactory.CreateWaternetZonesDailyChartDataCollection();

            chartDataCollection.Add(soilProfileChartData);
            chartDataCollection.Add(surfaceLineChartData);
            chartDataCollection.Add(surfaceLevelInsideChartData);
            chartDataCollection.Add(ditchPolderSideChartData);
            chartDataCollection.Add(bottomDitchPolderSideChartData);
            chartDataCollection.Add(bottomDitchDikeSideChartData);
            chartDataCollection.Add(ditchDikeSideChartData);
            chartDataCollection.Add(dikeToeAtPolderChartData);
            chartDataCollection.Add(shoulderTopInsideChartData);
            chartDataCollection.Add(shoulderBaseInsideChartData);
            chartDataCollection.Add(dikeTopAtPolderChartData);
            chartDataCollection.Add(dikeToeAtRiverChartData);
            chartDataCollection.Add(dikeTopAtRiverChartData);
            chartDataCollection.Add(surfaceLevelOutsideChartData);
            chartDataCollection.Add(tangentLinesData);
            chartDataCollection.Add(leftGridChartData);
            chartDataCollection.Add(rightGridChartData);
            chartDataCollection.Add(waternetZonesExtremeChartData);
            chartDataCollection.Add(waternetZonesDailyChartData);

            soilLayerChartDataLookup = new List<ChartMultipleAreaData>();

            SetChartData();

            chartControl.Data = chartDataCollection;

            UpdateChartTitle();
            UpdateTableData();
        }

        public object Data
        {
            get
            {
                return data;
            }
            set {}
        }

        public IChartControl Chart
        {
            get
            {
                return chartControl;
            }
        }

        protected override void Dispose(bool disposing)
        {
            calculationObserver.Dispose();
            calculationInputObserver.Dispose();
            hydraulicLocationCalculationObserver.Dispose();
            failureMechanismContributionObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateTableData()
        {
            soilLayerDataTable.SetData(GetSoilLayers().Select(l => l.Data));
        }

        private void UpdateChartTitle()
        {
            chartControl.ChartTitle = data.Name;
        }

        private void UpdateViewData()
        {
            UpdateChartData();
            UpdateTableData();
        }

        private void UpdateChartData()
        {
            SetChartData();

            chartDataCollection.Collection.ForEachElementDo(cd => cd.NotifyObservers());
            soilProfileChartData.Collection.ForEachElementDo(sp => sp.NotifyObservers());
        }

        private void SetChartData()
        {
            MacroStabilityInwardsInput macroStabilityInwardsInput = data.InputParameters;
            MacroStabilityInwardsSurfaceLine surfaceLine = macroStabilityInwardsInput.SurfaceLine;
            IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile = macroStabilityInwardsInput.StochasticSoilProfile?.SoilProfile;
            hydraulicLocationCalculationObserver.Observable = getHydraulicBoundaryLocationCalculationFunc();

            SetSurfaceLineChartData(surfaceLine);
            SetSoilProfileChartData(surfaceLine, soilProfile);

            SetWaternetExtremeChartData(DerivedMacroStabilityInwardsInput.GetWaternetExtreme(macroStabilityInwardsInput, GetEffectiveAssessmentLevel()), surfaceLine);
            SetWaternetDailyChartData(DerivedMacroStabilityInwardsInput.GetWaternetDaily(macroStabilityInwardsInput), surfaceLine);

            MacroStabilityInwardsGridDeterminationType gridDeterminationType = macroStabilityInwardsInput.GridDeterminationType;
            MacroStabilityInwardsGrid leftGrid = macroStabilityInwardsInput.LeftGrid;
            MacroStabilityInwardsGrid rightGrid = macroStabilityInwardsInput.RightGrid;

            leftGridChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(leftGrid, gridDeterminationType);
            rightGridChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(rightGrid, gridDeterminationType);

            tangentLinesData.Lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(macroStabilityInwardsInput.GridDeterminationType,
                                                                                                    macroStabilityInwardsInput.TangentLineDeterminationType,
                                                                                                    macroStabilityInwardsInput.TangentLineZBottom,
                                                                                                    macroStabilityInwardsInput.TangentLineZTop,
                                                                                                    macroStabilityInwardsInput.TangentLineNumber,
                                                                                                    macroStabilityInwardsInput.SurfaceLine);

            currentSoilProfile = soilProfile;
            if (surfaceLine != null)
            {
                if (currentSurfaceLine == null)
                {
                    currentSurfaceLine = new MacroStabilityInwardsSurfaceLine(surfaceLine.Name);
                }

                currentSurfaceLine.CopyProperties(surfaceLine);
            }
            else
            {
                currentSurfaceLine = null;
            }
        }

        private void SetWaternetExtremeChartData(MacroStabilityInwardsWaternet waternet, MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (!waternet.Equals(currentWaternetExtreme) || !SurfaceLineEqual(surfaceLine))
            {
                currentWaternetExtreme = waternet;
                SetWaternetZonesChartData(waternet, surfaceLine, waternetZonesExtremeChartData);
            }
        }

        private void SetWaternetDailyChartData(MacroStabilityInwardsWaternet waternet, MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (!waternet.Equals(currentWaternetDaily) || !SurfaceLineEqual(surfaceLine))
            {
                currentWaternetDaily = waternet;
                SetWaternetZonesChartData(waternet, surfaceLine, waternetZonesDailyChartData);
            }
        }

        private static void SetWaternetZonesChartData(MacroStabilityInwardsWaternet waternet, MacroStabilityInwardsSurfaceLine surfaceLine,
                                                      ChartDataCollection chartData)
        {
            chartData.Clear();

            foreach (MacroStabilityInwardsPhreaticLine phreaticLine in waternet.PhreaticLines)
            {
                ChartLineData phreaticLineChartData = MacroStabilityInwardsChartDataFactory.CreatePhreaticLineChartData(phreaticLine.Name, true);
                phreaticLineChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreatePhreaticLinePoints(phreaticLine);
                chartData.Add(phreaticLineChartData);
            }

            foreach (MacroStabilityInwardsWaternetLine waternetLine in waternet.WaternetLines)
            {
                ChartMultipleAreaData waternetLineChartData = MacroStabilityInwardsChartDataFactory.CreateWaternetZoneChartData(waternetLine.Name, true);
                waternetLineChartData.Areas = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, surfaceLine);
                chartData.Add(waternetLineChartData);
            }
        }

        private void SetSurfaceLineChartData(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            MacroStabilityInwardsChartDataFactory.UpdateSurfaceLineChartDataName(surfaceLineChartData, surfaceLine);

            surfaceLineChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLinePoints(surfaceLine);
            surfaceLevelInsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelInsidePoint(surfaceLine);
            ditchPolderSideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchPolderSidePoint(surfaceLine);
            bottomDitchPolderSideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchPolderSidePoint(surfaceLine);
            bottomDitchDikeSideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchDikeSidePoint(surfaceLine);
            ditchDikeSideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchDikeSidePoint(surfaceLine);
            dikeToeAtPolderChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtPolderPoint(surfaceLine);
            shoulderTopInsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderTopInsidePoint(surfaceLine);
            shoulderBaseInsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderBaseInsidePoint(surfaceLine);
            dikeTopAtPolderChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtPolderPoint(surfaceLine);
            dikeToeAtRiverChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine);
            dikeTopAtRiverChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtRiverPoint(surfaceLine);
            surfaceLevelOutsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelOutsidePoint(surfaceLine);
        }

        private void SetSoilProfileChartData(MacroStabilityInwardsSurfaceLine surfaceLine,
                                             IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile)
        {
            if (!ReferenceEquals(currentSoilProfile, soilProfile) || !SurfaceLineEqual(surfaceLine))
            {
                SetSoilProfileChartData();
            }

            SetSoilLayerAreas();
        }

        private bool SurfaceLineEqual(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return (surfaceLine != null || currentSurfaceLine != null) && currentSurfaceLine != null && currentSurfaceLine.Equals(surfaceLine);
        }

        private void SetSoilProfileChartData()
        {
            soilProfileChartData.Clear();
            soilLayerChartDataLookup.Clear();

            IEnumerable<MacroStabilityInwardsSoilLayer2D> soilLayers = GetSoilLayers().Reverse();

            soilLayers.Select(MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData)
                      .ForEachElementDo(sl =>
                      {
                          soilProfileChartData.Insert(0, sl);
                          soilLayerChartDataLookup.Add(sl);
                      });

            MacroStabilityInwardsChartDataFactory.UpdateSoilProfileChartDataName(soilProfileChartData, data.InputParameters.StochasticSoilProfile?.SoilProfile);
        }

        private void SetSoilLayerAreas()
        {
            var i = 0;
            foreach (MacroStabilityInwardsSoilLayer2D soilLayer in GetSoilLayers().Reverse())
            {
                ChartMultipleAreaData soilLayerData = soilLayerChartDataLookup[i++];
                soilLayerData.Areas = MacroStabilityInwardsChartDataPointsFactory.CreateOuterRingArea(soilLayer);
            }
        }

        private IEnumerable<MacroStabilityInwardsSoilLayer2D> GetSoilLayers()
        {
            return MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(data?.InputParameters.SoilProfileUnderSurfaceLine?.Layers);
        }

        private RoundedDouble GetEffectiveAssessmentLevel()
        {
            return data.InputParameters.UseAssessmentLevelManualInput
                       ? data.InputParameters.AssessmentLevel
                       : getHydraulicBoundaryLocationCalculationFunc()?.Output?.Result ?? RoundedDouble.NaN;
        }
    }
}