﻿using System;
using System.Collections.Generic;
using GeoAPI.Extensions.Feature;
using SharpMap.Api;
using SharpMap.Api.Layers;
using SharpMap.Layers;
using SharpMap.Rendering.Thematics;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class MapLayerLegend
    {
        private List<IStyle> styles;
        private ILayer layer;

        public MapLayerLegend(ILayer layer)
        {
            styles = new List<IStyle>();
            this.layer = layer;
            VectorLayer vectorLayer;
            if ((vectorLayer = layer as VectorLayer) != null)
            {
                if (vectorLayer.Theme != null)
                {
                    GradientTheme gradientTheme;
                    CustomTheme customTheme;
                    QuantityTheme quantityTheme;
                    CategorialTheme categorialTheme;
                    if ((customTheme = vectorLayer.Theme as CustomTheme) != null)
                    {
                        if (customTheme.StyleDelegate != null)
                        {
                            for (int i = 0; i < vectorLayer.DataSource.GetFeatureCount(); i++)
                            {
                                IFeature feature = vectorLayer.DataSource.GetFeature(i);
                                styles.Add(customTheme.GetStyle(feature));
                            }
                        }
                        else
                        {
                            styles.Add(customTheme.DefaultStyle);
                        }
                    }
                    else if ((categorialTheme = vectorLayer.Theme as CategorialTheme) != null)
                    {
                        foreach (CategorialThemeItem categorialThemeItem in categorialTheme.ThemeItems)
                        {
                            styles.Add(categorialThemeItem.Style);
                        }
                    }
                    else if ((gradientTheme = vectorLayer.Theme as GradientTheme) != null)
                    {
                        //should render two styles, which can be used to create a colorbar.
/*                        styles.Add(gradientTheme.MinStyle);
                        styles.Add(gradientTheme.MaxStyle);*/
                        foreach (GradientThemeItem gradientThemeItem in gradientTheme.ThemeItems)
                        {
                            styles.Add(gradientThemeItem.Style);
                        }
                    }
                    else if ((quantityTheme = vectorLayer.Theme as QuantityTheme) != null)
                    {
                        foreach (QuantityThemeItem quantityThemeItem in quantityTheme.ThemeItems)
                        {
                            styles.Add(quantityThemeItem.Style);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("This kind of theme is not supported");
                    }
                }
            }
        }

        public ITheme Theme
        {
            get
            {
                VectorLayer vectorLayer;
                if ((vectorLayer = layer as VectorLayer) != null)
                {
                    return vectorLayer.Theme;
                }
                return null;
            }
        }


        public List<IStyle> Styles
        {
            get { return styles; }
            set { styles = value; }
        }
    }
}