using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.GIS.GeoAPI.CoordinateSystems;

namespace Core.GIS.SharpMap.UI.Forms
{
    public partial class SelectCoordinateSystemDialog : Form
    {
        private readonly List<TreeNode> gcsNodes = new List<TreeNode>();
        private readonly List<TreeNode> pcsNodes = new List<TreeNode>();

        private readonly IList<ICoordinateSystem> supportedCoordinateSystems;
        private readonly IList<ICoordinateSystem> customCoordinateSystems;

        private ICoordinateSystem selectedCoordinateSystem;
        private readonly string geographicNodeName = "geographic";
        private readonly string projectedNodeName = "projected";

        public SelectCoordinateSystemDialog(IList<ICoordinateSystem> supportedCoordinateSystems, IList<ICoordinateSystem> customCoordinateSystems)
        {
            this.supportedCoordinateSystems = supportedCoordinateSystems;
            this.customCoordinateSystems = customCoordinateSystems;

            InitializeComponent();
        }

        public ICoordinateSystem SelectedCoordinateSystem
        {
            get
            {
                if (treeViewProjections.Nodes.Count > 0)
                {
                    var selectedNode = treeViewProjections.SelectedNode;
                    if (selectedNode != null)
                    {
                        return selectedNode.Tag as ICoordinateSystem;
                    }
                }
                return null;
            }
            set
            {
                selectedCoordinateSystem = value;

                UpdateSelectedCoordinateSystemNode();
            }
        }

        public Func<ICoordinateSystem, bool> CoordinateSystemFilter { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            CenterToScreen();

            FillProjectionsTreeView();

            treeViewProjections.ExpandAll();

            treeViewProjections.AfterSelect += TreeViewProjectionsOnAfterSelect;

            if (treeViewProjections.Nodes.ContainsKey(geographicNodeName))
            {
                gcsNodes.AddRange(((TreeNode) treeViewProjections.Nodes[geographicNodeName].Clone()).Nodes.Cast<TreeNode>());
            }
            // customNodes.AddRange(((TreeNode)treeViewProjections.Nodes[2].Clone()).Nodes.Cast<TreeNode>());

            if (treeViewProjections.Nodes.ContainsKey(projectedNodeName))
            {
                pcsNodes.AddRange(((TreeNode) treeViewProjections.Nodes[projectedNodeName].Clone()).Nodes.Cast<TreeNode>());
            }

            treeViewProjections.TopNode = treeViewProjections.Nodes[0];

            UpdateSelectedCoordinateSystemNode();

            textBoxFilter.Select();
        }

        private void UpdateSelectedCoordinateSystemNode()
        {
            if (selectedCoordinateSystem == null)
            {
                if (treeViewProjections.Nodes.Count > 0)
                {
                    treeViewProjections.SelectedNode = treeViewProjections.Nodes[geographicNodeName];
                }

                return; // no coordinate system selected
            }

            var node = gcsNodes.Concat(pcsNodes).FirstOrDefault(n => Equals(((ICoordinateSystem) n.Tag).WKT, selectedCoordinateSystem.WKT));
            if (node == null)
            {
                return; // can't find node for a given coordinate system
            }

            treeViewProjections.SelectedNode = node;
        }

        private bool ValidCoordinateSystem(ICoordinateSystem coordinateSystem)
        {
            return CoordinateSystemFilter == null || CoordinateSystemFilter(coordinateSystem);
        }

        private void FillProjectionsTreeView()
        {
            treeViewProjections.Nodes.Clear();

            var validCoordinateSystems =
                supportedCoordinateSystems.Where(ValidCoordinateSystem).GroupBy(cs => cs.IsGeographic).ToList();

            var cartesianSystems = validCoordinateSystems.FirstOrDefault(g => g.Key);

            if (cartesianSystems != null && cartesianSystems.Any())
            {
                var cartesionNode = treeViewProjections.Nodes.Add(geographicNodeName, "Geographic Coordinate Systems", 0);

                foreach (var coordinateSystem in cartesianSystems)
                {
                    var name = GetTitle(coordinateSystem);
                    var childNode = cartesionNode.Nodes.Add(name, name, 1);
                    childNode.SelectedImageIndex = 1;
                    childNode.Tag = coordinateSystem;
                }
            }

            var sphericalSystems = validCoordinateSystems.FirstOrDefault(g => !g.Key);

            if (sphericalSystems != null && sphericalSystems.Any())
            {
                var sphericalNode = treeViewProjections.Nodes.Add(projectedNodeName, "Projected Coordinate Systems", 0);

                foreach (var coordinateSystem in sphericalSystems)
                {
                    var name = GetTitle(coordinateSystem);
                    var childNode = sphericalNode.Nodes.Add(name, name, 2);
                    childNode.SelectedImageIndex = 2;
                    childNode.Tag = coordinateSystem;
                }
            }

            if (customCoordinateSystems != null)
            {
                var customNode = treeViewProjections.Nodes.Add("custom", "Custom Coordinate Systems", 0);

                foreach (var coordinateSystem in customCoordinateSystems)
                {
                    var name = GetTitle(coordinateSystem);
                    var childNode = customNode.Nodes.Add(name, name, 2);
                    childNode.SelectedImageIndex = 2;
                    childNode.Tag = coordinateSystem;
                }
            }
        }

        private static string GetTitle(ICoordinateSystem coordinateSystem)
        {
            return string.Format("{0} [EPSG:{1}]", coordinateSystem.Name, coordinateSystem.AuthorityCode);
        }

        private void TreeViewProjectionsOnAfterSelect(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            if (treeViewProjections.SelectedNode != null && treeViewProjections.SelectedNode.Tag is ICoordinateSystem)
            {
                var crs = treeViewProjections.SelectedNode.Tag as ICoordinateSystem;

                try
                {
                    textBoxSrs.Text = "PROJ.4: " + crs.PROJ4;
                }
                catch (Exception e)
                {
                    textBoxSrs.Text = "PROJ.4: " + e.Message;
                }

                textBoxSrs.Text += "\r\n\r\nWKT:\r\n" + crs.WKT.Replace("\n", "\r\n");
            }
            else
            {
                textBoxSrs.Text = "";
            }
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            FilterTree();
        }

        private void FilterTree()
        {
            var gcsNodesFiltered = new List<TreeNode>();
            var pcsNodesFiltered = new List<TreeNode>();
            var filterText = textBoxFilter.Text;

            treeViewProjections.BeginUpdate();
            try
            {
                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    var filter = filterText.ToLower().TrimStart(' ').TrimEnd(' ');
                    gcsNodesFiltered.AddRange(from node in gcsNodes where node.Name.ToLower().Contains(filter) select (TreeNode)node.Clone());
                    pcsNodesFiltered.AddRange(from node in pcsNodes where node.Name.ToLower().Contains(filter) select (TreeNode)node.Clone());
                }
                else
                {
                    gcsNodesFiltered.AddRange(from node in gcsNodes select (TreeNode)node.Clone());
                    pcsNodesFiltered.AddRange(from node in pcsNodes select (TreeNode)node.Clone());
                }

                if (treeViewProjections.Nodes.ContainsKey(geographicNodeName))
                {
                    var geographicNode = treeViewProjections.Nodes[geographicNodeName];

                    geographicNode.Nodes.Clear();
                    geographicNode.Nodes.AddRange(gcsNodesFiltered.ToArray());

                    if (geographicNode.IsExpanded)
                    {
                        geographicNode.Expand();
                    }
                }
                if (treeViewProjections.Nodes.ContainsKey(projectedNodeName))
                {
                    var projectedNode = treeViewProjections.Nodes[projectedNodeName];
                    projectedNode.Nodes.Clear();
                    projectedNode.Nodes.AddRange(pcsNodesFiltered.ToArray());

                    if (projectedNode.IsExpanded)
                    {
                        projectedNode.Expand();
                    }
                }

                treeViewProjections.TopNode = treeViewProjections.Nodes[0];
                treeViewProjections.SelectedNode = treeViewProjections.Nodes[0];
            }
            finally
            {
                treeViewProjections.EndUpdate();
            }
        }
    }
}