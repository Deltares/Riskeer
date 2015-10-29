using System.ComponentModel;
using Core.Common.Controls;
using Core.Common.Gui.Swf;
using Core.GIS.SharpMap.Styles;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class VectorStyleTreeViewNodePresenter : TreeViewNodePresenterBaseForPluginGui<VectorStyle>
    {
        public VectorStyleTreeViewNodePresenter() : base(null) {}

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