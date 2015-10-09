using System;
using System.Collections.Generic;
using System.Drawing;
using GeoAPI.Geometries;
using SharpMap.Api;
using SharpMap.Api.Layers;
using SharpMap.Layers;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;
using SharpMap.UI.Properties;

namespace SharpMap.UI.Tools.Decorations
{
    /// <summary>
    /// When this tool is active it displays a scalebar on the mapcontrol.
    /// </summary>
    public class LegendTool : LayoutComponentTool
    {
        private const int Indent = 10;
        private static readonly Bitmap layerThemeSymbol = Resources.legendlayergroupsymbol;
        private Size calculatedSize;
        private bool initScreenPosition;

        // Visual settings
        private Size padding = new Size(3, 3);
        private Font legendFont = new Font("Arial", 10);

        public LegendTool()
        {
            // LegendTool expects only top level layers, otherwise it gets confused
            LayerFilter = l => Map.Layers.Contains(l);
        }

        /// <summary>
        /// The size of the legend component (which can't be set but rather is a result of 
        /// the actual layer texts to show).
        /// </summary>
        public override Size Size
        {
            get
            {
                return calculatedSize;
            }
            set {} // Ignore new size
        }

        /// <summary>
        /// The amount of pixels skipped between the border and legend text (both top and bottom)
        /// </summary>
        public Size Padding
        {
            get
            {
                return padding;
            }
            set
            {
                padding = value;
            }
        }

        /// <summary>
        /// The font face to use for all text painted in the legend
        /// </summary>
        public Font LegendFont
        {
            get
            {
                return legendFont;
            }
            set
            {
                legendFont = value;
            }
        }

        public override void Render(Graphics graphics, Map mapBox)
        {
            if (!Visible)
            {
                base.Render(graphics, mapBox);
                return;
            }

            if (!initScreenPosition)
            {
                initScreenPosition = SetScreenLocationForAnchor();
            }

            // create root item
            var root = new LegendToolItem
            {
                Padding = Padding, Font = legendFont, Graphics = graphics
            };

            root.AddItem("Legend", true);

            AddLayerThemeItems(root, Layers);

            var rootSize = root.Size;

            rootSize.Width += (padding.Width + Indent)*root.Depth;

            var newSize = new Size((int) rootSize.Width, (int) rootSize.Height);

            var legendIsGrowing = newSize.Width > calculatedSize.Width;

            // Store our own new size (used by the component for dragging, etc.)
            calculatedSize = newSize;

            if (legendIsGrowing) //if we are growing, make sure we stay within the map area
            {
                CorrectScreenLocation();
            }

            // Paint a semi-transparent background
            using (var bgBrush = new SolidBrush(GetBackGroundColor()))
            {
                graphics.FillRectangle(bgBrush, new Rectangle(screenLocation, newSize));
            }

            if (Selected)
            {
                using (var selectionBrush = new SolidBrush(SelectionColor))
                {
                    graphics.FillRectangle(selectionBrush, new Rectangle(screenLocation, newSize));
                }
            }

            // Paint a black border
            graphics.DrawRectangle(Selected ? Pens.DodgerBlue : Pens.Black, new Rectangle(screenLocation, newSize));

            // draw root item
            DrawLegendItem(graphics, root, screenLocation.X, screenLocation.Y);

            base.Render(graphics, mapBox);
        }

        private static void AddLayerThemeItems(LegendToolItem parent, IEnumerable<ILayer> layers)
        {
            foreach (ILayer layer in layers)
            {
                if ((!layer.Visible) || (!layer.ShowInLegend))
                {
                    continue;
                }

                if (layer is IGroupLayer)
                {
                    // add a grouplayer item and then recursively call this function to add all the layers in the grouplayer
                    var newParent = parent.AddItem(layerThemeSymbol, layer.Name);
                    AddLayerThemeItems(newParent, ((IGroupLayer) layer).Layers);
                    continue;
                }

                if (layer is VectorLayer)
                {
                    AddLayerToLegend(layer as VectorLayer, parent);
                }
                else
                {
                    AddLayerToLegend(layer, parent);
                }
            }
        }

        private static void AddLayerToLegend(ILayer layer, LegendToolItem parent)
        {
            var title = layer.Name;
            var layerItem = parent.AddItem(title);
            if (layer.Theme != null && layer.Theme.ThemeItems != null)
            {
                AddThemeItemsAsLegendItems(layer.Theme.ThemeItems, layerItem, false);
            }
        }

        private static void AddLayerToLegend(VectorLayer layer, LegendToolItem parent)
        {
            // Add a legendItem for this layer
            var layerItem = parent.AddItem(layer.Name);

            // Use attribute name for layer name
            var attributeName = layer.Theme == null ? null : layer.Theme.AttributeName;
            if (!String.IsNullOrEmpty(attributeName) //not empty
                && !attributeName.Equals((layerItem.Text))) //not duplicate
            {
                layerItem.Text += String.Format(" ({0})", attributeName);
            }

            if (layer.Theme is CustomTheme)
            {
                layerItem.Symbol = ((VectorStyle) ((CustomTheme) layer.Theme).DefaultStyle).LegendSymbol;
            }
            else if (layer.Theme != null && layer.Theme.ThemeItems != null)
            {
                var isPointVectorLayer = layer.Style != null && layer.Style.GeometryType == typeof(IPoint);
                // Add vectorlayer theme items to the legend
                AddThemeItemsAsLegendItems(layer.Theme.ThemeItems, layerItem, isPointVectorLayer);
            }
            else if (layer.Style != null)
            {
                layerItem.Symbol = layer.Style.LegendSymbol;
            }
        }

        private static void AddThemeItemsAsLegendItems(IEnumerable<IThemeItem> themeItems, LegendToolItem rootItem,
                                                       bool isPointVectorLayer)
        {
            foreach (var themeItem in themeItems)
            {
                var legendItemLabel = themeItem.Label;

                if (!isPointVectorLayer)
                {
                    if (themeItem.Style is VectorStyle)
                    {
                        rootItem.AddItem((themeItem.Style as VectorStyle).LegendSymbol, legendItemLabel);
                    }
                }
                else
                {
                    rootItem.AddItem(themeItem.Symbol, themeItem.Label);
                }
            }
        }

        private void DrawLegendItem(Graphics graphics, LegendToolItem toolItem, float x, float y)
        {
            float curX = x;
            bool hasSymbol = toolItem.Symbol != null;
            bool hasText = toolItem.Text != null;

            if (hasSymbol)
            {
                var deltaY = (toolItem.InternalSize.Height - toolItem.Symbol.Height)/2;
                graphics.DrawImage(toolItem.Symbol, curX, y + deltaY);
                curX += toolItem.Symbol.Width;
            }

            if (hasText)
            {
                curX += (hasSymbol) ? padding.Width : 0;

                var deltaY = (toolItem.InternalSize.Height - graphics.MeasureString(toolItem.Text, toolItem.Font).Height)/2;
                graphics.DrawString(toolItem.Text, toolItem.Font, Brushes.Black, curX, y + deltaY);
            }

            if (hasText || hasSymbol)
            {
                y += toolItem.InternalSize.Height;
            }

            foreach (var subItem in toolItem.Items)
            {
                float deltaX = padding.Width + Indent;

                if (subItem.Centered && subItem.Symbol == null)
                {
                    float textWidth = graphics.MeasureString(subItem.Text, toolItem.Font).Width;
                    deltaX = (toolItem.Root.Size.Width - textWidth)/2;
                }

                DrawLegendItem(graphics, subItem, x + deltaX, y + padding.Height);
                y += subItem.Size.Height;
            }
        }
    }
}