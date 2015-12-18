using System.Windows.Forms;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Extensions.CoordinateSystems;
using Core.GIS.SharpMap.UI.Forms;
using Core.Plugins.SharpMapGis.Gui.Properties;

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
        }

        public override void Execute(params object[] arguments)
        {
            var activeView = SharpMapGisGuiPlugin.GetFocusedMapView();

            using (var selectCoordinateSystemDialog = new SelectCoordinateSystemDialog(Gui.MainWindow, OgrCoordinateSystemFactory.SupportedCoordinateSystems, GIS.SharpMap.Map.Map.CoordinateSystemFactory.CustomCoordinateSystems))
            {
                if (selectCoordinateSystemDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        activeView.Map.CoordinateSystem = selectCoordinateSystemDialog.SelectedCoordinateSystem;
                        activeView.Map.NotifyObservers();
                        activeView.Map.ZoomToExtents();
                    }
                    catch (CoordinateTransformException e)
                    {
                        var message = string.Format(Resources.MapChangeCoordinateSystemCommand_OnExecute_Cannot_convert_map_to_selected_coordinate_system_0_, e.Message);
                        MessageBox.Show(message, Resources.MapChangeCoordinateSystemCommand_OnExecute_Map_coordinate_system, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}