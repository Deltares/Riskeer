using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GeoAPI.Extensions.Feature;
using SharpMap.UI.Tools;

namespace SharpMap.UI.Forms
{
    public interface IMapControl
    {
        Map Map { get; set; }

        Image Image { get; }

        IList<IMapTool> Tools { get; }

        MoveTool MoveTool { get; }
        
        SelectTool SelectTool { get; }
        
        SnapTool SnapTool { get; }

        /// <summary>
        /// Gets the name of the tool by.
        /// </summary>
        /// <param name="toolName">Name of the tool.</param>
        /// <returns>An instance of IMapTool matching the given name, or null if no match was found.</returns>
        /// <remarks>Do not throw ArgumentOutOfRangeException UI handlers (button checked) can ask for not existing tool</remarks>
        IMapTool GetToolByName(string toolName);

        T GetToolByType<T>() where T : class;

        void ActivateTool(IMapTool tool);
        
        IEnumerable<IFeature> SelectedFeatures { get; set; }

        // common control methods
        Cursor Cursor { get; set; }

        /// <summary>
        /// Does a refresh when the timer ticks.
        /// </summary>
        void Refresh();
        Graphics CreateGraphics();
        Color BackColor { get; }
        Size ClientSize { get; }
        Size Size { get; }
        int Height { get; }
        int Width { get; }
        Rectangle ClientRectangle { get; }
        Point PointToScreen(Point location);
        Point PointToClient(Point p);

        void Invalidate(Rectangle rectangle);


        event MouseEventHandler MouseUp;

        event KeyEventHandler KeyUp;

        event KeyEventHandler KeyDown;

        /// <summary>
        /// True if map control is busy processing something in a separate thread.
        /// </summary>
        bool IsProcessing { get; }

        event EventHandler SelectedFeaturesChanged;
    }
}
