using System;
using System.Windows.Forms;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.UI.Forms;
using Core.GIS.SharpMap.UI.Tools;
using Core.GIS.SharpMap.UI.Tools.Zooming;
using NUnit.Framework;

namespace Core.GIS.SharpMap.UI.Tests.Forms
{
    [TestFixture]
    public class GeometryEditorTests
    {
        private Form geometryEditorForm;
        private MapControl mapControl;

        private ListBox listBoxTools;

        [Test]
        public void DefaultMapControlTools()
        {
            InitializeControls();

            // check for all default tools
            IMapTool mapTool = mapControl.SelectTool;
            Assert.IsNotNull(mapTool);
            SelectTool selectTool = mapTool as SelectTool;
            Assert.IsNotNull(selectTool);

            mapTool = mapControl.MoveTool;
            Assert.IsNotNull(mapTool);

            MoveTool moveTool = mapTool as MoveTool;
            Assert.IsNotNull(moveTool);
            Assert.AreEqual(FallOffType.None, moveTool.FallOffPolicy);

            mapTool = mapControl.GetToolByName("CurvePoint");
            Assert.IsNotNull(mapTool);
            CurvePointTool curvePointTool = mapTool as CurvePointTool;
            Assert.IsNotNull(curvePointTool);
        }

        private void InitializeControls()
        {
            geometryEditorForm = new Form();
            // Create map and map control
            Map.Map map = new Map.Map();

            mapControl = new MapControl();
            mapControl.Map = map;
            mapControl.Resize += delegate { mapControl.Refresh(); };
            mapControl.ActivateTool(mapControl.GetToolByType<PanZoomTool>());
            mapControl.Dock = DockStyle.Fill;
            // disable dragdrop because it breaks the test runtime
            mapControl.AllowDrop = false;

            // Create listbox to show all registered tools
            listBoxTools = new ListBox();
            listBoxTools.Dock = DockStyle.Left;
            listBoxTools.SelectedIndexChanged += listBoxTools_SelectedIndexChanged;

            map.ZoomToExtents();

            geometryEditorForm.Controls.Add(listBoxTools);
            geometryEditorForm.Controls.Add(mapControl);
        }

        private void listBoxTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (-1 != listBoxTools.SelectedIndex)
            {
                mapControl.ActivateTool(mapControl.GetToolByName(listBoxTools.Items[listBoxTools.SelectedIndex].ToString()));
            }
        }
    }
}