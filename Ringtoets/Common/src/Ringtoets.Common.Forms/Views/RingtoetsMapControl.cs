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

using Core.Common.Base;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Utils.TypeConverters;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class describes a map view with background.
    /// </summary>
    public class RingtoetsMapControl : MapControl
    {
        private readonly Observer backgroundDataObserver;

        private BackgroundData backgroundData;

        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsMapControl"/>.
        /// </summary>
        public RingtoetsMapControl()
        {
            backgroundDataObserver = new Observer(OnBackgroundDataUpdated);
        }

        /// <summary>
        /// Gets ans sets the <see cref="BackgroundData"/>.
        /// </summary>
        public BackgroundData BackgroundData
        {
            get
            {
                return backgroundData;
            }
            set
            {
                backgroundData = value;
                backgroundDataObserver.Observable = backgroundData;

                BackgroundMapData = backgroundData == null
                                        ? null
                                        : RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(backgroundData);
            }
        }

        public override void RemoveAllData()
        {
            Removing = true;
            BackgroundData = null;
            Data = null;
            Removing = false;
        }

        protected override void Dispose(bool disposing)
        {
            backgroundDataObserver.Dispose();

            base.Dispose(disposing);
        }

        private void OnBackgroundDataUpdated()
        {
            if (backgroundData.Configuration is WmtsBackgroundDataConfiguration && BackgroundMapData is WmtsMapData
                || backgroundData.Configuration is WellKnownBackgroundDataConfiguration && BackgroundMapData is WellKnownTileSourceMapData)
            {
                UpdateBackgroundMapData();
                BackgroundMapData.NotifyObservers();
            }
            else
            {
                BackgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(backgroundData);
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
                    ((WmtsMapData) BackgroundMapData).Configure(newWmtsData.SourceCapabilitiesUrl,
                                                                newWmtsData.SelectedCapabilityIdentifier,
                                                                newWmtsData.PreferredFormat);
                }
                else
                {
                    ((WmtsMapData) BackgroundMapData).RemoveConfiguration();
                }
            }
            else if (backgroundData.Configuration is WellKnownBackgroundDataConfiguration)
            {
                ((WellKnownTileSourceMapData) BackgroundMapData).TileSource = ((WellKnownTileSourceMapData) newData).TileSource;
            }

            BackgroundMapData.IsVisible = newData.IsVisible;
            BackgroundMapData.Name = newData.Name;
            BackgroundMapData.Transparency = newData.Transparency;
        }
    }
}