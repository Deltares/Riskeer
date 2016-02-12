using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Components.DotSpatial.Data;

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
            var points = new MapPointData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(1.5, 2),
                new Tuple<double, double>(1.1, 1),
                new Tuple<double, double>(0.8, 0.5)
            });
            var lines = new MapLineData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),
                new Tuple<double, double>(1.0, 2.1),
                new Tuple<double, double>(1.6, 1.6)
            });
            var polygons = new MapPolygonData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(1.0, 1.3),
                new Tuple<double, double>(3.0, 2.6),
                new Tuple<double, double>(5.6, 1.6)
            });

            documentViewController.DocumentViewsResolver.OpenViewForData(new MapDataCollection(new List<MapData> { points, lines, polygons }));
        }
    }
}