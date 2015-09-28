using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.TestUtils;
using GeoAPI.Geometries;
using NUnit.Framework;
using SharpMap.Api.Layers;
using SharpMap.UI.Forms;
using SharpMap.UI.Tools;

namespace SharpMap.UI.Tests.Tools
{
    [TestFixture]
    public class MapToolExample
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void MapToolMessageBoxEnabledTest()
        {
            var demoMapTool = new MapToolMessageBox();
            var mapControl = new MapControl() { Map = new Map(new Size(100, 100)) };
            mapControl.Tools.Add(demoMapTool);
            mapControl.ActivateTool(demoMapTool);
            demoMapTool.Enable();
            WindowsFormsTestHelper.ShowModal(mapControl);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void MapToolMessageBoxDisabledTest()
        {
            var demoMapTool = new MapToolMessageBox();
            var mapControl = new MapControl() { Map = new Map(new Size(100, 100)) };
            mapControl.Tools.Add(demoMapTool);
            mapControl.ActivateTool(demoMapTool);
            demoMapTool.Disable();
            WindowsFormsTestHelper.ShowModal(mapControl);
        }
    }

    public class MapToolMessageBox: IMapTool
    {
        private bool enabled = true;

        public void Disable()
        {
            enabled = false;
        }

        public void Enable()
        {
            enabled = true;
        }

        #region IMapTool Members

        public IMapControl MapControl
        {
            get; set;
        }

        public Cursor Cursor { get; set; }

        public void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e)
        {

        }

        public void OnBeforeMouseMove(ICoordinate worldPosition, MouseEventArgs e, ref bool handled)
        {
        }

        public void OnMouseMove(ICoordinate worldPosition, MouseEventArgs e)
        {

        }

        public void OnMouseUp(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (enabled)
            {
                MessageBox.Show("Hallo Rob", "Demo MapTool");
            }
        }

        public void OnMouseWheel(ICoordinate worldPosition, MouseEventArgs e)
        {

        }

        public void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        public void OnMouseHover(ICoordinate worldPosition, EventArgs e)
        {

        }

        public void OnKeyDown(KeyEventArgs e)
        {

        }

        public void OnKeyUp(KeyEventArgs e)
        {

        }

        public void OnPaint(PaintEventArgs e)
        {

        }

        public void Render(Graphics graphics, Map mapBox)
        {

        }

        public void OnMapLayerRendered(Graphics g, ILayer layer)
        {

        }

        public void OnMapPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        public void OnMapCollectionChanged(object sender, DelftTools.Utils.Collections.NotifyCollectionChangingEventArgs e)
        {

        }

        public IEnumerable<MapToolContextMenuItem> GetContextMenuItems(ICoordinate worldPosition)
        {
            yield break;
        }

        public void OnDragEnter(DragEventArgs e)
        {

        }

        public void OnDragDrop(DragEventArgs e)
        {

        }

        public bool IsBusy
        {
            get
            {
                return false;
            }
            set {
                enabled = value;
            }
        }

        public bool IsActive
        {
            get; set;
        }

        public bool Enabled
        {
            get { return enabled; }
        }

        public bool AlwaysActive
        {
            get { return false; }
        }

        public void Execute()
        {

        }

        public void Cancel()
        {

        }

        public string Name
        {
            get
            {
                return "";
            }
            set
            {
 }
        }

        public Func<ILayer, bool> LayerFilter { get; set; }

        public bool RendersInScreenCoordinates
        {
            get { return true; }
        }

        public IEnumerable<ILayer> Layers { get; private set; }

        public void ActiveToolChanged(IMapTool newTool)
        {
        }

        #endregion
    }
}
