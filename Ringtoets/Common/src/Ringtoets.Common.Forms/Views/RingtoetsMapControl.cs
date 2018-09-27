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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Util.TypeConverters;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Ringtoets map control with background data synchronization.
    /// </summary>
    public partial class RingtoetsMapControl : UserControl
    {
        private readonly Observer backgroundDataObserver;

        private BackgroundData backgroundData;

        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsMapControl"/>.
        /// </summary>
        public RingtoetsMapControl()
        {
            InitializeComponent();

            backgroundDataObserver = new Observer(OnBackgroundDataUpdated);
        }

        /// <summary>
        /// Gets the wrapped <see cref="IMapControl"/>.
        /// </summary>
        public IMapControl MapControl
        {
            get
            {
                return mapControl;
            }
        }

        /// <summary>
        /// Sets all data to the control.
        /// </summary>
        /// <param name="data">The <see cref="MapDataCollection"/> to set to the control.</param>
        /// <param name="background">The <see cref="BackgroundData"/> to set to the control.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters is <c>null</c>.</exception>
        public void SetAllData(MapDataCollection data, BackgroundData background)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (background == null)
            {
                throw new ArgumentNullException(nameof(background));
            }

            backgroundData = background;
            backgroundDataObserver.Observable = backgroundData;

            mapControl.Data = data;
            mapControl.BackgroundMapData = BackgroundDataConverter.ConvertFrom(backgroundData);
        }

        /// <summary>
        /// Removes all data from the control.
        /// </summary>
        public void RemoveAllData()
        {
            backgroundData = null;
            backgroundDataObserver.Observable = null;

            mapControl.RemoveAllData();
        }

        protected override void Dispose(bool disposing)
        {
            backgroundDataObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnBackgroundDataUpdated()
        {
            if (backgroundData.Configuration is WmtsBackgroundDataConfiguration && mapControl.BackgroundMapData is WmtsMapData
                || backgroundData.Configuration is WellKnownBackgroundDataConfiguration && mapControl.BackgroundMapData is WellKnownTileSourceMapData)
            {
                UpdateBackgroundMapData();
                mapControl.BackgroundMapData.NotifyObservers();
            }
            else
            {
                mapControl.BackgroundMapData = BackgroundDataConverter.ConvertFrom(backgroundData);
            }
        }

        private void UpdateBackgroundMapData()
        {
            ImageBasedMapData newData = BackgroundDataConverter.ConvertFrom(backgroundData);

            if (backgroundData.Configuration is WmtsBackgroundDataConfiguration)
            {
                if (newData.IsConfigured)
                {
                    var newWmtsData = (WmtsMapData) newData;
                    ((WmtsMapData) mapControl.BackgroundMapData).Configure(newWmtsData.SourceCapabilitiesUrl,
                                                                           newWmtsData.SelectedCapabilityIdentifier,
                                                                           newWmtsData.PreferredFormat);
                }
                else
                {
                    ((WmtsMapData) mapControl.BackgroundMapData).RemoveConfiguration();
                }
            }
            else if (backgroundData.Configuration is WellKnownBackgroundDataConfiguration)
            {
                ((WellKnownTileSourceMapData) mapControl.BackgroundMapData).SetTileSource(((WellKnownTileSourceMapData) newData).TileSource);
            }

            mapControl.BackgroundMapData.IsVisible = newData.IsVisible;
            mapControl.BackgroundMapData.Name = newData.Name;
            mapControl.BackgroundMapData.Transparency = newData.Transparency;
        }
    }
}