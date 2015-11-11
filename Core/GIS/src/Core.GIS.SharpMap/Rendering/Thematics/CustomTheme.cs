// Copyright 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Drawing;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections.Generic;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.SharpMap.Api;

namespace Core.GIS.SharpMap.Rendering.Thematics
{
    /// <summary>
    /// The CustomTheme class is used for defining your own thematic rendering by using a custom get-style-delegate.
    /// </summary>
    [Entity(FireOnCollectionChange = false)]
    public class CustomTheme : ITheme
    {
        /// <summary>
        /// Custom Style Delegate method
        /// </summary>
        /// <remarks>
        /// The GetStyle delegate is used for determining the style of a feature using the <see cref="StyleDelegate"/> property.
        /// The method must take a <see cref="SharpMap.Data.FeatureDataRow"/> and return an <see cref="IStyle"/>.
        /// If the method returns null, the default style will be used for rendering.
        /// <para>
        /// <example>
        /// The following example can used for highlighting all features where the attribute "NAME" starts with "S".
        /// <code lang="C#">
        /// SharpMap.Rendering.Thematics.CustomTheme iTheme = new SharpMap.Rendering.Thematics.CustomTheme(GetCustomStyle);
        /// SharpMap.Styles.VectorStyle defaultstyle = new SharpMap.Styles.VectorStyle(); //Create default renderstyle
        /// defaultstyle.Fill = Brushes.Gray;
        /// iTheme.DefaultStyle = defaultstyle;
        /// 
        /// [...]
        /// 
        /// //Set up delegate for determining fill-color.
        /// //Delegate will fill all objects with a yellow color where the attribute "NAME" starts with "S".
        /// private static SharpMap.Styles.VectorStyle GetCustomStyle(SharpMap.Data.FeatureDataRow row)
        /// {
        /// 
        /// 	if (row["NAME"].ToString().StartsWith("S"))
        /// 	{
        /// 		SharpMap.Styles.VectorStyle style = new SharpMap.Styles.VectorStyle();
        /// 		style.Fill = Brushes.Yellow;
        /// 		return style;
        /// 	}
        /// 	else
        /// 		return null; //Return null which will render the default style
        /// }
        /// </code>
        /// </example>
        /// </para>
        /// </remarks>
        /// <param name="feature">Feature</param>
        /// <returns>Style to be applied to feature</returns>
        public delegate IStyle GetStyleMethod(IFeature feature);

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTheme"/> class
        /// </summary>
        /// <param name="getStyleMethod"></param>
        public CustomTheme(GetStyleMethod getStyleMethod)
        {
            StyleDelegate = getStyleMethod;
        }

        /// <summary>
        /// Gets or sets the default style when an attribute isn't found in any bucket
        /// </summary>
        public IStyle DefaultStyle { get; set; }

        /// <summary>
        /// Gets or sets the style delegate used for determining the style of a feature
        /// </summary>
        /// <remarks>
        /// The delegate must take a <see cref="SharpMap.Data.FeatureDataRow"/> and return an <see cref="IStyle"/>.
        /// If the method returns null, the default style will be used for rendering.
        /// <example>
        /// The example below creates a delegate that can be used for assigning the rendering of a road theme. If the road-class
        /// is larger than '3', it will be rendered using a thick red line.
        /// <code lang="C#">
        /// private static SharpMap.Styles.VectorStyle GetRoadStyle(SharpMap.Data.FeatureDataRow row)
        /// {
        ///		SharpMap.Styles.VectorStyle style = new SharpMap.Styles.VectorStyle();
        ///		if(((int)row["RoadClass"])>3)
        ///			style.Line = new Pen(Color.Red,5f);
        ///		else
        ///			style.Line = new Pen(Color.Black,1f);
        ///		return style;
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <seealso cref="GetStyleMethod"/>
        public GetStyleMethod StyleDelegate { get; set; }

        public object Clone()
        {
            CustomTheme customTheme = new CustomTheme(StyleDelegate);
            if (DefaultStyle != null)
            {
                customTheme.DefaultStyle = (IStyle) DefaultStyle.Clone();
            }
            return customTheme;
        }

        #region ITheme Members

        /// <summary>
        /// Returns the <see cref="System.Drawing.Color">color</see> based on an attribute value
        /// </summary>
        /// <returns>Style generated by GetStyle-Delegate</returns>
        public IStyle GetStyle(IFeature feature)
        {
            if (StyleDelegate == null)
            {
                return DefaultStyle;
            }
            IStyle style = StyleDelegate(feature);
            if (style != null)
            {
                return style;
            }
            else
            {
                return DefaultStyle;
            }
        }

        public IStyle GetStyle<T>(T value) where T : IComparable<T>, IComparable
        {
            // TODO: This should use a delegate for GetStyle as well (but this isn't used by Ringtoets)
            return DefaultStyle;
        }

        public virtual IEventedList<IThemeItem> ThemeItems
        {
            get
            {
                //IEventedList<IThemeItem> themes = new EventedList<IThemeItem>();
                //themes.ad
                return new EventedList<IThemeItem>();
                //throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Color GetFillColor<T>(T value) where T : IComparable
        {
            throw new NotImplementedException();
        }

        public unsafe void GetFillColors<T>(int* colors, int length, T[] values) where T : IComparable
        {
            throw new NotImplementedException();
        }

        public virtual string AttributeName { get; set; }

        public void ScaleTo(double min, double max)
        {
            // no particular rescaling behavoir implemented
            return;
        }

        #endregion
    }
}