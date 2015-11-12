using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.UI.Tools
{
    public class DeleteTool : MapTool
    {
        public DeleteTool()
        {
            Name = "Delete";
        }

        public override bool AlwaysActive
        {
            get
            {
                return true;
            }
        }

        public override bool IsBusy
        {
            get
            {
                return false;
            }
        }

        public void DeleteSelection()
        {
            if (!MapControl.SelectTool.SelectedFeatureInteractors.Any(i => i.AllowDeletion()))
            {
                return;
            }

            var featuresDeleted = false;

            var interactors = MapControl.SelectTool.SelectedFeatureInteractors.Where(featureMutator => featureMutator.AllowDeletion()).ToArray();
            foreach (var interactor in interactors)
            {
                interactor.Delete();
                featuresDeleted = true;
            }

            if (!featuresDeleted)
            {
                return;
            }

            MapControl.SelectTool.Clear();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete)
            {
                return;
            }

            DeleteSelection();
        }

        public override IEnumerable<MapToolContextMenuItem> GetContextMenuItems(ICoordinate worldPosition)
        {
            var toolStripMenuItem = MapControl.SelectedFeatures == null || !MapControl.SelectedFeatures.Any()
                                        ? CreateContextMenuItemForFeaturesAtLocation(worldPosition, "Delete", DeleteFeature, false, (l, f) => !CanDeleteFeature(l, f))
                                        : (MapControl.SelectTool.SelectedFeatureInteractors.Any(i => i.AllowDeletion()) ? new ToolStripMenuItem("Delete selection", null, (sender, args) => DeleteSelection()) : null);

            if (toolStripMenuItem == null)
            {
                yield break;
            }

            yield return new MapToolContextMenuItem
            {
                Priority = 2,
                MenuItem = toolStripMenuItem
            };
        }

        private static bool CanDeleteFeature(ILayer layer, IFeature feature)
        {
            if (layer == null)
            {
                return false;
            }

            var interactor = layer.FeatureEditor == null ? null : layer.FeatureEditor.CreateInteractor(layer, feature);
            return interactor != null && interactor.AllowDeletion();
        }

        private static void DeleteFeature(ILayer layer, IFeature feature)
        {
            var interactor = layer.FeatureEditor == null ? null : layer.FeatureEditor.CreateInteractor(layer, feature);
            if (interactor == null)
            {
                return;
            }

            interactor.Delete();
            layer.RenderRequired = true;
        }
    }
}