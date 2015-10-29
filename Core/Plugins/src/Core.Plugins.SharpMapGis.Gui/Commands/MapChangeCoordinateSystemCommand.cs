using System.Windows.Forms;
using Core.Common.Controls.Swf;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Extensions.CoordinateSystems;
using Core.GIS.SharpMap.UI.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapChangeCoordinateSystemCommand : MapViewCommand
    {
        public override bool Checked
        {
            get
            {
                return false;
            }
            set {}
        }

        protected override void OnExecute(params object[] arguments)
        {
            var activeView = SharpMapGisGuiPlugin.GetFocusedMapView();

            var selectCoordinateSystemDialog = new SelectCoordinateSystemDialog(OgrCoordinateSystemFactory.SupportedCoordinateSystems, GIS.SharpMap.Map.Map.CoordinateSystemFactory.CustomCoordinateSystems);

            if (ModalHelper.ShowModal(selectCoordinateSystemDialog) == DialogResult.OK)
            {
                try
                {
                    activeView.Map.CoordinateSystem = selectCoordinateSystemDialog.SelectedCoordinateSystem;

                    activeView.Map.ZoomToExtents();
                }
                catch (CoordinateTransformException e)
                {
                    MessageBox.Show("Cannot convert map to selected coordinate system: " + e.Message,
                                    "Map coordinate system", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    base.OnExecute(arguments);
                }
            }

            base.OnExecute(arguments);
        }
    }
}