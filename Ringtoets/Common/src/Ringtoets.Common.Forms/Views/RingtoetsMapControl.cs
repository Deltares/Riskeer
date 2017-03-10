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

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class describes a map view with background.
    /// </summary>
    public class RingtoetsMapControl : MapControl
    {
        private readonly Observer backgroundMapDataObserver;

        private BackgroundData backgroundData;

        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsMapControl"/>.
        /// </summary>
        public RingtoetsMapControl()
        {
            backgroundMapDataObserver = new Observer(UpdateBackgroundMapData);
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
                backgroundMapDataObserver.Observable = backgroundData;

                BackgroundMapData = backgroundData == null
                                        ? null
                                        : RingtoetsBackgroundMapDataFactory.CreateImageBasedBackgroundMapData(backgroundData);
            }
        }

        protected override void Dispose(bool disposing)
        {
            backgroundMapDataObserver.Dispose();

            base.Dispose(disposing);
        }

        private void UpdateBackgroundMapData()
        {
            if (backgroundData.BackgroundMapDataType == BackgroundMapDataType.Wmts && BackgroundMapData is WmtsMapData)
            {
                RingtoetsBackgroundMapDataFactory.UpdateBackgroundMapData((WmtsMapData) BackgroundMapData, backgroundData);
                BackgroundMapData.NotifyObservers();
            }
            else if(backgroundData.BackgroundMapDataType == BackgroundMapDataType.WellKnown && BackgroundMapData is WellKnownTileSourceMapData)
            {
//                RingtoetsBackgroundMapDataFactory.UpdateBackgroundMapData(BackgroundMapData, backgroundData);
            }
            else
            {
                BackgroundMapData = RingtoetsBackgroundMapDataFactory.CreateImageBasedBackgroundMapData(backgroundData);
            }
        }
    }
}