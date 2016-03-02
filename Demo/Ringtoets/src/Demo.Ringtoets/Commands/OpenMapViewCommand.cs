using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Components.Gis.Data;
using Demo.Ringtoets.Properties;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// The command for opening a view for <see cref="MapData"/> with some arbitrary data.
    /// </summary>
    public class OpenMapViewCommand : ICommand
    {
        private readonly IDocumentViewController documentViewController;

        /// <summary>
        /// Creates a new instance of <see cref="OpenMapViewCommand"/>.
        /// </summary>
        /// <param name="documentViewController">The <see cref="IDocumentViewController"/> to use internally.</param>
        public OpenMapViewCommand(IDocumentViewController documentViewController)
        {
            this.documentViewController = documentViewController;
        }

        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public void Execute(params object[] arguments)
        {
            var points = new MapPointData(new Collection<Point2D>
            {
                new Point2D(1.5, 2),
                new Point2D(1.1, 1),
                new Point2D(0.8, 0.5)
            }, Resources.OpenMapViewCommand_Execute_Point_demo_data);
            var lines = new MapLineData(new Collection<Point2D>
            {
                new Point2D(0.0, 1.1),
                new Point2D(1.0, 2.1),
                new Point2D(1.6, 1.6)
            }, Resources.OpenMapViewCommand_Execute_Line_demo_data);
            var polygons = new MapPolygonData(new Collection<Point2D>
            {
                new Point2D(1.0, 1.3),
                new Point2D(3.0, 2.6),
                new Point2D(5.6, 1.6)
            }, Resources.OpenMapViewCommand_Execute_Polygon_demo_data);

            documentViewController.DocumentViewsResolver.OpenViewForData(new MapDataCollection(new List<MapData>
            {
                points, lines, polygons
            }, Resources.OpenMapViewCommand_Execute_List));
        }
    }
}