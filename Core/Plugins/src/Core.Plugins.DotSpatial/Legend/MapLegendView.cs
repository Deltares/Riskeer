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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial;
using Core.Components.Gis.Data;

using DotSpatialResources = Core.Plugins.DotSpatial.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.DotSpatial.Legend
{
    /// <summary>
    /// The view which shows the data that is added to a <see cref="BaseMap"/>.
    /// </summary>
    public sealed partial class MapLegendView : UserControl, IView
    {
        private readonly TreeViewControl treeViewControl;

        /// <summary>
        /// Creates a new instance of <see cref="MapLegendView"/>.
        /// </summary>
        public MapLegendView()
        {
            InitializeComponent();
            Text = DotSpatialResources.General_Map;

            treeViewControl = new TreeViewControl
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(treeViewControl);

            RegisterTreeNodeInfos();
        }

        public object Data
        {
            get
            {
                return (MapData) treeViewControl.Data;
            }
            set
            {
                if (IsDisposed)
                {
                    return;
                }

                treeViewControl.Data = (MapData) value;
            }
        }

        private void RegisterTreeNodeInfos()
        {
            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapPointData>
            {
                Text = mapPointData => DotSpatialResources.MapDataNodePresenter_Point_data_label,
                Image = mapPointData => DotSpatialResources.PointsIcon
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapLineData>
            {
                Text = mapLineData => DotSpatialResources.MapDataNodePresenter_Line_data_label,
                Image = mapLineData => DotSpatialResources.LineIcon
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapPolygonData>
            {
                Text = mapPolygonData => DotSpatialResources.MapDataNodePresenter_Polygon_data_label,
                Image = mapPolygonData => DotSpatialResources.AreaIcon
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapDataCollection>
            {
                Text = mapDataCollection => DotSpatialResources.General_Map,
                Image = mapDataCollection => GuiResources.folder,
                ChildNodeObjects = mapDataCollection => mapDataCollection.List.Reverse().Cast<object>().ToArray()
            });
        }
    }
}