﻿using Core.Common.Controls;
using Core.Common.Controls.Commands;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public abstract class CanvasCommand : Command
    {
        protected MapView CanvasEditor
        {
            get
            {
                return SharpMapGisGuiPlugin.GetFocusedMapView();
            }
        }

        public override void Execute(params object[] arguments) { }
    }
}