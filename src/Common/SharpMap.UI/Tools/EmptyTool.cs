using System;
using System.Windows.Forms;
using GeoAPI.Geometries;
using SharpMap.UI.Mapping;
using SharpMap.UI.Snapping;

namespace SharpMap.UI.Tools
{
    public class EmptyTool : IEditTool
    {
        #region IEditTool Members

        public EditorToolRole Role
        {
            get { return EditorToolRole.None; }
        }

        public void MouseDown(GeoAPI.Geometries.ICoordinate worldPos, MouseEventArgs imagePos)
        {
        }

        public void MouseMove(GeoAPI.Geometries.ICoordinate worldPos, MouseEventArgs imagePos)
        {
        }

        public void MouseUp(GeoAPI.Geometries.ICoordinate worldPos, MouseEventArgs imagePos)
        {
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
        }

        public void MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        public void ActiveToolChanged(IEditTool newTool)
        {
        }

        public ILayerEditor EditorLayer
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public virtual ISnapStrategy Snap(ILayerEditor sourceEditorLayer, IGeometry snapSource, ICoordinate worldPos, IEnvelope Envelope)
        {
            return null;
        }

        public bool IsEditing
        {
            get { return false; }
        }

        #endregion
    }
}
