using System.ComponentModel;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Swf;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class VectorStyleTreeViewNodePresenter : TreeViewNodePresenterBaseForPluginGui<VectorStyle>
    {
        public VectorStyleTreeViewNodePresenter() : base(null) {}

        public VectorStyleTreeViewNodePresenter(GuiPlugin guiPlugin) : base(guiPlugin) {}

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, VectorStyle style)
        {
            if (node.Tag != style)
            {
                node.Tag = style;
            }

            if (node.Text != string.Empty)
            {
                node.Text = string.Empty;
            }

            if (node.Image != style.LegendSymbol)
            {
                node.Image = style.LegendSymbol;
            }
        }

        protected override void OnPropertyChanged(VectorStyle vectorStyle, ITreeNode node, PropertyChangedEventArgs e)
        {
            if (node == null)
            {
                return;
            }
            UpdateNode(null, node, vectorStyle);
        }
    }
}