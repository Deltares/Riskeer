using System;
using System.Collections.Generic;
using Core.Common.Gui;
using Core.Components.DotSpatial.Data;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// This class describes the command for opening a <see cref="Core.Plugins.DotSpatial.Forms.MapDataView"/> with some arbitrary data.
    /// </summary>
    public class OpenMapViewCommand : IGuiCommand
    {
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

        public IGui Gui { get; set; }

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

            Gui.DocumentViewsResolver.OpenViewForData(data);
        }
    }
}