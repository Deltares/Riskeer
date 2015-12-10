﻿using System.ComponentModel;
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Utils.Attributes;
using Core.GIS.SharpMap.UI.Tools.Decorations;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [DisplayName("Tool properties")]
    public class LayoutComponentToolProperties : ObjectProperties<LayoutComponentTool>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [DisplayName("Anchor position")]
        public AnchorStyles Anchor
        {
            get
            {
                return data.Anchor;
            }
            set
            {
                data.Anchor = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [DisplayName("Use anchor position")]
        [Description("Align using the anchor property")]
        public bool UseAnchor
        {
            get
            {
                return data.UseAnchor;
            }
            set
            {
                data.UseAnchor = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [DisplayName("Background transparency %")]
        [Description("Percentage of transparency for the background.")]
        public int TransparencyPercentage
        {
            get
            {
                return data.BackGroundTransparencyPercentage;
            }
            set
            {
                data.BackGroundTransparencyPercentage = value;
            }
        }
    }
}