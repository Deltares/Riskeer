// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Core.Components.OxyPlot.DataSeries.Chart;
using Core.Components.OxyPlot.Forms.Properties;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with configured representation of axes.
    /// </summary>
    public partial class ChartControl : UserControl, IChartControl
    {
        private readonly RecursiveObserver<ChartDataCollection, ChartDataCollection> chartDataCollectionObserver;
        private readonly List<DrawnChartData> drawnChartDataList = new List<DrawnChartData>();
        private readonly PrivateFontCollection fonts = new PrivateFontCollection();

        private LinearPlotView plotView;
        private DynamicPlotController plotController;
        private ChartDataCollection data;

        /// <summary>
        /// Creates a new instance of <see cref="ChartControl"/>.
        /// </summary>
        public ChartControl()
        {
            InitializeComponent();

            InitializeFont();

            InitializePlotView();

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

                plotView.InvalidatePlot(true);
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

        public void ZoomToAllVisibleLayers(ChartData layerData)
        {
            Extent extent = CreateEnvelopeForAllVisibleLayers(layerData);

            if (!extent.IsNaN)
            {
                Extent extentWithPadding = extent.AddPadding(0.01);

                plotView.SetExtent(extentWithPadding);
                plotView.Refresh();
            }
        }

        protected override void Dispose(bool disposing)
        {
            plotView.Dispose();
            chartDataCollectionObserver.Dispose();

            base.Dispose(disposing);
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
                                                          IntPtr pdv, [In] ref uint pcFonts);

        private void InitializeFont()
        {
            byte[] fontData = Resources.Deltares_Riskeer_Symbols;
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Resources.Deltares_Riskeer_Symbols.Length);
            AddFontMemResourceEx(fontPtr, (uint) Resources.Deltares_Riskeer_Symbols.Length, IntPtr.Zero, ref dummy);
            Marshal.FreeCoTaskMem(fontPtr);

            var font = new Font(fonts.Families[0], 14.0F);
            panToolStripButton.Font = font;
            zoomToRectangleToolStripButton.Font = font;
            zoomToVisibleLayersToolStripButton.Font = font;
        }

        private void InitializePlotView()
        {
            plotController = new DynamicPlotController();

            plotView = new LinearPlotView
            {
                BackColor = Color.White,
                Model =
                {
                    IsLegendVisible = false
                },
                Controller = plotController
            };

            Controls.Add(plotView);
        }

        /// <summary>
        /// Defines the area taken up by the visible chart data based on the provided chart data.
        /// </summary>
        /// <param name="chartData">The data to determine the visible extent for.</param>
        /// <returns>The area definition.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="chartData"/> is
        /// not part of the drawn chart data.</exception>
        private Extent CreateEnvelopeForAllVisibleLayers(ChartData chartData)
        {
            if (chartData is ChartDataCollection collection)
            {
                return CreateEnvelopeForAllVisibleLayers(collection);
            }

            DrawnChartData drawnChartData = drawnChartDataList.FirstOrDefault(dmd => dmd.ChartData.Equals(chartData));

            if (drawnChartData == null)
            {
                throw new ArgumentException($@"Can only zoom to {nameof(ChartData)} that is part of this {nameof(ChartControl)}s drawn {nameof(chartData)}.",
                                            nameof(chartData));
            }

            var extent = new Extent();

            ChartData chartDataDrawn = drawnChartData.ChartData;

            if (chartDataDrawn.IsVisible && chartDataDrawn.HasData)
            {
                extent.ExpandToInclude(CreateExtentFor(drawnChartData.ChartDataSeries as XYAxisSeries));
            }

            return extent;
        }

        /// <summary>
        /// Defines the area taken up by the visible chart data based on the provided chart data.
        /// </summary>
        /// <param name="chartDataCollection">The data to determine the visible extent for.</param>
        /// <returns>The area definition.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="chartDataCollection"/> or
        /// any of its children is not part of the drawn chart data.</exception>
        private Extent CreateEnvelopeForAllVisibleLayers(ChartDataCollection chartDataCollection)
        {
            var envelope = new Extent();

            foreach (ChartData childChartData in chartDataCollection.Collection)
            {
                envelope.ExpandToInclude(CreateEnvelopeForAllVisibleLayers(childChartData));
            }

            return envelope;
        }

        private static Extent CreateExtentFor(XYAxisSeries chartData)
        {
            return new Extent
            (
                chartData.MinX,
                chartData.MaxX,
                chartData.MinY,
                chartData.MaxY
            );
        }

        private void HandleChartDataCollectionChange()
        {
            List<ChartData> chartDataThatShouldBeDrawn = Data.GetChartDataRecursively().ToList();
            Dictionary<ChartData, DrawnChartData> drawnChartDataLookup = drawnChartDataList.ToDictionary(dcd => dcd.ChartData, dcd => dcd);

            DrawMissingChartDataOnCollectionChange(chartDataThatShouldBeDrawn, drawnChartDataLookup);
            RemoveRedundantChartDataOnCollectionChange(chartDataThatShouldBeDrawn, drawnChartDataLookup);

            drawnChartDataLookup = drawnChartDataList.ToDictionary(dcd => dcd.ChartData, dcd => dcd);

            ReorderChartDataOnCollectionChange(chartDataThatShouldBeDrawn, drawnChartDataLookup);

            plotView.InvalidatePlot(true);
        }

        private void DrawMissingChartDataOnCollectionChange(IEnumerable<ChartData> chartDataThatShouldBeDrawn,
                                                            IDictionary<ChartData, DrawnChartData> drawnChartDataLookup)
        {
            foreach (ChartData chartDataToDraw in chartDataThatShouldBeDrawn.Where(chartDataToDraw => !drawnChartDataLookup.ContainsKey(chartDataToDraw)))
            {
                DrawChartData(chartDataToDraw);
            }
        }

        private void RemoveRedundantChartDataOnCollectionChange(IEnumerable<ChartData> chartDataThatShouldBeDrawn,
                                                                IDictionary<ChartData, DrawnChartData> drawnChartDataLookup)
        {
            foreach (ChartData chartData in drawnChartDataLookup.Keys.Except(chartDataThatShouldBeDrawn))
            {
                RemoveChartData(drawnChartDataLookup[chartData]);
            }
        }

        private void ReorderChartDataOnCollectionChange(IEnumerable<ChartData> chartDataThatShouldBeDrawn,
                                                        IDictionary<ChartData, DrawnChartData> drawnChartDataLookup)
        {
            plotView.Model.Series.Clear();

            foreach (ChartData chartData in chartDataThatShouldBeDrawn)
            {
                plotView.Model.Series.Add((Series) drawnChartDataLookup[chartData].ChartDataSeries);
            }
        }

        private void RemoveChartData(DrawnChartData drawnChartDataToRemove)
        {
            drawnChartDataToRemove.Observer.Dispose();
            drawnChartDataList.Remove(drawnChartDataToRemove);

            plotView.Model.Series.Remove((Series) drawnChartDataToRemove.ChartDataSeries);
        }

        private void DrawInitialChartData()
        {
            foreach (ChartData chartData in Data.GetChartDataRecursively())
            {
                DrawChartData(chartData);
            }
        }

        private void DrawChartData(ChartData chartData)
        {
            IChartDataSeries chartDataSeries = ChartDataSeriesFactory.Create(chartData);

            var drawnChartData = new DrawnChartData
            {
                ChartData = chartData,
                ChartDataSeries = chartDataSeries
            };

            drawnChartData.Observer = new Observer(() =>
            {
                drawnChartData.ChartDataSeries.Update();
                plotView.InvalidatePlot(true);
            })
            {
                Observable = chartData
            };

            drawnChartDataList.Add(drawnChartData);

            plotView.Model.Series.Add((Series) chartDataSeries);
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

        private void PanToolStripButtonClick(object sender, EventArgs e)
        {
            if (panToolStripButton.Checked)
            {
                return;
            }

            plotController.TogglePanning();

            panToolStripButton.Checked = true;
            zoomToRectangleToolStripButton.Checked = false;
        }

        private void ZoomToRectangleToolStripButtonClick(object sender, EventArgs e)
        {
            if (zoomToRectangleToolStripButton.Checked)
            {
                return;
            }

            plotController.ToggleRectangleZooming();

            panToolStripButton.Checked = false;
            zoomToRectangleToolStripButton.Checked = true;
        }

        private void ZoomToAllVisibleLayersToolStripButtonClick(object sender, EventArgs e)
        {
            ZoomToAllVisibleLayers(Data);
        }

        /// <summary>
        /// Lookup class for administration related to drawn chart data series.
        /// </summary>
        private class DrawnChartData
        {
            /// <summary>
            /// The chart data which the drawn <see cref="ChartDataSeries"/> is based upon.
            /// </summary>
            public ChartData ChartData { get; set; }

            /// <summary>
            /// The drawn chart data series.
            /// </summary>
            public IChartDataSeries ChartDataSeries { get; set; }

            /// <summary>
            /// The observer attached to <see cref="ChartData"/> and responsible for updating <see cref="ChartDataSeries"/>.
            /// </summary>
            public Observer Observer { get; set; }
        }
    }
}