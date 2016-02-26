﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using Core.Components.Gis;
using Fluent;

namespace Core.Plugins.DotSpatial
{
    /// <summary>
    /// This class represents the ribbon interaction which has to do with maps.
    /// </summary>
    public partial class MapRibbon : IRibbonCommandHandler
    {
        private IMap map;

        /// <summary>
        /// Creates a new instance of <see cref="MapRibbon"/>.
        /// </summary>
        public MapRibbon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the <see cref="IMap"/> to show the ribbon for.
        /// </summary>
        public IMap Map
        {
            private get
            {
                return map;
            }
            set
            {
                map = value;

                if (map != null)
                {
                    ShowMapTab();
                }
                else
                {
                    HideMapTab();
                }
            }
        }

        /// <summary>
        /// Sets the command used when the toggle legend view button is clicked.
        /// </summary>
        public ICommand ToggleLegendViewCommand { private get; set; }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                if (ToggleLegendViewCommand != null)
                {
                    yield return ToggleLegendViewCommand;
                }
            }
        }

        public Ribbon GetRibbonControl()
        {
            return RibbonControl;
        }

        public void ValidateItems()
        {
            ToggleLegendViewButton.IsChecked = ToggleLegendViewCommand != null && ToggleLegendViewCommand.Checked;
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            // TODO: Required only because this method is called each time ValidateItems is called in MainWindow
            // Once ValidateItems isn't responsible for showing/hiding contextual tabs, then this method can return false,
            // but more ideally be removed.
            return MapContextualGroup.Name == tabGroupName && MapContextualGroup.Visibility == Visibility.Visible;
        }

        private void ShowMapTab()
        {
            MapContextualGroup.Visibility = Visibility.Visible;
            ValidateItems();
        }

        private void HideMapTab()
        {
            MapContextualGroup.Visibility = Visibility.Collapsed;
        }

        private void ButtonToggleLegend_Click(object sender, RoutedEventArgs e)
        {
            ToggleLegendViewCommand.Execute();
        }

        private void ButtonZoomToAll_Click(object sender, RoutedEventArgs e)
        {
            Map.ZoomToAll();
        }
    }
}