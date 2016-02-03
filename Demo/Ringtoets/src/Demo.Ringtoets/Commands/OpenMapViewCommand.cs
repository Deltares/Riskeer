using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Components.DotSpatial.Data;
using Core.Plugins.DotSpatial.Forms;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// The command for opening a <see cref="MapDataView"/> with some arbitrary data.
    /// </summary>
    public class OpenMapViewCommand : ICommand
    {
        private readonly IDocumentViewController documentViewController;

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
                new Tuple<double, double>(10.5, 3),
                new Tuple<double, double>(11, 5),
                new Tuple<double, double>(11.5, 4)
            });

            documentViewController.DocumentViewsResolver.OpenViewForData(points);
        }
    }
}