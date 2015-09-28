using System;
using System.Collections.Generic;
using System.Linq;
using SharpMap.Api.Layers;
using SharpMap.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms
{
    public static class GeneratedMapLayerInfoUpgradeExtension
    {
        /// <summary>
        /// Upgrades old parent settings of GeneratedMapLayerInfo (level and parentName) to new parent path
        /// </summary>
        /// <param name="generatedMapLayerInfoList">List of GeneratedMapLayerInfo's to upgrade</param>
        /// <param name="generatedLayer">Generated map layer to extract the paths from</param>
        public static void UpgradeToParentPaths(this IList<GeneratedMapLayerInfo> generatedMapLayerInfoList, ILayer generatedLayer)
        {
            if (!generatedMapLayerInfoList.Any(mi => mi.ParentPath.StartsWith("*"))) return;

            SetParentPath(generatedLayer, 0, "", "", generatedMapLayerInfoList.GroupBy(mi => GetLevel(mi.ParentPath)).ToDictionary(g => g.Key, g => g.ToList()));
        }

        private static int GetLevel(string parentPath)
        {
            return Convert.ToInt32(GetLevelAndParentName(parentPath)[0]);
        }

        private static string GetParentName(string parentPath)
        {
            return GetLevelAndParentName(parentPath)[1];
        }

        private static string[] GetLevelAndParentName(string parentPath)
        {
            if (!parentPath.StartsWith("*"))
            {
                return new[] {"-1", parentPath};
            }

            return parentPath.TrimStart('*').Split(';');
        }

        private static void SetParentPath(ILayer layer, int level, string path, string parentName, IDictionary<int, List<GeneratedMapLayerInfo>> mapInfosByLevel)
        {
            List<GeneratedMapLayerInfo> layerInfoList;
            mapInfosByLevel.TryGetValue(level, out layerInfoList);

            if (layerInfoList != null)
            {
                var layerInfo = layerInfoList.FirstOrDefault(l => l.Name == layer.Name && GetParentName(l.ParentPath) == parentName);
                if (layerInfo != null)
                {
                    layerInfo.ParentPath = path;
                }
            }

            var groupLayer = layer as GroupLayer;
            if (groupLayer == null)
                return;

            var parentPath = String.Format("{0}\\{1}", path, groupLayer.Name);
            var subLayers = groupLayer.Layers.ToList();

            foreach (var subLayer in subLayers)
            {
                SetParentPath(subLayer, level + 1, parentPath, groupLayer.Name, mapInfosByLevel);
            }
        }
    }
}