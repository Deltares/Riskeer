// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Core.Components.Gis.IO.Importers;

namespace Application.Riskeer
{
    public class TestMapView : Control, IMapView
    {
        private readonly MapControl mapControl;

        public TestMapView()
        {
            var mapDataCollection = new MapDataCollection("test");

            new FeatureBasedMapDataImporter(mapDataCollection, GetFilePath("traject_6-3.shp")).Import();

            mapControl = new MapControl
            {
                Data = mapDataCollection,
                Dock = DockStyle.Fill
            };

            Controls.Add(mapControl);
        }

        public object Data { get; set; }

        public IMapControl Map => mapControl;

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(Path.GetDirectoryName(GetThisFilePath()), fileName);
        }

        private static string GetThisFilePath([CallerFilePath] string path = null)
        {
            return path;
        }
    }
}