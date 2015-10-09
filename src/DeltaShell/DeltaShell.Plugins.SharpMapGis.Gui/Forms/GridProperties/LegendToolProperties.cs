using System.Drawing;
using DelftTools.Utils;
using DeltaShell.Plugins.SharpMapGis.Gui.Properties;
using SharpMap.UI.Tools.Decorations;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "LegendToolProperties_DisplayName")]
    public class LegendToolProperties : LayoutComponentToolProperties
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "LegendToolProperties_Font_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LegendToolProperties_Font_Description")]
        public Font Font
        {
            get
            {
                return LegendTool.LegendFont;
            }
            set
            {
                LegendTool.LegendFont = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "LegendToolProperties_Padding_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LegendToolProperties_Padding_Description")]
        public Size Padding
        {
            get
            {
                return LegendTool.Padding;
            }
            set
            {
                LegendTool.Padding = value;
            }
        }

        private LegendTool LegendTool
        {
            get
            {
                return data as LegendTool;
            }
        }
    }
}