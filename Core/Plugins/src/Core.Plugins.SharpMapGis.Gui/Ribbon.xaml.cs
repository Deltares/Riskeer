using System;
using System.Collections.Generic;
using System.Windows;
using Core.Common.Controls;
using Core.Common.Gui.Forms;
using Core.Plugins.SharpMapGis.Gui.Commands;
using Fluent;

namespace Core.Plugins.SharpMapGis.Gui
{
    /// <summary>
    /// Interaction logic for Ribbon.xaml
    /// </summary>
    public partial class Ribbon : IRibbonCommandHandler
    {
        // view
        private readonly ICommand commandShowMapContents;

        // map decorations
        private readonly ICommand showNorthArrow;
        private readonly ICommand showMapLegend;
        private readonly ICommand showScaleBar;

        // map interaction
        private readonly ICommand selectButton;
        private readonly ICommand mapMeasureCommand;
        private readonly ICommand exportMapAsImage;

        // map zoom
        private readonly ICommand mapZoomInUsingRectangleCommand;
        private readonly ICommand mapFixedZoomInCommand;
        private readonly ICommand mapFixedZoomOutCommand;
        private readonly ICommand mapZoomToExtentsCommand;
        private readonly ICommand mapPanZoomCommand;
        private readonly ICommand mapZoomPreviousCommand;
        private readonly ICommand mapZoomNextCommand;

        // map coverage profile
        private readonly ICommand mapChangeCoordinateSystemCommand;

        public Ribbon()
        {
            InitializeComponent();

            // view
            commandShowMapContents = new ShowMapContentsCommand();

            // map decorations
            showNorthArrow = new ShowNorthArrowCommand();
            showMapLegend = new ShowMapLegendCommand();
            showScaleBar = new ShowScaleBarCommand();

            // map interaction
            selectButton = new SelectCommand();
            mapMeasureCommand = new MapMeasureCommand();
            exportMapAsImage = new ExportMapAsImageCommand();

            // map zoom
            mapZoomInUsingRectangleCommand = new MapZoomInUsingRectangleCommand();
            mapFixedZoomInCommand = new MapFixedZoomInCommand();
            mapFixedZoomOutCommand = new MapFixedZoomOutCommand();
            mapZoomToExtentsCommand = new MapZoomToExtentsCommand();
            mapPanZoomCommand = new MapPanZoomCommand();
            mapZoomPreviousCommand = new MapZoomPreviousCommand();
            mapZoomNextCommand = new MapZoomNextCommand();

            // map coverage profile
            mapChangeCoordinateSystemCommand = new MapChangeCoordinateSystemCommand();

            ButtonMapPanZoom.ToolTip = new ScreenTip
            {
                Title = "Pan",
                Text = "Pan accross the map." + Environment.NewLine +
                       "Alternatively, you can press CTRL+ALT or the middle (wheel/scroll) button in the mouse, if present, to pan across the map according to the movement of the mouse.",
                MaxWidth = 250
            };
            ButtonMapSelect.ToolTip = new ScreenTip
            {
                Title = "Select",
                Text = "Select single or multiple features by drawing a selection box." + Environment.NewLine +
                       "Pressing Esc when a map-related view is active, enables this tool.",
                MaxWidth = 250
            };

            // assign tabs to contextual groups
            mapTab.Group = geospatialContextualGroup;
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                // view
                yield return commandShowMapContents;

                // map decorations
                yield return showNorthArrow;
                yield return showMapLegend;
                yield return showScaleBar;

                // map interaction
                yield return selectButton;
                yield return mapMeasureCommand;
                yield return exportMapAsImage;

                // map zoom
                yield return mapZoomInUsingRectangleCommand;
                yield return mapFixedZoomInCommand;
                yield return mapFixedZoomOutCommand;
                yield return mapZoomToExtentsCommand;
                yield return mapPanZoomCommand;
                yield return mapZoomPreviousCommand;
                yield return mapZoomNextCommand;
            }
        }

        public void ValidateItems()
        {
            // view
            ButtonShowMapContentsToolWindow.IsEnabled = commandShowMapContents.Enabled;
            ButtonShowMapContentsToolWindow.IsChecked = commandShowMapContents.Checked;

            // map decorations
            ButtonMapShowNorthArrow.IsEnabled = showNorthArrow.Enabled;
            ButtonMapShowNorthArrow.IsChecked = showNorthArrow.Checked;
            ButtonMapShowLegend.IsEnabled = showMapLegend.Enabled;
            ButtonMapShowLegend.IsChecked = showMapLegend.Checked;
            ButtonMapShowScaleBar.IsEnabled = showScaleBar.Enabled;
            ButtonMapShowScaleBar.IsChecked = showScaleBar.Checked;

            // map interaction
            ButtonMapSelect.IsEnabled = selectButton.Enabled;
            ButtonMapSelect.IsChecked = selectButton.Checked;
            ButtonMapMeasure.IsEnabled = mapMeasureCommand.Enabled;
            ButtonMapMeasure.IsChecked = mapMeasureCommand.Checked;
            ButtonExportMapAsImage.IsEnabled = exportMapAsImage.Enabled;

            // map zoom
            ButtonMapZoomUsingRectangle.IsEnabled = mapZoomInUsingRectangleCommand.Enabled;
            ButtonMapZoomUsingRectangle.IsChecked = mapZoomInUsingRectangleCommand.Checked;
            ButtonMapZoomToExtent.IsEnabled = mapZoomToExtentsCommand.Enabled;
            ButtonMapPanZoom.IsEnabled = mapPanZoomCommand.Enabled;
            ButtonMapPanZoom.IsChecked = mapPanZoomCommand.Checked;
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return tabGroupName == geospatialContextualGroup.Name && tabName == mapTab.Name && IsActiveViewMapView();
        }

        public object GetRibbonControl()
        {
            return RibbonControl;
        }

        private bool IsActiveViewMapView()
        {
            return SharpMapGisGuiPlugin.GetFocusedMapView() != null;
        }

        private void ButtonShowMapContentsToolWindow_Click(object sender, RoutedEventArgs e)
        {
            commandShowMapContents.Execute();
            ValidateItems();
        }

        private void ButtonMapShowNorthArrow_Click(object sender, RoutedEventArgs e)
        {
            showNorthArrow.Execute();
            ValidateItems();
        }

        private void ButtonMapShowLegend_Click(object sender, RoutedEventArgs e)
        {
            showMapLegend.Execute();
            ValidateItems();
        }

        private void ButtonMapShowScaleBar_Click(object sender, RoutedEventArgs e)
        {
            showScaleBar.Execute();
            ValidateItems();
        }

        private void ButtonMapSelectFeature_Click(object sender, RoutedEventArgs e)
        {
            selectButton.Execute();
            ValidateItems();
        }

        private void ButtonMapMeasure_Click(object sender, RoutedEventArgs e)
        {
            mapMeasureCommand.Execute();
            ValidateItems();
        }

        private void ButtonExportMapAsImage_Click(object sender, RoutedEventArgs e)
        {
            exportMapAsImage.Execute();
            ValidateItems();
        }

        private void ButtonMapZoomUsingRectangle_Click(object sender, RoutedEventArgs e)
        {
            mapZoomInUsingRectangleCommand.Execute();
            ValidateItems();
        }

        private void ButtonMapZoomToExtent_Click(object sender, RoutedEventArgs e)
        {
            mapZoomToExtentsCommand.Execute();
            ValidateItems();
        }

        private void ButtonMapPanZoom_Click(object sender, RoutedEventArgs e)
        {
            mapPanZoomCommand.Execute();
            ValidateItems();
        }

        private void ButtonMapZoomPrevious_Click(object sender, RoutedEventArgs e)
        {
            mapZoomPreviousCommand.Execute();
            ValidateItems();
        }

        private void ButtonMapZoomNext_Click(object sender, RoutedEventArgs e)
        {
            mapZoomNextCommand.Execute();
            ValidateItems();
        }

        private void ButtonMapChangeCoordinateSystem_Click(object sender, RoutedEventArgs e)
        {
            mapChangeCoordinateSystemCommand.Execute();
            ValidateItems();
        }
    }
}