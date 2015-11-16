using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Map;
using Core.Plugins.SharpMapGis.Gui.Properties;
using log4net;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapAddLayerCommand : MapViewCommand
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapAddLayerCommand));

        private IEnumerable<ILayer> addedLayers;

        /// <summary>
        /// Shows a file dialog for adding layers
        /// </summary>
        /// <returns>The path that the user has chosen (default = "")</returns>
        public IEnumerable<string> ShowAddLayerFileDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = FileBasedLayerFactory.SupportedFormats,
                CheckFileExists = true,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileNames;
            }
            return new string[]
            {};
        }

        public void AddLayerFromFile(string path, Map mapToUse = null)
        {
            Map map = null;

            if (MapView != null)
            {
                map = MapView.Data as Map;
            }

            if (mapToUse != null)
            {
                map = mapToUse;
            }

            if (map == null)
            {
                return;
            }

            addedLayers = FileBasedLayerFactory.CreateLayersFromFile(path).ToList();
            foreach (ILayer layer in addedLayers)
            {
                map.Layers.Insert(0, layer);
            }
        }

        protected override void OnExecute(params object[] arguments)
        {
            string path = null;
            if (arguments.Length > 0)
            {
                path = (string) arguments[0];
            }

            if (File.Exists(path))
            {
                TryAddLayerFromFile(path);
                return;
            }
            foreach (string file in ShowAddLayerFileDialog())
            {
                TryAddLayerFromFile(file);
            }
        }

        private void TryAddLayerFromFile(string file)
        {
            try
            {
                AddLayerFromFile(file);
            }
            catch (Exception e)
            {
                var message = string.Format(Resources.MapAddLayerCommand_TryAddLayerFromFile_Cannot_create_layer_s__from_file__0____1_, file, e.Message);
                log.Error(message);
                MessageBox.Show(message, Resources.MapAddLayerCommand_TryAddLayerFromFile_Layer_creation_failed,
                                MessageBoxButtons.OK);
            }
        }
    }
}