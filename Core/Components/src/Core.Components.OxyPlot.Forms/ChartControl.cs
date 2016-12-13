// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Core.Components.OxyPlot.DataSeries;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with configured representation of axes.
    /// </summary>
    public sealed class ChartControl : Control, IChartControl
    {
        private readonly RecursiveObserver<ChartDataCollection, ChartDataCollection> chartDataCollectionObserver;
        private readonly IList<DrawnChartData> drawnChartDataList = new List<DrawnChartData>();

        private LinearPlotView plotView;
        private DynamicPlotController plotController;
        private ChartDataCollection data;

        /// <summary>
        /// Creates a new instance of <see cref="ChartControl"/>.
        /// </summary>
        public ChartControl()
        {
            InitializePlotView();
            MinimumSize = new Size(100, 100);

            chartDataCollectionObserver = new RecursiveObserver<ChartDataCollection, ChartDataCollection>(HandleChartDataCollectionChange, cdc => cdc.Collection);
        }

        public ChartDataCollection Data
        {
            get
            {
                return data;
            }
            set
            {
                if (data != null)
                {
                    ClearChartData();
                }

                data = value;

                chartDataCollectionObserver.Observable = data;

                if (data != null)
                {
                    DrawInitialChartData();
                }
            }
        }

        public string ChartTitle
        {
            get
            {
                return plotView.ModelTitle;
            }
            set
            {
                plotView.ModelTitle = value;
            }
        }

        public string BottomAxisTitle
        {
            get
            {
                return plotView.BottomAxisTitle;
            }
            set
            {
                plotView.BottomAxisTitle = value;
            }
        }

        public string LeftAxisTitle
        {
            get
            {
                return plotView.LeftAxisTitle;
            }
            set
            {
                plotView.LeftAxisTitle = value;
            }
        }

        public bool IsPanningEnabled
        {
            get
            {
                return plotController.IsPanningEnabled;
            }
        }

        public bool IsRectangleZoomingEnabled
        {
            get
            {
                return plotController.IsRectangleZoomingEnabled;
            }
        }

        public void TogglePanning()
        {
            plotController.TogglePanning();
        }

        public void ToggleRectangleZooming()
        {
            plotController.ToggleRectangleZooming();
        }

        public void ZoomToAll()
        {
            plotView.ZoomToAll();
        }

        protected override void Dispose(bool disposing)
        {
            plotView.Dispose();
            chartDataCollectionObserver.Dispose();

            base.Dispose(disposing);
        }

        private void InitializePlotView()
        {
            plotView = new LinearPlotView
            {
                BackColor = Color.White,
                Model =
                {
                    IsLegendVisible = false
                }
            };

            plotController = new DynamicPlotController();
            plotView.Controller = plotController;

            Controls.Add(plotView);
        }

        private static IEnumerable<ItemBasedChartData> GetItemBasedChartDataRecursively(ChartDataCollection chartDataCollection)
        {
            var itemBasedChartDataList = new List<ItemBasedChartData>();

            foreach (ChartData chartData in chartDataCollection.Collection)
            {
                var nestedChartDataCollection = chartData as ChartDataCollection;
                if (nestedChartDataCollection != null)
                {
                    itemBasedChartDataList.AddRange(GetItemBasedChartDataRecursively(nestedChartDataCollection));
                    continue;
                }

                itemBasedChartDataList.Add((ItemBasedChartData) chartData);
            }

            return itemBasedChartDataList;
        }

        private void HandleChartDataCollectionChange()
        {
            var chartDataThatShouldBeDrawn = GetItemBasedChartDataRecursively(Data).ToList();
            var drawnChartDataLookup = drawnChartDataList.ToDictionary(dcd => dcd.ItemBasedChartData, dcd => dcd);

            DrawMissingChartDataOnCollectionChange(chartDataThatShouldBeDrawn, drawnChartDataLookup);
            RemoveRedundantChartDataOnCollectionChange(chartDataThatShouldBeDrawn, drawnChartDataLookup);

            drawnChartDataLookup = drawnChartDataList.ToDictionary(dcd => dcd.ItemBasedChartData, dcd => dcd);

            ReorderChartDataOnCollectionChange(chartDataThatShouldBeDrawn, drawnChartDataLookup);

            plotView.InvalidatePlot(true);
        }

        private void DrawMissingChartDataOnCollectionChange(IEnumerable<ItemBasedChartData> chartDataThatShouldBeDrawn,
                                                            IDictionary<ItemBasedChartData, DrawnChartData> drawnChartDataLookup)
        {
            foreach (var chartDataToDraw in chartDataThatShouldBeDrawn.Where(chartDataToDraw => !drawnChartDataLookup.ContainsKey(chartDataToDraw))) 
            {
                DrawChartData(chartDataToDraw);
            }
        }

        private void RemoveRedundantChartDataOnCollectionChange(IEnumerable<ItemBasedChartData> chartDataThatShouldBeDrawn,
                                                                IDictionary<ItemBasedChartData, DrawnChartData> drawnChartDataLookup)
        {
            foreach (var itemBasedChartData in drawnChartDataLookup.Keys.Except(chartDataThatShouldBeDrawn))
            {
                RemoveChartData(drawnChartDataLookup[itemBasedChartData]);
            }
        }

        private void ReorderChartDataOnCollectionChange(IEnumerable<ItemBasedChartData> chartDataThatShouldBeDrawn,
                                                        IDictionary<ItemBasedChartData, DrawnChartData> drawnChartDataLookup)
        {
            plotView.Model.Series.Clear();

            foreach (ItemBasedChartData itemBasedChartData in chartDataThatShouldBeDrawn)
            {
                plotView.Model.Series.Add((Series) drawnChartDataLookup[itemBasedChartData].ItemBasedChartDataSeries);
            }
        }

        private void RemoveChartData(DrawnChartData drawnChartDataToRemove)
        {
            drawnChartDataToRemove.Observer.Dispose();
            drawnChartDataList.Remove(drawnChartDataToRemove);

            plotView.Model.Series.Remove((Series) drawnChartDataToRemove.ItemBasedChartDataSeries);
        }

        private void DrawInitialChartData()
        {
            foreach (var itemBasedChartData in GetItemBasedChartDataRecursively(Data))
            {
                DrawChartData(itemBasedChartData);
            }

            plotView.InvalidatePlot(true);
        }

        private void DrawChartData(ItemBasedChartData itemBasedChartData)
        {
            var itemBasedChartDataSeries = ItemBasedChartDataSeriesFactory.Create(itemBasedChartData);

            var drawnChartData = new DrawnChartData
            {
                ItemBasedChartData = itemBasedChartData,
                ItemBasedChartDataSeries = itemBasedChartDataSeries
            };

            drawnChartData.Observer = new Observer(() =>
            {
                drawnChartData.ItemBasedChartDataSeries.Update();
                plotView.InvalidatePlot(true);
            })
            {
                Observable = itemBasedChartData
            };

            drawnChartDataList.Add(drawnChartData);

            plotView.Model.Series.Add((Series) itemBasedChartDataSeries);
        }

        private void ClearChartData()
        {
            foreach (DrawnChartData drawnChartData in drawnChartDataList)
            {
                drawnChartData.Observer.Dispose();
            }

            drawnChartDataList.Clear();

            plotView.Model.Series.Clear();
        }

        /// <summary>
        /// Lookup class for administration related to drawn chart data series.
        /// </summary>
        private class DrawnChartData
        {
            /// <summary>
            /// The item based chart data which the drawn <see cref="ItemBasedChartDataSeries"/> is based upon.
            /// </summary>
            public ItemBasedChartData ItemBasedChartData { get; set; }

            /// <summary>
            /// The drawn chart data series.
            /// </summary>
            public IItemBasedChartDataSeries ItemBasedChartDataSeries { get; set; }

            /// <summary>
            /// The observer attached to <see cref="ItemBasedChartData"/> and responsible for updating <see cref="ItemBasedChartDataSeries"/>.
            /// </summary>
            public Observer Observer { get; set; }
        }
    }
}