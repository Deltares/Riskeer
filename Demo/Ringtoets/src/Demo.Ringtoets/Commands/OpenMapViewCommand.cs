using System;
using System.Collections.Generic;

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
            var data = new MapData();

            var paths = new List<string>
            {
                "Resources/DR10_dijkvakgebieden.shp",
                "Resources/DR10_cross_sections.shp",
                "Resources/DR10_dammen_caissons.shp"
            };

            foreach (string path in paths)
            {
                try
                {
                    data.AddShapeFile(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            documentViewController.DocumentViewsResolver.OpenViewForData(data);
        }
    }
}