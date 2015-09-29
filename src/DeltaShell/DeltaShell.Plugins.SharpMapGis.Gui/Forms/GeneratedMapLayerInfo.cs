using DelftTools.Utils.Data;
using DelftTools.Utils.Reflection;
using SharpMap.Api;
using SharpMap.Api.Layers;
using SharpMap.Layers;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms
{
    public class GeneratedMapLayerInfo : Unique<long>
    {
        public GeneratedMapLayerInfo()
        {
        }

        public GeneratedMapLayerInfo(ILayer layer)
        {
            Name = layer.Name;
            Visible = layer.Visible;
            AutoUpdateThemeOnDataSourceChanged = layer.AutoUpdateThemeOnDataSourceChanged;
            MaxVisible = layer.MaxVisible;
            MinVisible = layer.MinVisible;
            RenderOrder = layer.RenderOrder;
            Selectable = layer.Selectable;
            ShowInLegend = layer.ShowInLegend;
            ShowAttributeTable = layer.ShowAttributeTable;
            CanBeRemovedByUser = layer.CanBeRemovedByUser;

            if (TypeUtils.GetField(layer, "labelLayer") != null)
            {
                ShowLabels = layer.ShowLabels;
            
                LabelColumn = layer.LabelLayer.LabelColumn;
                LabelShowInTreeView = layer.LabelLayer.ShowInTreeView;
                LabelStyle = (LabelStyle) layer.LabelLayer.Style.Clone();
            }
            
            var theme = layer is Layer
                            ? TypeUtils.GetField(layer, "theme") as ITheme // Performance optimization
                            : layer.Theme;

            if (theme != null)
            {
                Theme = (ITheme)theme.Clone();
            }

            var vectorLayer = layer as VectorLayer;
            if (vectorLayer == null)
                return;

            VectorStyle = (VectorStyle) vectorLayer.Style.Clone();
        }

        public virtual string Name { get; set; }
        public virtual string ParentPath { get; set; }

        public virtual int RenderOrder { get; set; }
        public virtual double MaxVisible { get; set; }
        public virtual double MinVisible { get; set; }

        public virtual bool AutoUpdateThemeOnDataSourceChanged { get; set; }
        public virtual bool Visible { get; set; }
        public virtual bool Selectable { get; set; }
        public virtual bool ShowLabels { get; set; }
        public virtual bool ShowInLegend { get; set; }
        public virtual bool ShowAttributeTable { get; set; }
        public virtual bool CanBeRemovedByUser { get; set; }

        public virtual VectorStyle VectorStyle { get; set; }
        public virtual ITheme Theme { get; set; }
        public virtual LabelStyle LabelStyle { get; set; }
        public virtual string LabelColumn { get; set; }
        public virtual bool LabelShowInTreeView { get; set; }

        public virtual void SetToLayer(ILayer layer)
        {
            if (!layer.NameIsReadOnly)
            {
                layer.Name = Name;
            }

            layer.Visible = Visible;
            layer.AutoUpdateThemeOnDataSourceChanged = AutoUpdateThemeOnDataSourceChanged;
            layer.MaxVisible = MaxVisible;
            layer.MinVisible = MinVisible;
            layer.RenderOrder = RenderOrder;
            layer.Selectable = Selectable;
            layer.ShowInLegend = ShowInLegend;
            layer.ShowAttributeTable = ShowAttributeTable;
            layer.CanBeRemovedByUser = CanBeRemovedByUser;
            layer.Theme = Theme;

            if (LabelStyle != null && LabelColumn != null)
            {
                layer.LabelLayer = new LabelLayer
                                       {
                                           LabelColumn = LabelColumn,
                                           ShowInTreeView = LabelShowInTreeView,
                                           Style = (LabelStyle) LabelStyle.Clone()
                                       };
                layer.ShowLabels = ShowLabels;
            }

            var vectorLayer = layer as VectorLayer;
            if (vectorLayer == null)
                return;

            vectorLayer.Style = VectorStyle;
        }
    }
}